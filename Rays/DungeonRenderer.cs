using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Rays;

/// <summary>
///     Represents a camera in 3D space for the dungeon view.
/// </summary>
public class Camera3D
{
    public Vector3 Position { get; set; } = new(0, 1.7f, 0); // Player eye height
    public float Yaw { get; set; } = 0; // Horizontal rotation
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

    // Optional texture IDs (null means "use fallback solid color")
    public int? WallTextureId { get; set; }
    public int? FloorTextureId { get; set; }
    public int? CeilingTextureId { get; set; }
}

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

public class Actor
{
    public bool IsOnGround = true;
    public Vector3 Position;
    public float VerticalVelocity;

    public void Update(float deltaTime)
    {
        if (IsOnGround) return;

        VerticalVelocity += -9.8f * deltaTime;
        Position.Y += VerticalVelocity * deltaTime;

        if (!(Position.Y <= 1.7f)) return;

        Position.Y = 1.7f;
        IsOnGround = true;
        VerticalVelocity = 0f;
    }

    public void Jump()
    {
        if (!IsOnGround) return;

        VerticalVelocity = 5f;
        IsOnGround = false;
    }
}

/// <summary>
///     Renderer that builds quads from the map and rasterizes them.
/// </summary>
public class DungeonRenderer
{
    private readonly DungeonMap _map;
    private readonly Dictionary<int, Bitmap?> _textures;

    public DungeonRenderer(DungeonMap map, Dictionary<int, Bitmap?> textures)
    {
        _map = map;
        _textures = textures;
    }

    public RenderMode Mode { get; set; } = RenderMode.Wireframe;

    public Bitmap Render(IRenderer rast, Camera3D camera, int screenWidth, int screenHeight)
    {
        var aspect = (float)screenWidth / screenHeight;
        var view = camera.GetViewMatrix();
        var proj = camera.GetProjectionMatrix(aspect);
        var vp = view * proj;

        rast.Clear(Color.CornflowerBlue);

        for (var x = 0; x < _map.Width; x++)
        {
            for (var y = 0; y < _map.Height; y++)
            {
                var cell = _map.GetCell(x, y);

                // Frustum culling at cell level
                if (!FrustumCulling.IsCellVisible(vp, x, y, cell.FloorHeight, cell.CeilingHeight))
                    continue;

                DrawCell(rast, camera, cell, x, y, vp, screenWidth, screenHeight);
            }
        }

        return rast.GetFrame();
    }

    /// <summary>
    /// Draws all quads of a single cell.
    /// </summary>
    private void DrawCell(IRenderer rast, Camera3D camera, MapCell3D cell, int x, int y, Matrix4x4 vp, int screenWidth, int screenHeight)
    {
        // Walls
        if (cell.HasWallNorth)
            DrawQuad(rast, camera.Position, new Vector3(x, cell.FloorHeight, y),
                new Vector3(x + 1, cell.FloorHeight, y),
                new Vector3(x + 1, cell.CeilingHeight, y),
                new Vector3(x, cell.CeilingHeight, y),
                vp, screenWidth, screenHeight, cell.WallTextureId, Color.Gray);

        if (cell.HasWallSouth)
            DrawQuad(rast, camera.Position, new Vector3(x + 1, cell.FloorHeight, y + 1),
                new Vector3(x, cell.FloorHeight, y + 1),
                new Vector3(x, cell.CeilingHeight, y + 1),
                new Vector3(x + 1, cell.CeilingHeight, y + 1),
                vp, screenWidth, screenHeight, cell.WallTextureId, Color.Gray);

        if (cell.HasWallEast)
            DrawQuad(rast, camera.Position, new Vector3(x + 1, cell.FloorHeight, y),
                new Vector3(x + 1, cell.FloorHeight, y + 1),
                new Vector3(x + 1, cell.CeilingHeight, y + 1),
                new Vector3(x + 1, cell.CeilingHeight, y),
                vp, screenWidth, screenHeight, cell.WallTextureId, Color.Gray);

        if (cell.HasWallWest)
            DrawQuad(rast, camera.Position, new Vector3(x, cell.FloorHeight, y + 1),
                new Vector3(x, cell.FloorHeight, y),
                new Vector3(x, cell.CeilingHeight, y),
                new Vector3(x, cell.CeilingHeight, y + 1),
                vp, screenWidth, screenHeight, cell.WallTextureId, Color.Gray);

        // Floor
        if (cell.HasFloor)
            DrawQuad(rast, camera.Position, new Vector3(x, cell.FloorHeight, y),
                new Vector3(x, cell.FloorHeight, y + 1),
                new Vector3(x + 1, cell.FloorHeight, y + 1),
                new Vector3(x + 1, cell.FloorHeight, y),
                vp, screenWidth, screenHeight, cell.FloorTextureId, Color.DarkGray);

        // Ceiling
        if (cell.HasCeiling)
            DrawQuad(rast, camera.Position, new Vector3(x, cell.CeilingHeight, y + 1),
                new Vector3(x, cell.CeilingHeight, y),
                new Vector3(x + 1, cell.CeilingHeight, y),
                new Vector3(x + 1, cell.CeilingHeight, y + 1),
                vp, screenWidth, screenHeight, cell.CeilingTextureId, Color.LightGray);
    }

