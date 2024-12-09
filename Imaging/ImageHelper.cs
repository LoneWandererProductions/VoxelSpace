/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageHelper.cs
 * PURPOSE:     Here I try to minimize the footprint of my class and pool all shared methods
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://en.wikipedia.org/wiki/Midpoint_circle_algorithm
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace Imaging
{
    /// <summary>
    ///     Helper methods for image processing.
    /// </summary>
    internal static class ImageHelper
    {
        /// <summary>
        ///     Gets all points in a Circle.
        ///     Uses the  Bresenham's circle drawing algorithm.
        ///     https://en.wikipedia.org/wiki/Midpoint_circle_algorithm
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="length">The length.</param>
        /// <param name="width">The height.</param>
        /// <returns>List of Points</returns>
        internal static List<Point> GetCirclePoints(Point center, int radius, int length, int width)
        {
            var points = new List<Point>();

            for (var x = Math.Max(0, center.X - radius); x <= Math.Min(width - 1, center.X + radius); x++)
            {
                var dx = x - center.X;
                var height = (int)Math.Sqrt(radius * radius - dx * dx);

                for (var y = Math.Max(0, center.Y - height); y <= Math.Min(length - 1, center.Y + height); y++)
                    points.Add(new Point(x, y));
            }

            return points;
        }

        /// <summary>
        ///     Generates a Gaussian kernel.
        /// </summary>
        /// <param name="sigma">The sigma value for the Gaussian distribution.</param>
        /// <param name="size">The size of the kernel (must be odd).</param>
        /// <returns>A 2D array representing the Gaussian kernel.</returns>
        internal static double[,] GenerateGaussianKernel(double sigma, int size)
        {
            var kernel = new double[size, size];
            var mean = size / 2.0;
            var sum = 0.0;

            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                kernel[y, x] =
                    Math.Exp(-0.5 * (Math.Pow((x - mean) / sigma, 2.0) + Math.Pow((y - mean) / sigma, 2.0)))
                    / (2 * Math.PI * sigma * sigma);
                sum += kernel[y, x];
            }

            // Normalize the kernel
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
                kernel[y, x] /= sum;

            return kernel;
        }

        /// <summary>
        ///     Gets the color of the region pixels and the mean color.
        /// </summary>
        /// <param name="dbm">The DirectBitmap base.</param>
        /// <param name="region">The region to process.</param>
        /// <returns>A tuple containing a list of colors and the mean color.</returns>
        internal static (List<Color> Pixels, Color Mean) GetRegionPixelsAndMeanColor(DirectBitmap dbm, Rectangle region)
        {
            var (pixels, meanColor) = ProcessPixels(dbm, region);
            return (pixels, meanColor ?? Color.Black); // Ensure a non-null value for meanColor
        }

        /// <summary>
        ///     Gets the mean color of a specified region.
        /// </summary>
        /// <param name="dbm">The DirectBitmap base.</param>
        /// <param name="region">The region to process.</param>
        /// <returns>The mean color of the specified region.</returns>
        internal static Color GetMeanColor(DirectBitmap dbm, Rectangle region)
        {
            var processPixels = ProcessPixels(dbm, region);
            return processPixels.Mean ?? Color.Black; // Handle case where meanColor is null
        }

        /// <summary>
        ///     Gets the non transparent bounds.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Area as rectangle that is not transparent anymore</returns>
        internal static Rectangle GetNonTransparentBounds(Bitmap image)
        {
            var minX = image.Width;
            var maxX = 0;
            var minY = image.Height;
            var maxY = 0;

            var hasNonTransparentPixel = false;

            for (var y = 0; y < image.Height; y++)
            for (var x = 0; x < image.Width; x++)
            {
                var pixel = image.GetPixel(x, y);
                if (pixel.A != 0) // Not fully transparent
                {
                    hasNonTransparentPixel = true;
                    if (x < minX) minX = x;

                    if (x > maxX) maxX = x;

                    if (y < minY) minY = y;

                    if (y > maxY) maxY = y;
                }
            }

            // If all pixels are transparent, return a zero-sized rectangle
            return !hasNonTransparentPixel
                ? new Rectangle(0, 0, 0, 0)
                : new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        /// <summary>
        ///     Handles the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <exception cref="System.ApplicationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void HandleException(Exception ex)
        {
            // Log the exception details (implementation may vary)
            Trace.WriteLine($"Exception Type: {ex.GetType().Name}");
            Trace.WriteLine($"Message: {ex.Message}");
            Trace.WriteLine($"Stack Trace: {ex.StackTrace}");

            // Optionally, rethrow or handle further
            if (ex is ArgumentException or InvalidOperationException or NotSupportedException or UriFormatException
                or IOException)
                throw new ApplicationException("An error occurred while processing the image.", ex);
        }

        /// <summary>
        ///     Validates the image.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="image">The image.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ValidateImage(string method, Bitmap image)
        {
            if (image != null) return;

            var innerException =
                new ArgumentNullException(string.Concat(method, ImagingResources.Spacing, nameof(image)));
            throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
        }

        /// <summary>
        ///     Validates the BitmapImage.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="image">The BitmapImage.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ValidateImage(string method, BitmapImage image)
        {
            if (image != null) return;

            var innerException =
                new ArgumentNullException(string.Concat(method, ImagingResources.Spacing, nameof(image)));
            throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
        }

        /// <summary>
        ///     Retrieves pixel colors from a region and optionally calculates the mean color.
        /// </summary>
        /// <param name="dbmBase">The DirectBitmap base.</param>
        /// <param name="region">The region to process.</param>
        /// <param name="calculateMeanColor">If true, calculates the mean color. Defaults to true.</param>
        /// <returns>A tuple containing a list of colors and optionally the mean color.</returns>
        private static (List<Color> Pixels, Color? Mean) ProcessPixels(DirectBitmap dbmBase, Rectangle region,
            bool calculateMeanColor = true)
        {
            var pixels = new List<Color>();
            int rSum = 0, gSum = 0, bSum = 0;
            var count = 0;

            for (var y = region.Top; y < region.Bottom; y++)
            for (var x = region.Left; x < region.Right; x++)
            {
                var pixel = dbmBase.GetPixel(x, y);
                pixels.Add(pixel);
                rSum += pixel.R;
                gSum += pixel.G;
                bSum += pixel.B;
                count++;
            }

            if (!calculateMeanColor || count <= 0) return (pixels, null);

            var averageRed = rSum / count;
            var averageGreen = gSum / count;
            var averageBlue = bSum / count;
            Color? meanColor = Color.FromArgb(averageRed, averageGreen, averageBlue);

            return (pixels, meanColor);
        }

        /// <summary>
        ///     Interpolates the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <param name="t">The t.</param>
        /// <returns>Interpolation</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Interpolate(double a, double b, double t)
        {
            return a * (1 - t) + b * t;
        }

        /// <summary>
        ///     Clamps the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Clamp(double value, double min = 0, double max = 255)
        {
            return (int)Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        ///     Validates the file path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="System.IO.IOException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ValidateFilePath(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorFileNotFound, innerException);
            }
        }

        /// <summary>
        ///     Validates the parameters.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="alpha">The alpha.</param>
        /// <exception cref="ArgumentException">
        ///     minValue and maxValue must be between 0 and 255, and minValue must not be greater than maxValue.
        ///     or
        ///     Alpha must be between 0 and 255.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ValidateParameters(int minValue, int maxValue, int alpha)
        {
            if (minValue is < 0 or > 255 || maxValue is < 0 or > 255 || minValue > maxValue)
                throw new ArgumentException(
                    ImagingResources.ErrorColorRange);

            if (alpha is < 0 or > 255) throw new ArgumentException(ImagingResources.ErrorColorRange);
        }
    }
}