using System;
using Imaging;
using Viewer;

namespace Rays
{
    public class TexturedFloorCeilingRenderer : IFloorCeilingRenderer
    {
        private readonly DirectBitmap _ceilingTexture;
        private readonly DirectBitmap _floorTexture;

        public TexturedFloorCeilingRenderer(DirectBitmap floor, DirectBitmap ceiling)
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
                var stepX = rowDistance * Math.Cos(DegreeToRadian(camera.Angle));
                var stepY = rowDistance * Math.Sin(DegreeToRadian(camera.Angle));
                var floorX = camera.X + rowDistance * Math.Cos(DegreeToRadian(camera.Angle));
                var floorY = camera.Y + rowDistance * Math.Sin(DegreeToRadian(camera.Angle));

                // Cache the Y row in the floor and ceiling textures
                var textureRowY = (int)floorY % _floorTexture.Height;
                if (textureRowY < 0) textureRowY += _floorTexture.Height;

                var textureCeilingY = (int)floorY % _ceilingTexture.Height;
                if (textureCeilingY < 0) textureCeilingY += _ceilingTexture.Height;

                var floorRowCache = _floorTexture.GetRow(textureRowY);
                var ceilingRowCache = _ceilingTexture.GetRow(textureRowY);

                for (var x = 0; x < context.ScreenWidth; x++)
                {
                    var cellX = (int)floorX % _floorTexture.Width;
                    if (cellX < 0) cellX += _floorTexture.Width;

                    // Use pre-cached row
                    var floorColor = floorRowCache[cellX];
                    var ceilingColor = ceilingRowCache[cellX];

                    dbm.SetPixel(x, y, floorColor);
                    dbm.SetPixel(x, context.ScreenHeight - y, ceilingColor);

                    floorX += stepX;
                    floorY += stepY;
                }
            }
        }

        private static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180.0;
        }
    }
}