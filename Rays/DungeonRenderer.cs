using RenderEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Numerics;

namespace Rays;

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

    public Bitmap Render(IRenderer rast, Camera3D camera, int screenWidth, int screenHeight, List<PaperDoll>? dolls = null)
    {
        var aspect = (float)screenWidth / screenHeight;
        var view = camera.GetViewMatrix();
        var proj = camera.GetProjectionMatrix(aspect);
        var vp = view * proj;

        rast.Clear(Color.CornflowerBlue);

        // 1. Draw dungeon cells
        for (var x = 0; x < _map.Width; x++)
        {
            for (var y = 0; y < _map.Height; y++)
            {
                var cell = _map.GetCell(x, y);
                cell.PrecomputeCorners(x, y);

                if (!FrustumCulling.IsCellVisible(vp, x, y, cell.FloorHeight, cell.CeilingHeight))
                    continue;

                DrawCell(rast, camera, cell, screenWidth, screenHeight, vp);
            }
        }

        // 2. Draw paper dolls after world geometry
        if (dolls != null && dolls.Count > 0)
        {
            DrawPaperDolls(rast, camera, dolls, screenWidth, screenHeight, vp);
        }

        // 3. Return final image
        return rast.GetFrame();
    }

    private void DrawCell(IRenderer rast, Camera3D camera, MapCell3D cell, int screenWidth, int screenHeight, Matrix4x4 vp)
    {
        var c = cell.PrecomputedCorners;

        // North wall
        if (cell.HasWallNorth)
            DrawQuad(rast, camera.Position, c[0], c[1], c[5], c[4], vp, screenWidth, screenHeight, cell.WallTextureId, Color.Gray);

        // South wall
        if (cell.HasWallSouth)
            DrawQuad(rast, camera.Position, c[3], c[2], c[6], c[7], vp, screenWidth, screenHeight, cell.WallTextureId, Color.Gray);

        // East wall
        if (cell.HasWallEast)
            DrawQuad(rast, camera.Position, c[1], c[3], c[7], c[5], vp, screenWidth, screenHeight, cell.WallTextureId, Color.Gray);

        // West wall
        if (cell.HasWallWest)
            DrawQuad(rast, camera.Position, c[2], c[0], c[4], c[6], vp, screenWidth, screenHeight, cell.WallTextureId, Color.Gray);

        // Floor
        if (cell.HasFloor)
            DrawQuad(rast, camera.Position, c[0], c[2], c[3], c[1], vp, screenWidth, screenHeight, cell.FloorTextureId, Color.DarkGray);

        // Ceiling
        if (cell.HasCeiling)
            DrawQuad(rast, camera.Position, c[6], c[4], c[5], c[7], vp, screenWidth, screenHeight, cell.CeilingTextureId, Color.LightGray);
    }

    /// <summary>
    /// Draws paper dolls, respecting occlusion by walls or props (blocky boxes).
    /// </summary>
    private void DrawPaperDolls(IRenderer rast, Camera3D camera, List<PaperDoll> dolls, int screenWidth, int screenHeight, Matrix4x4 vp)
    {
        foreach (var doll in dolls)
        {
            // Transform doll position to screen space
            var dollPos = doll.Position;
            var clipPos = Vector3.Transform(dollPos, vp);

            // Skip if behind camera
            if (clipPos.Z <= 0)
                continue;

            // Project to screen
            var screenX = (int)((clipPos.X / clipPos.Z + 1f) * 0.5f * screenWidth);
            var screenY = (int)((1f - (clipPos.Y / clipPos.Z + 1f) * 0.5f) * screenHeight);

            // Check against occluding cells
            bool occluded = false;

            // Determine which cell the doll is in
            int cellX = (int)Math.Floor(doll.Position.X);
            int cellY = (int)Math.Floor(doll.Position.Z); // assuming Y-up

            var cell = _map.GetCell(cellX, cellY);

            if (cell != null)
            {
                // Simple occlusion: check height
                if (doll.Position.Y < cell.CeilingHeight)
                {
                    // Doll is behind the wall/ceiling
                    occluded = true;
                }
            }

            // Optionally check neighboring cells for props
            // foreach (var neighbor in GetNeighborCells(cellX, cellY)) { ... }

            if (occluded)
                continue;

            // Draw sprite centered on screenX, screenY
            var halfW = doll.Sprite.Width / 2;
            var halfH = doll.Sprite.Height / 2;

            if (doll.Sprite == null)
            {
                rast.DrawSolidQuad(
                new Point(screenX - halfW, screenY - halfH),
                new Point(screenX + halfW, screenY - halfH),
                new Point(screenX + halfW, screenY + halfH),
                new Point(screenX - halfW, screenY + halfH),
                Color.White); // Or integrate with a sprite blit function
            }
            else 
            {
                // Define the top-left point
                var topLeft = new Point(screenX - halfW, screenY - halfH);

                // Draw sprite
                rast.DrawSprite(topLeft, doll.Sprite);
            }
        }
    }

    private void DrawQuad(IRenderer rast,
        Vector3 cameraPos,
        Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3,
        Matrix4x4 vp, int screenWidth, int screenHeight,
        int? textureId, Color fallbackColor)
    {
        // Compute normal and backface cull
        var normal = Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0));
        var toCamera = cameraPos - v0;
        if (Vector3.Dot(normal, toCamera) <= 0)
            return;

        // Project vertices to screen space
        var screenVerts = RasterHelpers.ProjectQuad(v0, v1, v2, v3, vp, screenWidth, screenHeight);
        if (screenVerts[0].X < -5000) return;

        // Select texture
        UnmanagedImageBuffer? tex = null;
        if (textureId.HasValue && _textures.TryGetValue(textureId.Value, out var bmp) && bmp != null)
            tex = UnmanagedImageBuffer.FromBitmap(bmp);

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
                        tex);
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