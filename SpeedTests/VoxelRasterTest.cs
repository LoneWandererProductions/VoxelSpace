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
        public VoxelRasterTest(int x, int y, int degree, int height, int horizon, int scale, int distance,
            Bitmap colorMap,
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
        ///     The color map
        ///     Buffer/array to hold color values (1024*1024)
        /// </summary>
        public Color[,] ColorMap { get; set; }

        /// <summary>
        ///     The color width
        /// </summary>
        public int ColorWidth { get; set; }

        /// <summary>
        ///     The height map
        ///     Buffer/array to hold height values (1024*1024)
        /// </summary>
        public int[,] HeightMap { get; set; }

        /// <summary>
        ///     The topography height
        /// </summary>
        public int TopographyHeight { get; set; }

        /// <summary>
        ///     The topography width
        /// </summary>
        public int TopographyWidth { get; set; }

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
            if (HeightMap == null) return null;

            var raster = new VoxelRaster2D();

            return raster.RenderImmediate(ColorMap, HeightMap, Camera, TopographyHeight, TopographyWidth,
                _colorHeight, ColorWidth);
        }

        /// <summary>
        ///     Renders the with container.
        /// </summary>
        /// <returns></returns>
        public Bitmap RenderWithContainer()
        {
            if (HeightMap == null) return null;

            var raster = new VoxelRaster2D();

            return raster.RenderWithContainer(ColorMap, HeightMap, Camera, TopographyHeight, TopographyWidth,
                _colorHeight, ColorWidth);
        }

        public Bitmap RenderDepth()
        {
            if (HeightMap == null) return null;

            var raster = new VoxelRaster2D();

            return raster.RenderWithDepthBuffer(ColorMap, HeightMap, Camera, TopographyHeight, TopographyWidth,
                _colorHeight, ColorWidth);
        }

        /// <summary>
        ///     Processes the height map.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        private void ProcessHeightMap(Bitmap bmp)
        {
            if (bmp == null) return;

            TopographyHeight = bmp.Height;
            TopographyWidth = bmp.Width;

            var dbm = DirectBitmap.GetInstance(bmp);

            HeightMap = new int[bmp.Width, bmp.Height];
            for (var i = 0; i < bmp.Width; i++)
            for (var j = 0; j < bmp.Height; j++)
                HeightMap[i, j] = dbm.GetPixel(i, j).R;
        }

        /// <summary>
        ///     Processes the color map.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        private void ProcessColorMap(Bitmap bmp)
        {
            if (bmp == null) return;

            _colorHeight = bmp.Height;
            ColorWidth = bmp.Width;

            var dbm = DirectBitmap.GetInstance(bmp);

            ColorMap = new Color[bmp.Width, bmp.Height];
            for (var i = 0; i < bmp.Width; i++)
            for (var j = 0; j < bmp.Height; j++)
                ColorMap[i, j] = dbm.GetPixel(i, j);
        }
    }
}