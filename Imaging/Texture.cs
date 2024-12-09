/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/Texture.cs
 * PURPOSE:     Basic stuff for generating textures
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * Sources:     https://lodev.org/cgtutor/randomnoise.html
 */

using System;
using System.Collections.Generic;
using System.Drawing;

// ReSharper disable UnusedMember.Local

namespace Imaging
{
    /// <summary>
    ///     Class that generates Textures
    /// </summary>
    internal static class Texture
    {
        /// <summary>
        ///     The noise width
        /// </summary>
        private const int NoiseWidth = 320; // Width for noise generation

        /// <summary>
        ///     The noise height
        /// </summary>
        private const int NoiseHeight = 240; // Height for noise generation

        /// <summary>
        ///     The noise
        /// </summary>
        private static readonly double[,] Noise = new double[NoiseHeight, NoiseWidth];

        /// <summary>
        ///     Generates the noise bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="useSmoothNoise">if set to <c>true</c> [use smooth noise].</param>
        /// <param name="useTurbulence">if set to <c>true</c> [use turbulence].</param>
        /// <param name="turbulenceSize">Size of the turbulence.</param>
        /// <returns>Texture Bitmap</returns>
        internal static Bitmap GenerateNoiseBitmap(
            int width,
            int height,
            int minValue = 0,
            int maxValue = 255,
            int alpha = 255,
            bool useSmoothNoise = false,
            bool useTurbulence = false,
            double turbulenceSize = 64)
        {
            // Validate parameters
            ImageHelper.ValidateParameters(minValue, maxValue, alpha);

            // Generate base noise
            GenerateBaseNoise();

            // Create DirectBitmap
            var noiseBitmap = new DirectBitmap(width, height);

            // Create an enumerable to collect pixel data
            var pixelData = new List<(int x, int y, Color color)>();

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                double value;

                if (useTurbulence)
                    value = Turbulence(x, y, turbulenceSize);
                else if (useSmoothNoise)
                    value = SmoothNoise(x, y);
                else
                    value = Noise[y % NoiseHeight, x % NoiseWidth];

                var colorValue = minValue + (int)((maxValue - minValue) * value);
                colorValue = Math.Max(minValue, Math.Min(maxValue, colorValue));
                var color = Color.FromArgb(alpha, colorValue, colorValue, colorValue);

                pixelData.Add((x, y, color));
            }

            // Set pixels using SIMD
            noiseBitmap.SetPixelsSimd(pixelData);

            return noiseBitmap.Bitmap;
        }

        /// <summary>
        ///     Generates the clouds bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="turbulenceSize">Size of the turbulence.</param>
        /// <returns>Texture Bitmap</returns>
        internal static Bitmap GenerateCloudsBitmap(
            int width,
            int height,
            int minValue = 0,
            int maxValue = 255,
            int alpha = 255,
            double turbulenceSize = 64)
        {
            ImageHelper.ValidateParameters(minValue, maxValue, alpha);
            GenerateBaseNoise();

            var cloudsBitmap = new DirectBitmap(width, height);
            var pixelData = new List<(int x, int y, Color color)>();

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var value = Turbulence(x, y, turbulenceSize);
                var l = Math.Clamp(minValue + (int)(value / 4.0), minValue, maxValue);
                var color = Color.FromArgb(alpha, l, l, l);
                pixelData.Add((x, y, color));
            }

            // Convert list to array for SIMD processing
            var pixelArray = pixelData.ToArray();
            cloudsBitmap.SetPixelsSimd(pixelArray);
            pixelData.Clear();

