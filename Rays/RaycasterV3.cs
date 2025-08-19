/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        RaycasterV3.cs
 * PURPOSE:     3D raycaster with per-cell primitives and floor/ceiling rendering
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
        private readonly MapCell[,] _map;
        private readonly int _mapWidth;
        private readonly int _mapHeight;
        private readonly DirectBitmap[] _wallTextures;
        private readonly DirectBitmap _grayTexture;
        private readonly DirectBitmap _floorTexture;
        private readonly DirectBitmap _ceilingTexture;

        public RaycasterV3(MapCell[,] map, CameraContext context, DirectBitmap[] wallTextures,
            DirectBitmap? floorTexture = null, DirectBitmap? ceilingTexture = null)
        {
            _map = map;
            _mapWidth = map.GetLength(1);
            _mapHeight = map.GetLength(0);
            _context = context;

            _wallTextures = (wallTextures != null && wallTextures.Length > 0)
                ? wallTextures
                : new[] { _grayTexture = new DirectBitmap(1, 1, Color.Gray) };

            _floorTexture = floorTexture ?? _wallTextures[0];
            _ceilingTexture = ceilingTexture ?? (_wallTextures.Length > 1 ? _wallTextures[1] : _wallTextures[0]);
        }

        public RenderResult Render(RvCamera camera)
        {
            var dbm = new DirectBitmap(_context.ScreenWidth, _context.ScreenHeight, Color.Black);

            RenderFloorCeiling(dbm, camera);

            double halfFov = _context.Fov / 2.0;
            double angleStep = _context.Fov / _context.ScreenWidth;

            for (int x = 0; x < _context.ScreenWidth; x++)
            {
                double rayAngle = camera.Angle - halfFov + x * angleStep;
                double rayRad = DegreeToRadian(rayAngle);
                double rayDirX = Math.Cos(rayRad);
                double rayDirY = Math.Sin(rayRad);

                var (hitPrim, distance, texX, texY) = CastRayPrimitives(camera.X, camera.Y, rayDirX, rayDirY,
                    camera.Z, camera.Z + _context.CellSize);

                if (hitPrim == null || distance > _context.Distance)
                    continue;

                double angleDiff = DegreeToRadian(rayAngle - camera.Angle);
                double perpDist = distance * Math.Cos(angleDiff);

                double wallHeight = _context.ScreenHeight / perpDist;
                double verticalOffset = (camera.Z / (double)_context.CellSize) * wallHeight;

                int wallTop = Math.Max(0, (int)Math.Round(-wallHeight / 2.0 + _context.ScreenHeight / 2.0 - verticalOffset));
                int wallBottom = Math.Min(_context.ScreenHeight - 1, (int)Math.Round(wallTop + wallHeight));

                DirectBitmap texture = hitPrim.Texture ?? _wallTextures[hitPrim.TextureId % _wallTextures.Length];

                Color[] column = texture.GetColumn(texX % texture.Width);
                int columnHeight = column.Length;

                for (int y = wallTop; y < wallBottom; y++)
                {
                    double t = (y - wallTop) / (double)(wallBottom - wallTop);
                    int ty = Math.Clamp((int)(t * columnHeight), 0, columnHeight - 1);
                    dbm.SetPixel(x, y, column[ty]);
                }
            }

            return new RenderResult
            {
                Bitmap = dbm.Bitmap,
                Bytes = ToByteArray(dbm.Bits)
            };
        }

        private void RenderFloorCeiling(DirectBitmap dbm, RvCamera camera)
        {
            int halfHeight = _context.ScreenHeight / 2;

            for (int y = halfHeight; y < _context.ScreenHeight; y++)
            {
                double p = y - halfHeight;
                double rowDistance = (0.5 * _context.ScreenHeight) / p;

                double floorStepX = rowDistance * Math.Cos(DegreeToRadian(camera.Angle)) / _context.CellSize;
                double floorStepY = rowDistance * Math.Sin(DegreeToRadian(camera.Angle)) / _context.CellSize;

                double floorX = camera.X + rowDistance * Math.Cos(DegreeToRadian(camera.Angle));
                double floorY = camera.Y + rowDistance * Math.Sin(DegreeToRadian(camera.Angle));

                int floorTexY = (int)(floorY % _floorTexture.Height);
                if (floorTexY < 0) floorTexY += _floorTexture.Height;

                int ceilTexY = (int)(floorY % _ceilingTexture.Height);
                if (ceilTexY < 0) ceilTexY += _ceilingTexture.Height;

                var floorRow = _floorTexture.GetRow(floorTexY);
                var ceilingRow = _ceilingTexture.GetRow(ceilTexY);

                for (int x = 0; x < _context.ScreenWidth; x++)
                {
                    int floorTexX = (int)(floorX % _floorTexture.Width);
                    if (floorTexX < 0) floorTexX += _floorTexture.Width;

                    int ceilTexX = (int)(floorX % _ceilingTexture.Width);
                    if (ceilTexX < 0) ceilTexX += _ceilingTexture.Width;

                    dbm.SetPixel(x, y, floorRow[floorTexX]);
                    dbm.SetPixel(x, _context.ScreenHeight - y - 1, ceilingRow[ceilTexX]);

                    floorX += floorStepX;
                    floorY += floorStepY;
                }
            }
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
                        var (hit, dist, tX, tY) = prim.IntersectRay(startX, startY, rayDirX, rayDirY, zStart, zEnd,
                            _context.CellSize);
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
                        // ignore
                    }
                }

                if (closestPrim != null)
                    return (closestPrim, closestDist, texX, texY);
            }
        }

        private static double DegreeToRadian(double degree) => degree * Math.PI / 180.0;

        private static byte[] ToByteArray(int[] bits)
        {
            var bytes = new byte[bits.Length * sizeof(int)];
            Buffer.BlockCopy(bits, 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
