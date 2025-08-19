/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        TexturedFloorCeilingRendererV3.cs
 * PURPOSE:     Raycasting floor and ceiling renderer for RaycasterV3
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using Imaging;
using Viewer;

namespace Rays
{
    /// <summary>
    /// Per-pixel textured floor and ceiling renderer for RaycasterV3.
    /// </summary>
    public class TexturedFloorCeilingRendererV3 : IFloorCeilingRenderer
    {
        private readonly DirectBitmap _floorTexture;
        private readonly DirectBitmap _ceilingTexture;

        /// <summary>
        /// Creates a floor/ceiling renderer with given textures.
        /// </summary>
        /// <param name="floor">DirectBitmap for the floor texture</param>
        /// <param name="ceiling">DirectBitmap for the ceiling texture</param>
        public TexturedFloorCeilingRendererV3(DirectBitmap floor, DirectBitmap ceiling)
        {
            _floorTexture = floor;
            _ceilingTexture = ceiling;
        }

        /// <summary>
        /// Renders floor and ceiling for the given camera and screen context.
        /// </summary>
        public void Render(DirectBitmap dbm, RvCamera camera, CameraContext context)
        {
            int screenWidth = context.ScreenWidth;
            int screenHeight = context.ScreenHeight;
            double halfHeight = screenHeight / 2.0;

            for (int y = 0; y < screenHeight; y++)
            {
                // Skip rows above horizon for floor and below horizon for ceiling
                bool isFloor = y > halfHeight;
                if (!isFloor && y < halfHeight) continue;

                // Distance from camera to the row in world space
                double rowDistance = (camera.Z) / (y - halfHeight);
                if (rowDistance <= 0.0) continue;

                for (int x = 0; x < screenWidth; x++)
                {
                    // Compute ray direction for this pixel
                    double rayAngle = camera.Angle - context.Fov / 2.0 + x * (context.Fov / screenWidth);
                    double rayDirX = Math.Cos(DegreeToRadian(rayAngle));
                    double rayDirY = Math.Sin(DegreeToRadian(rayAngle));

                    // World coordinates on floor/ceiling
                    double worldX = camera.X + rowDistance * rayDirX;
                    double worldY = camera.Y + rowDistance * rayDirY;

                    // Wrap texture coordinates
                    int texX = ((int)worldX) % _floorTexture.Width;
                    int texY = ((int)worldY) % _floorTexture.Height;
                    if (texX < 0) texX += _floorTexture.Width;
                    if (texY < 0) texY += _floorTexture.Height;

                    // Get color from correct texture
                    var color = isFloor
                        ? _floorTexture.GetPixel(texX, texY)
                        : _ceilingTexture.GetPixel(texX, texY);

                    // Draw pixel
                    int drawY = isFloor ? y : (int)(screenHeight - y - 1);
                    dbm.SetPixel(x, drawY, color);
                }
            }
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        private static double DegreeToRadian(double degree) => degree * Math.PI / 180.0;
    }
}
