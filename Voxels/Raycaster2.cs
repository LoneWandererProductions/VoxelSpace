using System;
using System.Drawing;
using Mathematics;

namespace Voxels
{
    public class Camera2
    {
        public double X { get; set; }  // Camera's X position
        public double Y { get; set; }  // Camera's Y position
        public double Angle { get; set; }  // Camera's viewing angle in radians

        public Camera2(double x, double y, double angle)
        {
            X = x;
            Y = y;
            Angle = angle;
        }
    }

    public class Raycaster2
    {
        // Parameters
        private const int ScreenWidth = 800;      // Screen width
        private const int ScreenHeight = 600;     // Screen height
        private const int CellSize = 64;          // Cell size in the map
        private const double Fov = Math.PI / 2;   // Field of view (90° in radians)
        private const int MaxViewDistance = 3;    // Maximum raycast range in cells
        private const int RayCount = ScreenWidth; // Number of rays (1 per screen column)

        // Map (1 = wall, 0 = empty space)
        private readonly int[,] Map = new int[10, 10]
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 1, 0, 0, 0, 1, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 1, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
        };

        public Bitmap RenderFrame(Camera2 camera)
        {
            var bmp = new Bitmap(ScreenWidth, ScreenHeight);

            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.Black);

            for (var i = 0; i < RayCount; i++)
            {
                // Calculate the angle for the current ray
                var rayAngle = camera.Angle - Fov / 2 + (Fov / RayCount) * i;

                CastRay(g, i, rayAngle, camera);
            }

            return bmp;
        }

        private void CastRay(Graphics g, int screenColumn, double rayAngle, Camera2 camera)
        {
            // Raycasting variables
            var rayX = camera.X;
            var rayY = camera.Y;
            var rayDirX = (float)Math.Cos(rayAngle);
            var rayDirY = (float)Math.Sin(rayAngle);
            double distance = 0;

            // Step through the map
            while (distance < MaxViewDistance)
            {
                rayX += rayDirX * 0.1f; // Small increments for precision
                rayY += rayDirY * 0.1f;
                distance += 0.1;

                // Check if ray hits a wall
                var mapX = (int)rayX;
                var mapY = (int)rayY;

                if (mapX < 0 || mapX >= Map.GetLength(0) || mapY < 0 || mapY >= Map.GetLength(1) ||
                    Map[mapX, mapY] != 1) continue;

                // Calculate wall height based on distance
                var wallHeight = (int)(ScreenHeight / (distance * CellSize / 64));
                var wallTop = ScreenHeight / 2 - wallHeight / 2;
                var wallBottom = wallTop + wallHeight;

                // Choose a flat color based on the wall type (map value)
                var wallColor = Color.FromArgb(255, 200, 0); // Example color

                // Draw the vertical stripe for this ray
                using var pen = new Pen(wallColor);
                g.DrawLine(pen, screenColumn, wallTop, screenColumn, wallBottom);

                break; // Stop tracing this ray once it hits a wall
            }
        }

        public void MovePlayer(double deltaX, double deltaY, Camera2 camera)
        {
            camera.X += deltaX;
            camera.Y += deltaY;
        }

        public void RotatePlayer(double deltaAngle, Camera2 camera)
        {
            camera.Angle += deltaAngle;
            camera.Angle %= 2 * Math.PI; // Keep angle in the range [0, 2π]
        }
    }
}
