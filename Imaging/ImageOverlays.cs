/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageFilterStream.cs
 * PURPOSE:     Generic Image Operations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://lodev.org/cgtutor/imagearithmetic.html
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Imaging
{
    /// <summary>
    ///     Combine two Images in various ways
    /// </summary>
    internal static class ImageOverlays
    {
        /// <summary>
        ///     Combines two images by averaging their pixel values.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>A bitmap resulting from the average of the two images, or null if an error occurs.</returns>
        public static Bitmap AverageImages(Image imgOne, Image imgTwo)
        {
            return ProcessImages(imgOne, imgTwo, (color1, color2) =>
            {
                var r = (color1.R + color2.R) / 2;
                var g = (color1.G + color2.G) / 2;
                var b = (color1.B + color2.B) / 2;
                return Color.FromArgb(r, g, b);
            });
        }

        /// <summary>
        ///     Combines two images by adding their pixel values.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>A bitmap resulting from the combination of the two images, or null if an error occurs.</returns>
        public static Bitmap AddImages(Image imgOne, Image imgTwo)
        {
            return ProcessImages(imgOne, imgTwo, (color1, color2) =>
            {
                var r = ImageHelper.Clamp(color1.R + color2.R);
                var g = ImageHelper.Clamp(color1.G + color2.G);
                var b = ImageHelper.Clamp(color1.B + color2.B);
                return Color.FromArgb(r, g, b);
            });
        }

        /// <summary>
        ///     Combines two images by subtracting the pixel values of the first image from the second image.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>A bitmap resulting from the subtraction of the two images, or null if an error occurs.</returns>
        public static Bitmap SubtractImages(Image imgOne, Image imgTwo)
        {
            return ProcessImages(imgOne, imgTwo, (color1, color2) =>
            {
                var r = ImageHelper.Clamp(color2.R - color1.R);
                var g = ImageHelper.Clamp(color2.G - color1.G);
                var b = ImageHelper.Clamp(color2.B - color1.B);
                return Color.FromArgb(r, g, b);
            });
        }

        /// <summary>
        ///     Combines two images by multiplying their pixel values.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>A bitmap resulting from the multiplication of the two images, or null if an error occurs.</returns>
        public static Bitmap MultiplyImages(Image imgOne, Image imgTwo)
        {
            return ProcessImages(imgOne, imgTwo, (color1, color2) =>
            {
                var r = ImageHelper.Clamp(color1.R * color2.R / 255);
                var g = ImageHelper.Clamp(color1.G * color2.G / 255);
                var b = ImageHelper.Clamp(color1.B * color2.B / 255);
                return Color.FromArgb(r, g, b);
            });
        }

        /// <summary>
        ///     Cross-fades between two images based on the given factor.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <param name="factor">The blending factor (0.0 to 1.0).</param>
        /// <returns>A bitmap resulting from the cross-fading of the two images, or null if an error occurs.</returns>
        public static Bitmap CrossFadeImages(Image imgOne, Image imgTwo, float factor)
        {
            return ProcessImages(imgOne, imgTwo, (color1, color2) =>
            {
                var r = ImageHelper.Clamp((int)((color1.R * (1 - factor)) + (color2.R * factor)));
                var g = ImageHelper.Clamp((int)((color1.G * (1 - factor)) + (color2.G * factor)));
                var b = ImageHelper.Clamp((int)((color1.B * (1 - factor)) + (color2.B * factor)));
                return Color.FromArgb(r, g, b);
            });
        }

        /// <summary>
        ///     Finds the minimum color values from two images.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>A bitmap resulting from the minimum values of the two images, or null if an error occurs.</returns>
        public static Bitmap MinImages(Image imgOne, Image imgTwo)
        {
            return ProcessImages(imgOne, imgTwo, (color1, color2) =>
            {
                var r = Math.Min(color1.R, color2.R);
                var g = Math.Min(color1.G, color2.G);
                var b = Math.Min(color1.B, color2.B);
                return Color.FromArgb(r, g, b);
            });
        }

        /// <summary>
        ///     Finds the maximum color values from two images.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>A bitmap resulting from the maximum values of the two images, or null if an error occurs.</returns>
        public static Bitmap MaxImages(Image imgOne, Image imgTwo)
        {
            return ProcessImages(imgOne, imgTwo, (color1, color2) =>
            {
                var r = Math.Max(color1.R, color2.R);
                var g = Math.Max(color1.G, color2.G);
                var b = Math.Max(color1.B, color2.B);
                return Color.FromArgb(r, g, b);
            });
        }

        /// <summary>
        ///     Calculates the amplitude of the pixel values between two images.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>A bitmap resulting from the amplitude of the two images, or null if an error occurs.</returns>
        public static Bitmap AmplitudeImages(Image imgOne, Image imgTwo)
        {
            return ProcessImages(imgOne, imgTwo, (color1, color2) =>
            {
                var r = (int)Math.Sqrt((color1.R * color1.R) + (color2.R * color2.R));
                var g = (int)Math.Sqrt((color1.G * color1.G) + (color2.G * color2.G));
                var b = (int)Math.Sqrt((color1.B * color1.B) + (color2.B * color2.B));
                return Color.FromArgb(ImageHelper.Clamp(r), ImageHelper.Clamp(g), ImageHelper.Clamp(b));
            });
        }

        /// <summary>
        ///     Processes two images with a specified pixel operation.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <param name="pixelOperation">A function that defines how to combine the pixel values.</param>
        /// <returns>A bitmap resulting from applying the pixel operation, or null if an error occurs.</returns>
        private static Bitmap ProcessImages(Image imgOne, Image imgTwo, Func<Color, Color, Color> pixelOperation)
        {
            if (imgOne.Width != imgTwo.Width || imgOne.Height != imgTwo.Height)
            {
                throw new ArgumentException("All images must have the same dimensions.");
            }

            var width = imgOne.Width;
            var height = imgOne.Height;

            var result = new DirectBitmap(width, height);
            var pixelsToSet = new List<(int x, int y, Color color)>();

            using (var dbmOne = new DirectBitmap(imgOne))
            using (var dbmTwo = new DirectBitmap(imgTwo))
            {
                for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    var color1 = dbmOne.GetPixel(x, y);
                    var color2 = dbmTwo.GetPixel(x, y);

                    // Use the provided pixel operation function
                    var resultColor = pixelOperation(color1, color2);

                    pixelsToSet.Add((x, y, resultColor));
                }
            }

            try
            {
                result.SetPixelsSimd(pixelsToSet);
                return result.Bitmap;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{ImagingResources.ErrorPixel} {ex.Message}");
                return null;
            }
        }
    }
}
