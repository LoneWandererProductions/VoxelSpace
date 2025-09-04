using RenderEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Rays;

/// <summary>
///     Renderer that builds quads from the map and rasterizes them.
/// </summary>
public partial class DungeonRenderer
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

                // Compute min/max bounds of the cell cube
                var min = new Vector3(x, cell.FloorHeight, y);
                var max = new Vector3(x + 1, cell.CeilingHeight, y + 1);

                // Use generalized frustum culling
                if (!FrustumCulling.IsCellVisible(vp, min, max))
                    continue;

                DrawCell(rast, camera, cell, screenWidth, screenHeight, vp, x, y);

                // Draw layers (water, grass, etc.)
                DrawCellLayers(rast, camera, cell, screenWidth, screenHeight, vp);
            }
        }

        // 2. Draw paper dolls after world geometry
        if (dolls != null && dolls.Count > 0)
        {
            // Cull dolls by bounding sphere before drawing
            var visibleDolls = dolls
                .Where(d => FrustumCulling.IsSphereVisible(vp, d.Position, d.Radius))
                .ToList();

            DrawPaperDolls(rast, camera, visibleDolls, screenWidth, screenHeight, vp);
        }

        // 3. Return final image
        return rast.GetFrame();
    }

    private void DrawCell(
        IRenderer rast,
        Camera3D camera,
        MapCell3D cell,
        int screenWidth,
        int screenHeight,
        Matrix4x4 vp,
        int x,
        int y)
    {
        // Walls
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            var wallBounds = cell.GetWallBounds(dir, x, y);
            if (wallBounds.HasValue)
            {
                var (min, max) = wallBounds.Value;

                DrawBox(
                    rast,
                    camera,
                    min,
                    max,
                    vp,
                    screenWidth,
                    screenHeight,
                    cell.WallTextureId,
                    Color.Gray
                );
            }
        }

        // Floor
        var floorBounds = cell.GetFloorBounds(x, y);
        if (floorBounds.HasValue)
        {
            var (min, max) = floorBounds.Value;

            DrawBox(
                rast,
                camera,
                min,
                max,
                vp,
                screenWidth,
                screenHeight,
                cell.FloorTextureId,
                Color.DarkGray
            );
        }

        // Ceiling
        var ceilingBounds = cell.GetCeilingBounds(x, y);
        if (ceilingBounds.HasValue)
        {
            var (min, max) = ceilingBounds.Value;

            DrawBox(
                rast,
                camera,
                min,
                max,
                vp,
                screenWidth,
                screenHeight,
                cell.CeilingTextureId,
                Color.LightGray
            );
        }
    }


    private void DrawBox(IRenderer rast, Camera3D camera,
    Vector3 min, Vector3 max,
    Matrix4x4 vp, int screenWidth, int screenHeight,
    int? textureId, Color fallbackColor)
    {
        // Build 8 corners
        var c000 = new Vector3(min.X, min.Y, min.Z);
        var c100 = new Vector3(max.X, min.Y, min.Z);
        var c010 = new Vector3(min.X, min.Y, max.Z);
        var c110 = new Vector3(max.X, min.Y, max.Z);

        var c001 = new Vector3(min.X, max.Y, min.Z);
        var c101 = new Vector3(max.X, max.Y, min.Z);
        var c011 = new Vector3(min.X, max.Y, max.Z);
        var c111 = new Vector3(max.X, max.Y, max.Z);

        // 6 faces (same winding as existing DrawQuad)
        DrawQuad(rast, camera.Position, c000, c100, c101, c001, vp, screenWidth, screenHeight, textureId, fallbackColor); // front
        DrawQuad(rast, camera.Position, c100, c110, c111, c101, vp, screenWidth, screenHeight, textureId, fallbackColor); // right
        DrawQuad(rast, camera.Position, c110, c010, c011, c111, vp, screenWidth, screenHeight, textureId, fallbackColor); // back
        DrawQuad(rast, camera.Position, c010, c000, c001, c011, vp, screenWidth, screenHeight, textureId, fallbackColor); // left
        DrawQuad(rast, camera.Position, c001, c101, c111, c011, vp, screenWidth, screenHeight, textureId, fallbackColor); // top
        DrawQuad(rast, camera.Position, c000, c010, c110, c100, vp, screenWidth, screenHeight, textureId, fallbackColor); // bottom
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

    /// <summary>
    /// Draws all layers in a cell (water, grass, etc.) using the generic CellLayer system.
    /// </summary>
    private void DrawCellLayers(IRenderer rast, Camera3D camera, MapCell3D cell,
        int screenWidth, int screenHeight, Matrix4x4 vp)
    {
        foreach (var layer in cell.Layers)
        {
            float y = cell.FloorHeight + layer.Height;

            var v0 = new Vector3(cell.PrecomputedCorners[0].X, y, cell.PrecomputedCorners[0].Z);
            var v1 = new Vector3(cell.PrecomputedCorners[1].X, y, cell.PrecomputedCorners[1].Z);
            var v2 = new Vector3(cell.PrecomputedCorners[3].X, y, cell.PrecomputedCorners[3].Z);
            var v3 = new Vector3(cell.PrecomputedCorners[2].X, y, cell.PrecomputedCorners[2].Z);

            // Backface culling
            var normal = Vector3.UnitY;
            if (Vector3.Dot(normal, camera.Position - v0) <= 0)
                continue;

            var screenVerts = RasterHelpers.ProjectQuad(v0, v1, v2, v3, vp, screenWidth, screenHeight);
            if (screenVerts.Any(p => float.IsNaN(p.X) || float.IsNaN(p.Y)))
                continue;

            // Custom draw
            if (layer.CustomDraw != null)
            {
                var topLeft = new Point((int)screenVerts[0].X, (int)screenVerts[0].Y);
                layer.CustomDraw(rast, topLeft, layer.Mask!);
                continue;
            }

            // Color/alpha blending
            var col = layer.Color;
            if (layer.AlphaBlend)
            {
                float visibility = Math.Clamp((camera.Position.Y - cell.FloorHeight) / (layer.Height + 0.001f), 0f, 1f);
                col = Color.FromArgb((int)(col.A * visibility), col);
            }

            if (layer.Mask != null)
            {
                rast.BlitRegion(layer.Mask, 0, 0, layer.Mask.Width, layer.Mask.Height,
                    (int)screenVerts[0].X, (int)screenVerts[0].Y);
            }
            else
            {
                rast.DrawSolidQuad(
                    Point.Round(screenVerts[0]),
                    Point.Round(screenVerts[1]),
                    Point.Round(screenVerts[2]),
                    Point.Round(screenVerts[3]),
                    col);
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
}