/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        RaycasterV3.cs
 * PURPOSE:     3D raycaster with per-cell primitives (cubes, ramps, multi-layered)
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Drawing;
using Imaging;
using Viewer;

namespace Rays
{
    public sealed class RaycasterV3
    {
        private readonly CameraContext _context;
        private readonly IFloorCeilingRenderer? _floorCeilingRenderer;
        private readonly MapCell[,] _map;
        private readonly int _mapHeight;
        private readonly int _mapWidth;
        private readonly DirectBitmap[] _wallTextures;
        private readonly DirectBitmap _grayTexture;

        public RaycasterV3(MapCell[,] map, CameraContext context, DirectBitmap[] wallTextures,
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
            var dbm = new DirectBitmap(_context.ScreenWidth, _context.ScreenHeight, Color.Black);

            _floorCeilingRenderer?.Render(dbm, camera, _context);

            double halfFov = _context.Fov / 2.0;
            double angleStep = _context.Fov / _context.ScreenWidth;

            for (int x = 0; x < _context.ScreenWidth; x++)
            {
                double rayAngle = camera.Angle - halfFov + x * angleStep;
                double rayAngleRad = DegreeToRadian(rayAngle);

                double rayDirX = Math.Cos(rayAngleRad);
                double rayDirY = Math.Sin(rayAngleRad);

                var (hitPrimitive, distance, texX, texY) = CastRayPrimitives(camera.X, camera.Y, rayDirX, rayDirY, camera.Z, camera.Z + _context.CellSize);

                if (hitPrimitive == null || distance > _context.Distance)
                    continue;

                // Fish-eye correction
                double angleDiff = DegreeToRadian(rayAngle - camera.Angle);
                double perpWallDist = distance * Math.Cos(angleDiff);

                double wallHeight = _context.ScreenHeight / perpWallDist;
                double verticalOffset = (camera.Z / (double)_context.CellSize) * wallHeight;

                int wallTop = Math.Max(0, (int)Math.Round(-wallHeight / 2.0 + _context.ScreenHeight / 2.0 - verticalOffset));
                int wallBottom = Math.Min(_context.ScreenHeight - 1, (int)Math.Round(wallTop + wallHeight));

                // SIMD-style column fill (simplified)
                var column = hitPrimitive.Texture.GetColumn(texX);
                int columnHeight = column.Length;

                for (int y = wallTop; y < wallBottom; y++)
                {
                    double t = (y - wallTop) / (double)(wallBottom - wallTop);
                    int ty = Math.Clamp((int)(t * columnHeight), 0, columnHeight - 1);
                    //dbm.SetPixel(x, y, column[ty]);
                    dbm.SetPixel(x, y, Color.FromArgb(column[ty]));
                }
            }

            return new RenderResult
            {
                Bitmap = dbm.Bitmap,
                Bytes = ToByteArray(dbm.Bits)
            };
        }

        private (CellPrimitive? HitPrimitive, double Distance, int TexX, int TexY) CastRayPrimitives(
            double startX, double startY, double rayDirX, double rayDirY, double zStart, double zEnd)
        {
            int mapX = (int)(startX / _context.CellSize);
            int mapY = (int)(startY / _context.CellSize);

            double deltaDistX = rayDirX == 0 ? double.MaxValue : Math.Abs(_context.CellSize / rayDirX);
            double deltaDistY = rayDirY == 0 ? double.MaxValue : Math.Abs(_context.CellSize / rayDirY);

            int stepX = rayDirX < 0 ? -1 : 1;
            int stepY = rayDirY < 0 ? -1 : 1;

            double sideDistX = rayDirX < 0
                ? (startX - mapX * _context.CellSize) * deltaDistX / _context.CellSize
                : ((mapX + 1) * _context.CellSize - startX) * deltaDistX / _context.CellSize;

            double sideDistY = rayDirY < 0
                ? (startY - mapY * _context.CellSize) * deltaDistY / _context.CellSize
                : ((mapY + 1) * _context.CellSize - startY) * deltaDistY / _context.CellSize;

            while (true)
            {
                if (sideDistX < sideDistY)
                {
                    sideDistX += deltaDistX;
                    mapX += stepX;
                }
                else
                {
                    sideDistY += deltaDistY;
                    mapY += stepY;
                }

                if (mapX < 0 || mapX >= _mapWidth || mapY < 0 || mapY >= _mapHeight)
                    return (null, double.MaxValue, 0, 0);

                var cell = _map[mapY, mapX];
                CellPrimitive? closestPrim = null;
                double closestDist = double.MaxValue;
                int texX = 0, texY = 0;

                foreach (var prim in cell.Primitives)
                {
                    try
                    {
                        var (hit, dist, tX, tY) = prim.IntersectRay(startX, startY, rayDirX, rayDirY, zStart, zEnd, _context.CellSize);
                        if (hit && dist < closestDist)
                        {
                            closestDist = dist;
                            closestPrim = prim;
                            texX = tX;
                            texY = tY;
                        }
                    }
                    catch
                    {
                        // swallow individual primitive exceptions to avoid crashing render
                    }
                }

                if (closestPrim != null)
                    return (closestPrim, closestDist, texX, texY);
            }
        }

        private static byte[] ToByteArray(int[] bits)
        {
            try
            {
                var bytes = new byte[bits.Length * sizeof(int)];
                Buffer.BlockCopy(bits, 0, bytes, 0, bytes.Length);
                return bytes;
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        private static double DegreeToRadian(double degree) => degree * Math.PI / 180.0;
    }
}
