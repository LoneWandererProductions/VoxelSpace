/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SpeedTests
 * FILE:        SpeedTests/VoxelRasterTest.cs
 * PURPOSE:     Tests for Renderer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * Source:      https://github.com/s-macke/VoxelSpace
 */

// ReSharper disable PossibleLossOfFraction

using System.Drawing;
using Imaging;
using Voxels;

namespace SpeedTests
{
    /// <summary>
    ///     https://www.youtube.com/watch?v=bQBY9BM9g_Y
    /// </summary>
    internal sealed class VoxelRasterTest
    {
        /// <summary>
        ///     The color height
        /// </summary>
        private int _colorHeight;

        /// <summary>
        ///     The color map
        ///     Buffer/array to hold color values (1024*1024)
        /// </summary>
        private Color[,] _colorMap;

        /// <summary>
        ///     The color width
        /// </summary>
        private int _colorWidth;

        /// <summary>
        ///     The height map
        ///     Buffer/array to hold height values (1024*1024)
        /// </summary>
        private int[,] _heightMap;

        /// <summary>
        ///     The topography height
        /// </summary>
        private int _topographyHeight;

        /// <summary>
        ///     The topography width
        /// </summary>
        private int _topographyWidth;

        /// <summary>
        ///     Initializes a new instance of the <see cref="VoxelRasterTest" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="degree">The degree.</param>
        /// <param name="height">The height.</param>
        /// <param name="horizon">The horizon.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="colorMap">The bitmap with the colors</param>
        /// <param name="heightMap">The bitmap with the height map.</param>
        public VoxelRasterTest(int x, int y, int degree, int height, int horizon, int scale, int distance, Bitmap colorMap,
            Bitmap heightMap)
        {
            Camera = new Camera
            {
                X = x,
                Y = y,
                Angle = degree,
                Height = height,
                Horizon = horizon,
                Scale = scale,
                ZFar = distance
            };

            ProcessColorMap(colorMap);

            ProcessHeightMap(heightMap);
        }

        /// <summary>
        ///     Gets or sets the camera.
        ///     Only here just in case if KeyInput is not available
        /// </summary>
        /// <value>
        ///     The camera.
        /// </value>
        public Camera Camera { get; set; }

        /// <summary>
        ///     Renders the image directly onto the Bitmap.
        /// </summary>
        /// <returns>The finished Bitmap created directly.</returns>
        public Bitmap RenderDirect()
        {
            if (_heightMap == null) return null;

            var raster = new Raster();

            return raster.RenderImmediate(_colorMap, _heightMap, Camera, _topographyHeight, _topographyWidth,
                _colorHeight, _colorWidth);
        }

        /// <summary>
        ///     Renders the with container.
        /// </summary>
        /// <returns></returns>
        public Bitmap RenderWithContainer()
        {
            if (_heightMap == null) return null;

            var raster = new Raster();

            return raster.RenderWithContainer(_colorMap, _heightMap, Camera, _topographyHeight, _topographyWidth,
                _colorHeight, _colorWidth);
        }

        public Bitmap RenderDepth()
        {
            if (_heightMap == null) return null;

            var raster = new Raster();

            return raster.RenderWithDepthBuffer(_colorMap, _heightMap, Camera, _topographyHeight, _topographyWidth,
                _colorHeight, _colorWidth);
        }

        /// <summary>
        ///     Processes the height map.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        private void ProcessHeightMap(Bitmap bmp)
        {
            if (bmp == null) return;

            _topographyHeight = bmp.Height;
            _topographyWidth = bmp.Width;

            var dbm = DirectBitmap.GetInstance(bmp);

            _heightMap = new int[bmp.Width, bmp.Height];
            for (var i = 0; i < bmp.Width; i++)
            for (var j = 0; j < bmp.Height; j++)
                _heightMap[i, j] = dbm.GetPixel(i, j).R;
        }

        /// <summary>
        ///     Processes the color map.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        private void ProcessColorMap(Bitmap bmp)
        {
            if (bmp == null) return;

            _colorHeight = bmp.Height;
            _colorWidth = bmp.Width;

            var dbm = DirectBitmap.GetInstance(bmp);

            _colorMap = new Color[bmp.Width, bmp.Height];
            for (var i = 0; i < bmp.Width; i++)
            for (var j = 0; j < bmp.Height; j++)
                _colorMap[i, j] = dbm.GetPixel(i, j);
        }
    }
}