            return cloudsBitmap.Bitmap;
        }

        /// <summary>
        ///     Generates the marble bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="xPeriod">The x period.</param>
        /// <param name="yPeriod">The y period.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="turbulencePower">The turbulence power.</param>
        /// <param name="turbulenceSize">Size of the turbulence.</param>
        /// <param name="baseColor">Color of the base.</param>
        /// <returns>
        ///     Texture Bitmap
        /// </returns>
        internal static Bitmap GenerateMarbleBitmap(
            int width,
            int height,
            double xPeriod = 5.0,
            double yPeriod = 10.0,
            int alpha = 255,
            double turbulencePower = 5.0,
            double turbulenceSize = 32.0,
            Color baseColor = default)
        {
            baseColor = baseColor == default ? Color.FromArgb(30, 10, 0) : baseColor;
            GenerateBaseNoise();

            var marbleBitmap = new DirectBitmap(width, height);
            var pixelData = new List<(int x, int y, Color color)>();

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var xyValue = x * xPeriod / NoiseWidth + y * yPeriod / NoiseHeight +
                              turbulencePower * Turbulence(x, y, turbulenceSize) / 256.0;
                var sineValue = 226 * Math.Abs(Math.Sin(xyValue * Math.PI));
                var r = Math.Clamp(baseColor.R + (int)sineValue, 0, 255);
                var g = Math.Clamp(baseColor.G + (int)sineValue, 0, 255);
                var b = Math.Clamp(baseColor.B + (int)sineValue, 0, 255);

                var color = Color.FromArgb(alpha, r, g, b);
                pixelData.Add((x, y, color));
            }

            // Convert list to array for SIMD processing
            var pixelArray = pixelData.ToArray();
            marbleBitmap.SetPixelsSimd(pixelArray);
            pixelData.Clear();

            return marbleBitmap.Bitmap;
        }

        /// <summary>
        ///     Generates the wood bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="xyPeriod">The xy period.</param>
        /// <param name="turbulencePower">The turbulence power.</param>
        /// <param name="turbulenceSize">Size of the turbulence.</param>
        /// <param name="baseColor">Color of the base.</param>
        /// <returns>
        ///     Texture Bitmap
        /// </returns>
        internal static Bitmap GenerateWoodBitmap(
            int width,
            int height,
            int alpha = 255,
            double xyPeriod = 12.0,
            double turbulencePower = 0.1,
            double turbulenceSize = 32.0,
            Color baseColor = default)
        {
            baseColor = baseColor == default ? Color.FromArgb(80, 30, 30) : baseColor;
            GenerateBaseNoise();

            var woodBitmap = new DirectBitmap(width, height);
            var pixelData = new List<(int x, int y, Color color)>();

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var xValue = (x - width / 2.0) / width;
                var yValue = (y - height / 2.0) / height;
                var distValue = Math.Sqrt(xValue * xValue + yValue * yValue) +
                                turbulencePower * Turbulence(x, y, turbulenceSize) / 256.0;
                var sineValue = 128.0 * Math.Abs(Math.Sin(2 * xyPeriod * distValue * Math.PI));

                var r = Math.Clamp(baseColor.R + (int)sineValue, 0, 255);
                var g = Math.Clamp(baseColor.G + (int)sineValue, 0, 255);
                var b = Math.Clamp((int)baseColor.B, 0, 255);

                var color = Color.FromArgb(alpha, r, g, b);
                pixelData.Add((x, y, color));
            }

            // Convert list to array for SIMD processing
            var pixelArray = pixelData.ToArray();
            woodBitmap.SetPixelsSimd(pixelArray);
            pixelData.Clear();

            return woodBitmap.Bitmap;
        }

        /// <summary>
        ///     Generates the wave bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="xyPeriod">The xy period.</param>
        /// <param name="turbulencePower">The turbulence power.</param>
        /// <param name="turbulenceSize">Size of the turbulence.</param>
        /// <returns>
        ///     Texture Bitmap
        /// </returns>
        internal static Bitmap GenerateWaveBitmap(
            int width,
            int height,
            int alpha = 255,
            double xyPeriod = 12.0,
            double turbulencePower = 0.1,
            double turbulenceSize = 32.0)
        {
            GenerateBaseNoise();

            var waveBitmap = new DirectBitmap(width, height);
            var pixelData = new List<(int x, int y, Color color)>();

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var turbulenceValue = Turbulence(x, y, turbulenceSize);
                var xValue = (x - width / 2.0) / width + turbulencePower * turbulenceValue / 256.0;
                var yValue = (y - height / 2.0) / height +
                             turbulencePower * Turbulence(height - y, width - x, turbulenceSize) / 256.0;

                var sineValue = 22.0 *
                                Math.Abs(Math.Sin(xyPeriod * xValue * Math.PI) +
                                         Math.Sin(xyPeriod * yValue * Math.PI));
                var hsvColor = new ColorHsv(sineValue, 1.0, 1.0, alpha);

                pixelData.Add((x, y, hsvColor.GetDrawingColor()));
            }

            // Convert list to array for SIMD processing
            var pixelArray = pixelData.ToArray();
            waveBitmap.SetPixelsSimd(pixelArray);
            pixelData.Clear();

            return waveBitmap.Bitmap;
        }

        /// <summary>
        ///     Generates a crosshatch texture bitmap.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="lineSpacing">The spacing between lines.</param>
        /// <param name="lineColor">The color of the lines.</param>
        /// <param name="lineThickness">The thickness of the lines.</param>
        /// <param name="angle1">The angle of the first set of lines, in degrees.</param>
        /// <param name="angle2">The angle of the second set of lines, in degrees.</param>
        /// <param name="alpha">The alpha value for the color.</param>
        /// <returns>Texture Bitmap</returns>
        internal static Bitmap GenerateCrosshatchBitmap(
            int width,
            int height,
            int lineSpacing = 10,
            Color lineColor = default,
            int lineThickness = 1,
            double angle1 = 45.0,
            double angle2 = 135.0,
            int alpha = 255)
        {
            lineColor = lineColor == default ? Color.Black : lineColor;

            var crosshatchBitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(crosshatchBitmap);
            graphics.Clear(Color.White); // Background color

            using var pen = new Pen(Color.FromArgb(alpha, lineColor), lineThickness);
            // Convert angles from degrees to radians
            var radAngle1 = angle1 * Math.PI / 180.0;
            var radAngle2 = angle2 * Math.PI / 180.0;

            // Calculate the line direction vectors
            var dx1 = Math.Cos(radAngle1);
            var dy1 = Math.Sin(radAngle1);
            var dx2 = Math.Cos(radAngle2);
            var dy2 = Math.Sin(radAngle2);

            // Draw first set of lines
            for (var y = 0; y < height; y += lineSpacing)
                graphics.DrawLine(
                    pen,
                    0, y,
                    (int)(width * dx1 + width * dy1),
                    (int)(y * dx1 + width * dy1));

            for (var x = 0; x < width; x += lineSpacing)
                graphics.DrawLine(
                    pen,
                    x, 0,
                    (int)(x * dx1 + height * dx1),
                    (int)(height * dx1 + height * dy1));

            // Draw second set of lines
            for (var y = 0; y < height; y += lineSpacing)
                graphics.DrawLine(
                    pen,
                    0, y,
                    (int)(width * dx2 + height * dy2),
                    (int)(y * dx2 + height * dy2));

            for (var x = 0; x < width; x += lineSpacing)
                graphics.DrawLine(
                    pen,
                    x, 0,
                    (int)(x * dx2 + width * dx2),
                    (int)(width * dx2 + height * dy2));

            return crosshatchBitmap;
        }

        //TODO wire in the two Textures, add the settings to the config menu

        /// <summary>
        ///     Generates a concrete texture bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="minValue">The minimum grayscale value.</param>
        /// <param name="maxValue">The maximum grayscale value.</param>
        /// <param name="alpha">The alpha transparency level.</param>
        /// <param name="turbulenceSize">Size of the turbulence.</param>
        /// <returns>Concrete Texture Bitmap</returns>
        internal static Bitmap GenerateConcreteBitmap(
            int width,
            int height,
            int minValue = 50,
            int maxValue = 200,
            int alpha = 255,
            double turbulenceSize = 16)
        {
            ImageHelper.ValidateParameters(minValue, maxValue, alpha);
            GenerateBaseNoise();

            var concreteBitmap = new DirectBitmap(width, height);
            var pixelData = new List<(int x, int y, Color color)>();

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                // Turbulence creates irregular, rough texture
                var noiseValue = Turbulence(x, y, turbulenceSize);
                var colorValue = minValue + (int)((maxValue - minValue) * noiseValue);

                // Add minor random variation for a "gritty" look
                colorValue = Math.Clamp(colorValue + RandomVariation(-15, 15), minValue, maxValue);

                var color = Color.FromArgb(alpha, colorValue, colorValue, colorValue);
                pixelData.Add((x, y, color));
            }

            concreteBitmap.SetPixelsSimd(pixelData.ToArray());
            return concreteBitmap.Bitmap;
        }

        /// <summary>
        ///     Generates a canvas texture bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="lineSpacing">The spacing between fibers.</param>
        /// <param name="lineColor">The color of the fibers.</param>
        /// <param name="lineThickness">The thickness of the fibers.</param>
        /// <param name="alpha">The alpha transparency level.</param>
        /// <returns>Canvas Texture Bitmap</returns>
        internal static Bitmap GenerateCanvasBitmap(
            int width,
            int height,
            int lineSpacing = 8,
            Color lineColor = default,
            int lineThickness = 1,
            int alpha = 255)
        {
            var canvasBitmap = new DirectBitmap(width, height);


            //no Simd for now
            using (var g = Graphics.FromImage(canvasBitmap.Bitmap))
            {
                g.Clear(Color.White);

                // Draw vertical fibers
                for (var x = 0; x < width; x += lineSpacing)
                    using (var fiberBrush = new SolidBrush(Color.FromArgb(alpha, lineColor)))
                    {
                        g.FillRectangle(fiberBrush, x, 0, lineThickness, height);
                    }

                // Draw horizontal fibers
                for (var y = 0; y < height; y += lineSpacing)
                    using (var fiberBrush = new SolidBrush(Color.FromArgb(alpha, lineColor)))
                    {
                        g.FillRectangle(fiberBrush, 0, y, width, lineThickness);
                    }
            }

            return canvasBitmap.Bitmap;
        }


        /// <summary>
        ///     Generates the base noise.
        /// </summary>
        private static void GenerateBaseNoise()
        {
            var random = new Random();
            for (var y = 0; y < NoiseHeight; y++)
            for (var x = 0; x < NoiseWidth; x++)
                Noise[y, x] = random.NextDouble(); // Random value between 0.0 and 1.0
        }

        /// <summary>
        ///     Turbulence the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="size">The size.</param>
        /// <returns>Generate Turbulence</returns>
        private static double Turbulence(int x, int y, double size)
        {
            var value = 0.0;
            var initialSize = size;
            while (size >= 1)
            {
                value += SmoothNoise(x / size, y / size) * size;
                size /= 2;
            }

            return Math.Abs(value / initialSize);
        }

        /// <summary>
        ///     Smooths the noise.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Generate Noise</returns>
        private static double SmoothNoise(double x, double y)
        {
            var intX = (int)x;
            var fracX = x - intX;
            var intY = (int)y;
            var fracY = y - intY;

            var v1 = Noise[intY % NoiseHeight, intX % NoiseWidth];
            var v2 = Noise[intY % NoiseHeight, (intX + 1) % NoiseWidth];
            var v3 = Noise[(intY + 1) % NoiseHeight, intX % NoiseWidth];
            var v4 = Noise[(intY + 1) % NoiseHeight, (intX + 1) % NoiseWidth];

            var i1 = ImageHelper.Interpolate(v1, v2, fracX);
            var i2 = ImageHelper.Interpolate(v3, v4, fracX);

            return ImageHelper.Interpolate(i1, i2, fracY);
        }

        /// <summary>
        ///     Adds a minor random variation to an integer value within a given range.
        /// </summary>
        private static int RandomVariation(int min, int max)
        {
            var rand = new Random();
            return rand.Next(min, max);
        }
    }
}