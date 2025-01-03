﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Imaging;
using Mathematics;

namespace Voxels
{
    public sealed class VoxelRaster3D : IDisposable
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

        private readonly CameraContext _context;
        private DirectBitmap _directBitmap;
        private Color[] _flatColorMap;
        private int[] _flatHeightMap;
        private readonly int _topographyWidth;
        private readonly int _topographyHeight;
        private readonly int _colorWidth;
        private readonly int _colorHeight;

        private const float DzIncrement = 0.005f;

        public VoxelRaster3D(CameraContext context, Color[,] colorMap, int[,] heightMap, int topographyWidth, int topographyHeight, int colorWidth, int colorHeight)
        {
            _context = context;
            _topographyWidth = topographyWidth;
            _topographyHeight = topographyHeight;
            _colorWidth = colorWidth;
            _colorHeight = colorHeight;
            InitializeBuffers(colorMap, heightMap, topographyWidth, topographyHeight, colorWidth, colorHeight);
        }

        private void InitializeBuffers(Color[,] colorMap, int[,] heightMap, int topographyWidth, int topographyHeight, int colorWidth, int colorHeight)
        {
            _yBuffer = new float[_context.ScreenWidth];
            _columnSlices = new List<int[]>(_context.ScreenWidth);
            for (var i = 0; i < _context.ScreenWidth; i++)
            {
                _columnSlices.Add(new int[_context.ScreenHeight]);
            }

            // Flatten heightMap to 1D array
            _flatHeightMap = new int[topographyWidth * topographyHeight];
            for (var y = 0; y < topographyHeight; y++)
            {
                for (var x = 0; x < topographyWidth; x++)
                {
                    _flatHeightMap[y * topographyWidth + x] = heightMap[x, y];
                }
            }

            // Flatten colorMap to 1D array
            _flatColorMap = new Color[colorWidth * colorHeight];

            for (var y = 0; y < colorHeight; y++)
            {
                for (var x = 0; x < colorWidth; x++)
                {
                    _flatColorMap[y * colorWidth + x] = colorMap[x, y];
                }
            }
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
        /// <param name="camera">The camera.</param>
        /// <returns>
        ///     Finished Bitmap
        /// </returns>
        public Bitmap RenderWithContainer(RVCamera camera)
        {
            Array.Fill(_yBuffer, _context.ScreenHeight);

            Parallel.For(0, _columnSlices.Count, i => Array.Fill(_columnSlices[i], 0));

            var colorDictionary = new Dictionary<int, Color>();
            var sinPhi = ExtendedMath.CalcSin(camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(camera.Angle);

            float z = camera.Z;
            float dz = 1;

            var tanFovHalf = (float)Math.Tan(_context.Fov / 2.0 * Math.PI / 180.0);

            while (z < camera.ZFar)
            {
                var halfWidth = z * tanFovHalf;

                var pLeftX = (float)(-cosPhi * halfWidth - sinPhi * z) + camera.X;
                var pLeftY = (float)(sinPhi * halfWidth - cosPhi * z) + camera.Y;

                var pRightX = (float)(cosPhi * halfWidth - sinPhi * z) + camera.X;
                var pRightY = (float)(-sinPhi * halfWidth - cosPhi * z) + camera.Y;

                var dx = (pRightX - pLeftX) / _context.ScreenWidth;
                var dy = (pRightY - pLeftY) / _context.ScreenWidth;

                for (var i = 0; i < _context.ScreenWidth; i++)
                {
                    var diffuseX = (int)pLeftX;
                    var diffuseY = (int)pLeftY;
                    var heightX = (int)pLeftX;
                    var heightY = (int)pLeftY;

                    //access via Indexing
                    var wrappedHeightX = heightX & (_topographyWidth - 1);
                    var wrappedHeightY = heightY & (_topographyHeight - 1);
                    var wrappedColorX = diffuseX & (_colorWidth - 1);
                    var wrappedColorY = diffuseY & (_colorHeight - 1);

                    // Access the flattened arrays using the calculated indices
                    var heightOfHeightMap = _flatHeightMap[wrappedHeightY * _topographyWidth + wrappedHeightX];
                    var color = _flatColorMap[wrappedColorY * _colorWidth + wrappedColorX];

                    var heightOnScreen = (float)(((_context.Height - heightOfHeightMap) / z * _context.Scale + camera.Horizon) -
                                                 ExtendedMath.CalcTan(camera.Pitch) * _context.Scale);

                    var y1 = (int)heightOnScreen;

                    if (y1 < _yBuffer[i] && y1 >= 0 && y1 < _context.ScreenHeight)
                    {
                        _yBuffer[i] = heightOnScreen;

                        if (color != Color.Transparent)
                        {
                            var colorId = color.ToArgb();
                            colorDictionary[colorId] = color;
                            _columnSlices[i][y1] = colorId;
                        }
                    }

                    pLeftX += dx;
                    pLeftY += dy;
                }

                z += dz;
                dz += DzIncrement;
            }

            var pixelTuples = FillMissingColors(colorDictionary);

            _directBitmap = new DirectBitmap(_context.ScreenWidth, _context.ScreenHeight);
            _directBitmap.SetPixelsSimd(pixelTuples);

            Array.Clear(_yBuffer, 0, _yBuffer.Length);

            return _directBitmap.Bitmap;
        }

        private IEnumerable<(int x, int y, Color color)> FillMissingColors(IReadOnlyDictionary<int, Color> colorDictionary)
        {
            var filledPixelTuples = new ConcurrentBag<(int x, int y, Color color)>();

            Parallel.For(0, _columnSlices.Count, x =>
            {
                Span<int> columnSlice = _columnSlices[x]; // Span used for performance
                var lastKnownColorId = 0;

                for (var y = 0; y < columnSlice.Length; y++)
                {
                    if (columnSlice[y] != 0)
                    {
                        lastKnownColorId = columnSlice[y];
                    }
                    else if (lastKnownColorId != 0)
                    {
                        columnSlice[y] = lastKnownColorId;
                    }

                    if (columnSlice[y] == 0) continue;

                    if (colorDictionary.TryGetValue(columnSlice[y], out var color))
                    {
                        filledPixelTuples.Add((x, y, color));
                    }
                    else
                    {
                        Trace.WriteLine($"Warning: Color ID {columnSlice[y]} not found in the dictionary.");
                    }
                }
            });

            return filledPixelTuples;
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose of managed resources.
                _directBitmap.Dispose();
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