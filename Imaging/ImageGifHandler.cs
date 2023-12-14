/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageGifHandler.cs
 * PURPOSE:     Some processing stuff for Gif Images, not perfect, the files are slightly bigger though.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://stackoverflow.com/questions/18719302/net-creating-a-looping-gif-using-gifbitmapencoder
 *              https://debugandrelease.blogspot.com/2018/12/creating-gifs-in-c.html
 *              http://www.matthewflickinger.com/lab/whatsinagif/bits_and_bytes.asp
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ExtendedSystemObjects;
using FileHandler;
using Size = System.Drawing.Size;

namespace Imaging
{
    public static class ImageGifHandler
    {
        /// <summary>
        ///     Gets the image information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>gif Infos</returns>
        public static ImageGifInfo GetImageInfo(string path)
        {
            var info = new ImageGifInfo();

            try
            {
                using var image = Image.FromFile(path);
                info.Height = image.Height;
                info.Width = image.Width;
                info.Name = Path.GetFileName(path);
                info.Size = image.Size;

                if (!image.RawFormat.Equals(ImageFormat.Gif))
                {
                    return null;
                }

                if (!ImageAnimator.CanAnimate(image))
                {
                    return info;
                }

                var frameDimension = new FrameDimension(image.FrameDimensionsList[0]);

                var frameCount = image.GetFrameCount(frameDimension);

                info.AnimationLength = frameCount / 10 * frameCount;

                info.IsAnimated = true;

                info.IsLooped = BitConverter.ToInt16(image.GetPropertyItem(20737)?.Value!, 0) != 1;

                info.Frames = frameCount;
            }
            catch (OutOfMemoryException ex)
            {
                var currentProcess = Process.GetCurrentProcess();
                var memorySize = currentProcess.PrivateMemorySize64;

                Trace.WriteLine(string.Concat(ex, ImagingResources.Separator, ImagingResources.ErrorMemory,
                    memorySize));

                //enforce clean up and hope for the best
                GC.Collect();
            }

            return info;
        }

        /// <summary>
        ///     Splits the GIF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of Images from gif</returns>
        internal static List<Bitmap> SplitGif(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            var lst = new List<Bitmap>();

            try
            {
                using var image = Image.FromFile(path);

                var numberOfFrames = image.GetFrameCount(FrameDimension.Time);

                for (var i = 0; i < numberOfFrames; i++)
                {
                    image.SelectActiveFrame(FrameDimension.Time, i);
                    var bmp = new Bitmap(image);
                    lst.Add(bmp);
                }
            }
            //try to handle potential Memory problem, a bit of a hack
            catch (OutOfMemoryException ex)
            {
                var currentProcess = Process.GetCurrentProcess();
                var memorySize = currentProcess.PrivateMemorySize64;

                Trace.WriteLine(string.Concat(ex, ImagingResources.Separator, ImagingResources.ErrorMemory,
                    memorySize));
                lst.Clear();
                //enforce clean up and hope for the best
                GC.Collect();

                ImageRegister.Count++;

                if (ImageRegister.Count < 3)
                {
                    SplitGif(path);
                }
                else
                {
                    ImageRegister.Count = 0;
                    throw;
                }
            }

            return lst;
        }

        /// <summary>
        ///     Loads the GIF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of Images from gif as ImageSource</returns>
        internal static List<ImageSource> LoadGif(string path)
        {
            var list = SplitGif(path);

            return list.Select(image => image.ToBitmapImage()).Cast<ImageSource>().ToList();
        }

        /// <summary>
        ///     Creates the gif.
        ///     The gif is slightly bigger for now
        ///     Sources:
        ///     https://stackoverflow.com/questions/18719302/net-creating-a-looping-gif-using-gifbitmapencoder
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <param name="target">The target path.</param>
        internal static void CreateGif(string path, string target)
        {
            //get all allowed files from target folder
            var lst = FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix, false);

            lst = lst.PathSort();

            //collect and convert all images
            var btm = lst.ConvertAll(ImageStream.GetOriginalBitmap);

            if (btm.IsNullOrEmpty())
            {
                return;
            }

            var gEnc = new GifBitmapEncoder();

            //TODO encode and change to one size, add more sanity checks

            foreach (var src in btm.Select(bmpImage => bmpImage.GetHbitmap()).Select(bmp =>
                         System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                             bmp,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions())))
            {
                gEnc.Frames.Add(BitmapFrame.Create(src));
            }

            using var ms = new MemoryStream();
            gEnc.Save(ms);
            var fileBytes = ms.ToArray();
            // write custom header
            // This is the NETSCAPE2.0 Application Extension.
            var applicationExtension =
                new byte[] { 33, 255, 11, 78, 69, 84, 83, 67, 65, 80, 69, 50, 46, 48, 3, 1, 0, 0, 0 };
            var newBytes = new List<byte>();
            newBytes.AddRange(fileBytes.Take(13));
            newBytes.AddRange(applicationExtension);
            newBytes.AddRange(fileBytes.Skip(13));
            File.WriteAllBytes(target, newBytes.ToArray());
        }
    }

    /// <summary>
    ///     The infos about the gif
    /// </summary>
    public sealed class ImageGifInfo
    {
        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height { get; internal set; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public int Width { get; internal set; }

        /// <summary>
        ///     Gets the length of the animation.
        /// </summary>
        /// <value>
        ///     The length of the animation.
        /// </value>
        public int AnimationLength { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is animated.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is animated; otherwise, <c>false</c>.
        /// </value>
        internal bool IsAnimated { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is looped.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is looped; otherwise, <c>false</c>.
        /// </value>
        internal bool IsLooped { get; set; }

        /// <summary>
        ///     Gets the frames.
        /// </summary>
        /// <value>
        ///     The frames.
        /// </value>
        public int Frames { get; internal set; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; internal set; }

        /// <summary>
        ///     Gets the size.
        /// </summary>
        /// <value>
        ///     The size.
        /// </value>
        public Size Size { get; internal set; }
    }
}
