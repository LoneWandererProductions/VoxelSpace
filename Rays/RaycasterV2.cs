/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        RaycasterV2.cs
 * PURPOSE:     Raycasting with straight walls, SIMD-style pixel filling
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Drawing;
using Imaging;
using Viewer;

namespace Rays;

public sealed class RaycasterV2
{
    private readonly CameraContext _context;
    private readonly IFloorCeilingRenderer? _floorCeilingRenderer;
    private readonly DirectBitmap _grayTexture;
    private readonly MapCells[,] _map;
    private readonly int _mapHeight;
    private readonly int _mapWidth;
    private readonly DirectBitmap[] _wallTextures;

    public RaycasterV2(MapCells[,] map, CameraContext context, DirectBitmap[] wallTextures,
        IFloorCeilingRenderer? floorCeilingRenderer = null)
    {
        _map = map;
        _mapWidth = map.GetLength(1);
        _mapHeight = map.GetLength(0);
        _context = context;

        _wallTextures = wallTextures != null && wallTextures.Length > 0
            ? wallTextures
            : new[] { _grayTexture = new DirectBitmap(1, 1, Color.Gray) };

        // Floor/Ceiling renderer
        _floorCeilingRenderer = floorCeilingRenderer ?? new TexturedFloorCeilingRenderer(
            _wallTextures[0], _wallTextures.Length > 1 ? _wallTextures[1] : _wallTextures[0]);
    }

    public RenderResult Render(RvCamera camera)
    {
        DirectBitmap dbm = new(_context.ScreenWidth, _context.ScreenHeight, Color.Black);

        _floorCeilingRenderer?.Render(dbm, camera, _context);

        var halfFov = _context.Fov / 2.0;
        var angleStep = _context.Fov / _context.ScreenWidth;

        for (var x = 0; x < _context.ScreenWidth; x++)
        {
            var rayAngle = camera.Angle - halfFov + x * angleStep;
            var rayRad = DegreeToRadian(rayAngle);
            var rayDirX = Math.Cos(rayRad);
            var rayDirY = Math.Sin(rayRad);

            var (distanceToWall, hitX, hitY, wallId, hitVertical) =
                CastRayWithHitSide(camera.X, camera.Y, rayDirX, rayDirY);

            if (distanceToWall > _context.Distance || wallId <= 0 || wallId > _wallTextures.Length)
                continue;

            // Fish-eye correction
            var angleDiff = DegreeToRadian(rayAngle - camera.Angle);
            var perpWallDist = distanceToWall * Math.Cos(angleDiff);

            // Wall height and offset
            var wallHeight = _context.ScreenHeight / perpWallDist;
            var verticalOffset = camera.Z / (double)_context.CellSize * wallHeight;

            var wallTopF = -wallHeight / 2.0 + _context.ScreenHeight / 2.0 - verticalOffset;
            var wallBottomF = wallTopF + wallHeight;

            var wallTop = Math.Max(0, (int)Math.Round(wallTopF));
            var wallBottom = Math.Min(_context.ScreenHeight - 1, (int)Math.Round(wallBottomF));

            var texture = _wallTextures[wallId - 1];

            double cellSize = _context.CellSize;
            var hitInCellX = hitX % cellSize;
            if (hitInCellX < 0) hitInCellX += cellSize;
            var hitInCellY = hitY % cellSize;
            if (hitInCellY < 0) hitInCellY += cellSize;

            var texX = hitVertical
                ? (int)(hitInCellY / cellSize * texture.Width)
                : (int)(hitInCellX / cellSize * texture.Width);
            texX = Math.Clamp(texX, 0, texture.Width - 1);

            var textureColumn = texture.GetColumn(texX);
            var columnHeight = wallBottomF - wallTopF;

            // SIMD-style filling using Span
            var bitsSpan = dbm.Bits.AsSpan();
            var screenWidth = _context.ScreenWidth;

            for (var y = wallTop; y < wallBottom; y++)
            {
                var t = (y - wallTopF) / columnHeight;
                var texY = Math.Clamp((int)(t * texture.Height), 0, texture.Height - 1);

                var baseColor = textureColumn[texY];
                var foggedColor = ApplyFog(baseColor, distanceToWall);

                var pixelValue = (foggedColor.A << 24) | (foggedColor.R << 16) | (foggedColor.G << 8) | foggedColor.B;
                bitsSpan[y * screenWidth + x] = pixelValue;
            }
        }

        return new RenderResult
        {
            Bitmap = dbm.Bitmap,
            Bytes = ToByteArray(dbm.Bits)
        };
    }

