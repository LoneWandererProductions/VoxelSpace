// ReSharper disable PossibleLossOfFraction

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using Imaging;

namespace VoxelEngine
{
    /// <summary>
    ///     https://www.youtube.com/watch?v=bQBY9BM9g_Y
    /// </summary>
    public class VoxelEngine
    {
        private readonly Camera _camera = new();

        /// <summary>
        /// The y buffer
        /// </summary>
        private float[] _yBuffer;

        private int _width = 1024;

        private int _height = 1024;

        /// <summary>
        ///     The color map
        ///     Buffer/array to hold color values (1024*1024)
        /// </summary>
        private Color[,] _colorMap;

        /// <summary>
        ///     The height map
        ///     Buffer/array to hold height values (1024*1024)
        /// </summary>
        private int[,] _heightMap;

        /// <summary>
        ///     The scale factor
        /// </summary>
        private readonly double _scaleFactor = 70.0;

        private readonly int _screenHeight = 519;

        private readonly int _screenWidth = 772;

        private DirectBitmap _dbm;

        private Bitmap _bmp;

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

            var sinPhi = Math.Sin(Math.PI * degrees / 180);
            var cosphi = Math.Cos(Math.PI * degrees / 180);

            float z = 1;
            float dz = 1;
            while (z < distance)
            {
                var pLeft = new PointF(
                    (float)(-cosphi * z - sinPhi * z) + point.X,
                    (float)(sinPhi * z - cosphi * z) + point.Y);
                var pRight = new PointF(
                    (float)(cosphi * z - sinPhi * z) + point.X,
                    (float)(-sinPhi * z - cosphi * z) + point.Y);

                var dx = (pRight.X - pLeft.X) / _screenWidth;
                var dy = (pRight.Y - pLeft.Y) / _screenWidth;

                for (var i = 0; i < _screenWidth; i++)
                {
                    var diffuseX = (int)pLeft.X & (_width - 1);
                    var diffuseY = (int)pLeft.Y & (_height - 1);
                    var heightX = (int)pLeft.X & (_width - 1);
                    var heightY = (int)pLeft.Y & (_height - 1);

                    //catch out of bound exception
                    float heightOfHeightMap = _heightMap[heightX & (_heightMap.GetLength(1) - 1), heightY & (_heightMap.GetLength(0) - 1)];

                    var heightOnScreen = (height - heightOfHeightMap) / z * scaleHeight + horizon;

                    //catch out of bound exception
                    var color = _colorMap[diffuseX & (_colorMap.GetLength(1) - 1),
                        diffuseY & (_colorMap.GetLength(0) - 1)];

                    //TODO error hre
                    var length = (int)(heightOnScreen - _yBuffer[i]);

                    //DrawVerticalLine(i, (int)heightOnScreen, _yBuffer[i], color, g);
                    if (heightOnScreen > _yBuffer[i]) continue;

                    _dbm = DirectBitmap.GetInstance(_bmp);
                    _dbm.DrawVerticalLine(i, (int)_yBuffer[i], length, color);


                    if (heightOnScreen < _yBuffer[i]) _yBuffer[i] = heightOnScreen;

                    pLeft.X += dx;
                    pLeft.Y += dy;
                }

                z += dz;
                dz += 0.005f;
            }

            return _dbm.Bitmap;
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
            _bmp = new Bitmap(_screenWidth + 1, _screenHeight +1);

            using var g = Graphics.FromImage(_bmp);

            //set background Color
            var color = Color.FromArgb(0, 36, 36, 56);

            var b = new SolidBrush(color);

            g.FillRectangle(b, 0, 0, _screenWidth, _screenHeight);
        }

        private void ProcessHeightMap(Bitmap bmp)
        {
            if (bmp == null) return;
            var heightMap = DirectBitmap.GetInstance(bmp);

            _heightMap = new int[bmp.Width, bmp.Height];
            for (var i = 0; i < bmp.Width; i++)
            for (var j = 0; j < bmp.Height; j++)
                _heightMap[i, j] = heightMap.GetPixel(i, j).R;
        }

        private void ProcessColorMap(Bitmap bmp)
        {
            if (bmp == null) return;
            var colorMap = DirectBitmap.GetInstance(bmp);

            _colorMap = new Color[bmp.Width, bmp.Height];
            for (var i = 0; i < bmp.Width; i++)
            for (var j = 0; j < bmp.Height; j++)
                _colorMap[i, j] = colorMap.GetPixel(i, j);
        }

    }
}