// ReSharper disable PossibleLossOfFraction

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Imaging;
using Mathematics;

namespace VoxelEngine
{
    /// <summary>
    ///     https://www.youtube.com/watch?v=bQBY9BM9g_Y
    /// </summary>
    public sealed class VoxelRaster
    {
        private readonly Camera _camera = new();

        private readonly int _screenHeight = 480;

        private readonly int _screenWidth = 640;

        private Bitmap _bmp;

        private int _colorHeight;

        private readonly List<Slice> _raster = new();

        /// <summary>
        ///     The color map
        ///     Buffer/array to hold color values (1024*1024)
        /// </summary>
        private Color[,] _colorMap;

        private int _colorWidth;

        /// <summary>
        ///     The height map
        ///     Buffer/array to hold height values (1024*1024)
        /// </summary>
        private int[,] _heightMap;

        private int _topographyHeight;

        private int _topographyWidth;

        /// <summary>
        ///     The y buffer
        /// </summary>
        private float[] _yBuffer;

        public VoxelRaster(int x, int y, int degree, int height, int horizon, int scale, int distance)
        {
            _camera = new Camera
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

        public Bitmap Render()
        {
            ClearFrame();

            _yBuffer = new float[_screenWidth];

            for (var i = 0; i < _yBuffer.Length; i++) _yBuffer[i] = _screenHeight;

            var sinPhi = ExtendedMath.CalcSin(_camera.Angle); //Math.Sin(Math.PI * degrees / 180d);
            var cosPhi = ExtendedMath.CalcCos(_camera.Angle); //Math.Cos(Math.PI * degrees / 180);

            float z = 1;
            float dz = 1;

            while (z < _camera.ZFar)
            {
                var pLeft = new PointF(
                    (float)(-cosPhi * z - sinPhi * z) + _camera.X,
                    (float)(sinPhi * z - cosPhi * z) + _camera.Y);
                var pRight = new PointF(
                    (float)(cosPhi * z - sinPhi * z) + _camera.X,
                    (float)(-sinPhi * z - cosPhi * z) + _camera.Y);

                var dx = (pRight.X - pLeft.X) / _screenWidth;
                var dy = (pRight.Y - pLeft.Y) / _screenWidth;

                for (var i = 0; i < _screenWidth; i++)
                {
                    var diffuseX = (int)pLeft.X & (_colorWidth - 1);
                    var diffuseY = (int)pLeft.Y & (_colorHeight - 1);
                    var heightX = (int)pLeft.X & (_topographyWidth - 1);
                    var heightY = (int)pLeft.Y & (_topographyHeight - 1);

                    float heightOfHeightMap = _heightMap[heightX & (_topographyWidth - 1), heightY & (_topographyHeight - 1)];

                    var heightOnScreen = (_camera.Height - heightOfHeightMap) / z * _camera.Scale + _camera.Horizon;
                    var color = _colorMap[diffuseX & (_colorWidth - 1), diffuseY & (_colorHeight - 1)];

                    GenerateSlice(color,i, (int)heightOnScreen, _yBuffer[i]);

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
                X2 = x,
                Y1 = heightOnScreen,
                Y2 = buffer
            };

            _raster.Add(slice);
        }

        private void DrawRaster()
        {
            foreach (var slice in _raster)
            {
                DrawVerticalLine(slice.Shade, slice.X1, slice.Y1, slice.Y2);
            }
        }

        private void DrawVerticalLine(Color col, int x, int heightOnScreen, float buffer)
        {
            if (heightOnScreen > buffer) return;

            using var g = Graphics.FromImage(_bmp);
            g.DrawLine(new Pen(new SolidBrush(col)), x, heightOnScreen, x, buffer);
        }

        public void KeyInput()
        {
            if (Keyboard.IsKeyDown(Key.Up))
            {
                //_camera.X += Math.Cos(_camera.Angle);
                //_camera.Y += Math.Sin(_camera.Angle);
            }

            if (Keyboard.IsKeyDown(Key.Down))
            {
                //_camera.X -= Math.Cos(_camera.Angle);
                //_camera.Y -= Math.Sin(_camera.Angle);
            }

            if (Keyboard.IsKeyDown(Key.Left)) _camera.Angle--;
            if (Keyboard.IsKeyDown(Key.Right)) _camera.Angle++;
            if (Keyboard.IsKeyDown(Key.E)) _camera.Height++;
            if (Keyboard.IsKeyDown(Key.D)) _camera.Height--;
            if (Keyboard.IsKeyDown(Key.S)) _camera.Horizon++;
            if (Keyboard.IsKeyDown(Key.W)) _camera.Horizon--;
        }

        private void ClearFrame()
        {
            _bmp = new Bitmap(_screenWidth, _screenHeight);

            using var g = Graphics.FromImage(_bmp);

            //set background Color
            var color = Color.FromArgb(0, 36, 36, 56);

            var b = new SolidBrush(color);

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