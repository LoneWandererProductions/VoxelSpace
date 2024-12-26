using System;
using System.Drawing;

namespace Voxels
{
    public class Raycaster6
    {
        private readonly int[,] _map;
        private readonly int _cellSize;
        private readonly int _mapWidth;
        private readonly int _mapHeight;

        public Raycaster6(int[,] map, int cellSize)
        {
            _map = map;
            _cellSize = cellSize;
            _mapWidth = map.GetLength(1);
            _mapHeight = map.GetLength(0);
        }

        public Bitmap Render(Camera6 camera, int screenWidth, int screenHeight)
        {
            Bitmap bitmap = new(screenWidth, screenHeight);
            using var g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            var halfFov = camera.Fov / 2.0;
            var angleStep = camera.Fov / screenWidth;

            for (var x = 0; x < screenWidth; x++)
            {
                var rayAngle = (camera.Direction - halfFov) + x * angleStep;
                var rayX = Math.Cos(DegreeToRadian(rayAngle));
                var rayY = Math.Sin(DegreeToRadian(rayAngle));

                var distanceToWall = CastRay(camera.X, camera.Y, rayX, rayY);

                // Adjusted wall height calculation
                var wallHeight = (int)(screenHeight / distanceToWall);
                var wallTop = Math.Max(0, (screenHeight - wallHeight) / 2);
                var wallBottom = Math.Min(screenHeight, (screenHeight + wallHeight) / 2);

                var wallColor = GetWallColor(distanceToWall);

                g.DrawLine(new Pen(wallColor), x, wallTop, x, wallBottom);
            }

            return bitmap;
        }

        //for testing public
        public double CastRay(double startX, double startY, double rayDirX, double rayDirY)
        {
            double x = startX;
            double y = startY;

            while (true)
            {
                int mapX = (int)(x / _cellSize);
                int mapY = (int)(y / _cellSize);

                if (mapX < 0 || mapX >= _mapWidth || mapY < 0 || mapY >= _mapHeight)
                    return double.MaxValue; // Ray out of bounds.

                if (_map[mapY, mapX] > 0)
                    return Math.Sqrt((x - startX) * (x - startX) + (y - startY) * (y - startY)) / _cellSize;

                x += rayDirX * 0.1; // Step ray.
                y += rayDirY * 0.1;
            }
        }


        private Color GetWallColor(double distance)
        {
            return Color.Blue;

            var intensity = Math.Max(0, 255 - (int)(distance * 10));
            return Color.FromArgb(intensity, intensity, intensity);
        }

        private static double DegreeToRadian(double degree) => degree * Math.PI / 180.0;
    }
}