using System;
using System.Drawing;
using Imaging;
using Viewer;

namespace Rays
{
    public class TexturedFloorCeilingRenderer : IFloorCeilingRenderer
    {
        private readonly DirectBitmap _floorTexture;
        private readonly DirectBitmap _ceilingTexture;

        public TexturedFloorCeilingRenderer(DirectBitmap floor, DirectBitmap ceiling)
        {
            _floorTexture = floor;
            _ceilingTexture = ceiling;
        }

        public void Render(DirectBitmap dbm, RvCamera camera, CameraContext context)
        {
            // Classic raycasting floor rendering
            for (int y = context.ScreenHeight / 2 + 1; y < context.ScreenHeight; y++)
            {
                double rowDistance = context.ScreenHeight / (2.0 * y - context.ScreenHeight);
                double stepX = rowDistance * Math.Cos(DegreeToRadian(camera.Angle));
                double stepY = rowDistance * Math.Sin(DegreeToRadian(camera.Angle));
                double floorX = camera.X + rowDistance * Math.Cos(DegreeToRadian(camera.Angle));
                double floorY = camera.Y + rowDistance * Math.Sin(DegreeToRadian(camera.Angle));

                // Cache the Y row in the floor and ceiling textures
                int textureRowY = ((int)floorY) % _floorTexture.Height;
                if (textureRowY < 0) textureRowY += _floorTexture.Height;

                int textureCeilingY = ((int)floorY) % _ceilingTexture.Height;
                if (textureCeilingY < 0) textureCeilingY += _ceilingTexture.Height;

                Color[] floorRowCache = _floorTexture.GetRow(textureRowY);
                Color[] ceilingRowCache = _ceilingTexture.GetRow(textureRowY);

                for (int x = 0; x < context.ScreenWidth; x++)
                {
                    int cellX = ((int)floorX) % _floorTexture.Width;
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

        private static double DegreeToRadian(double degree) => degree * Math.PI / 180.0;
    }
}
