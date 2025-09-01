using System.Collections.Generic;
using System.Numerics;

namespace Rays;

/// <summary>
///     Represents one cell of the dungeon map.
/// </summary>
public class MapCell3D
{
    public bool HasWallNorth { get; set; }
    public bool HasWallSouth { get; set; }
    public bool HasWallEast { get; set; }
    public bool HasWallWest { get; set; }

    public bool HasFloor { get; set; } = true;
    public bool HasCeiling { get; set; } = true;

    public float FloorHeight { get; set; } = 0;
    public float CeilingHeight { get; set; } = 2.5f;

    public int? WallTextureId { get; set; }
    public int? FloorTextureId { get; set; }
    public int? CeilingTextureId { get; set; }

    // Generic list of layers
    public List<CellLayer> Layers = new List<CellLayer>();

    // Precomputed corners (8 corners of the cell cube)
    public Vector3[] PrecomputedCorners { get; private set; } = new Vector3[8];

    public void PrecomputeCorners(int x, int y)
    {
        PrecomputedCorners[0] = new Vector3(x, FloorHeight, y);
        PrecomputedCorners[1] = new Vector3(x + 1, FloorHeight, y);
        PrecomputedCorners[2] = new Vector3(x, FloorHeight, y + 1);
        PrecomputedCorners[3] = new Vector3(x + 1, FloorHeight, y + 1);
        PrecomputedCorners[4] = new Vector3(x, CeilingHeight, y);
        PrecomputedCorners[5] = new Vector3(x + 1, CeilingHeight, y);
        PrecomputedCorners[6] = new Vector3(x, CeilingHeight, y + 1);
        PrecomputedCorners[7] = new Vector3(x + 1, CeilingHeight, y + 1);
    }
}
