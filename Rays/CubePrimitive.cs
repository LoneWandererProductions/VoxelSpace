/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        CubePrimitive.cs
 * PURPOSE:     3D raycaster with per-cell primitives (cubes, ramps, multi-layered)
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using Rays;
using System;

public class CubePrimitive : CellPrimitive
{
    public double Height { get; set; } = 1.0;

    public CubePrimitive(int textureId)
        : base(PrimitiveType.Cube, textureId)
    {
    }

    public override (bool Hit, double Distance, int TexX, int TexY) IntersectRay(
        double startX, double startY, double rayDirX, double rayDirY, double zStart, double zEnd, double cellSize)
    {
        // Compute cell bounds
        double cellX = Math.Floor(startX / cellSize) * cellSize;
        double cellY = Math.Floor(startY / cellSize) * cellSize;

        // Simple 2D ray intersection against cell bounds
        double tMinX = (rayDirX != 0) ? (rayDirX > 0 ? cellX + cellSize - startX : cellX - startX) / rayDirX : double.MaxValue;
        double tMinY = (rayDirY != 0) ? (rayDirY > 0 ? cellY + cellSize - startY : cellY - startY) / rayDirY : double.MaxValue;

        double t = Math.Min(tMinX, tMinY);
        if (t < 0) return (false, double.MaxValue, 0, 0);

        double hitX = startX + rayDirX * t;
        double hitY = startY + rayDirY * t;

        // Check within cube cell
        if (hitX < cellX || hitX > cellX + cellSize || hitY < cellY || hitY > cellY + cellSize)
            return (false, double.MaxValue, 0, 0);

        // Texture coordinates
        int texX = (int)((hitX - cellX) / cellSize * Texture!.Width);
        int texY = (int)((zStart / Height) * Texture.Height);
        texX = Math.Clamp(texX, 0, Texture.Width - 1);
        texY = Math.Clamp(texY, 0, Texture.Height - 1);

        return (true, t, texX, texY);
    }
}
