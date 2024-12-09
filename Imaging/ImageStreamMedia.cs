/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageStreamMedia.cs
 * PURPOSE:     Does all the leg work for the Image operations, in this case the newer Media.Imaging
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCE:      https://lodev.org/cgtutor/floodfill.html
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Imaging
{
    /// <summary>
    ///     Handle the more newer wpf Libraries
    /// </summary>
    public static class ImageStreamMedia
    {
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
        public static BitmapImage GetBitmapImage(string path, int width = 0, int height = 0)
        {
            ImageHelper.ValidateFilePath(path);
            try
            {
                var bmp = new BitmapImage { CreateOptions = BitmapCreateOptions.DelayCreation };
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(path);
                if (width > 0 && height > 0)
                {
                    bmp.DecodePixelWidth = width;
                    bmp.DecodePixelHeight = height;
                }

                bmp.EndInit();
                return bmp;
            }
            catch (Exception ex)
            {
                ImageHelper.HandleException(ex);
                // Optionally, rethrow or handle further as needed
                throw; // This will preserve the original stack trace and exception details
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
        public static BitmapImage GetBitmapImageFileStream(string path, int width = 0, int height = 0)
        {
            ImageHelper.ValidateFilePath(path);

            var bmp = new BitmapImage { CreateOptions = BitmapCreateOptions.DelayCreation };

            try
            {
                using var flStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;

                if (width > 0 && height > 0)
                {
                    bmp.DecodePixelWidth = width;
                    bmp.DecodePixelHeight = height;
                }

                bmp.StreamSource = flStream;
                bmp.EndInit();

                return bmp;
            }
            catch (FileFormatException ex)
            {
                Trace.Write(ex.ToString());
                return null;
            }
            catch (Exception ex)
            {
                ImageHelper.HandleException(ex);
                // Optionally, rethrow or handle further as needed
                throw; // This will preserve the original stack trace and exception details
            }
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
            ImageHelper.ValidateImage(nameof(BitmapImageToBitmap), image);

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
            ImageHelper.ValidateImage(nameof(BitmapToBitmapImage), image);

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
    }
}