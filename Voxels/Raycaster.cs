using System;
using System.Drawing;
using System.IO;
using Imaging;

namespace Voxels
{
    public sealed class Raycaster
    {
        private readonly CameraContext _context;
        private readonly int[,] _map;
        private readonly int _mapHeight;
        private readonly int _mapWidth;

        public class RenderResult : IDisposable
        {
            public Bitmap Bitmap { get; set; }
            public byte[] Bytes { get; set; }
            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }


        public Raycaster(int[,] map, CameraContext context)
        {
            _map = map;
            _mapWidth = map.GetLength(1);
            _mapHeight = map.GetLength(0);
            _context = context;
        }

        public RenderResult Render(RvCamera camera)
        {
            DirectBitmap dbm = new(_context.ScreenWidth, _context.ScreenHeight, Color.Black);

            var halfFov = _context.Fov / 2.0;
            var angleStep = _context.Fov / _context.ScreenWidth;

            for (var x = 0; x < _context.ScreenWidth; x++)
            {
                var rayAngle = camera.Angle - halfFov + x * angleStep;
                var rayX = Math.Cos(DegreeToRadian(rayAngle));
                var rayY = Math.Sin(DegreeToRadian(rayAngle));

                var distanceToWall = CastRay(camera.X, camera.Y, rayX, rayY);

                if (distanceToWall > _context.Distance)
                    continue;

                var wallHeight = (int)(_context.ScreenHeight / distanceToWall);
                var zOffset = camera.Z / _context.CellSize * wallHeight;
                var wallTop = Math.Max(0, (_context.ScreenHeight - wallHeight) / 2 - zOffset);
                var wallBottom = Math.Min(_context.ScreenHeight, (_context.ScreenHeight + wallHeight) / 2 - zOffset);

                var wallColor = GetWallColor(distanceToWall);

                dbm.DrawVerticalLine(x, wallTop, wallBottom - wallTop, wallColor);
            }

            // Create PNG-encoded byte[] from the bitmap
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


        public double CastRay(double startX, double startY, double rayDirX, double rayDirY)
        {
            var x = startX;
            var y = startY;

            while (true)
            {
                var mapX = (int)(x / _context.CellSize);
                var mapY = (int)(y / _context.CellSize);

                if (mapX < 0 || mapX >= _mapWidth || mapY < 0 || mapY >= _mapHeight)
                    return double.MaxValue; // Ray out of bounds.

                if (_map[mapY, mapX] > 0)
                    return Math.Sqrt((x - startX) * (x - startX) + (y - startY) * (y - startY)) / _context.CellSize;

                x += rayDirX * 0.1; // Step ray.
                y += rayDirY * 0.1;
            }
        }

        private Color GetWallColor(double distance)
        {
            // Linear blend between wall color and background color based on distance
            var fogFactor = Math.Min(1.0, distance / _context.Distance);
            var wallBaseColor = Color.Gray; // Wall base color
            var backgroundColor = Color.Black; // Background fog color

            // Blend colors based on fog factor
            var red = (int)((1 - fogFactor) * wallBaseColor.R + fogFactor * backgroundColor.R);
            var green = (int)((1 - fogFactor) * wallBaseColor.G + fogFactor * backgroundColor.G);
            var blue = (int)((1 - fogFactor) * wallBaseColor.B + fogFactor * backgroundColor.B);

            return Color.FromArgb(red, green, blue);
        }

        private static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180.0;
        }
    }
}