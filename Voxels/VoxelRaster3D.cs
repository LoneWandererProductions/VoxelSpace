using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private List<int[]> _columnSlices;
        private Dictionary<int, Color> _colorDictionary;

        private const float DzIncrement = 0.005f;

        //TODO for the future
        public VoxelRaster3D(int screenWidth, int screenHeight)
        {
            // Initialize column slices with the screen dimensions
            _columnSlices = new List<int[]>(screenWidth);
            for (var x = 0; x < screenWidth; x++)
            {
                _columnSlices.Add(new int[screenHeight]);
            }

            // Create a deep copy of _columnSlices for this render call
            // var currentColumnSlices = _columnSlices.Select(slice => (int[])slice.Clone()).ToList();
        }

        public VoxelRaster3D()
        {
        }

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
            // Initialize the Y-buffer to store the closest Y values for depth testing
            _yBuffer = new float[camera.ScreenWidth];
            Array.Fill(_yBuffer, camera.ScreenHeight); // Initially set all values to the screen height (farthest)

            // Step 1: Initialize the slices and the color dictionary
            _columnSlices = new List<int[]>(camera.ScreenWidth);
            _colorDictionary = new Dictionary<int, Color>();
            for (var x = 0; x < camera.ScreenWidth; x++)
            {
                _columnSlices.Add(new int[camera.ScreenHeight]); // Initialize each column slice with screenHeight
            }
            //TODO in the future move into constructor and use a copy

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

                    if (y1 < _yBuffer[i] && y1 >= 0 && y1 < camera.ScreenHeight)
                    {
                        _yBuffer[i] = heightOnScreen;

                        pixelTuples.Add((i, y1, color));

                        if (color != Color.Transparent)
                        {
                            var colorId = color.ToArgb();  // Convert color to a unique integer ID
                            _colorDictionary[colorId] = color;  // Store the color in the dictionary

                            _columnSlices[i][y1] = colorId;  // Assign the color ID to the slice
                        }
                    }

                    pLeftX += dx;
                    pLeftY += dy;
                }

                z += dz;
                dz += DzIncrement;
            }

            pixelTuples = FillMissingColors();

            var dbm = new DirectBitmap(camera.ScreenWidth, camera.ScreenHeight);
            dbm.SetPixelsSimd(pixelTuples);

            // After rendering, clear the buffers:
            Array.Clear(_yBuffer, 0, _yBuffer.Length);

            return dbm.Bitmap;
        }


        private List<(int x, int y, Color color)> FillMissingColors()
        {
            var filledPixelTuples = new List<(int x, int y, Color color)>(_columnSlices.Count * _columnSlices[0].Length);

            // Loop through each column slice (x-axis)
            for (var x = 0; x < _columnSlices.Count; x++)
            {
                var columnSlice = _columnSlices[x];
                var lastKnownColorId = 0; // Start with no known color ID

                // Loop through each pixel in the column (y-axis)
                for (var y = 0; y < columnSlice.Length; y++)
                {
                    if (columnSlice[y] != 0) // If there's a valid color ID
                    {
                        lastKnownColorId = columnSlice[y]; // Update the last known color ID
                    }
                    else if (lastKnownColorId != 0) // If transparent, replace with last known color ID
                    {
                        columnSlice[y] = lastKnownColorId;
                    }

                    // Add the filled pixel to the output tuples
                    if (columnSlice[y] == 0) continue;

                    var color = _colorDictionary[columnSlice[y]]; // Get color from dictionary
                    filledPixelTuples.Add((x, y, color));
                }
            }

            return filledPixelTuples;
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