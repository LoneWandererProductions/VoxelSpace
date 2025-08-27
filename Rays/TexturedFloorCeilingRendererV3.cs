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

namespace Rays;

/// <summary>
///     Per-pixel textured floor and ceiling renderer for RaycasterV3.
/// </summary>
public class TexturedFloorCeilingRendererV3 : IFloorCeilingRenderer
{
    private readonly DirectBitmap _ceilingTexture;
    private readonly DirectBitmap _floorTexture;

    /// <summary>
    ///     Creates a floor/ceiling renderer with given textures.
    /// </summary>
    /// <param name="floor">DirectBitmap for the floor texture</param>
    /// <param name="ceiling">DirectBitmap for the ceiling texture</param>
    public TexturedFloorCeilingRendererV3(DirectBitmap floor, DirectBitmap ceiling)
    {
        _floorTexture = floor;
        _ceilingTexture = ceiling;
    }

    /// <summary>
    ///     Renders floor and ceiling for the given camera and screen context.
    /// </summary>
    public void Render(DirectBitmap dbm, RvCamera camera, CameraContext context)
    {
        var screenWidth = context.ScreenWidth;
        var screenHeight = context.ScreenHeight;
        var halfHeight = screenHeight / 2.0;

        for (var y = 0; y < screenHeight; y++)
        {
            // Skip rows above horizon for floor and below horizon for ceiling
            var isFloor = y > halfHeight;
            if (!isFloor && y < halfHeight) continue;

            // Distance from camera to the row in world space
            var rowDistance = camera.Z / (y - halfHeight);
            if (rowDistance <= 0.0) continue;

            for (var x = 0; x < screenWidth; x++)
            {
                // Compute ray direction for this pixel
                var rayAngle = camera.Angle - context.Fov / 2.0 + x * (context.Fov / screenWidth);
                var rayDirX = Math.Cos(DegreeToRadian(rayAngle));
                var rayDirY = Math.Sin(DegreeToRadian(rayAngle));

                // World coordinates on floor/ceiling
                var worldX = camera.X + rowDistance * rayDirX;
                var worldY = camera.Y + rowDistance * rayDirY;

                // Wrap texture coordinates
                var texX = (int)worldX % _floorTexture.Width;
                var texY = (int)worldY % _floorTexture.Height;
                if (texX < 0) texX += _floorTexture.Width;
                if (texY < 0) texY += _floorTexture.Height;

                // Get color from correct texture
                var color = isFloor
                    ? _floorTexture.GetPixel(texX, texY)
                    : _ceilingTexture.GetPixel(texX, texY);

                // Draw pixel
                var drawY = isFloor ? y : screenHeight - y - 1;
                dbm.SetPixel(x, drawY, color);
            }
        }
    }

    /// <summary>
    ///     Converts degrees to radians.
    /// </summary>
    private static double DegreeToRadian(double degree)
    {
        return degree * Math.PI / 180.0;
    }
}