using System;
using System.Drawing;
using System.Numerics;

namespace Rays
{
    /// <summary>
    /// Rendering modes for the dungeon.
    /// </summary>
    public enum RenderMode
    {
        Wireframe,
        Textured
    }

    /// <summary>
    /// Represents a camera in 3D space for the dungeon view.
    /// </summary>
    public class Camera
    {
        public Vector3 Position { get; set; } = new Vector3(0, 1.7f, 0); // Player eye height
        public float Yaw { get; set; } = 0;  // Horizontal rotation
        public float Pitch { get; set; } = 0; // Vertical rotation
        public float Fov { get; set; } = (float)(Math.PI / 3); // 60°

        public Matrix4x4 GetViewMatrix()
        {
            // Build orientation
            var rotation = Matrix4x4.CreateFromYawPitchRoll(Yaw, Pitch, 0);

            // Basis vectors
            var forward = Vector3.TransformNormal(Vector3.UnitZ, rotation);
            var up = Vector3.TransformNormal(Vector3.UnitY, rotation);

            return Matrix4x4.CreateLookAt(Position, Position + forward, up);
        }

        public Matrix4x4 GetProjectionMatrix(float aspectRatio, float near = 0.1f, float far = 100f)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(Fov, aspectRatio, near, far);
        }
    }

    /// <summary>
    /// Represents one cell of the dungeon map.
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

        // Optional texture IDs (null means "use fallback solid color")
        public int? WallTextureId { get; set; }
        public int? FloorTextureId { get; set; }
        public int? CeilingTextureId { get; set; }
    }

    /// <summary>
    /// Simple grid-based dungeon map.
    /// </summary>
    public class DungeonMap
    {
        private readonly MapCell3D[,] _cells;

        public int Width { get; }
        public int Height { get; }

        public DungeonMap(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new MapCell3D[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    _cells[x, y] = new MapCell3D();
        }

        public MapCell3D GetCell(int x, int y) => _cells[x, y];
    }

    /// <summary>
    /// Renderer that builds quads from the map and rasterizes them.
    /// </summary>
    public class DungeonRenderer
    {
        private readonly Camera _camera;
        private readonly DungeonMap _map;

        public RenderMode Mode { get; set; } = RenderMode.Wireframe;

        public DungeonRenderer(Camera camera, DungeonMap map)
        {
            _camera = camera;
            _map = map;
        }

        public void Render(SoftwareRasterizer rast)
        {
            float aspect = (float)rast.GetFrame().Width / rast.GetFrame().Height;
            Matrix4x4 view = _camera.GetViewMatrix();
            Matrix4x4 proj = _camera.GetProjectionMatrix(aspect);
            Matrix4x4 vp = proj * view;

            rast.Clear(Color.CornflowerBlue);

            for (int x = 0; x < _map.Width; x++)
            {
                for (int y = 0; y < _map.Height; y++)
                {
                    var cell = _map.GetCell(x, y);

                    // Walls
                    if (cell.HasWallNorth)
                        DrawQuad(rast,
                            new Vector3(x, cell.FloorHeight, y),
                            new Vector3(x + 1, cell.FloorHeight, y),
                            new Vector3(x + 1, cell.CeilingHeight, y),
                            new Vector3(x, cell.CeilingHeight, y),
                            vp,
                            cell.WallTextureId,
                            Color.Gray);

                    if (cell.HasWallSouth)
                        DrawQuad(rast,
                            new Vector3(x + 1, cell.FloorHeight, y + 1),
                            new Vector3(x, cell.FloorHeight, y + 1),
                            new Vector3(x, cell.CeilingHeight, y + 1),
                            new Vector3(x + 1, cell.CeilingHeight, y + 1),
                            vp,
                            cell.WallTextureId,
                            Color.Gray);

                    if (cell.HasWallEast)
                        DrawQuad(rast,
                            new Vector3(x + 1, cell.FloorHeight, y),
                            new Vector3(x + 1, cell.FloorHeight, y + 1),
                            new Vector3(x + 1, cell.CeilingHeight, y + 1),
                            new Vector3(x + 1, cell.CeilingHeight, y),
                            vp,
                            cell.WallTextureId,
                            Color.Gray);

                    if (cell.HasWallWest)
                        DrawQuad(rast,
                            new Vector3(x, cell.FloorHeight, y + 1),
                            new Vector3(x, cell.FloorHeight, y),
                            new Vector3(x, cell.CeilingHeight, y),
                            new Vector3(x, cell.CeilingHeight, y + 1),
                            vp,
                            cell.WallTextureId,
                            Color.Gray);

                    // Floor
                    if (cell.HasFloor)
                        DrawQuad(rast,
                            new Vector3(x, cell.FloorHeight, y),
                            new Vector3(x, cell.FloorHeight, y + 1),
                            new Vector3(x + 1, cell.FloorHeight, y + 1),
                            new Vector3(x + 1, cell.FloorHeight, y),
                            vp,
                            cell.FloorTextureId,
                            Color.DarkGray);

                    // Ceiling
                    if (cell.HasCeiling)
                        DrawQuad(rast,
                            new Vector3(x, cell.CeilingHeight, y + 1),
                            new Vector3(x, cell.CeilingHeight, y),
                            new Vector3(x + 1, cell.CeilingHeight, y),
                            new Vector3(x + 1, cell.CeilingHeight, y + 1),
                            vp,
                            cell.CeilingTextureId,
                            Color.LightGray);
                }
            }
        }

        private void DrawQuad(
            SoftwareRasterizer rast,
            Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3,
            Matrix4x4 vp,
            int? textureId,
            Color fallbackColor)
        {
            if (Mode == RenderMode.Wireframe)
            {
                rast.DrawLine(v0, v1, vp, Color.Black);
                rast.DrawLine(v1, v2, vp, Color.Black);
                rast.DrawLine(v2, v3, vp, Color.Black);
                rast.DrawLine(v3, v0, vp, Color.Black);
            }
            else
            {
                if (textureId.HasValue)
                {
                    // TODO: real textured rendering

                    var bitmap = new Bitmap(90,90);
                    //textureId.Value
                    rast.DrawTexturedQuad(v0, v1, v2, v3, vp, bitmap);
                }
                else
                {
                    rast.DrawSolidQuad(v0, v1, v2, v3, vp, fallbackColor);
                }
            }
        }
    }
}
