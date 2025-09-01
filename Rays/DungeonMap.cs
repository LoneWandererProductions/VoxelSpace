namespace Rays;

/// <summary>
///     Simple grid-based dungeon map.
/// </summary>
public class DungeonMap
{
    private readonly MapCell3D[,] _cells;

    public DungeonMap(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new MapCell3D[width, height];

        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
            _cells[x, y] = new MapCell3D();
    }

    public int Width { get; }
    public int Height { get; }

    public MapCell3D GetCell(int x, int y)
    {
        return _cells[x, y];
    }
}
