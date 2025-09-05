namespace Rays;

/// <summary>
///     Simple grid-based dungeon map.
/// </summary>
public class DungeonMap
{
    private readonly MapCell3D[,,] _cells;

    public int Width { get; }
    public int Height { get; }
    public int Levels { get; }   // Z layers

    public float BaseHeight { get; set; } = 2.5f; // default cell height


    public DungeonMap(int width, int height, int levels)
    {
        Width = width;
        Height = height;
        Levels = levels;
        _cells = new MapCell3D[width, height, levels];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < levels; z++)
                    _cells[x, y, z] = new MapCell3D(x, y, z);
    }

    public MapCell3D GetCell(int x, int y, int z)
    {
        return _cells[x, y, z];
    }
}

