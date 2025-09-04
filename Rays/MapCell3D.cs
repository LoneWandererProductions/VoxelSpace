using System.Collections.Generic;
using System.Numerics;

namespace Rays;

/// <summary>
/// Represents one cell of the dungeon map.
/// </summary>
public class MapCell3D
{
    // --- Walls ---
    public bool HasWallNorth { get; set; }
    public bool HasWallSouth { get; set; }
    public bool HasWallEast { get; set; }
    public bool HasWallWest { get; set; }

    /// <summary>
    /// Thickness of walls (depth into the cell space).
    /// </summary>
    public float WallThickness { get; set; } = 0.1f;

    // --- Floor / Ceiling ---
    public bool HasFloor { get; set; } = true;
    public bool HasCeiling { get; set; } = true;

    public float FloorHeight { get; set; } = 0f;
    public float CeilingHeight { get; set; } = 2.5f;

    /// <summary>
    /// Thickness of floor slab (solid volume below the walkable floor).
    /// </summary>
    public float FloorThickness { get; set; } = 0.2f;

    /// <summary>
    /// Thickness of ceiling slab (solid volume above the ceiling).
    /// </summary>
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
    /// Precompute the 8 corners of this cell cube at integer grid coords.
    /// </summary>
    public void PrecomputeCorners(int x, int y)
    {
        float f = FloorHeight;
        float c = CeilingHeight;

        PrecomputedCorners[0] = new Vector3(x, f, y);
        PrecomputedCorners[1] = new Vector3(x + 1, f, y);
        PrecomputedCorners[2] = new Vector3(x, f, y + 1);
        PrecomputedCorners[3] = new Vector3(x + 1, f, y + 1);
        PrecomputedCorners[4] = new Vector3(x, c, y);
        PrecomputedCorners[5] = new Vector3(x + 1, c, y);
        PrecomputedCorners[6] = new Vector3(x, c, y + 1);
        PrecomputedCorners[7] = new Vector3(x + 1, c, y + 1);
    }

    /// <summary>
    /// Get the bounding box for a wall (if it exists).
    /// </summary>
    public (Vector3 min, Vector3 max)? GetWallBounds(Direction dir, int cellX, int cellY)
    {
        float f = FloorHeight;
        float c = CeilingHeight;
        float t = WallThickness;

        switch (dir)
        {
            case Direction.North when HasWallNorth:
                return (new Vector3(cellX, f, cellY),
                        new Vector3(cellX + 1, c, cellY + t));

            case Direction.South when HasWallSouth:
                return (new Vector3(cellX, f, cellY + 1 - t),
                        new Vector3(cellX + 1, c, cellY + 1));

            case Direction.West when HasWallWest:
                return (new Vector3(cellX, f, cellY),
                        new Vector3(cellX + t, c, cellY + 1));

            case Direction.East when HasWallEast:
                return (new Vector3(cellX + 1 - t, f, cellY),
                        new Vector3(cellX + 1, c, cellY + 1));
        }

        return null;
    }

    /// <summary>
    /// Get the bounding box for the floor slab (if any).
    /// </summary>
    public (Vector3 min, Vector3 max)? GetFloorBounds(int cellX, int cellY)
    {
        if (!HasFloor || FloorThickness <= 0f) return null;

        return (new Vector3(cellX, FloorHeight - FloorThickness, cellY),
                new Vector3(cellX + 1, FloorHeight, cellY + 1));
    }

    /// <summary>
    /// Get the bounding box for the ceiling slab (if any).
    /// </summary>
    public (Vector3 min, Vector3 max)? GetCeilingBounds(int cellX, int cellY)
    {
        if (!HasCeiling || CeilingThickness <= 0f) return null;

        return (new Vector3(cellX, CeilingHeight, cellY),
                new Vector3(cellX + 1, CeilingHeight + CeilingThickness, cellY + 1));
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
