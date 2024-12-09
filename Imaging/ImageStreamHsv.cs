/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     Imaging
* FILE:        Imaging/ColorHsv.cs
* PURPOSE:     General Conversions for images via my Hsv class
* PROGRAMER:   Peter Geinitz (Wayfarer)
* SOURCE:      https://lodev.org/cgtutor/color.html#Color_Model_Conversions_
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Imaging
{
    /// <summary>
    ///     Another round of minor Image manipulations
    /// </summary>
    internal static class ImageStreamHsv
    {
        /// <summary>
        ///     Adjusts the hue.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="hueShift">The hue shift.</param>
        /// <returns>Processed Image</returns>
        internal static Bitmap AdjustHue(Bitmap image, double hueShift)
        {
            return ProcessImage(image, colorHsv =>
            {
                // Adjust hue
                colorHsv.H = (colorHsv.H + hueShift) % 360;
                colorHsv.H = colorHsv.H < 0 ? colorHsv.H + 360 : colorHsv.H;
            });
        }

        /// <summary>
        ///     Adjusts the saturation.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="saturationFactor">The saturation factor.</param>
        /// <returns>Processed Image</returns>
        internal static Bitmap AdjustSaturation(Bitmap image, double saturationFactor)
        {
            return ProcessImage(image, colorHsv => colorHsv.S = ImageHelper.Clamp(colorHsv.S * saturationFactor, 0, 1));
        }

        /// <summary>
        ///     Adjusts the brightness.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="brightnessFactor">The brightness factor.</param>
        /// <returns>Processed Image</returns>
        internal static Bitmap AdjustBrightness(Bitmap image, double brightnessFactor)
        {
            return ProcessImage(image, colorHsv => colorHsv.V = ImageHelper.Clamp(colorHsv.V * brightnessFactor, 0, 1));
        }

        /// <summary>
        ///     Applies the gamma correction.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="gamma">The gamma.</param>
        /// <returns>Processed Image</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">gamma - Gamma must be greater than 0.</exception>
        internal static Bitmap ApplyGammaCorrection(Bitmap image, double gamma)
        {
            if (gamma <= 0) throw new ArgumentOutOfRangeException(nameof(gamma), "Gamma must be greater than 0.");

            var gammaCorrection = 1.0 / gamma;
            return ProcessImage(image, colorHsv =>
            {
                colorHsv.R = ImageHelper.Clamp(Math.Pow(colorHsv.R / 255.0, gammaCorrection) * 255);
                colorHsv.G = ImageHelper.Clamp(Math.Pow(colorHsv.G / 255.0, gammaCorrection) * 255);
                colorHsv.B = ImageHelper.Clamp(Math.Pow(colorHsv.B / 255.0, gammaCorrection) * 255);
            });
        }

        /// <summary>
        ///     Processes the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="pixelOperation">The pixel operation.</param>
        /// <returns>Processed Image</returns>
        private static Bitmap ProcessImage(Bitmap image, Action<ColorHsv> pixelOperation)
        {
            ImageHelper.ValidateImage(nameof(ProcessImage), image);

            var source = new DirectBitmap(image);
            var result = new DirectBitmap(source.Width, source.Height);
            var pixelsToSet = new List<(int x, int y, Color color)>();

            for (var y = 0; y < source.Height; y++)
            for (var x = 0; x < source.Width; x++)
            {
                var pixelColor = source.GetPixel(x, y);
                var colorHsv = new ColorHsv(pixelColor.R, pixelColor.G, pixelColor.B, pixelColor.A);

                // Apply the pixel operation (adjustments)
                pixelOperation(colorHsv);

                // Add modified pixel to the list
                pixelsToSet.Add((x, y, Color.FromArgb(colorHsv.A, colorHsv.R, colorHsv.G, colorHsv.B)));
            }

            return ApplyPixelChanges(result, pixelsToSet);
        }

        /// <summary>
        ///     Applies the pixel changes.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="pixelsToSet">The pixels to set.</param>
        /// <returns>Processed Image</returns>
        private static Bitmap ApplyPixelChanges(DirectBitmap result, List<(int x, int y, Color color)> pixelsToSet)
        {
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