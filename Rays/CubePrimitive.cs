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
        double startX, double startY, double rayDirX, double rayDirY,
        double zStart, double zEnd, double cellSize)
    {
        double hitX = 0, hitY = 0;
        double t = 0;

        double cellX = Math.Floor(startX / cellSize) * cellSize;
        double cellY = Math.Floor(startY / cellSize) * cellSize;

        if (rayDirX != 0)
        {
            double tx = (rayDirX > 0 ? cellX + cellSize : cellX) - startX;
            tx /= rayDirX;
            hitX = startX + rayDirX * tx;
            hitY = startY + rayDirY * tx;
            t = tx;
        }
        else if (rayDirY != 0)
        {
            double ty = (rayDirY > 0 ? cellY + cellSize : cellY) - startY;
            ty /= rayDirY;
            hitX = startX + rayDirX * ty;
            hitY = startY + rayDirY * ty;
            t = ty;
        }

        if (hitX < cellX || hitX > cellX + cellSize || hitY < cellY || hitY > cellY + cellSize)
            return (false, double.MaxValue, 0, 0);

        if (Texture == null)
            return (true, t, 0, 0);

        int texX = (int)((hitX - cellX) / cellSize * Texture.Width);
        int texY = (int)((zStart / Height) * Texture.Height);

        texX = Math.Clamp(texX, 0, Texture.Width - 1);
        texY = Math.Clamp(texY, 0, Texture.Height - 1);

        return (true, t, texX, texY);
    }
}
