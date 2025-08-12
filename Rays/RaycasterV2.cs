/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        RaycasterV2.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Drawing;
using Imaging;
using Viewer;

namespace Rays
{
    public sealed class RaycasterV2
    {
        private readonly CameraContext _context;
        private readonly IFloorCeilingRenderer? _floorCeilingRenderer;
        private readonly MapCell[,] _map;
        private readonly int _mapHeight;
        private readonly int _mapWidth;
        private readonly DirectBitmap[] _wallTextures;
        private readonly DirectBitmap _grayTexture;

        public RaycasterV2(MapCell[,] map, CameraContext context, DirectBitmap[] wallTextures,
            IFloorCeilingRenderer? floorCeilingRenderer = null)
        {
            _map = map;
            _mapWidth = map.GetLength(1);
            _mapHeight = map.GetLength(0);
            _context = context;
            _wallTextures = wallTextures;
            // Create 1×1 gray texture as fallback
            _grayTexture = new DirectBitmap(1, 1, Color.Gray);
        
            _floorCeilingRenderer = floorCeilingRenderer ??
                                    new TexturedFloorCeilingRenderer(
                                        _wallTextures[0] ?? _grayTexture,
                                        _wallTextures[1] ?? _grayTexture
                                    );
        }

        public RenderResult Render(RvCamera camera)
        {
            DirectBitmap dbm = new(_context.ScreenWidth, _context.ScreenHeight, Color.Black);

            // Render floor and ceiling first
            _floorCeilingRenderer?.Render(dbm, camera, _context);

            var halfFov = _context.Fov / 2.0;
            var angleStep = _context.Fov / _context.ScreenWidth;

            for (var x = 0; x < _context.ScreenWidth; x++)
            {
                var rayAngle = camera.Angle - halfFov + x * angleStep;
                var rayAngleRad = DegreeToRadian(rayAngle);

                var rayX = Math.Cos(rayAngleRad);
                var rayY = Math.Sin(rayAngleRad);

                var (distanceToWall, hitX, hitY, wallId) = CastRay(camera.X, camera.Y, rayX, rayY);

                if (distanceToWall > _context.Distance || wallId <= 0 || wallId > _wallTextures.Length)
                    continue;

                var wallHeight = (int)(_context.ScreenHeight / distanceToWall);
                var zOffset = camera.Z / _context.CellSize * wallHeight;
                var wallTop = Math.Max(0, (_context.ScreenHeight - wallHeight) / 2 - zOffset);
                var wallBottom = Math.Min(_context.ScreenHeight, (_context.ScreenHeight + wallHeight) / 2 - zOffset);

                var texture = _wallTextures[wallId - 1]; // assuming wallId is 1-based

                // Texture X offset
                double cellSize = _context.CellSize;
                var hitInCellX = hitX % cellSize;
                var hitInCellY = hitY % cellSize;

                var isVertical = Math.Abs(hitInCellX - cellSize / 2) < Math.Abs(hitInCellY - cellSize / 2);

                var texX = isVertical
                    ? (int)(hitY % cellSize / cellSize * texture.Width)
                    : (int)(hitX % cellSize / cellSize * texture.Width);

                texX = Math.Clamp(texX, 0, texture.Width - 1);

                // ✅ Cache the vertical texture column once
                var textureColumn = texture.GetColumn(texX);

                for (var y = wallTop; y < wallBottom; y++)
                {
                    var t = (y - wallTop) / (double)(wallBottom - wallTop);
                    var texY = (int)(t * texture.Height);
                    texY = Math.Clamp(texY, 0, texture.Height - 1);

                    var baseColor = textureColumn[texY];
                    var foggedColor = ApplyFog(baseColor, distanceToWall);
                    dbm.SetPixel(x, y, foggedColor);
                }
            }

            var imageBytes = ToByteArray(dbm.Bits);

            return new RenderResult
            {
                Bitmap = dbm.Bitmap,
                Bytes = imageBytes
            };
        }


        private static byte[] ToByteArray(int[] bits)
        {
            var bytes = new byte[bits.Length * sizeof(int)];
            Buffer.BlockCopy(bits, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private (double Distance, double HitX, double HitY, int WallId) CastRay(double startX, double startY,
            double rayDirX, double rayDirY)
        {
            const double nearClipDistance = 0.01; // avoid walls too close to camera

            // Map cell size and map indices
            double cellSize = _context.CellSize;
            var mapX = (int)(startX / cellSize);
            var mapY = (int)(startY / cellSize);

            // Direction step and initial side distance
            var deltaDistX = rayDirX == 0 ? double.MaxValue : Math.Abs(cellSize / rayDirX);
            var deltaDistY = rayDirY == 0 ? double.MaxValue : Math.Abs(cellSize / rayDirY);

            int stepX, stepY;
            double sideDistX, sideDistY;

            // Calculate step and initial side distances
            if (rayDirX < 0)
            {
                stepX = -1;
                sideDistX = (startX - mapX * cellSize) * deltaDistX / cellSize;
            }
            else
            {
                stepX = 1;
                sideDistX = ((mapX + 1) * cellSize - startX) * deltaDistX / cellSize;
            }

            if (rayDirY < 0)
            {
                stepY = -1;
                sideDistY = (startY - mapY * cellSize) * deltaDistY / cellSize;
            }
            else
            {
                stepY = 1;
                sideDistY = ((mapY + 1) * cellSize - startY) * deltaDistY / cellSize;
            }

            var hit = false;
            var hitVertical = false;

            while (!hit)
            {
                // Step to next map square in either x or y
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

                // Bounds check
                if (mapX < 0 || mapX >= _mapWidth || mapY < 0 || mapY >= _mapHeight)
                    return (double.MaxValue, 0, 0, 0);

                var cell = _map[mapY, mapX];
                if (cell.WallId > 0)
                    hit = true;
            }

            // Compute exact hit position
            double distance;
            double hitX, hitY;

            if (hitVertical)
            {
                distance = (mapX - startX / cellSize + (1 - stepX) / 2) * cellSize / rayDirX;
                hitX = mapX * cellSize;
                hitY = startY + distance * rayDirY;
            }
            else
            {
                distance = (mapY - startY / cellSize + (1 - stepY) / 2) * cellSize / rayDirY;
                hitY = mapY * cellSize;
                hitX = startX + distance * rayDirX;
            }

            // Normalize distance to avoid FOV distortion
            var dx = hitX - startX;
            var dy = hitY - startY;
            var correctedDistance = Math.Sqrt(dx * dx + dy * dy) / cellSize;

            // Apply near clipping
            if (correctedDistance < nearClipDistance)
                return (double.MaxValue, 0, 0, 0);

            var wallId = _map[mapY, mapX].WallId;

            return (correctedDistance, hitX, hitY, wallId);
        }

        private Color ApplyFog(Color baseColor, double distance)
        {
            var fogFactor = Math.Min(1.0, distance / _context.Distance);
            var backgroundColor = Color.Black;

            var red = (int)((1 - fogFactor) * baseColor.R + fogFactor * backgroundColor.R);
            var green = (int)((1 - fogFactor) * baseColor.G + fogFactor * backgroundColor.G);
            var blue = (int)((1 - fogFactor) * baseColor.B + fogFactor * backgroundColor.B);

            return Color.FromArgb(red, green, blue);
        }

        private static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180.0;
        }
    }
}
