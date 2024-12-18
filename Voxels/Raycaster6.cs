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
            using Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            double halfFov = camera.Fov / 2.0;
            double angleStep = camera.Fov / screenWidth;

            for (int x = 0; x < screenWidth; x++)
            {
                double rayAngle = (camera.Direction - halfFov) + x * angleStep;
                double rayX = Math.Cos(DegreeToRadian(rayAngle));
                double rayY = Math.Sin(DegreeToRadian(rayAngle));

                double distanceToWall = CastRay(camera.X, camera.Y, rayX, rayY);

                int wallHeight = (int)(_cellSize * screenHeight / (distanceToWall * _cellSize));
                int wallTop = Math.Max(0, (screenHeight - wallHeight) / 2);
                int wallBottom = Math.Min(screenHeight, (screenHeight + wallHeight) / 2);

                Color wallColor = GetWallColor(distanceToWall);

                g.DrawLine(new Pen(wallColor), x, wallTop, x, wallBottom);
            }

            return bitmap;
        }

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
                    return Math.Sqrt((x - startX) * (x - startX) + (y - startY) * (y - startY));

                x += rayDirX * 0.1; // Step ray.
                y += rayDirY * 0.1;
            }
        }

        private Color GetWallColor(double distance)
        {
            int intensity = Math.Max(0, 255 - (int)(distance * 10));
            return Color.FromArgb(intensity, intensity, intensity);
        }

        private double DegreeToRadian(double degree) => degree * Math.PI / 180.0;
    }
}

public class Camera6
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Fov { get; set; }
    public double Direction { get; set; }

    public Camera6(double x, double y, double fov, double direction)
    {
        X = x;
        Y = y;
        Fov = fov;
        Direction = direction;
    }
}
