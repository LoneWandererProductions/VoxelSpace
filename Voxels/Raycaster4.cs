using System;
using System.Drawing;

namespace Voxels
{
    public class Camera4
    {
        // Camera position in the world
        public double PosX { get; set; }
        public double PosY { get; set; }

        // Direction and Field of View as angles in degrees
        public double DirectionAngle { get; set; } // Direction of the camera in degrees
        public double FieldOfView { get; private set; } = 60.0; // Default FOV in degrees

        public Camera4(double posX, double posY, double directionAngle, double fieldOfView = 60.0)
        {
            PosX = posX;
            PosY = posY;
            DirectionAngle = directionAngle;
            FieldOfView = fieldOfView;
        }

        public void Move(double deltaX, double deltaY)
        {
            PosX += deltaX;
            PosY += deltaY;
        }

        public void Rotate(double angle)
        {
            DirectionAngle += angle;
            if (DirectionAngle >= 360) DirectionAngle -= 360;
            if (DirectionAngle < 0) DirectionAngle += 360;
        }
    }

    public class Raycaster4
    {
        private readonly int[,] Map;
        private const int ScreenWidth = 800;
        private const int ScreenHeight = 600;

        public Raycaster4(int[,] map)
        {
            Map = map;
        }

        public Bitmap Render(Camera4 camera)
        {
            Bitmap bitmap = new Bitmap(ScreenWidth, ScreenHeight);
            using Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            // Convert camera's direction and FOV angles to radians
            double dirAngleRad = camera.DirectionAngle * Math.PI / 180;
            double fovRad = camera.FieldOfView * Math.PI / 180;

            // Calculate direction and FOV plane vectors
            double dirX = Math.Cos(dirAngleRad);
            double dirY = Math.Sin(dirAngleRad);
            double planeX = -dirY * Math.Tan(fovRad / 2);
            double planeY = dirX * Math.Tan(fovRad / 2);

            for (int x = 0; x < ScreenWidth; x++)
            {
                // Map x-coordinate to camera space
                double cameraX = 2 * x / (double)ScreenWidth - 1; // Range: [-1, 1]
                double rayDirX = dirX + planeX * cameraX;
                double rayDirY = dirY + planeY * cameraX;

                int mapX = (int)camera.PosX;
                int mapY = (int)camera.PosY;

                double deltaDistX = (rayDirX == 0) ? double.MaxValue : Math.Abs(1 / rayDirX);
                double deltaDistY = (rayDirY == 0) ? double.MaxValue : Math.Abs(1 / rayDirY);

                double sideDistX, sideDistY;

                int stepX, stepY;

                if (rayDirX < 0)
                {
                    stepX = -1;
                    sideDistX = (camera.PosX - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0 - camera.PosX) * deltaDistX;
                }

                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = (camera.PosY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0 - camera.PosY) * deltaDistY;
                }

                bool hit = false;
                int side = 0;

                while (!hit)
                {
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }

                    if (Map[mapX, mapY] > 0) hit = true;
                }

                double perpWallDist = (side == 0)
                    ? (mapX - camera.PosX + (1 - stepX) / 2) / rayDirX
                    : (mapY - camera.PosY + (1 - stepY) / 2) / rayDirY;

                int lineHeight = (int)(ScreenHeight / perpWallDist);
                int drawStart = -lineHeight / 2 + ScreenHeight / 2;
                int drawEnd = lineHeight / 2 + ScreenHeight / 2;

                if (drawStart < 0) drawStart = 0;
                if (drawEnd >= ScreenHeight) drawEnd = ScreenHeight - 1;

                Color wallColor = side == 0 ? Color.Green : Color.DarkGreen;
                using Pen pen = new Pen(wallColor);
                g.DrawLine(pen, x, drawStart, x, drawEnd);
            }

            return bitmap;
        }
    }
}