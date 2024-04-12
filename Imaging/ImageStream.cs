/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageStream.cs
 * PURPOSE:     Does all the leg work for the Image operations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using ExtendedSystemObjects;
using Mathematics;

namespace Imaging
{
    /// <summary>
    ///     Loads a BitMapImage out of a specific path
    ///     Can Combine two Images and returns a new one
    /// </summary>
    public static class ImageStream
    {
        /// <summary>
        ///     Loads File one Time
        ///     Can only used to load an Image Once
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <returns>An Image as <see cref="BitmapImage" />.</returns>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="IOException">Could not find the File</exception>
        public static BitmapImage GetBitmapImage(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            try
            {
                var bmp = new BitmapImage { CreateOptions = BitmapCreateOptions.DelayCreation };
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(path);
                bmp.EndInit();
                return bmp;
            }
            catch (UriFormatException ex)
            {
                throw new UriFormatException(path, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.ToString());
            }
            catch (NotSupportedException ex)
            {
                throw new NotSupportedException(ex.ToString());
            }
        }

        /// <summary>
        ///     Loads File one Time
        ///     Can only used to load an Image Once
        ///     Use this for huge amounts of image that will be resized, else we will break memory limits
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>
        ///     An Image as <see cref="BitmapImage" />.
        /// </returns>
        /// <exception cref="IOException">Could not find the File</exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        public static BitmapImage GetBitmapImage(string path, int width, int height)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            try
            {
                var bmp = new BitmapImage { CreateOptions = BitmapCreateOptions.DelayCreation };
                bmp.BeginInit();
                bmp.DecodePixelHeight = height;
                bmp.DecodePixelWidth = width;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(path);
                bmp.EndInit();
                return bmp;
            }
            catch (UriFormatException ex)
            {
                throw new UriFormatException(path, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.ToString());
            }
            catch (NotSupportedException ex)
            {
                throw new NotSupportedException(ex.ToString());
            }
        }

