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

        // 1. Draw dungeon cells (stacked Z-levels)
        for (var z = 0; z < _map.Levels; z++)
        {
            for (var x = 0; x < _map.Width; x++)
            {
                for (var y = 0; y < _map.Height; y++)
                {
                    var cell = _map.GetCell(x, y, z);

                    // Precompute corners using the cell's own X,Y,Z & heights
                    cell.PrecomputeCorners();

                    // Use the cell's per-level height as the vertical stride between layers
                    float levelHeight = cell.CeilingHeight - cell.FloorHeight;
                    float floor = cell.FloorHeight + z * levelHeight;
                    float ceil = cell.CeilingHeight + z * levelHeight;

                    var min = new Vector3(x, floor, y);
                    var max = new Vector3(x + 1, ceil, y + 1);

                    // Frustum culling
                    if (!FrustumCulling.IsCellVisible(vp, min, max))
                        continue;

                    // Draw the cell
                    DrawCell(rast, camera, cell, screenWidth, screenHeight, vp);

                    // Draw cell layers (water, grass, etc.)
                    DrawCellLayers(rast, camera, cell, screenWidth, screenHeight, vp);
                }
            }
        }

        // 2. Paper dolls
        if (dolls != null && dolls.Count > 0)
        {
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
        Matrix4x4 vp)
    {
        // Walls
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            var wallBounds = cell.GetWallBounds(dir);
            if (wallBounds.HasValue)
            {
                var (min, max) = wallBounds.Value;
                DrawBox(
                    rast, camera,
                    min, max,
                    vp, screenWidth, screenHeight,
                    cell.WallTextureId,
                    Color.Gray
                );
            }
        }

        // Floor slab
        var floorBounds = cell.GetFloorBounds();
        if (floorBounds.HasValue)
        {
            var (min, max) = floorBounds.Value;
            DrawBox(
                rast, camera,
                min, max,
                vp, screenWidth, screenHeight,
                cell.FloorTextureId,
                Color.DarkGray
            );
        }

        // Ceiling slab
        var ceilingBounds = cell.GetCeilingBounds();
        if (ceilingBounds.HasValue)
        {
            var (min, max) = ceilingBounds.Value;
            DrawBox(
                rast, camera,
                min, max,
                vp, screenWidth, screenHeight,
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
    private void DrawPaperDolls(
        IRenderer rast,
        Camera3D camera,
        List<PaperDoll> dolls,
        int screenWidth,
        int screenHeight,
        Matrix4x4 vp)
    {
        foreach (var doll in dolls)
        {
            // Transform doll position into clip space
            var dollPos = doll.Position;
            var clipPos = Vector3.Transform(dollPos, vp);

            // Skip if behind camera
            if (clipPos.Z <= 0)
                continue;

            // Project to screen space
            int screenX = (int)((clipPos.X / clipPos.Z + 1f) * 0.5f * screenWidth);
            int screenY = (int)((1f - (clipPos.Y / clipPos.Z + 1f) * 0.5f) * screenHeight);

            // Determine which cell the doll stands in
            int cellX = (int)Math.Floor(doll.Position.X);
            int cellY = (int)Math.Floor(doll.Position.Z);
            int cellZ = (int)Math.Floor(doll.Position.Y / (_map.BaseHeight));


            var cell = _map.GetCell(cellX, cellY, cellZ);

            bool occluded = false;
            if (cell != null)
            {
                // Check if doll is under the ceiling or inside solid part of this cell
                if (doll.Position.Y < cell.CeilingHeight &&
                    doll.Position.Y > cell.FloorHeight)
                {
                    occluded = true;
                }
            }

            if (occluded)
                continue;

            // Draw sprite centered on screenX, screenY
            int halfW = doll.Sprite?.Width / 2 ?? 8;
            int halfH = doll.Sprite?.Height / 2 ?? 8;

            if (doll.Sprite == null)
            {
                rast.DrawSolidQuad(
                    new Point(screenX - halfW, screenY - halfH),
                    new Point(screenX + halfW, screenY - halfH),
                    new Point(screenX + halfW, screenY + halfH),
                    new Point(screenX - halfW, screenY + halfH),
                    Color.White
                );
            }
            else
            {
                var topLeft = new Point(screenX - halfW, screenY - halfH);
                rast.DrawSprite(topLeft, doll.Sprite);
            }
        }
    }

    /// <summary>
    /// Draws all layers in a cell (water, grass, etc.) using the generic CellLayer system.
    /// </summary>
    /// <param name="rast">The rast.</param>
    /// <param name="camera">The camera.</param>
    /// <param name="cell">The cell.</param>
    /// <param name="screenWidth">Width of the screen.</param>
    /// <param name="screenHeight">Height of the screen.</param>
    /// <param name="vp">The vp.</param>
    private static void DrawCellLayers(IRenderer rast, Camera3D camera, MapCell3D cell,
        int screenWidth, int screenHeight, Matrix4x4 vp)
    {
        if (cell.Layers.Count == 0) return;

        // vertical offset for this Z-level
        float levelHeight = cell.CeilingHeight - cell.FloorHeight;
        float zOffsetY = cell.Z * levelHeight;

        foreach (var layer in cell.Layers)
        {
            float y = cell.FloorHeight + zOffsetY + layer.Height;

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
                float visibility = Math.Clamp(
                    (camera.Position.Y - (cell.FloorHeight + zOffsetY)) / (layer.Height + 0.001f),
                    0f, 1f
                );
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