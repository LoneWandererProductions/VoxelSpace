using System.Collections.Generic;
using System.Numerics;

namespace Rays;

/// <summary>
/// Represents one cell of the dungeon map (stackable in X, Y, Z).
/// </summary>
public class MapCell3D
{
    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public MapCell3D(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    // --- Walls ---
    public bool HasWallNorth { get; set; }
    public bool HasWallSouth { get; set; }
    public bool HasWallEast { get; set; }
    public bool HasWallWest { get; set; }

    /// <summary>Thickness of walls (depth into the cell space).</summary>
    public float WallThickness { get; set; } = 0.1f;

    // --- Floor / Ceiling ---
    public bool HasFloor { get; set; } = true;
    public bool HasCeiling { get; set; } = true;

    /// <summary>Floor offset inside this cell (relative to Z).</summary>
    public float FloorHeight { get; set; } = 0f;

    /// <summary>Ceiling offset inside this cell (relative to Z).</summary>
    public float CeilingHeight { get; set; } = 2.5f;

    /// <summary>Thickness of floor slab (solid volume below the walkable floor).</summary>
    public float FloorThickness { get; set; } = 0.2f;

    /// <summary>Thickness of ceiling slab (solid volume above the ceiling).</summary>
    public float CeilingThickness { get; set; } = 0.2f;

    // --- Textures ---
    public int? NorthWallTextureId { get; set; }
    public int? SouthWallTextureId { get; set; }
    public int? EastWallTextureId { get; set; }
    public int? WestWallTextureId { get; set; }

    public int? WallTextureId { get; set; }
    public int? FloorTextureId { get; set; }
    public int? CeilingTextureId { get; set; }

    // --- Layers (water, grass, lava, etc.) ---
    public List<CellLayer> Layers { get; } = new();

    // --- Precomputed corners (8 corners of the cell cube) ---
    public Vector3[] PrecomputedCorners { get; } = new Vector3[8];

    /// <summary>
    /// Precompute the 8 corners of this cell cube, respecting X, Y, Z.
    /// </summary>
    public void PrecomputeCorners()
    {
        float zOffset = Z * (CeilingHeight - FloorHeight); // base height per layer
        float f = FloorHeight + zOffset;
        float c = CeilingHeight + zOffset;

        PrecomputedCorners[0] = new Vector3(X, f, Y);
        PrecomputedCorners[1] = new Vector3(X + 1, f, Y);
        PrecomputedCorners[2] = new Vector3(X, f, Y + 1);
        PrecomputedCorners[3] = new Vector3(X + 1, f, Y + 1);

        PrecomputedCorners[4] = new Vector3(X, c, Y);
        PrecomputedCorners[5] = new Vector3(X + 1, c, Y);
        PrecomputedCorners[6] = new Vector3(X, c, Y + 1);
        PrecomputedCorners[7] = new Vector3(X + 1, c, Y + 1);
    }

    /// <summary>Get the bounding box for a wall (if it exists).</summary>
    public (Vector3 min, Vector3 max)? GetWallBounds(Direction dir)
    {
        float zOffset = Z * (CeilingHeight - FloorHeight);
        float f = FloorHeight + zOffset;
        float c = CeilingHeight + zOffset;
        float t = WallThickness;

        switch (dir)
        {
            case Direction.North when HasWallNorth:
                return (new Vector3(X, f, Y),
                        new Vector3(X + 1, c, Y + t));

            case Direction.South when HasWallSouth:
                return (new Vector3(X, f, Y + 1 - t),
                        new Vector3(X + 1, c, Y + 1));

            case Direction.West when HasWallWest:
                return (new Vector3(X, f, Y),
                        new Vector3(X + t, c, Y + 1));

            case Direction.East when HasWallEast:
                return (new Vector3(X + 1 - t, f, Y),
                        new Vector3(X + 1, c, Y + 1));
        }

        return null;
    }

    /// <summary>Get the bounding box for the floor slab (if any).</summary>
    public (Vector3 min, Vector3 max)? GetFloorBounds()
    {
        if (!HasFloor || FloorThickness <= 0f) return null;

        float zOffset = Z * (CeilingHeight - FloorHeight);
        float f = FloorHeight + zOffset;

        return (new Vector3(X, f - FloorThickness, Y),
                new Vector3(X + 1, f, Y + 1));
    }

    /// <summary>Get the bounding box for the ceiling slab (if any).</summary>
    public (Vector3 min, Vector3 max)? GetCeilingBounds()
    {
        if (!HasCeiling || CeilingThickness <= 0f) return null;

        float zOffset = Z * (CeilingHeight - FloorHeight);
        float c = CeilingHeight + zOffset;

        return (new Vector3(X, c, Y),
                new Vector3(X + 1, c + CeilingThickness, Y + 1));
    }
}

/// <summary>
/// Cardinal directions for wall lookup.
/// </summary>
public enum Direction
{
    North,
    South,
    East,
    West
}
