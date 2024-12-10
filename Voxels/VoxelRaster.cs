/*
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
        /// If the cif Format is active or not
        /// </summary>
        private const bool CifFormat = false;

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
        /// The color map cif
        /// Holds the color values (1024*1024)
        /// </summary>
        private Cif _colorMapCif;

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
        ///     The height map
        ///     Holds the height values (1024*1024)
        /// </summary>
        private Cif _heightMapCif;

        /// <summary>
        ///     The Slices of the Image
        /// </summary>
        private List<Slice> _raster;

        /// <summary>
        ///     The topography height
        /// </summary>
        private int _topographyHeight;

        /// <summary>
        ///     The topography width
        /// </summary>
        private int _topographyWidth;

        /// <summary>
        ///     The y buffer
        /// </summary>
        private float[] _yBuffer;

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
        ///     Gets or sets the camera.
        ///     Only here just in case if KeyInput is not available
        /// </summary>
        /// <value>
        ///     The camera.
        /// </value>
        public Camera Camera { get; set; }

        /// <summary>
        /// Renders the image directly onto the Bitmap.
        /// </summary>
        /// <returns>The finished Bitmap created directly.</returns>
        public Bitmap RenderDirect()
        {
            if (_colorMapCif == null || _heightMap == null) return null;

            var bmp = ClearFrame();
            _yBuffer = new float[Camera.ScreenWidth];

            for (var i = 0; i < _yBuffer.Length; i++)
                _yBuffer[i] = Camera.ScreenHeight;

            var sinPhi = ExtendedMath.CalcSin(Camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(Camera.Angle);

            float z = 1;
            float dz = 1;

            while (z < Camera.ZFar)
            {
                var pLeft = new PointF(
                    (float)(-cosPhi * z - sinPhi * z) + Camera.X,
                    (float)(sinPhi * z - cosPhi * z) + Camera.Y);

                var pRight = new PointF(
                    (float)(cosPhi * z - sinPhi * z) + Camera.X,
                    (float)(-sinPhi * z - cosPhi * z) + Camera.Y);

                var dx = (pRight.X - pLeft.X) / Camera.ScreenWidth;
                var dy = (pRight.Y - pLeft.Y) / Camera.ScreenWidth;

                for (var i = 0; i < Camera.ScreenWidth; i++)
                {
                    var diffuseX = (int)pLeft.X;
                    var diffuseY = (int)pLeft.Y;
                    var heightX = (int)pLeft.X;
                    var heightY = (int)pLeft.Y;

                    Color color;
                    int heightOfHeightMap;

                    if (CifFormat)
                    {
                        var x = diffuseX & (_colorWidth - 1);
                        var y = diffuseY & (_colorHeight - 1);
                        var id = (y * _colorWidth) + x;
                        color = _colorMapCif.GetColor(id);

                        x = heightX & (_topographyWidth - 1);
                        y = heightY & (_topographyHeight - 1);
                        id = (y * _colorWidth) + x;
                        heightOfHeightMap = _heightMapCif.GetColor(id).R;
                    }
                    else
                    {
                        heightOfHeightMap =
                            _heightMap[heightX & (_topographyWidth - 1), heightY & (_topographyHeight - 1)];

                        color = _colorMap[diffuseX & (_colorWidth - 1), diffuseY & (_colorHeight - 1)];
                    }

                    var heightOnScreen = (Camera.Height - heightOfHeightMap) / z * Camera.Scale + Camera.Horizon;

                    DrawVerticalLine(color, i, (int)heightOnScreen, (int)_yBuffer[i], bmp);

                    if (heightOnScreen < _yBuffer[i])
                        _yBuffer[i] = heightOnScreen;

                    pLeft.X += dx;
                    pLeft.Y += dy;
                }

                z += dz;
                dz += 0.005f;
            }

            return bmp;
        }

        /// <summary>
        /// Renders the image by creating the data in a container, then creating the Bitmap.
        /// </summary>
        /// <returns>The finished Bitmap created via container data.</returns>
        public Bitmap RenderWithContainer()
        {
            if (_colorMapCif == null || _heightMap == null) return null;

            _raster = new List<Slice>();
            _yBuffer = new float[Camera.ScreenWidth];

            for (var i = 0; i < _yBuffer.Length; i++)
                _yBuffer[i] = Camera.ScreenHeight;

            var sinPhi = ExtendedMath.CalcSin(Camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(Camera.Angle);

            float z = 1;
            float dz = 1;

            while (z < Camera.ZFar)
            {
                var pLeft = new PointF(
                    (float)(-cosPhi * z - sinPhi * z) + Camera.X,
                    (float)(sinPhi * z - cosPhi * z) + Camera.Y);

                var pRight = new PointF(
                    (float)(cosPhi * z - sinPhi * z) + Camera.X,
                    (float)(-sinPhi * z - cosPhi * z) + Camera.Y);

                var dx = (pRight.X - pLeft.X) / Camera.ScreenWidth;
                var dy = (pRight.Y - pLeft.Y) / Camera.ScreenWidth;

                for (var i = 0; i < Camera.ScreenWidth; i++)
                {
                    var diffuseX = (int)pLeft.X;
                    var diffuseY = (int)pLeft.Y;
                    var heightX = (int)pLeft.X;
                    var heightY = (int)pLeft.Y;

                    Color color;
                    int heightOfHeightMap;

                    if (CifFormat)
                    {
                        var x = diffuseX & (_colorWidth - 1);
                        var y = diffuseY & (_colorHeight - 1);
                        var id = (y * _colorWidth) + x;
                        color = _colorMapCif.GetColor(id);

                        x = heightX & (_topographyWidth - 1);
                        y = heightY & (_topographyHeight - 1);
                        id = (y * _colorWidth) + x;
                        heightOfHeightMap = _heightMapCif.GetColor(id).R;
                    }
                    else
                    {
                        heightOfHeightMap =
                            _heightMap[heightX & (_topographyWidth - 1), heightY & (_topographyHeight - 1)];

                        color = _colorMap[diffuseX & (_colorWidth - 1), diffuseY & (_colorHeight - 1)];
                    }

                    var heightOnScreen = (Camera.Height - heightOfHeightMap) / z * Camera.Scale + Camera.Horizon;

                    _raster.Add(new Slice
                    {
                        Shade = color,
                        X1 = i,
                        Y1 = (int)heightOnScreen,
                        Y2 = (int)_yBuffer[i]
                    });

                    if (heightOnScreen < _yBuffer[i])
                        _yBuffer[i] = heightOnScreen;

                    pLeft.X += dx;
                    pLeft.Y += dy;
                }

                z += dz;
                dz += 0.005f;
            }

            return CreateBitmapFromContainer();
        }

        /// <summary>
        /// Creates a Bitmap using the data in the _raster container.
        /// </summary>
        /// <returns>The created Bitmap.</returns>
        private Bitmap CreateBitmapFromContainer()
        {
            var bmp = ClearFrame();

            foreach (var slice in _raster)
                DrawVerticalLine(slice.Shade, slice.X1, slice.Y1, slice.Y2, bmp);

            return bmp;
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
                    Camera.X -= (int) (10 * ExtendedMath.CalcSin(Camera.Angle));
                    Camera.Y -= (int) (10 * ExtendedMath.CalcCos(Camera.Angle));
                    break;
                case Key.S: //Backward
                    Camera.X += (int) (10 * ExtendedMath.CalcSin(Camera.Angle));
                    Camera.Y += (int) (10 * ExtendedMath.CalcCos(Camera.Angle));
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
        ///     Generates the slice.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="x">The x.</param>
        /// <param name="heightOnScreen">The height on screen.</param>
        /// <param name="buffer">The buffer.</param>
        private void GenerateSlice(Color color, int x, int heightOnScreen, float buffer)
        {
            var slice = new Slice
            {
                Shade = color,
                X1 = x,
                Y1 = heightOnScreen,
                Y2 = buffer
            };

            _raster.Add(slice);
        }

        /// <summary>
        ///     Draws the vertical line.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="x">The x.</param>
        /// <param name="heightOnScreen">The height on screen.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bmp">The bitmap we draw on</param>
        private static void DrawVerticalLine(Color col, int x, int heightOnScreen, float buffer, Bitmap bmp)
        {
            if (heightOnScreen > buffer) return;

            using var g = Graphics.FromImage(bmp);
            g.DrawLine(new Pen(new SolidBrush(col)), x, heightOnScreen, x, buffer);
        }

        /// <summary>
        /// Clears the frame.
        /// </summary>
        /// <returns>A new frame to draw on</returns>
        private Bitmap ClearFrame()
        {
            var bmp = new Bitmap(Camera.ScreenWidth, Camera.ScreenHeight);

            using var g = Graphics.FromImage(bmp);

            //set background Color
            var backGround = new SolidBrush(Camera.BackgroundColor);

            g.FillRectangle(backGround, 0, 0, Camera.ScreenWidth, Camera.ScreenHeight);

            return bmp;
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

            _heightMapCif= new Cif(bmp);

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

            _colorMapCif = new Cif(bmp);

            _colorMap = new Color[bmp.Width, bmp.Height];
            for (var i = 0; i < bmp.Width; i++)
                for (var j = 0; j < bmp.Height; j++)
                    _colorMap[i, j] = dbm.GetPixel(i, j);
        }
    }
}