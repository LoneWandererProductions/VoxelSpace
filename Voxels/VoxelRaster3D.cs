using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Threading.Tasks;
using ExtendedSystemObjects;
using Imaging;
using Mathematics;

namespace Voxels
{
    public sealed class VoxelRaster3D : IDisposable
    {
        private const float DzIncrement = 0.005f;
        private readonly int _colorHeight;
        private readonly int _colorWidth;

        private readonly CameraContext _context;
        private readonly int _topographyHeight;
        private readonly int _topographyWidth;

        private List<int[]> _columnSlices;
        private DirectBitmap _directBitmap;

        /// <summary>
        ///     The disposed
        /// </summary>
        private bool _disposed;

        private Color[] _flatColorMap;
        private int[] _flatHeightMap;
        private ImmutableDictionary<int, Color> _colorDictionary;

        /// <summary>
        ///     The y buffer
        /// </summary>
        private float[] _yBuffer;

        public VoxelRaster3D(CameraContext context, Color[,] colorMap, int[,] heightMap, int topographyWidth,
            int topographyHeight, int colorWidth, int colorHeight)
        {
            _context = context;
            _topographyWidth = topographyWidth;
            _topographyHeight = topographyHeight;
            _colorWidth = colorWidth;
            _colorHeight = colorHeight;
            InitializeBuffers(colorMap, heightMap, topographyWidth, topographyHeight, colorWidth, colorHeight);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the Dispose method with true to release resources.
            Dispose(true);
            // Suppress finalization to avoid it being called twice.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes the buffers.
        /// </summary>
        /// <param name="colorMap">The color map.</param>
        /// <param name="heightMap">The height map.</param>
        /// <param name="topographyWidth">Width of the topography.</param>
        /// <param name="topographyHeight">Height of the topography.</param>
        /// <param name="colorWidth">Width of the color.</param>
        /// <param name="colorHeight">Height of the color.</param>
        private void InitializeBuffers(Color[,] colorMap, int[,] heightMap, int topographyWidth, int topographyHeight,
            int colorWidth, int colorHeight)
        {
            _yBuffer = new float[_context.ScreenWidth];
            _columnSlices = new List<int[]>(_context.ScreenWidth);
            for (var i = 0; i < _context.ScreenWidth; i++) _columnSlices.Add(new int[_context.ScreenHeight]);

            // Flatten heightMap to 1D array
            _flatHeightMap = new int[topographyWidth * topographyHeight];
            for (var y = 0; y < topographyHeight; y++)
            for (var x = 0; x < topographyWidth; x++)
                _flatHeightMap[y * topographyWidth + x] = heightMap[x, y];

            // Flatten colorMap to 1D array
            _flatColorMap = new Color[colorWidth * colorHeight];

            for (var y = 0; y < colorHeight; y++)
            for (var x = 0; x < colorWidth; x++)
                _flatColorMap[y * colorWidth + x] = colorMap[x, y];

            // Populate the ImmutableDictionary
            var dictionaryBuilder = ImmutableDictionary.CreateBuilder<int, Color>();
            for (var y = 0; y < colorHeight; y++)
                for (var x = 0; x < colorWidth; x++)
                {
                    var color = colorMap[x, y];
                    dictionaryBuilder[color.ToArgb()] = color; // Add ARGB as key and Color as value
                }

            _colorDictionary = dictionaryBuilder.ToImmutable();
        }

        /// <summary>
        ///     Creates the bitmap from container.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <returns>
        ///     Finished Bitmap
        /// </returns>
        public Bitmap RenderWithContainer(RvCamera camera)
        {
            Array.Fill(_yBuffer, _context.ScreenHeight);

            Parallel.For(0, _columnSlices.Count, i => Array.Fill(_columnSlices[i], 0));

            var colorDictionary = new Dictionary<int, Color>();
            var sinPhi = ExtendedMath.CalcSinF(camera.Angle);
            var cosPhi = ExtendedMath.CalcCosF(camera.Angle);

            float z = camera.Z;
            float dz = 1;

            var fov = (int)(_context.Fov / 2.0);

            var tanFovHalf = ExtendedMath.CalcTanF(fov);

            while (z < camera.ZFar)
            {
                var halfWidth = z * tanFovHalf;

                var pLeftX = -cosPhi * halfWidth - sinPhi * z + camera.X;
                var pLeftY = sinPhi * halfWidth - cosPhi * z + camera.Y;

                var pRightX = cosPhi * halfWidth - sinPhi * z + camera.X;
                var pRightY = -sinPhi * halfWidth - cosPhi * z + camera.Y;

                var dx = (pRightX - pLeftX) / _context.ScreenWidth;
                var dy = (pRightY - pLeftY) / _context.ScreenWidth;

                for (var i = 0; i < _context.ScreenWidth; i++)
                {
                    var diffuseX = pLeftX.RoundToInt();
                    var diffuseY = pLeftY.RoundToInt();
                    var heightX = pLeftX.RoundToInt();
                    var heightY = pLeftY.RoundToInt();

                    //access via Indexing
                    var wrappedHeightX = heightX & (_topographyWidth - 1);
                    var wrappedHeightY = heightY & (_topographyHeight - 1);
                    var wrappedColorX = diffuseX & (_colorWidth - 1);
                    var wrappedColorY = diffuseY & (_colorHeight - 1);

                    // Access the flattened arrays using the calculated indices
                    var heightOfHeightMap = _flatHeightMap[wrappedHeightY * _topographyWidth + wrappedHeightX];
                    var color = _flatColorMap[wrappedColorY * _colorWidth + wrappedColorX];

                    var heightOnScreen = (_context.Height - heightOfHeightMap) / z * _context.Scale + camera.Horizon -
                                         ExtendedMath.CalcTanF(camera.Pitch) * _context.Scale;

                    var y1 = heightOnScreen.RoundToInt();

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

            var lines = RasterHelper.FillMissingColorsLines(_columnSlices, _colorDictionary);
            //var points = RasterHelper.FillMissingColorsPoints(_columnSlices, _colorDictionary);
            //var all = RasterHelper.FillMissingColorsOld(_columnSlices, _colorDictionary);

            //var both = RasterHelper.GetPointsAndLines(_columnSlices, _colorDictionary);

            _directBitmap = new DirectBitmap(_context.ScreenWidth, _context.ScreenHeight);

            _directBitmap.DrawVerticalLines(lines);
            //_directBitmap.SetPixelsSimd(both.points);
            Array.Clear(_yBuffer, 0, _yBuffer.Length);

            return _directBitmap.Bitmap;
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                // Dispose of managed resources.
                _directBitmap.Dispose();

            // Dispose of unmanaged resources if needed.

            _disposed = true;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="VoxelRaster3D" /> class.
        /// </summary>
        ~VoxelRaster3D()
        {
            // Finalizer calls Dispose(false).
            Dispose(false);
        }
    }
}