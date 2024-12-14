using System;
using System.Drawing;

namespace Voxels
{
    public class RasterRaycast
    {
        private Camera _camera;
        private int[,] _map;

        public RasterRaycast(Camera camera, int[,] map)
        {
            _camera = camera;
            _map = map;
        }

        public Bitmap Render()
        {
            var bmp = new Bitmap(_camera.ScreenWidth, _camera.ScreenHeight);
            float rayAngle = _camera.Angle - (float)Math.PI / 4; // Starting angle of the first ray (viewing angle)
            var angleIncrement = (float)Math.PI / 180; // The angle between rays (1 degree)

            for (var x = 0; x < _camera.ScreenWidth; x++)
            {
                // Calculate direction of ray for each column of the screen
                var rayDirX = (float)Math.Cos(rayAngle);
                var rayDirY = (float)Math.Sin(rayAngle);

                float rayPosX = _camera.X;
                float rayPosY = _camera.Y;

                // Step sizes for horizontal and vertical ray movement
                var deltaDistX = Math.Abs(1 / rayDirX);
                var deltaDistY = Math.Abs(1 / rayDirY);

                var mapX = (int)rayPosX;
                var mapY = (int)rayPosY;

                var stepX = (rayDirX < 0) ? -1 : 1;
                var stepY = (rayDirY < 0) ? -1 : 1;

                var sideDistX = (rayDirX < 0) ? (rayPosX - mapX) * deltaDistX : (mapX + 1.0f - rayPosX) * deltaDistX;
                var sideDistY = (rayDirY < 0) ? (rayPosY - mapY) * deltaDistY : (mapY + 1.0f - rayPosY) * deltaDistY;

                var hitWall = false;
                var side = 0;

                // Raycasting loop
                while (!hitWall)
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

                    // Check if the ray has hit a wall
                    if (_map[mapX, mapY] == 1)
                        hitWall = true;
                }

                // Calculate the perpendicular distance to the wall
                var perpWallDist = (side == 0) ? (sideDistX - deltaDistX) : (sideDistY - deltaDistY);

                // Calculate the height of the wall based on the distance
                var lineHeight = (int)(Math.Abs(_camera.ScreenHeight / perpWallDist));

                // Calculate the vertical position on the screen where the wall should be drawn
                var drawStart = Math.Max(0, (_camera.ScreenHeight / 2) - (lineHeight / 2));
                var drawEnd = Math.Min(_camera.ScreenHeight - 1, (_camera.ScreenHeight / 2) + (lineHeight / 2));

                // Choose wall color based on the side it was hit for shading effect
                var wallColor = (side == 0) ? Color.Red : Color.Blue;

                // Draw the vertical line for the wall in the bitmap
                for (var y = drawStart; y < drawEnd; y++)
                {
                    bmp.SetPixel(x, y, wallColor);
                }

                // Update the ray angle for the next column (moving the camera's view angle)
                rayAngle += angleIncrement;
                if (rayAngle >= 2 * Math.PI) rayAngle -= (float)(2 * Math.PI); // Ensure rayAngle is within 0 to 2*Pi
            }

            return bmp;
        }
    }
}
