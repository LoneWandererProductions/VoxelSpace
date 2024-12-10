using Mathematics;
using System.Collections.Generic;
using System.Drawing;

namespace Voxels
{
    internal static class Raster
    {
        internal static List<Slice> GenerateRaster(Color[,] colorMap, int[,] heightMap, Camera camera, int topographyHeight, int topographyWidth, int colorHeight, int colorWidth)
        {
            var raster = new List<Slice>();
            var yBuffer = new float[camera.ScreenWidth];

            for (var i = 0; i < yBuffer.Length; i++)
                yBuffer[i] = camera.ScreenHeight;

            var sinPhi = ExtendedMath.CalcSin(camera.Angle);
            var cosPhi = ExtendedMath.CalcCos(camera.Angle);

            float z = 1;
            float dz = 1;

            while (z < camera.ZFar)
            {
                var pLeft = new PointF(
                    (float)(-cosPhi * z - sinPhi * z) + camera.X,
                    (float)(sinPhi * z - cosPhi * z) + camera.Y);

                var pRight = new PointF(
                    (float)(cosPhi * z - sinPhi * z) + camera.X,
                    (float)(-sinPhi * z - cosPhi * z) + camera.Y);

                var dx = (pRight.X - pLeft.X) / camera.ScreenWidth;
                var dy = (pRight.Y - pLeft.Y) / camera.ScreenWidth;

                for (var i = 0; i < camera.ScreenWidth; i++)
                {
                    var diffuseX = (int)pLeft.X;
                    var diffuseY = (int)pLeft.Y;
                    var heightX = (int)pLeft.X;
                    var heightY = (int)pLeft.Y;

                    Color color;
                    int heightOfHeightMap;

                    heightOfHeightMap =
                        heightMap[heightX & (topographyWidth - 1), heightY & (topographyHeight - 1)];

                    color = colorMap[diffuseX & (colorWidth - 1), diffuseY & (colorHeight - 1)];


                    var heightOnScreen = (camera.Height - heightOfHeightMap) / z * camera.Scale + camera.Horizon;

                    raster.Add(new Slice
                    {
                        Shade = color,
                        X1 = i,
                        Y1 = (int)heightOnScreen,
                        Y2 = (int)yBuffer[i]
                    });

                    if (heightOnScreen < yBuffer[i])
                        yBuffer[i] = heightOnScreen;

                    pLeft.X += dx;
                    pLeft.Y += dy;
                }

                z += dz;
                dz += 0.005f;
            }

            return raster;
        }

        internal static Bitmap CreateBitmapFromContainer(List<Slice> raster, int screenWidth, int screenHeight, Color backgroundColor)
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

        private static void DrawVerticalLine(Color col, int x, int heightOnScreen, float buffer, Bitmap bmp)
        {
            if (heightOnScreen > buffer) return;

            using var g = Graphics.FromImage(bmp);
            g.DrawLine(new Pen(new SolidBrush(col)), x, heightOnScreen, x, buffer);
        }
    }
}