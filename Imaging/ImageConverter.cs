/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageConverter.cs
 * PURPOSE:     Image Converter
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Imaging
{
    /// <summary>
    ///     Image Converter stuff
    /// </summary>
    internal static class ImageConverter
    {
        /// <summary>
        ///     Bitmaps to base64.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>Image as string</returns>
        internal static string BitmapToBase64(Bitmap bitmap)
        {
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            var imageBytes = memoryStream.ToArray();
            return Convert.ToBase64String(imageBytes);
        }

        /// <summary>
        ///     Converts a BitmapImage to a Base64 string.
        /// </summary>
        /// <param name="bitmapImage">The BitmapImage to convert.</param>
        /// <returns>Base64 encoded image as a string.</returns>
        internal static string BitmapImageToBase64(BitmapImage bitmapImage)
        {
            using var memoryStream = new MemoryStream();

            // Create a PNG encoder and save the bitmap to the memory stream
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(memoryStream);

            var imageBytes = memoryStream.ToArray();
            return Convert.ToBase64String(imageBytes);
        }
    }
}