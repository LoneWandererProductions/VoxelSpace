using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        private readonly SolidBrush _solidBrush;

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
        /// <returns></returns>
        internal List<Slice> GenerateRaster(Color[,] colorMap, int[,] heightMap, Camera camera, int topographyHeight,
            int topographyWidth, int colorHeight, int colorWidth)
        {
            var raster = new List<Slice>();
            _yBuffer = new float[camera.ScreenWidth];

            for (var i = 0; i < _yBuffer.Length; i++)
                _yBuffer[i] = camera.ScreenHeight;

            var sinPhi = ExtendedMath.CalcSin(camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(camera.Angle);

            float z = 1;
            float dz = 1;

            while (z < camera.ZFar)
            {
                // Precompute values that do not change per pixel
                var pLeftX = (float)(-cosPhi * z - sinPhi * z) + camera.X;
                var pLeftY = (float)(sinPhi * z - cosPhi * z) + camera.Y;

                var pRightX = (float)(cosPhi * z - sinPhi * z) + camera.X;
                var pRightY = (float)(-sinPhi * z - cosPhi * z) + camera.Y;

                var dx = (pRightX - pLeftX) / camera.ScreenWidth;
                var dy = (pRightY - pLeftY) / camera.ScreenWidth;

                // Loop through screen width
                for (var i = 0; i < camera.ScreenWidth; i++)
                {
                    var diffuseX = (int)pLeftX;
                    var diffuseY = (int)pLeftY;
                    var heightX = (int)pLeftX;
                    var heightY = (int)pLeftY;

                    // Access height map and color map for each pixel
                    var heightOfHeightMap =
                        heightMap[heightX & (topographyWidth - 1), heightY & (topographyHeight - 1)];
                    var color = colorMap[diffuseX & (colorWidth - 1), diffuseY & (colorHeight - 1)];

                    // Calculate height on screen
                    var heightOnScreen = (camera.Height - heightOfHeightMap) / z * camera.Scale + camera.Horizon;

                    // Add the slice to the raster
                    raster.Add(new Slice
                    {
                        Shade = color,
                        X1 = i,
                        Y1 = (int)heightOnScreen,
                        Y2 = (int)_yBuffer[i]
                    });

                    // Update the buffer for the next iteration
                    if (heightOnScreen < _yBuffer[i])
                        _yBuffer[i] = heightOnScreen;

                    // Update pLeft for the next pixel in the row
                    pLeftX += dx;
                    pLeftY += dy;
                }

                // Move to the next slice
                z += dz;
                dz += 0.005f; // Increment dz for the next depth slice
            }

            return raster;
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
        internal Bitmap CreateBitmapWithDepthBuffer(Color[,] colorMap, int[,] heightMap, Camera camera,
            int topographyHeight, int topographyWidth, int colorHeight, int colorWidth)
        {
            var bmp = new Bitmap(camera.ScreenWidth, camera.ScreenHeight);

            //set background Color
            //using var g = Graphics.FromImage(bmp);
            //var backGround = new SolidBrush(camera.BackgroundColor);

            //g.FillRectangle(backGround, 0, 0, camera.ScreenWidth, camera.ScreenHeight);

            var dbm = new DirectBitmap(bmp);

            //do the work
            //before dbm: 8 ms, after: 4ms

            var depthBuffer = new float[camera.ScreenWidth, camera.ScreenHeight];

            // Initialize depth buffer with "infinity" (or a very large value)
            for (var x = 0; x < camera.ScreenWidth; x++)
            for (var y = 0; y < camera.ScreenHeight; y++)
                depthBuffer[x, y] = float.MaxValue;

            using var graphics = Graphics.FromImage(bmp);
            graphics.Clear(Color.Black); // Background color

            var sinPhi = ExtendedMath.CalcSin(camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(camera.Angle);

            float z = 1;
            float dz = 1;

            while (z < camera.ZFar)
            {
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

                    var heightOnScreen = (camera.Height - height) / z * camera.Scale + camera.Horizon;

                    // Check depth buffer for visibility
                    var screenY = (int)heightOnScreen;
                    if (screenY >= 0 && screenY < camera.ScreenHeight && z < depthBuffer[x, screenY])
                    {
                        // Update depth buffer and set pixel color
                        depthBuffer[x, screenY] = z;
                        dbm.SetPixel(x, screenY, color);
                    }

                    // Update for next pixel
                    pLeftX += dx;
                    pLeftY += dy;
                }

                // Move to the next slice
                z += dz;
                dz += 0.005f; // Increment dz for the next depth slice
            }

            //dbm =  ApplyLineSmoothing(dbm);

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
        internal Bitmap CreateBitmapFromContainer(Color[,] colorMap, int[,] heightMap, Camera camera,
            int topographyHeight,
            int topographyWidth, int colorHeight, int colorWidth)
        {
            // Use custom PixelKey struct to reduce hash computation overhead
            var pixelMap = new Dictionary<PixelKey, Color>(camera.ScreenWidth * camera.ScreenHeight);

            // Initialize the Y-buffer for each pixel (this represents the farthest visible pixel on the screen for each column)
            _yBuffer = new float[camera.ScreenWidth];
            for (var i = 0; i < _yBuffer.Length; i++)
                _yBuffer[i] = camera.ScreenHeight;

            var sinPhi = ExtendedMath.CalcSin(camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(camera.Angle);

            float z = 1;
            float dz = 1;

            // Loop through depth slices
            while (z < camera.ZFar)
            {
                // Precompute values that don't change per pixel
                var pLeftX = (float)(-cosPhi * z - sinPhi * z) + camera.X;
                var pLeftY = (float)(sinPhi * z - cosPhi * z) + camera.Y;

                var pRightX = (float)(cosPhi * z - sinPhi * z) + camera.X;
                var pRightY = (float)(-sinPhi * z - cosPhi * z) + camera.Y;

                var dx = (pRightX - pLeftX) / camera.ScreenWidth;
                var dy = (pRightY - pLeftY) / camera.ScreenWidth;

                // Loop through screen width
                for (var i = 0; i < camera.ScreenWidth; i++)
                {
                    var diffuseX = (int)pLeftX;
                    var diffuseY = (int)pLeftY;
                    var heightX = (int)pLeftX;
                    var heightY = (int)pLeftY;

                    // Access height map and color map for each pixel
                    var heightOfHeightMap =
                        heightMap[heightX & (topographyWidth - 1), heightY & (topographyHeight - 1)];
                    var color = colorMap[diffuseX & (colorWidth - 1), diffuseY & (colorHeight - 1)];

                    // Calculate height on screen
                    var heightOnScreen = (camera.Height - heightOfHeightMap) / z * camera.Scale + camera.Horizon;

                    // Replace or add the pixel to the map (only if it passes the Y-buffer test)
                    var y1 = (int)heightOnScreen;
                    if (y1 < _yBuffer[i])
                    {
                        _yBuffer[i] = heightOnScreen;
                        var pixelKey = new PixelKey(i, y1); // Use the optimized PixelKey
                        pixelMap[pixelKey] = color; // Add or replace pixel color
                    }

                    // Update pLeft for the next pixel in the row
                    pLeftX += dx;
                    pLeftY += dy;
                }

                // Move to the next slice
                z += dz;
                dz += 0.005f; // Increment dz for the next depth slice
            }

            // Create a DirectBitmap to store the final image
            var dbm = new DirectBitmap(camera.ScreenWidth, camera.ScreenHeight);

            // Set the pixels using SIMD (assuming SetPixelsSimd can handle this)
            dbm.SetPixelsSimd(pixelMap.Select(kvp => (kvp.Key.X, kvp.Key.Y, kvp.Value)));

            return dbm.Bitmap;
        }


        //TODO still in the progress of refinement
        private DirectBitmap ApplyLineSmoothing(DirectBitmap btm)
        {
            var width = btm.Width;
            var height = btm.Height;

            // You can apply a custom smoothing technique here for the lines
            // For simplicity, let's assume you are blending horizontally across lines
            for (var y = 0; y < height; y++)
            {
                var previousPixel = btm.GetPixel(0, y);

                for (var x = 1; x < width; x++)
                {
                    var currentPixel = btm.GetPixel(x, y);

                    // If a gap is detected, blend the current pixel with the previous one
                    if (currentPixel.A == 0) // Assume transparency for gaps
                    {
                        var blendedColor = BlendColors(previousPixel, currentPixel);
                        btm.SetPixel(x, y, blendedColor);
                    }
                    else
                    {
                        previousPixel = currentPixel;
                    }
                }
            }

            return btm;
        }

        // Simple color blending function
        private Color BlendColors(Color color1, Color color2)
        {
            // Here we blend based on alpha, you could apply different strategies
            const float alpha = 0.5f;
            var r = (int)(color1.R * (1 - alpha) + color2.R * alpha);
            var g = (int)(color1.G * (1 - alpha) + color2.G * alpha);
            var b = (int)(color1.B * (1 - alpha) + color2.B * alpha);

            return Color.FromArgb(r, g, b);
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