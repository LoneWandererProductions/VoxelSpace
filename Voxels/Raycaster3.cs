using System;
using System.Drawing;

namespace Voxels
{    public class Camera3
    {
        //These represent the position of the camera in the 2D world:
        public double PosX { get; set; }
        public double PosY { get; set; }
        //These represent the direction vector of the camera:
        public double DirX { get; set; }
        public double DirY { get; set; }
        //FOV
        public double PlaneX { get; set; }
        public double PlaneY { get; set; }
        public double FieldOfView { get; private set; } = Math.PI / 3; // Default FOV ~60°

        public Camera3(double posX, double posY, double dirX, double dirY, double planeX, double planeY)
        {
            PosX = posX;
            PosY = posY;
            DirX = dirX;
            DirY = dirY;
            PlaneX = planeX;
            PlaneY = planeY;
        }

        public void Move(double deltaX, double deltaY)
        {
            PosX += deltaX;
            PosY += deltaY;
        }

        public void Rotate(double angle)
        {
            double oldDirX = DirX;
            DirX = DirX * Math.Cos(angle) - DirY * Math.Sin(angle);
            DirY = oldDirX * Math.Sin(angle) + DirY * Math.Cos(angle);

            double oldPlaneX = PlaneX;
            PlaneX = PlaneX * Math.Cos(angle) - PlaneY * Math.Sin(angle);
            PlaneY = oldPlaneX * Math.Sin(angle) + PlaneY * Math.Cos(angle);
        }
    }

    public class Raycaster3
    {
        private readonly int[,] Map;
        private const int CellSize = 64;
        private const int ScreenWidth = 800;
        private const int ScreenHeight = 600;

        public Raycaster3(int[,] map)
        {
            Map = map;
        }

        public Bitmap Render(Camera3 camera)
        {
            Bitmap bitmap = new Bitmap(ScreenWidth, ScreenHeight);
            using Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            for (int x = 0; x < ScreenWidth; x++)
            {
                double cameraX = 2 * x / (double)ScreenWidth - 1;
                double rayDirX = camera.DirX + camera.PlaneX * cameraX;
                double rayDirY = camera.DirY + camera.PlaneY * cameraX;

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