    private void DrawQuad(IRenderer rast,
        Vector3 cameraPos,
        Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3,
        Matrix4x4 vp, int screenWidth, int screenHeight,
        int? textureId, Color fallbackColor)
    {
        var normal = Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0));
        var toCamera = cameraPos - v0;

        if (Vector3.Dot(normal, toCamera) <= 0)
            return;

        var screenVerts = RasterHelpers.ProjectQuad(v0, v1, v2, v3, vp, screenWidth, screenHeight);
        if (screenVerts[0].X < -5000) return;

        Bitmap? tex = null;
        if (textureId.HasValue && _textures.ContainsKey(textureId.Value))
            tex = _textures[textureId.Value];

        switch (Mode)
        {
            case RenderMode.Wireframe:
                rast.DrawLine(Point.Round(screenVerts[0]), Point.Round(screenVerts[1]), Color.Black);
                rast.DrawLine(Point.Round(screenVerts[1]), Point.Round(screenVerts[2]), Color.Black);
                rast.DrawLine(Point.Round(screenVerts[2]), Point.Round(screenVerts[3]), Color.Black);
                rast.DrawLine(Point.Round(screenVerts[3]), Point.Round(screenVerts[0]), Color.Black);
                break;

            case RenderMode.Textured:
                if (tex != null)
                    rast.DrawTexturedQuad(
                        Point.Round(screenVerts[0]),
                        Point.Round(screenVerts[1]),
                        Point.Round(screenVerts[2]),
                        Point.Round(screenVerts[3]),
                        null);
                else
                    rast.DrawSolidQuad(
                        Point.Round(screenVerts[0]),
                        Point.Round(screenVerts[1]),
                        Point.Round(screenVerts[2]),
                        Point.Round(screenVerts[3]),
                        fallbackColor);
                break;
        }
    }

    /// <summary>
    /// Lightweight frustum culling helper
    /// </summary>
    private static class FrustumCulling
    {
        public static bool IsCellVisible(Matrix4x4 vp, int x, int y, float floor, float ceiling)
        {
            var corners = new[]
            {
                    new Vector3(x, floor, y),
                    new Vector3(x + 1, floor, y),
                    new Vector3(x, floor, y + 1),
                    new Vector3(x + 1, floor, y + 1),
                    new Vector3(x, ceiling, y),
                    new Vector3(x + 1, ceiling, y),
                    new Vector3(x, ceiling, y + 1),
                    new Vector3(x + 1, ceiling, y + 1)
                };

            foreach (var corner in corners)
            {
                var clip = Vector3.Transform(corner, vp);
                if (clip.X >= -clip.Z && clip.X <= clip.Z &&
                    clip.Y >= -clip.Z && clip.Y <= clip.Z &&
                    clip.Z >= 0 && clip.Z <= clip.Z)
                {
                    return true;
                }
            }

            return false;
        }
    }
}