        /// <summary>
        ///     Loads File in a Stream
        ///     takes longer but can be changed on Runtime
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <returns>An Image as <see cref="BitmapImage" />.</returns>
        /// <exception cref="ArgumentException">No Correct Argument were provided</exception>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        /// <exception cref="IOException">Error while we try to access the File</exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="IOException">Could not find the File</exception>
        public static BitmapImage GetBitmapImageFileStream(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            var bmp = new BitmapImage { CreateOptions = BitmapCreateOptions.DelayCreation };

            try
            {
                using var flStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = flStream;
                bmp.EndInit();

                return bmp;
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.ToString());
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException(e.ToString());
            }
            catch (IOException e)
            {
                throw new IOException(e.ToString());
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.ToString());
            }
        }

        /// <summary>
        ///     Loads File in a Stream
        ///     takes longer but can be changed on Runtime
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>
        ///     An Image as <see cref="BitmapImage" />.
        /// </returns>
        /// <exception cref="IOException">
        /// </exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException">Error while we try to access the File</exception>
        /// <exception cref="ArgumentException">No Correct Argument were provided</exception>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="IOException">Error while we try to access the File</exception>
        public static BitmapImage GetBitmapImageFileStream(string path, int width, int height)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            var bmp = new BitmapImage { CreateOptions = BitmapCreateOptions.DelayCreation };

            try
            {
                using var flStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.DecodePixelHeight = height;
                bmp.DecodePixelWidth = width;
                bmp.StreamSource = flStream;
                bmp.EndInit();

                return bmp;
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.ToString());
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException(e.ToString());
            }
            catch (FileFormatException e)
            {
                Trace.Write(e.ToString());
                return null;
            }
            catch (IOException e)
            {
                throw new IOException(e.ToString());
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.ToString());
            }
        }

        /// <summary>
        ///     Get the bitmap file.
        ///     Will  leak like crazy. Only use it if we load Icons or something.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     The Image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="IOException">File not Found</exception>
        internal static Bitmap GetBitmapFile(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                return new Bitmap(path, true);
            }

            var innerException = path != null
                ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                : new IOException(nameof(path));
            throw new IOException(ImagingResources.ErrorMissingFile, innerException);
        }

        /// <summary>
        ///     Gets the original bitmap file.
        ///     Takes longer but produces a higher quality
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     The Image in Original size
        /// </returns>
        /// <exception cref="IOException">
        /// </exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        internal static Bitmap GetOriginalBitmap(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            try
            {
                using var flStream = new FileStream(path, FileMode.Open);
                // Original picture information
                var original = new Bitmap(flStream);

                var bmp = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppPArgb);

                using var graph = Graphics.FromImage(bmp);
                graph.Clear(Color.Transparent);
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graph.DrawImage(original, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0,
                    original.Width,
                    original.Height, GraphicsUnit.Pixel);

                return bmp;
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.ToString());
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException(e.ToString());
            }
            catch (IOException e)
            {
                throw new IOException(e.ToString());
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.ToString());
            }
        }

        /// <summary>
        ///     Resizes an image
        /// </summary>
        /// <param name="image">The image to resize</param>
        /// <param name="width">The new width in pixels</param>
        /// <param name="height">The new height in pixels</param>
        /// <returns>
        ///     A resized version of the original image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="InsufficientMemoryException"></exception>
        public static Bitmap BitmapScaling(Bitmap image, int width, int height)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(BitmapScaling), ImagingResources.Spacing,
                        nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            var btm = new Bitmap(width, height);
            //fix Resolution
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            try
            {
                using var graph = Graphics.FromImage(btm);
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graph.DrawImage(image, 0, 0, width, height);
                return new Bitmap(btm);
            }
            catch (InsufficientMemoryException ex)
            {
                Trace.WriteLine(ex);
                throw new InsufficientMemoryException(ex.ToString());
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                throw new ArgumentException(ex.ToString());
            }
            catch (OutOfMemoryException ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        ///     Bitmaps the scaling.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="scaling">The scaling.</param>
        /// <returns>
        ///     A resized version of the original image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        internal static Bitmap BitmapScaling(Bitmap image, float scaling)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(BitmapScaling), ImagingResources.Spacing,
                        nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            var width = (int)(image.Width * scaling);
            var height = (int)(image.Height * scaling);

            //needed because of: A Graphics object cannot be created from an image that has an indexed pixel format
            var btm = new Bitmap(width, height);
            //fix Resolution
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graph = Graphics.FromImage(btm))
            {
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var rect = new Rectangle(0, 0, width, height);

                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graph.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }

            return new Bitmap(btm);
        }

        /// <summary>
        ///     Combines the bitmaps overlays them and merges them into one Image
        ///     Can and will throw Exceptions as part of nameof(ToBitmapImage)
        ///     The size will be trimmed to the size of the first Image
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>
        ///     a new Bitmap with all combined Images
        /// </returns>
        /// <exception cref="ArgumentNullException">if file is null or Empty</exception>
        [return: MaybeNull]
        internal static Bitmap CombineBitmap(List<string> files)
        {
            if (files.IsNullOrEmpty())
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(CombineBitmap), ImagingResources.Spacing,
                        nameof(files)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //read all images into memory
            var images = new List<Bitmap>();

            foreach (var image in files)
            {
                //create a Bitmap from the file and add it to the list
                if (!File.Exists(image))
                {
                    Trace.WriteLine(string.Concat(ImagingResources.ErrorMissingFile, image));
                    continue;
                }

                var bitmap = new Bitmap(image);

                images.Add(bitmap);
            }

            if (images.IsNullOrEmpty())
            {
                Trace.WriteLine(ImagingResources.ErrorMissingFile);
                return null;
            }

            //get the correct size of the Final Image
            var bmp = images[0];

            //create a bitmap to hold the combined image
            var btm = new Bitmap(bmp.Width, bmp.Height);

            //get a graphics object from the image so we can draw on it
            using (var graph = Graphics.FromImage(btm))
            {
                //go through each image and draw it on the final image
                foreach (var image in images)
                {
                    graph.DrawImage(image,
                        new Rectangle(0, 0, image.Width, image.Height));
                }
            }

            foreach (var image in images)
            {
                image.Dispose();
            }

            //before return please Convert
            return btm;
        }

        /// <summary>
        ///     Combines the bitmaps.
        /// </summary>
        /// <param name="original">The original image.</param>
        /// <param name="overlay">The overlay image.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>Combined Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Bitmap CombineBitmap(Bitmap original, Bitmap overlay, int x, int y)
        {
            if (original == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(CombineBitmap),
                    ImagingResources.Spacing, nameof(original)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (overlay == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(CombineBitmap),
                    ImagingResources.Spacing, nameof(overlay)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //get a graphics object from the image so we can draw on it
            using var graph = Graphics.FromImage(original);

            graph.DrawImage(overlay,
                new Rectangle(x, y, overlay.Width, overlay.Height));

            return original;
        }

        /// <summary>
        ///     Cuts a piece out of a bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>The cut Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Bitmap CutBitmap(Bitmap image, int x, int y, int height, int width)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(
                        string.Concat(nameof(CutBitmaps), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (height == 0 || width == 0)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(CombineBitmap),
                    ImagingResources.Spacing, nameof(height), ImagingResources.Spacing, nameof(width)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            var btm = new Bitmap(width, height);

            using var graph = Graphics.FromImage(btm);
            graph.CompositingMode = CompositingMode.SourceCopy;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graph.Clear(Color.White);
            var rect = new Rectangle(x, y, width, height);

            graph.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);

            return btm;
        }

        /// <summary>
        ///     Cuts a bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>List of cut Images</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<Bitmap> CutBitmaps(Bitmap image, int x, int y, int height, int width)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(
                        string.Concat(nameof(CutBitmaps), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //read all images into memory
            var images = new List<Bitmap>(x * y);

            for (var j = 0; j < y; j++)
            for (var i = 0; i < x; i++)
            {
                var img = CutBitmap(image, i * width, j * height, height, width);
                images.Add(img);
            }

            return images;
        }

        /// <summary>
        ///     Erases the rectangle from an Image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>Original Image with the erased area</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Bitmap EraseRectangle(Bitmap image, int x, int y, int height, int width)
        {
            if (image == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(EraseRectangle),
                    ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            using var graph = Graphics.FromImage(image);

            graph.CompositingMode = CompositingMode.SourceCopy;

            using var br = new SolidBrush(Color.FromArgb(0, 255, 255, 255));

            graph.FillRectangle(br, new Rectangle(x, y, width, height));

            return image;
        }

        /// <summary>
        ///     Converts an image to gray scale
        ///     Source:
        ///     https://web.archive.org/web/20110525014754/http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
        /// </summary>
        /// <param name="image">The image to gray scale</param>
        /// <param name="filter">Image Filter</param>
        /// <returns>
        ///     A filtered version of the image
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        /// <exception cref="OutOfMemoryException"></exception>
        [return: MaybeNull]
        internal static Bitmap FilterImage(Bitmap image, ImageFilter filter)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(FilterImage), ImagingResources.Spacing,
                        nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //create a blank bitmap the same size as original
            var btm = new Bitmap(image.Width, image.Height);
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //get a graphics object from the new image
            using var graph = Graphics.FromImage(btm);
            //create some image attributes
            using var atr = new ImageAttributes();

            //set the color matrix attribute
            switch (filter)
            {
                case ImageFilter.GrayScale:
                    atr.SetColorMatrix(ImageRegister.GrayScale);
                    break;
                case ImageFilter.Invert:
                    atr.SetColorMatrix(ImageRegister.Invert);
                    break;
                case ImageFilter.Sepia:
                    atr.SetColorMatrix(ImageRegister.Sepia);
                    break;
                case ImageFilter.BlackAndWhite:
                    atr.SetColorMatrix(ImageRegister.BlackAndWhite);
                    break;
                case ImageFilter.Polaroid:
                    atr.SetColorMatrix(ImageRegister.Polaroid);
                    break;
                default:
                    return null;
            }

            try
            {
                //draw the original image on the new image
                //using the gray scale color matrix
                graph.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    0, 0, image.Width, image.Height, GraphicsUnit.Pixel, atr);
            }
            catch (OutOfMemoryException ex)
            {
                Trace.WriteLine(ex);
                throw;
            }

            //convert to BitmapImage
            return btm;
        }

        /// <summary>
        ///     Bitmaps the image2 bitmap.
        /// </summary>
        /// <param name="image">The bitmap image.</param>
        /// <returns>
        ///     The Image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        internal static Bitmap BitmapImageToBitmap(BitmapImage image)
        {
            if (image == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(BitmapImageToBitmap),
                    ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            using var outStream = new MemoryStream();
            var enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(image));
            enc.Save(outStream);
            var bitmap = new Bitmap(outStream);

            return new Bitmap(bitmap);
        }

        /// <summary>
        ///     Converts to bitmap image.
        ///     https://stackoverflow.com/questions/5199205/how-do-i-rotate-image-then-move-to-the-top-left-0-0-without-cutting-off-the-imag/5200280#5200280
        /// </summary>
        /// <param name="image">The bitmap.</param>
        /// The Image as
        /// <see cref="BitmapImage" />
        /// .
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        internal static BitmapImage BitmapToBitmapImage(Bitmap image)
        {
            if (image == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(BitmapToBitmapImage),
                    ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            using var memory = new MemoryStream();
            image.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        /// <summary>
        ///     Rotates the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="degree">The angle in degrees</param>
        /// <returns>
        ///     Rotated Image
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        /// <exception cref="OverflowException">Degrees have a certain allowed radius</exception>
        internal static Bitmap RotateImage(Bitmap image, int degree)
        {
            //no need to do anything
            if (degree is 360 or 0)
            {
                return image;
            }

            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(RotateImage), ImagingResources.Spacing,
                        nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (degree is > 360 or < -360)
            {
                var innerException = new OverflowException(nameof(degree));
                throw new OverflowException(string.Concat(ImagingResources.ErrorWrongParameters, degree),
                    innerException);
            }

            //Calculate the size of the new Bitmap because if we rotate the Image it will be bigger
            var wOver = image.Width / 2.0f;
            var hOver = image.Height / 2.0f;

            // Get the coordinates of the corners, taking the origin to be the centre of the bitmap.
            PointF[] corners = { new(-wOver, -hOver), new(+wOver, -hOver), new(+wOver, +hOver), new(-wOver, +hOver) };

            for (var i = 0; i < 4; i++)
            {
                var point = corners[i];
                corners[i] =
                    new PointF(
                        (float)((point.X * ExtendedMath.CalcCos(degree)) - (point.Y * ExtendedMath.CalcSin(degree))),
                        (float)((point.X * ExtendedMath.CalcSin(degree)) + (point.Y * ExtendedMath.CalcCos(degree))));
            }

            // Find the min and max x and y coordinates.
            var minX = corners[0].X;
            var maxX = minX;
            var minY = corners[0].Y;
            var maxY = minY;

            for (var i = 1; i < 4; i++)
            {
                var p = corners[i];
                minX = Math.Min(minX, p.X);
                maxX = Math.Max(maxX, p.X);
                minY = Math.Min(minY, p.Y);
                maxY = Math.Max(maxY, p.Y);
            }

            // Get the size of the new bitmap.
            var newSize = new SizeF(maxX - minX, maxY - minY);
            // create it.
            var btm = new Bitmap((int)Math.Ceiling(newSize.Width), (int)Math.Ceiling(newSize.Height));
            //fix Resolution
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //  draw the old bitmap on it.
            using var graph = Graphics.FromImage(btm);
            graph.TranslateTransform(newSize.Width / 2.0f, newSize.Height / 2.0f);
            graph.RotateTransform(degree);
            graph.TranslateTransform(-image.Width / 2.0f, -image.Height / 2.0f);
            graph.DrawImage(image, 0, 0);

            return btm;
        }

        /// <summary>
        ///     Crops the image.
        ///     Non Edges are defined as transparent
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Coped Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Bitmap CropImage(Bitmap image)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(CropImage), ImagingResources.Spacing,
                        nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            var first = new Point();
            var second = new Point();

            var dbm = DirectBitmap.GetInstance(image);

            // determine new left
            var left = -1;
            var right = -1;
            var top = -1;
            var bottom = -1;

            //Get the Top
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    var color = dbm.GetPixel(x, y);
                    if (CheckTransparent(color))
                    {
                        continue;
                    }

                    // this pixel is either not white or not fully transparent
                    top = x;
                    break;
                }

                if (top != -1)
                {
                    break;
                }
            }

            //Get the Bottom
            for (var x = image.Width - 1; x >= 0; --x)
            {
                for (var y = image.Height - 1; y >= 0; --y)
                {
                    var color = dbm.GetPixel(x, y);
                    if (CheckTransparent(color))
                    {
                        continue;
                    }

                    // this pixel is either not white or not fully transparent
                    bottom = x;
                    break;
                }

                if (bottom != -1)
                {
                    break;
                }
            }

            //Get the left
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = image.Height - 1; y >= 0; --y)
                {
                    var color = dbm.GetPixel(x, y);
                    if (CheckTransparent(color))
                    {
                        continue;
                    }

                    // this pixel is either not white or not fully transparent
                    left = x;
                    break;
                }

                if (left != -1)
                {
                    break;
                }
            }

            //Get the right
            for (var x = image.Width - 1; x >= 0; --x)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    var color = dbm.GetPixel(x, y);
                    if (CheckTransparent(color))
                    {
                        continue;
                    }

                    // this pixel is either not white or not fully transparent
                    right = x;
                    break;
                }

                if (right != -1)
                {
                    break;
                }
            }

            first.X = left;
            first.Y = top;

            second.X = right;
            second.Y = bottom;

            //calculate the measures
            var width = Math.Abs(second.X - first.X);
            var height = Math.Abs(second.Y - first.Y);

            // Create a new bitmap from the crop rectangle, cut out the image
            var cropRectangle = new Rectangle(first.X, first.Y, width, height);
            var btm = new Bitmap(width, height);
            //fix Resolution
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //draw the cut out onto a new image
            using (var graph = Graphics.FromImage(btm))
            {
                graph.DrawImage(image, 0, 0, cropRectangle, GraphicsUnit.Pixel);
            }

            //clear up
            dbm.Dispose();

            return btm;
        }

        /// <summary>
        ///     Saves the bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="path">The path where we want to save it.</param>
        /// <param name="format">The Image format.</param>
        /// <exception cref="ArgumentNullException">Wrong parameters</exception>
        /// <exception cref="IOException">File already exists</exception>
        /// <exception cref="ExternalException">Errors with the Path</exception>
        public static void SaveBitmap(Bitmap image, string path, ImageFormat format)
        {
            if (format == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(SaveBitmap), ImagingResources.Spacing,
                        nameof(format)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(
                        string.Concat(nameof(SaveBitmap), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (string.IsNullOrEmpty(path))
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(SaveBitmap), ImagingResources.Spacing,
                        nameof(path)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (File.Exists(path))
            {
                var count = 1;

                var fileNameOnly = Path.GetFileNameWithoutExtension(path);
                var extension = Path.GetExtension(path);
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    return;
                }

                var newPath = path;

                while (File.Exists(newPath))
                {
                    var tempFileName = $"{fileNameOnly}({count++})";
                    newPath = Path.Combine(directory!, tempFileName + extension);
                }

                SaveBitmap(image, newPath, format);
            }

            try
            {
                image.Save(path, format);
            }
            catch (ExternalException ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        ///     Converts White to Transparent.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="threshold">The threshold when the color is still white.</param>
        /// <returns>The Transparent Image</returns>
        /// <exception cref="ArgumentNullException">Wrong parameters</exception>
        internal static Bitmap ConvertWhiteToTransparent(Bitmap image, int threshold)
        {
            if (image == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(ConvertWhiteToTransparent),
                    ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //use our new Format
            var dbm = DirectBitmap.GetInstance(image);

            //255,255,255 is White
            var replacementColor = Color.FromArgb(255, 255, 255);

            for (var x = 0; x < dbm.Width; x++)
            for (var y = 0; y < dbm.Height; y++)
            {
                var color = dbm.GetPixel(x, y);

                //not in the area? continue, 255 is White
                if (255 - color.R >= threshold || 255 - color.G >= threshold || 255 - color.B >= threshold)
                {
                    continue;
                }

                //replace Value under the threshold with pure White
                dbm.SetPixel(x, y, replacementColor);
            }

            //get the Bitmap
            var btm = new Bitmap(dbm.Bitmap);
            //make Transparent
            btm.MakeTransparent(replacementColor);
            //cleanup
            dbm.Dispose();
            return btm;
        }

        /// <summary>
        ///     Gets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The Color at the point
        /// </returns>
        /// <exception cref="ArgumentNullException">nameof(image)</exception>
        internal static Color GetPixel(Bitmap image, Point point)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(GetPixel), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //use our new Format
            var dbm = DirectBitmap.GetInstance(image);
            return dbm.GetPixel(point.X, point.Y);
        }

        /// <summary>
        ///     Gets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>
        ///     The Color at the Point
        /// </returns>
        /// <exception cref="ArgumentNullException">nameof(image)</exception>
        internal static Color GetPixel(Bitmap image, Point point, int radius)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(GetPixel), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            var points = GetCirclePoints(point, radius, image.Height, image.Width);

            if (points.Count == 0)
            {
                return GetPixel(image, point);
            }

            var r = 0;
            var g = 0;
            var b = 0;

            foreach (var color in points.Select(pointSingle => GetPixel(image, pointSingle)))
            {
                r += color.R;
                g += color.G;
                b += color.B;
            }

            r /= points.Count;
            g /= points.Count;
            b /= points.Count;

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        ///     Sets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="color">The color.</param>
        /// <returns>
        ///     The changed image as Bitmap
        /// </returns>
        /// <exception cref="ArgumentNullException">nameof(image)</exception>
        internal static Bitmap SetPixel(Bitmap image, Point point, Color color)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(SetPixel), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //use our new Format
            var dbm = DirectBitmap.GetInstance(image);
            dbm.SetPixel(point.X, point.Y, color);

            return dbm.Bitmap;
        }

        /// <summary>
        ///     Sets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="color">The color.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>The Changed Image</returns>
        /// <exception cref="ArgumentNullException">nameof(image)</exception>
        internal static Bitmap SetPixel(Bitmap image, Point point, Color color, int radius)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(SetPixel), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            var points = GetCirclePoints(point, radius, image.Height, image.Width);

            return points.Aggregate(image, (current, pointSingle) => SetPixel(current, pointSingle, color));
        }

        /// <summary>
        ///     Checks if the Color is  transparent.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>True if conditions are met</returns>
        private static bool CheckTransparent(Color color)
        {
            //0,0,0 is Black or Transparent
            return color.R == 0 && color.G == 0 && color.B == 0;
        }

        /// <summary>
        ///     Gets all points in a Circle.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="length">The length.</param>
        /// <param name="width">The height.</param>
        /// <returns>List of Points</returns>
        private static List<Point> GetCirclePoints(Point point, int radius, int length, int width)
        {
            var lst = new List<Point>();

            var minX = point.X - radius;
            if (minX < 0)
            {
                minX = 0;
            }

            var maxX = point.X + radius;
            if (maxX > width)
            {
                maxX = width;
            }

            var minY = point.Y - radius;
            if (minY < 0)
            {
                minY = 0;
            }

            var maxY = point.Y + radius;
            if (maxY > width)
            {
                maxY = length;
            }

            for (var x = minX; x <= maxX; x++)
            for (var y = minY; y <= maxY; y++)
            {
                var calcPoint = new Point(x, y);

                var dist = Math.Sqrt(Math.Pow(calcPoint.X - point.X, 2) + Math.Pow(calcPoint.Y - point.Y, 2));

                if (dist <= radius)
                {
                    lst.Add(calcPoint);
                }
            }

            return lst;
        }
    }
}
