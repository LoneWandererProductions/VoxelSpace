﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Imaging;
using Mathematics;

namespace Voxels
{
    internal sealed class Raster : IDisposable
    {
        // At the class level, define the Pen and SolidBrush (reuse them later).

        /// <summary>
        ///     The line pen
        /// </summary>
        private readonly Pen _linePen;

        /// <summary>
        ///     The solid brush
        /// </summary>
        private SolidBrush _solidBrush;

        private bool _disposed;

        /// <summary>
        ///     The y buffer
        /// </summary>
        private float[] _yBuffer;

        public Raster()
        {
            // Initialize the Pen and SolidBrush once.
            _linePen = new Pen(Color.Black); // Adjust the color as needed
            _solidBrush = new SolidBrush(Color.Black); // Adjust the color as needed
        }

        public void Dispose()
        {
            // Call the Dispose method with true to release resources.
            Dispose(true);
            // Suppress finalization to avoid it being called twice.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Generates the raster.
        /// </summary>
        /// <param name="colorMap">The color map.</param>
        /// <param name="heightMap">The height map.</param>
        /// <param name="camera">The camera.</param>
        /// <param name="topographyHeight">Height of the topography.</param>
        /// <param name="topographyWidth">Width of the topography.</param>
        /// <param name="colorHeight">Height of the color.</param>
        /// <param name="colorWidth">Width of the color.</param>
        /// <returns>
        ///     Finished Bitmap
        /// </returns>
        /// <summary>
        ///     Renders the image directly onto the Bitmap.
        /// </summary>
        /// <returns>The finished Bitmap created directly.</returns>
        public Bitmap RenderImmediate(Color[,] colorMap, int[,] heightMap, Camera camera,
            int topographyHeight, int topographyWidth, int colorHeight, int colorWidth)
        {
            if (heightMap == null) return null;

            var bmp = new Bitmap(camera.ScreenWidth, camera.ScreenHeight);

            _yBuffer = new float[camera.ScreenWidth];

            // Initialize the y-buffer to the screen height
            for (var i = 0; i < _yBuffer.Length; i++)
                _yBuffer[i] = camera.ScreenHeight;

            // Precompute yaw (left-right rotation)
            var sinPhi = ExtendedMath.CalcSin(camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(camera.Angle);

            // Precompute pitch (up-down rotation)
            var sinTheta = ExtendedMath.CalcSin(camera.Pitch);
            var cosTheta = ExtendedMath.CalcCos(camera.Pitch);

            float z = 1;
            float dz = 1;

            while (z < camera.ZFar)
            {
                // Horizontal line start (left and right positions) based on yaw
                var pLeft = new PointF(
                    (float)(-cosPhi * z - sinPhi * z) + camera.X,
                    (float)(-sinPhi * z + cosPhi * z) + camera.Y
                );

                var pRight = new PointF(
                    (float)(cosPhi * z - sinPhi * z) + camera.X,
                    (float)(sinPhi * z + cosPhi * z) + camera.Y
                );

                // Step size for each pixel column
                var dx = (pRight.X - pLeft.X) / camera.ScreenWidth;
                var dy = (pRight.Y - pLeft.Y) / camera.ScreenWidth;

                for (var i = 0; i < camera.ScreenWidth; i++)
                {
                    var diffuseX = (int)pLeft.X;
                    var diffuseY = (int)pLeft.Y;
                    var heightX = (int)pLeft.X;
                    var heightY = (int)pLeft.Y;

                    // Wrap around height and color map indices
                    var heightOfHeightMap = heightMap[heightX & (topographyWidth - 1), heightY & (topographyHeight - 1)];
                    var color = colorMap[diffuseX & (colorWidth - 1), diffuseY & (colorHeight - 1)];

                    // Adjust the height on screen based on pitch
                    var heightOnScreen = (float)((camera.Height - heightOfHeightMap) * cosTheta - z * sinTheta) / z
                                         * camera.Scale + camera.Horizon;

                    // Draw the vertical column
                    DrawVerticalLine(color, i, (int)heightOnScreen, (int)_yBuffer[i], bmp);

                    // Update the y-buffer
                    if (heightOnScreen < _yBuffer[i])
                        _yBuffer[i] = heightOnScreen;

                    // Move to the next pixel column
                    pLeft.X += dx;
                    pLeft.Y += dy;
                }

                // Increase z (depth) and dz (step increment)
                z += dz;
                dz += 0.005f;
            }

            return bmp;
        }

        /// <summary>
        ///     Draws the vertical line.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="x">The x.</param>
        /// <param name="heightOnScreen">The height on screen.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bmp">The bitmap we draw on</param>
        private void DrawVerticalLine(Color col, int x, int heightOnScreen, float buffer, Bitmap bmp)
        {
            if (heightOnScreen > buffer) return;

            var lineHeight = (int) (buffer - heightOnScreen);
            var rect = new Rectangle(x, heightOnScreen, 1, lineHeight);

            _solidBrush = new SolidBrush(col); // Update the brush color
            using var g = Graphics.FromImage(bmp);
            g.FillRectangle(_solidBrush, rect);
        }


        /// <summary>
        ///     Creates the bitmap with depth buffer.
        /// </summary>
        /// <param name="colorMap">The color map.</param>
        /// <param name="heightMap">The height map.</param>
        /// <param name="camera">The camera.</param>
        /// <param name="topographyHeight">Height of the topography.</param>
        /// <param name="topographyWidth">Width of the topography.</param>
        /// <param name="colorHeight">Height of the color.</param>
        /// <param name="colorWidth">Width of the color.</param>
        /// <returns>
        ///     Finished Bitmap
        /// </returns>
        internal Bitmap RenderWithDepthBuffer(Color[,] colorMap, int[,] heightMap, Camera camera,
            int topographyHeight, int topographyWidth, int colorHeight, int colorWidth)
        {
            var bmp = new Bitmap(camera.ScreenWidth, camera.ScreenHeight);
            var dbm = new DirectBitmap(bmp);

            // Create a 1D depth buffer using Span<T>
            var depthBuffer = new float[camera.ScreenWidth * camera.ScreenHeight];
            var depthBufferSpan = depthBuffer.AsSpan();

            // Initialize depth buffer with "infinity"
            depthBufferSpan.Fill(float.MaxValue);

            using var graphics = Graphics.FromImage(bmp);
            graphics.Clear(camera.BackgroundColor); // Use the camera's background color

            var sinPhi = ExtendedMath.CalcSin(camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(camera.Angle);
            var sinPitch = ExtendedMath.CalcSin(camera.Pitch);
            var cosPitch = ExtendedMath.CalcCos(camera.Pitch);

            float z = 1;
            float dz = 1;

            while (z < camera.ZFar)
            {
                // Calculate the left and right endpoints of the view slice at depth z
                var pLeftX = (float)(-cosPhi * z - sinPhi * z) + camera.X;
                var pLeftY = (float)(sinPhi * z - cosPhi * z) + camera.Y;

                var pRightX = (float)(cosPhi * z - sinPhi * z) + camera.X;
                var pRightY = (float)(-sinPhi * z - cosPhi * z) + camera.Y;

                var dx = (pRightX - pLeftX) / camera.ScreenWidth;
                var dy = (pRightY - pLeftY) / camera.ScreenWidth;

                for (var x = 0; x < camera.ScreenWidth; x++)
                {
                    var heightX = (int)pLeftX;
                    var heightY = (int)pLeftY;

                    var height = heightMap[heightX & (topographyWidth - 1), heightY & (topographyHeight - 1)];
                    var color = colorMap[heightX & (colorWidth - 1), heightY & (colorHeight - 1)];

                    // Adjust height for pitch angle
                    var adjustedHeight = (camera.Height - height) * cosPitch - z * sinPitch;

                    // Project height onto screen space
                    var heightOnScreen = adjustedHeight / z * camera.Scale + camera.Horizon;

                    // Check depth buffer for visibility
                    var screenY = (int)heightOnScreen;

                    if (screenY >= 0 && screenY < camera.ScreenHeight)
                    {
                        var index = screenY * camera.ScreenWidth + x;

                        if (z < depthBufferSpan[index])
                        {
                            // Update depth buffer and set pixel color
                            depthBufferSpan[index] = z;
                            dbm.SetPixel(x, screenY, color);
                        }
                    }

                    // Update for next pixel
                    pLeftX += dx;
                    pLeftY += dy;
                }

                // Move to the next slice
                z += dz;
                dz += 0.005f; // Increment dz for the next depth slice
            }

            return dbm.Bitmap;
        }

        /// <summary>
        ///     Creates the bitmap from container.
        /// </summary>
        /// <param name="colorMap">The color map.</param>
        /// <param name="heightMap">The height map.</param>
        /// <param name="camera">The camera.</param>
        /// <param name="topographyHeight">Height of the topography.</param>
        /// <param name="topographyWidth">Width of the topography.</param>
        /// <param name="colorHeight">Height of the color.</param>
        /// <param name="colorWidth">Width of the color.</param>
        /// <returns>
        ///     Finished Bitmap
        /// </returns>
        internal Bitmap RenderWithContainer(
            Color[,] colorMap, int[,] heightMap, Camera camera,
            int topographyHeight, int topographyWidth, int colorHeight, int colorWidth)
        {
            // Preallocate flat array for colors and initialize the Y-buffer
            var pixelColors = new Color[camera.ScreenWidth * camera.ScreenHeight];
            _yBuffer = new float[camera.ScreenWidth];
            for (var i = 0; i < _yBuffer.Length; i++)
                _yBuffer[i] = camera.ScreenHeight;

            var pixelTuples = new List<(int x, int y, Color color)>(camera.ScreenWidth * camera.ScreenHeight);

            var sinPhi = ExtendedMath.CalcSin(camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(camera.Angle);

            float z = 1;
            float dz = 1;

            // Process depth slices
            while (z < camera.ZFar)
            {
                var pLeftX = (float)(-cosPhi * z - sinPhi * z) + camera.X;
                var pLeftY = (float)(sinPhi * z - cosPhi * z) + camera.Y;

                var pRightX = (float)(cosPhi * z - sinPhi * z) + camera.X;
                var pRightY = (float)(-sinPhi * z - cosPhi * z) + camera.Y;

                var dx = (pRightX - pLeftX) / camera.ScreenWidth;
                var dy = (pRightY - pLeftY) / camera.ScreenWidth;

                for (var i = 0; i < camera.ScreenWidth; i++)
                {
                    var diffuseX = (int)pLeftX;
                    var diffuseY = (int)pLeftY;
                    var heightX = (int)pLeftX;
                    var heightY = (int)pLeftY;

                    var heightOfHeightMap =
                        heightMap[heightX & (topographyWidth - 1), heightY & (topographyHeight - 1)];

                    var color = colorMap[diffuseX & (colorWidth - 1), diffuseY & (colorHeight - 1)];

                    var heightOnScreen = (float)(((camera.Height - heightOfHeightMap) / z * camera.Scale + camera.Horizon) -
                                                 //handle Looking up and down
                                                 ExtendedMath.CalcTan(camera.Pitch) * camera.Scale);

                    var y1 = (int)heightOnScreen;

                    if (y1 < _yBuffer[i])
                    {
                        _yBuffer[i] = heightOnScreen;

                        // Calculate flat array index and set the color
                        var index = y1 * camera.ScreenWidth + i;
                        if (index >= 0 && index < pixelColors.Length) // Bounds check
                        {
                            pixelColors[index] = color;

                            // Add to the list for DirectBitmap
                            pixelTuples.Add((i, y1, color));
                        }
                    }

                    pLeftX += dx;
                    pLeftY += dy;
                }

                z += dz;
                dz += 0.005f;
            }

            // Create the final bitmap
            var dbm = new DirectBitmap(camera.ScreenWidth, camera.ScreenHeight);

            // Use SetPixelsSimd with the prepared tuples
            dbm.SetPixelsSimd(pixelTuples);

            return dbm.Bitmap;
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose of managed resources.
                _linePen?.Dispose();
                _solidBrush?.Dispose();
            }

            // Dispose of unmanaged resources if needed.

            _disposed = true;
        }

        ~Raster()
        {
            // Finalizer calls Dispose(false).
            Dispose(false);
        }
    }
}