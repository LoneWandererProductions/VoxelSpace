using System;
using System.Drawing;
using Voxels;

namespace SpeedTests
{
    public class Raycaster6
    {
        private readonly int[,] _map;
        private readonly int _mapWidth;
        private readonly int _mapHeight;
        private readonly CameraContext _context;

        public Raycaster6(int[,] map, CameraContext context)
        {
            _map = map;
            _mapWidth = map.GetLength(1);
            _mapHeight = map.GetLength(0);
            _context = context;
        }

        public Bitmap Render(Camera camera)
        {
            Bitmap bitmap = new(_context.ScreenWidth, _context.ScreenHeight);
            using var g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            var halfFov = camera.Fov / 2.0;
            var angleStep = camera.Fov / _context.ScreenWidth;

            for (var x = 0; x < _context.ScreenWidth; x++)
            {
                var rayAngle = (camera.Angle - halfFov) + x * angleStep;
                var rayX = Math.Cos(DegreeToRadian(rayAngle));
                var rayY = Math.Sin(DegreeToRadian(rayAngle));

                var distanceToWall = CastRay(camera.X, camera.Y, rayX, rayY);

                // Adjusted wall height calculation
                var wallHeight = (int)(_context.ScreenHeight / distanceToWall);

                // Apply pitch adjustment
                var pitchOffset = (int)(camera.Pitch * (_context.ScreenHeight / 180.0)); // Scale pitch to screen height
                var wallTop = Math.Max(0, (_context.ScreenHeight - wallHeight) / 2 - pitchOffset);
                var wallBottom = Math.Min(_context.ScreenHeight, (_context.ScreenHeight + wallHeight) / 2 - pitchOffset);

                var wallColor = GetWallColor(distanceToWall);

                g.DrawLine(new Pen(wallColor), x, wallTop, x, wallBottom);
            }

            return bitmap;
        }

        //for testing public
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
            var intensity = Math.Max(0, 255 - (int)(distance * 10));
            return Color.FromArgb(intensity, intensity, intensity);
        }

        private static double DegreeToRadian(double degree) => degree * Math.PI / 180.0;
    }
}
