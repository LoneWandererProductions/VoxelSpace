//Source: https://github.com/s-macke/VoxelSpace

// ReSharper disable PossibleLossOfFraction

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        /// The Slices of the Image
        /// </summary>
        private List<Slice> _raster;

        private readonly int _screenHeight = 200;

        private readonly int _screenWidth = 300;

        private Bitmap _bmp;

        /// <summary>
        ///     The color map
        ///     Buffer/array to hold color values (1024*1024)
        /// </summary>
        private Color[,] _colorMap;

        /// <summary>
        /// The color height
        /// </summary>
        private int _colorHeight;

        /// <summary>
        /// The color width
        /// </summary>
        private int _colorWidth;

        /// <summary>
        ///     The height map
        ///     Buffer/array to hold height values (1024*1024)
        /// </summary>
        private int[,] _heightMap;

        /// <summary>
        /// The topography height
        /// </summary>
        private int _topographyHeight;

        /// <summary>
        /// The topography width
        /// </summary>
        private int _topographyWidth;

        /// <summary>
        ///     The y buffer
        /// </summary>
        private float[] _yBuffer;

        public VoxelRaster(int x, int y, int degree, int height, int horizon, int scale, int distance)
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

            var bmp = new Bitmap(Image.FromFile(string.Concat(Directory.GetCurrentDirectory(), "\\Terrain\\C1W.png")));
            ProcessColorMap(bmp);

            bmp = new Bitmap(Image.FromFile(string.Concat(Directory.GetCurrentDirectory(), "\\Terrain\\D1.png")));
            ProcessHeightMap(bmp);
        }

        /// <summary>
        ///     Gets or sets the camera.
        ///     Only here just in case if KeyInput is not available
        /// </summary>
        /// <value>
        ///     The camera.
        /// </value>
        private Camera Camera { get; }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>The finished Image</returns>
        public Bitmap Render()
        {
            ClearFrame();
            _raster = new List<Slice>();

            _yBuffer = new float[_screenWidth];

            for (var i = 0; i < _yBuffer.Length; i++) _yBuffer[i] = _screenHeight;

            var sinPhi = ExtendedMath.CalcSin(Camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(Camera.Angle);

            float z = 1;
            float dz = 1;

            while (z < Camera.ZFar)
            {
                var pLeft = new PointF(
                    (float) (-cosPhi * z - sinPhi * z) + Camera.X,
                    (float) (sinPhi * z - cosPhi * z) + Camera.Y);

                var pRight = new PointF(
                    (float) (cosPhi * z - sinPhi * z) + Camera.X,
                    (float) (-sinPhi * z - cosPhi * z) + Camera.Y);

                var dx = (pRight.X - pLeft.X) / _screenWidth;
                var dy = (pRight.Y - pLeft.Y) / _screenWidth;

                for (var i = 0; i < _screenWidth; i++)
                {
                    var diffuseX = (int) pLeft.X & (_colorWidth - 1);
                    var diffuseY = (int) pLeft.Y & (_colorHeight - 1);
                    var heightX = (int) pLeft.X & (_topographyWidth - 1);
                    var heightY = (int) pLeft.Y & (_topographyHeight - 1);

                    float heightOfHeightMap =
                        _heightMap[heightX & (_topographyWidth - 1), heightY & (_topographyHeight - 1)];

                    var heightOnScreen = (Camera.Height - heightOfHeightMap) / z * Camera.Scale + Camera.Horizon;
                    var color = _colorMap[diffuseX & (_colorWidth - 1), diffuseY & (_colorHeight - 1)];

                    GenerateSlice(color, i, (int) heightOnScreen, _yBuffer[i]);

                    if (heightOnScreen < _yBuffer[i]) _yBuffer[i] = heightOnScreen;
                    pLeft.X += dx;
                    pLeft.Y += dy;
                }

                z += dz;
                dz += 0.005f;
            }

            DrawRaster();

            return _bmp;
        }

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

        private void DrawRaster()
        {
            foreach (var slice in _raster) DrawVerticalLine(slice.Shade, slice.X1, slice.Y1, slice.Y2);
        }

        private void DrawVerticalLine(Color col, int x, int heightOnScreen, float buffer)
        {
            if (heightOnScreen > buffer) return;

            using var g = Graphics.FromImage(_bmp);
            g.DrawLine(new Pen(new SolidBrush(col)), x, heightOnScreen, x, buffer);
        }

        public void KeyInput(Key key)
        {
            switch (key)
            {
                case Key.W: //Forward
                    Camera.X -= (float) (10 * Math.Sin(Camera.Angle));
                    Camera.X -= (float) (10 * Math.Cos(Camera.Angle));
                    break;
                case Key.S: //Backward
                    Camera.X += (float) (10 * Math.Sin(Camera.Angle));
                    Camera.Y += (float) (10 * Math.Cos(Camera.Angle));
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

        private void ClearFrame()
        {
            _bmp = null;
            _bmp = new Bitmap(_screenWidth, _screenHeight);

            using var g = Graphics.FromImage(_bmp);

            //set background Color
            var b = new SolidBrush(Camera.BackgroundColor);

            g.FillRectangle(b, 0, 0, _screenWidth, _screenHeight);
        }

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