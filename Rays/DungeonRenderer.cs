using System;
using System.Numerics;
using System.Collections.Generic;

namespace Rays
{
    /// <summary>
    /// Represents a camera in 3D space for the dungeon view.
    /// </summary>
    public class Camera
    {
        public Vector3 Position { get; set; } = new Vector3(0, 1.7f, -5); // Start a bit back
        public float Yaw { get; set; } = 0;  // Horizontal rotation
        public float Pitch { get; set; } = 0; // Vertical rotation
        public float Fov { get; set; } = (float)(Math.PI / 3); // 60°

        public Matrix4x4 GetViewMatrix()
        {
            // Build look direction from yaw/pitch
            var forward = new Vector3(
                (float)(Math.Cos(Pitch) * Math.Cos(Yaw)),
                (float)(Math.Sin(Pitch)),
                (float)(Math.Cos(Pitch) * Math.Sin(Yaw))
            );
            var target = Position + forward;
            var up = Vector3.UnitY;

            return Matrix4x4.CreateLookAt(Position, target, up);
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

        // Later: texture IDs, slope flags, etc.
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

        public DungeonRenderer(Camera camera, DungeonMap map)
        {
            _camera = camera;
            _map = map;
        }

        public void Render(int screenWidth, int screenHeight)
        {
            float aspect = (float)screenWidth / screenHeight;
            Matrix4x4 view = _camera.GetViewMatrix();
            Matrix4x4 proj = _camera.GetProjectionMatrix(aspect);

            Matrix4x4 vp = view * proj; // IMPORTANT: in .NET it's usually world * view * proj

            // Walk through map cells
            for (int x = 0; x < _map.Width; x++)
                for (int y = 0; y < _map.Height; y++)
                {
                    var cell = _map.GetCell(x, y);

                    if (cell.HasWallNorth)
                    {
                        DrawQuad(
                            new Vector3(x, cell.FloorHeight, y),
                            new Vector3(x + 1, cell.FloorHeight, y),
                            new Vector3(x + 1, cell.CeilingHeight, y),
                            new Vector3(x, cell.CeilingHeight, y),
                            vp, screenWidth, screenHeight
                        );
                    }
                    // TODO: South, East, West, floor, ceiling
                }
        }

        private void DrawQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Matrix4x4 vp, int screenWidth, int screenHeight)
        {
            Vector2[] pts = new Vector2[4];
            Vector3[] verts = { v0, v1, v2, v3 };

            for (int i = 0; i < 4; i++)
            {
                Vector4 t = Vector4.Transform(new Vector4(verts[i], 1), vp);

                // Clip near plane
                if (t.W <= 0.1f) return;

                // Perspective divide
                t /= t.W;

                // Convert to screen coords
                float sx = (t.X * 0.5f + 0.5f) * screenWidth;
                float sy = (1 - (t.Y * 0.5f + 0.5f)) * screenHeight;
                pts[i] = new Vector2(sx, sy);
            }

            // For now: just dump coords
            Console.WriteLine($"Quad at: {pts[0]}, {pts[1]}, {pts[2]}, {pts[3]}");

            // TODO: integrate with your DirectBitmapImage and draw lines:
            //   DrawLine(pts[0], pts[1]);
            //   DrawLine(pts[1], pts[2]);
            //   DrawLine(pts[2], pts[3]);
            //   DrawLine(pts[3], pts[0]);
        }
    }
}
