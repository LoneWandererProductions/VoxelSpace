/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        CellPrimitive.cs
 * PURPOSE:     3D raycaster with per-cell primitives (cubes, ramps, multi-layered)
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using Imaging;

namespace Rays
{
    /// <summary>
    /// Abstract base for all cell primitives (wall, floor, ceiling, cube, ramp, etc).
    /// </summary>
    public abstract class CellPrimitive
    {
        public PrimitiveType Type { get; }
        public int TextureId { get; }

        /// <summary>
        /// Optional texture reference (set by renderer or asset manager).
        /// </summary>
        public DirectBitmap? Texture { get; set; }

        protected CellPrimitive(PrimitiveType type, int textureId)
        {
            Type = type;
            TextureId = textureId;
        }

        /// <summary>
        /// Intersects a ray with this primitive.
        /// </summary>
        public abstract (bool Hit, double Distance, int TexX, int TexY) IntersectRay(
            double startX, double startY,
            double rayDirX, double rayDirY,
            double zStart, double zEnd,
            double cellSize);
    }

    /// <summary>
    /// A wall segment inside a map cell.
    /// </summary>
    public sealed class WallPrimitive : CellPrimitive
    {
        public WallPrimitive(int textureId)
            : base(PrimitiveType.Wall, textureId) { }

        public override (bool Hit, double Distance, int TexX, int TexY) IntersectRay(
            double startX, double startY,
            double rayDirX, double rayDirY,
            double zStart, double zEnd,
            double cellSize)
        {
            // TODO: implement wall intersection
            return (false, double.MaxValue, 0, 0);
        }
    }

    /// <summary>
    /// The floor surface of a cell.
    /// </summary>
    public sealed class FloorPrimitive : CellPrimitive
    {
        public FloorPrimitive(int textureId)
            : base(PrimitiveType.Floor, textureId) { }

        public override (bool Hit, double Distance, int TexX, int TexY) IntersectRay(
            double startX, double startY,
            double rayDirX, double rayDirY,
            double zStart, double zEnd,
            double cellSize)
        {
            // TODO: implement floor intersection
            return (false, double.MaxValue, 0, 0);
        }
    }

    /// <summary>
    /// The ceiling surface of a cell.
    /// </summary>
    public sealed class CeilingPrimitive : CellPrimitive
    {
        public CeilingPrimitive(int textureId)
            : base(PrimitiveType.Ceiling, textureId) { }

        public override (bool Hit, double Distance, int TexX, int TexY) IntersectRay(
            double startX, double startY,
            double rayDirX, double rayDirY,
            double zStart, double zEnd,
            double cellSize)
        {
            // TODO: implement ceiling intersection
            return (false, double.MaxValue, 0, 0);
        }
    }

    /// <summary>
    /// A sloped surface (e.g., stairs or ramps).
    /// </summary>
    public sealed class RampPrimitive : CellPrimitive
    {
        public RampPrimitive(int textureId)
            : base(PrimitiveType.Ramp, textureId) { }

        public override (bool Hit, double Distance, int TexX, int TexY) IntersectRay(
            double startX, double startY,
            double rayDirX, double rayDirY,
            double zStart, double zEnd,
            double cellSize)
        {
            // TODO: implement ramp intersection
            return (false, double.MaxValue, 0, 0);
        }
    }

    /// <summary>
    /// A cube object inside the world (block, crate, pillar).
    /// </summary>
    public sealed class CubePrimitive : CellPrimitive
    {
        public CubePrimitive(int textureId)
            : base(PrimitiveType.Cube, textureId) { }

        public override (bool Hit, double Distance, int TexX, int TexY) IntersectRay(
            double startX, double startY,
            double rayDirX, double rayDirY,
            double zStart, double zEnd,
            double cellSize)
        {
            // TODO: implement cube intersection
            return (false, double.MaxValue, 0, 0);
        }
    }

    /// <summary>
    /// Non-blocking decoration (torch, statue, plant, etc).
    /// </summary>
    public sealed class DecorationPrimitive : CellPrimitive
    {
        public DecorationPrimitive(int textureId)
            : base(PrimitiveType.Decoration, textureId) { }

        public override (bool Hit, double Distance, int TexX, int TexY) IntersectRay(
            double startX, double startY,
            double rayDirX, double rayDirY,
            double zStart, double zEnd,
            double cellSize)
        {
            // TODO: implement decoration intersection
            return (false, double.MaxValue, 0, 0);
        }
    }
}

