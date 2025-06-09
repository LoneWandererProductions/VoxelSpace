using System;
using System.Drawing;
using Imaging;
using Viewer;

namespace Rays
{
    public class TexturedFloorCeilingRenderer : IFloorCeilingRenderer
    {
        private readonly Bitmap _floorTexture;
        private readonly Bitmap _ceilingTexture;

        public TexturedFloorCeilingRenderer(Bitmap floor, Bitmap ceiling)
        {
            _floorTexture = floor;
            _ceilingTexture = ceiling;
        }

        public void Render(DirectBitmap dbm, RvCamera camera, CameraContext context)
        {
            // Classic raycasting floor rendering
            for (var y = context.ScreenHeight / 2 + 1; y < context.ScreenHeight; y++)
            {
                var rowDistance = context.ScreenHeight / (2.0 * y - context.ScreenHeight);

                var floorStepX = rowDistance * Math.Cos(DegreeToRadian(camera.Angle));
                var floorStepY = rowDistance * Math.Sin(DegreeToRadian(camera.Angle));

                var floorX = camera.X + rowDistance * Math.Cos(DegreeToRadian(camera.Angle));
                var floorY = camera.Y + rowDistance * Math.Sin(DegreeToRadian(camera.Angle));

                for (var x = 0; x < context.ScreenWidth; x++)
                {
                    var cellX = (int)(floorX) % _floorTexture.Width;
                    var cellY = (int)(floorY) % _floorTexture.Height;

                    var floorColor = _floorTexture.GetPixel(cellX, cellY);
                    var ceilingColor = _ceilingTexture.GetPixel(cellX, cellY);

                    dbm.SetPixel(x, y, floorColor);
                    dbm.SetPixel(x, context.ScreenHeight - y, ceilingColor);

                    floorX += floorStepX;
                    floorY += floorStepY;
                }
            }
        }

        private static double DegreeToRadian(double degree) => degree * Math.PI / 180.0;
    }
}
