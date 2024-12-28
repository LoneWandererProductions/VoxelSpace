using System;
using System.Collections.Generic;
using System.Drawing;
using Imaging;
using Mathematics;

namespace Voxels
{
    internal sealed class VoxelRaster3D : IDisposable
    {
        /// <summary>
        /// The disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The y buffer
        /// </summary>
        private float[] _yBuffer;

        private const float DzIncrement = 0.005f;

        /// <inheritdoc />
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the Dispose method with true to release resources.
            Dispose(true);
            // Suppress finalization to avoid it being called twice.
            GC.SuppressFinalize(this);
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
            var pixelColors = new Color[camera.ScreenWidth * camera.ScreenHeight];

            // Initialize the Y-buffer to store the closest Y values for depth testing
            _yBuffer = new float[camera.ScreenWidth];
            Array.Fill(_yBuffer, camera.ScreenHeight); // Initially set all values to the screen height (farthest)


            // Initialize y-buffer to the maximum height of the screen
            Array.Fill(_yBuffer, camera.ScreenHeight);

            var pixelTuples = new List<(int x, int y, Color color)>(camera.ScreenWidth * camera.ScreenHeight);

            var sinPhi = ExtendedMath.CalcSin(camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(camera.Angle);

            float z = camera.Z; // Start at the camera's current Z position
            float dz = 1;

            // Calculate the tangent of half the field of view (FOV) angle for the camera's projection
            var tanFovHalf = (float)Math.Tan(camera.Fov / 2.0 * Math.PI / 180.0);


            while (z < camera.ZFar)
            {
                // Calculate frustum width based on FOV and depth (z)
                var halfWidth = z * tanFovHalf;

                // Frustum bounds
                var pLeftX = (float)(-cosPhi * halfWidth - sinPhi * z) + camera.X;
                var pLeftY = (float)(sinPhi * halfWidth - cosPhi * z) + camera.Y;

                var pRightX = (float)(cosPhi * halfWidth - sinPhi * z) + camera.X;
                var pRightY = (float)(-sinPhi * halfWidth - cosPhi * z) + camera.Y;

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
                                                 ExtendedMath.CalcTan(camera.Pitch) * camera.Scale);

                    var y1 = (int)heightOnScreen;

                    if (y1 < _yBuffer[i])
                    {
                        _yBuffer[i] = heightOnScreen;

                        var index = y1 * camera.ScreenWidth + i;
                        if (index >= 0 && index < pixelColors.Length)
                        {
                            pixelColors[index] = color;
                            pixelTuples.Add((i, y1, color));
                        }
                    }

                    pLeftX += dx;
                    pLeftY += dy;
                }

                z += dz;
                dz += DzIncrement;
            }

            var dbm = new DirectBitmap(camera.ScreenWidth, camera.ScreenHeight);
            dbm.SetPixelsSimd(pixelTuples);

            // After rendering, clear the buffers:
            Array.Clear(_yBuffer, 0, _yBuffer.Length);
            Array.Clear(pixelColors, 0, pixelColors.Length);

            return dbm.Bitmap;
        }


        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose of managed resources.
            }

            // Dispose of unmanaged resources if needed.

            _disposed = true;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="VoxelRaster3D"/> class.
        /// </summary>
        ~VoxelRaster3D()
        {
            // Finalizer calls Dispose(false).
            Dispose(false);
        }
    }
}