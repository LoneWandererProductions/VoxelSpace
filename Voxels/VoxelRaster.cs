﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Voxels
 * FILE:        Voxels/VoxelRaster.cs
 * PURPOSE:     Main Renderer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * Source:      https://github.com/s-macke/VoxelSpace
 */

// ReSharper disable PossibleLossOfFraction

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using Imaging;
using Mathematics;

namespace Voxels
{
    /// <summary>
    ///     https://www.youtube.com/watch?v=bQBY9BM9g_Y
    /// </summary>
    public sealed class VoxelRaster
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
        ///     Initializes a new instance of the <see cref="VoxelRaster" /> class.
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
        public VoxelRaster(int x, int y, int degree, int height, int horizon, int scale, int distance, Bitmap colorMap,
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
        ///     The y buffer
        /// </summary>
        private float[] YBuffer { get; set; }

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

            return raster.CreateBitmapFromContainer(_colorMap, _heightMap, Camera, _topographyHeight, _topographyWidth,
                _colorHeight, _colorWidth);
        }

        /// <summary>
        ///     Keys the input.
        /// </summary>
        /// <param name="key">The key.</param>
        public void KeyInput(Key key)
        {
            switch (key)
            {
                case Key.W: //Forward
                    Camera.X -= (int)(10 * ExtendedMath.CalcSin(Camera.Angle));
                    Camera.Y -= (int)(10 * ExtendedMath.CalcCos(Camera.Angle));
                    break;
                case Key.S: //Backward
                    Camera.X += (int)(10 * ExtendedMath.CalcSin(Camera.Angle));
                    Camera.Y += (int)(10 * ExtendedMath.CalcCos(Camera.Angle));
                    break;
                case Key.A: //Turn Left
                    Camera.Angle += 10;
                    break;
                case Key.D: //Turn Right
                    Camera.Angle -= 10;
                    break;
                case Key.O: //Look up
                    Camera.Horizon += 10;
                    break;
                case Key.P: //Look down
                    Camera.Horizon -= 10;
                    break;
            }
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