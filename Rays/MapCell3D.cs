using System.Collections.Generic;
using System.Numerics;

namespace Rays;

/// <summary>
/// Represents one cell of the dungeon map.
/// </summary>
public class MapCell3D
{
    // Walls
    public bool HasWallNorth { get; set; }
    public bool HasWallSouth { get; set; }
    public bool HasWallEast { get; set; }
    public bool HasWallWest { get; set; }

    // Floor / Ceiling
    public bool HasFloor { get; set; } = true;
    public bool HasCeiling { get; set; } = true;

    public float FloorHeight { get; set; } = 0f;
    public float CeilingHeight { get; set; } = 2.5f;

    // Textures
    public int? WallTextureId { get; set; }
    public int? FloorTextureId { get; set; }
    public int? CeilingTextureId { get; set; }

    // Layers (water, grass, lava, etc.)
    public List<CellLayer> Layers { get; } = new();

    // Precomputed corners (8 corners of the cell cube)
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
}
