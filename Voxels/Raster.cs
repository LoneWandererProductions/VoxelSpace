using Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Voxels
{
    internal class Raster
    {
        private float[] _yBuffer;

        internal List<Slice> GenerateRaster(Color[,] colorMap, int[,] heightMap, Camera camera, int topographyHeight, int topographyWidth, int colorHeight, int colorWidth)
        {
            var raster = new List<Slice>();
            _yBuffer = new float[camera.ScreenWidth];

            for (var i = 0; i < _yBuffer.Length; i++)
                _yBuffer[i] = camera.ScreenHeight;

            var sinPhi = ExtendedMath.CalcSin(camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(camera.Angle);

            float z = 1;
            float dz = 1;

            while (z < camera.ZFar)
            {
                // Precompute values that do not change per pixel
                var pLeftX = (float)(-cosPhi * z - sinPhi * z) + camera.X;
                var pLeftY = (float)(sinPhi * z - cosPhi * z) + camera.Y;

                var pRightX = (float)(cosPhi * z - sinPhi * z) + camera.X;
                var pRightY = (float)(-sinPhi * z - cosPhi * z) + camera.Y;

                var dx = (pRightX - pLeftX) / camera.ScreenWidth;
                var dy = (pRightY - pLeftY) / camera.ScreenWidth;

                // Loop through screen width
                for (var i = 0; i < camera.ScreenWidth; i++)
                {
                    var diffuseX = (int)pLeftX;
                    var diffuseY = (int)pLeftY;
                    var heightX = (int)pLeftX;
                    var heightY = (int)pLeftY;

                    // Access height map and color map for each pixel
                    int heightOfHeightMap = heightMap[heightX & (topographyWidth - 1), heightY & (topographyHeight - 1)];
                    Color color = colorMap[diffuseX & (colorWidth - 1), diffuseY & (colorHeight - 1)];

                    // Calculate height on screen
                    var heightOnScreen = (camera.Height - heightOfHeightMap) / z * camera.Scale + camera.Horizon;

                    // Add the slice to the raster
                    raster.Add(new Slice
                    {
                        Shade = color,
                        X1 = i,
                        Y1 = (int)heightOnScreen,
                        Y2 = (int)_yBuffer[i]
                    });

                    // Update the buffer for the next iteration
                    if (heightOnScreen < _yBuffer[i])
                        _yBuffer[i] = heightOnScreen;

                    // Update pLeft for the next pixel in the row
                    pLeftX += dx;
                    pLeftY += dy;
                }

                // Move to the next slice
                z += dz;
                dz += 0.005f;  // Increment dz for the next depth slice
            }

            return raster;
        }


        internal Bitmap CreateBitmapFromContainer(List<Slice> raster, int screenWidth, int screenHeight, Color backgroundColor)
        {
            {
                var bmp = new Bitmap(screenWidth, screenHeight);

                using var g = Graphics.FromImage(bmp);

                //set background Color
                var backGround = new SolidBrush(backgroundColor);

                g.FillRectangle(backGround, 0, 0, screenWidth, screenHeight);

                foreach (var slice in raster)
                    DrawVerticalLine(slice.Shade, slice.X1, slice.Y1, slice.Y2, bmp);

                return bmp;
            }
        }

        private void DrawVerticalLine(Color col, int x, int heightOnScreen, float buffer, Bitmap bmp)
        {
            if (heightOnScreen > buffer) return;

            using var g = Graphics.FromImage(bmp);
            g.DrawLine(new Pen(new SolidBrush(col)), x, heightOnScreen, x, buffer);
        }
    }
}