    private (double Distance, double HitX, double HitY, int WallId, bool HitVertical)
        CastRayWithHitSide(double startX, double startY, double rayDirX, double rayDirY)
    {
        const double nearClip = 0.01;
        double cellSize = _context.CellSize;

        var mapX = (int)(startX / cellSize);
        var mapY = (int)(startY / cellSize);

        var deltaDistX = rayDirX == 0 ? double.MaxValue : Math.Abs(cellSize / rayDirX);
        var deltaDistY = rayDirY == 0 ? double.MaxValue : Math.Abs(cellSize / rayDirY);

        var stepX = rayDirX < 0 ? -1 : 1;
        var stepY = rayDirY < 0 ? -1 : 1;

        var sideDistX = stepX < 0
            ? (startX - mapX * cellSize) * deltaDistX / cellSize
            : ((mapX + 1) * cellSize - startX) * deltaDistX / cellSize;

        var sideDistY = stepY < 0
            ? (startY - mapY * cellSize) * deltaDistY / cellSize
            : ((mapY + 1) * cellSize - startY) * deltaDistY / cellSize;

        var hit = false;
        var hitVertical = false;

        while (!hit)
        {
            if (sideDistX < sideDistY)
            {
                sideDistX += deltaDistX;
                mapX += stepX;
                hitVertical = true;
            }
            else
            {
                sideDistY += deltaDistY;
                mapY += stepY;
                hitVertical = false;
            }

            if (mapX < 0 || mapX >= _mapWidth || mapY < 0 || mapY >= _mapHeight)
                return (double.MaxValue, 0, 0, 0, false);

            if (_map[mapY, mapX].WallId > 0)
                hit = true;
        }

        double hitX, hitY, distance;

        if (hitVertical)
        {
            hitX = mapX * cellSize + (stepX < 0 ? cellSize : 0);
            hitY = startY + (hitX - startX) * rayDirY / rayDirX;
            distance = Math.Abs((hitX - startX) / rayDirX);
        }
        else
        {
            hitY = mapY * cellSize + (stepY < 0 ? cellSize : 0);
            hitX = startX + (hitY - startY) * rayDirX / rayDirY;
            distance = Math.Abs((hitY - startY) / rayDirY);
        }

        distance /= cellSize; // normalize
        if (distance < nearClip) return (double.MaxValue, 0, 0, 0, hitVertical);

        var wallId = _map[mapY, mapX].WallId;
        return (distance, hitX, hitY, wallId, hitVertical);
    }

    private Color ApplyFog(Color baseColor, double distance)
    {
        var fogFactor = Math.Min(1.0, distance / _context.Distance);
        var bg = Color.Black;
        return Color.FromArgb(
            (int)((1 - fogFactor) * baseColor.R + fogFactor * bg.R),
            (int)((1 - fogFactor) * baseColor.G + fogFactor * bg.G),
            (int)((1 - fogFactor) * baseColor.B + fogFactor * bg.B)
        );
    }

    private static double DegreeToRadian(double degree)
    {
        return degree * Math.PI / 180.0;
    }

    private static byte[] ToByteArray(int[] bits)
    {
        try
        {
            var bytes = new byte[bits.Length * sizeof(int)];
            Buffer.BlockCopy(bits, 0, bytes, 0, bytes.Length);
            return bytes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ToByteArray failed: {ex}");
            return Array.Empty<byte>();
        }
    }
}