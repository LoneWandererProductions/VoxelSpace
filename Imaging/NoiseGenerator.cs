/*
 * COPYRIGHT:   See COPYING in the top-level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/NoiseGenerator.cs
 * PURPOSE:     Provides noise generation utilities for procedural texture generation.
 * AUTHOR:      Peter Geinitz (Wayfarer)
 */

using System;

namespace Imaging
{
    /// <summary>
    ///     Generates procedural noise for texture generation.
    /// </summary>
    internal sealed class NoiseGenerator
    {
        /// <summary>
        ///     The height of the noise map.
        /// </summary>
        private readonly int _height;

        /// <summary>
        ///     A 2D array storing precomputed noise values.
        /// </summary>
        private readonly double[,] _noise;

        /// <summary>
        ///     The width of the noise map.
        /// </summary>
        private readonly int _width;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NoiseGenerator" /> class.
        ///     Generates a 2D noise map with random values.
        /// </summary>
        /// <param name="width">The width of the noise map.</param>
        /// <param name="height">The height of the noise map.</param>
        public NoiseGenerator(int width, int height)
        {
            _width = width;
            _height = height;
            _noise = GenerateNoise();
        }

        /// <summary>
        ///     Generates a 2D array filled with random noise values between 0 and 1.
        /// </summary>
        /// <returns>A 2D array representing the noise map.</returns>
        private double[,] GenerateNoise()
        {
            var rand = new Random();
            var noiseData = new double[_height, _width];

            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    noiseData[y, x] =
                        rand.NextDouble(); // Assign a random value between 0 and 1random.NextDouble(); // Random value between 0.0 and 1.0
                }
            }

            return noiseData;
        }

        /// <summary>
        ///     Retrieves the noise value at a given coordinate.
        ///     Performs wrapping to ensure seamless tiling.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>A noise value between 0 and 1.</returns>
        public double GetNoise(int x, int y)
        {
            x = (x + _width) % _width; // Wrap around horizontally
            y = (y + _height) % _height; // Wrap around vertically
            return _noise[y, x];
        }

        /// <summary>
        ///     Computes turbulence noise by layering multiple octaves of smooth noise.
        ///     Used to create more complex and natural-looking textures.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="size">The initial size of the noise layers (higher values create larger patterns).</param>
        /// <returns>A turbulence value that enhances visual texture variation.</returns>
        public double Turbulence(int x, int y, double size)
        {
            double value = 0.0, initialSize = size;

            while (size >= 1)
            {
                value += SmoothNoise(x / size, y / size) * size;
                size /= 2.0;
            }

            return 128.0 * value / initialSize;
        }

        /// <summary>
        ///     Computes smooth noise using bilinear interpolation of four neighboring noise values.
        ///     Helps reduce sharp transitions between noise values, creating a smoother effect.
        /// </summary>
        /// <param name="x">The x-coordinate (floating-point for interpolation).</param>
        /// <param name="y">The y-coordinate (floating-point for interpolation).</param>
        /// <returns>A smoothed noise value between 0 and 1.</returns>
        public double SmoothNoise(double x, double y)
        {
            var xInt = (int)x;
            var yInt = (int)y;

            var xFrac = x - xInt;
            var yFrac = y - yInt;

            // Retrieve noise values from surrounding grid points
            var v1 = GetNoise(xInt, yInt);
            var v2 = GetNoise(xInt + 1, yInt);
            var v3 = GetNoise(xInt, yInt + 1);
            var v4 = GetNoise(xInt + 1, yInt + 1);

            // Bilinear interpolation
            var i1 = ImageHelper.Interpolate(v1, v2, xFrac);
            var i2 = ImageHelper.Interpolate(v3, v4, xFrac);

            return ImageHelper.Interpolate(i1, i2, yFrac);
        }
    }
}
