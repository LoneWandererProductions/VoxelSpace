// ReSharper disable PossibleLossOfFraction

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Input;
using Imaging;

namespace VoxelEngine
{
    /// <summary>
    ///     https://www.youtube.com/watch?v=bQBY9BM9g_Y
    /// </summary>
    public sealed class VoxelEngine
    {
        private readonly Camera _camera = new();

        /// <summary>
        ///     The scale factor
        /// </summary>
        private readonly double _scaleFactor = 70.0;

        private readonly int _screenHeight = 519;

        private readonly int _screenWidth = 772;

        private int _distance = 300;

        private Bitmap _bmp;

        private int _colorHeight;

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

        public VoxelEngine()
        {
            var bmp = new Bitmap(Image.FromFile(string.Concat(Directory.GetCurrentDirectory(), "\\Terrain\\C1W.png")));
            ProcessColorMap(bmp);

            bmp = new Bitmap(Image.FromFile(string.Concat(Directory.GetCurrentDirectory(), "\\Terrain\\D1.png")));
            ProcessHeightMap(bmp);
        }

        public Bitmap Render(PointF point, double degrees, int height, int horizon, int scaleHeight, int distance)
        {
            ClearFrame();

            _yBuffer = new float[_screenWidth];

            for (var i = 0; i < _yBuffer.Length; i++) _yBuffer[i] = _screenHeight;


            var sinPhi = Math.Sin(Math.PI * degrees / 180d);
            var cosPhi = Math.Cos(Math.PI * degrees / 180);

            float z = 1;
            float dz = 1;

            while (z < distance)
            {
                var pLeft = new PointF(
                    (float)(-cosPhi * z - sinPhi * z) + point.X,
                    (float)(sinPhi * z - cosPhi * z) + point.Y);
                var pRight = new PointF(
                    (float)(cosPhi * z - sinPhi * z) + point.X,
                    (float)(-sinPhi * z - cosPhi * z) + point.Y);

                var dx = (pRight.X - pLeft.X) / _screenWidth;
                var dy = (pRight.Y - pLeft.Y) / _screenWidth;

                for (var i = 0; i < _screenWidth; i++)
                {
                    var diffuseX = (int)pLeft.X & (_colorWidth - 1);
                    var diffuseY = (int)pLeft.Y & (_colorHeight - 1);
                    var heightX = (int)pLeft.X & (_topographyWidth - 1);
                    var heightY = (int)pLeft.Y & (_topographyHeight - 1);

                    float heightOfHeightMap = _heightMap[heightX & (_topographyWidth - 1), heightY & (_topographyHeight - 1)];

                    var heightOnScreen = (height - heightOfHeightMap) / z * scaleHeight + horizon;
                    var color = _colorMap[diffuseX & (_colorWidth - 1), diffuseY & (_colorHeight - 1)];

                    DrawVerticalLine(color, i, (int)heightOnScreen, _yBuffer[i], _bmp);

                    if (heightOnScreen < _yBuffer[i]) _yBuffer[i] = heightOnScreen;
                    pLeft.X += dx;
                    pLeft.Y += dy;
                }

                z += dz;
                dz += 0.005f;
            }

            //var bmp = _dbm.Bitmap;
            _bmp.Save("Result.jpg", ImageFormat.Jpeg);

            return _bmp;
        }

        private void DrawVerticalLine(Color col, int x, int heightOnScreen, float buffer, Image bmp)
        {
            if (heightOnScreen > buffer) return;

            using var g = Graphics.FromImage(bmp);
            g.DrawLine(new Pen(new SolidBrush(col)), x, heightOnScreen, x, buffer);
        }

        public void KeyInput()
        {
            if (Keyboard.IsKeyDown(Key.Up))
            {
                _camera.X += Math.Cos(_camera.Angle);
                _camera.Y += Math.Sin(_camera.Angle);
            }

            if (Keyboard.IsKeyDown(Key.Down))
            {
                _camera.X -= Math.Cos(_camera.Angle);
                _camera.Y -= Math.Sin(_camera.Angle);
            }

            if (Keyboard.IsKeyDown(Key.Left)) _camera.Angle -= 0.01;
            if (Keyboard.IsKeyDown(Key.Right)) _camera.Angle += 0.01;
            if (Keyboard.IsKeyDown(Key.E)) _camera.Height++;
            if (Keyboard.IsKeyDown(Key.D)) _camera.Height--;
            if (Keyboard.IsKeyDown(Key.S)) _camera.Horizon += 1.5;
            if (Keyboard.IsKeyDown(Key.W)) _camera.Horizon -= 1.5;
        }

        private void ClearFrame()
        {
            _bmp = new Bitmap(_screenWidth, _screenHeight);

            using var g = Graphics.FromImage(_bmp);

            //set background Color
            var color = Color.FromArgb(0, 36, 36, 56);

            var b = new SolidBrush(color);

            g.FillRectangle(b, 0, 0, _screenWidth, _screenHeight);

            _bmp.Save("Frame.jpg", ImageFormat.Jpeg);
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