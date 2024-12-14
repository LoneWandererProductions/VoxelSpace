using System;
using System.Drawing;
using Mathematics;

namespace Voxels
{
    public class RasterRaycast
    {
        internal Camera Camera { get; set; }

        private int[,] _map;

        public RasterRaycast(Camera camera, int[,] map)
        {
            Camera = camera;
            _map = map;
        }

        public Bitmap Render()
        {
            var bmp = new Bitmap(Camera.ScreenWidth, Camera.ScreenHeight);

            // Starting the raycasting at the player's camera angle
            var rayAngle = Camera.Angle - 45; // Horizontal angle of the first ray (adjust for camera field of view)
            var pitchAngle = Camera.Pitch; // Vertical angle (pitch)
            var angleIncrement = 1; // 1 degree per step for horizontal angles

            // Loop over every vertical pixel on the screen (each column of pixels)
            for (var x = 0; x < Camera.ScreenWidth; x++)
            {
                // Ray direction based on the camera angle and pitch
                var rayDirX = ExtendedMath.CalcCos(rayAngle) * ExtendedMath.CalcCos(pitchAngle);
                var rayDirY = ExtendedMath.CalcSin(rayAngle) * ExtendedMath.CalcCos(pitchAngle);

                float rayPosX = Camera.X;
                float rayPosY = Camera.Y;

                // Step size for ray movement
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

                // Raycasting loop: cast rays until they hit a wall
                while (!hitWall)
                {
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0; // x-axis collision
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1; // y-axis collision
                    }

                    // Check if the ray has hit a wall
                    if (_map[mapX, mapY] == 1)
                        hitWall = true;
                }

                // Calculate the perpendicular distance to the wall
                var perpWallDist = (side == 0) ? (sideDistX - deltaDistX) : (sideDistY - deltaDistY);

                // Calculate the height of the wall (how tall it appears on the screen)
                var lineHeight = (int)(Math.Abs(Camera.ScreenHeight / perpWallDist));

                // Calculate the vertical position on the screen for drawing the wall
                var drawStart = Math.Max(0, (Camera.ScreenHeight / 2) - (lineHeight / 2));
                var drawEnd = Math.Min(Camera.ScreenHeight - 1, (Camera.ScreenHeight / 2) + (lineHeight / 2));

                // Choose wall color based on the side it was hit for shading
                var wallColor = (side == 0) ? Color.Red : Color.Blue;

                // Draw the wall strip for the column x
                for (var y = drawStart; y < drawEnd; y++)
                {
                    bmp.SetPixel(x, y, wallColor);
                }

                // Move to the next angle for the next ray (column of pixels)
                rayAngle += angleIncrement;
                if (rayAngle >= 360) rayAngle -= 360; // Keep the angle within 0-360 degrees
            }

            return bmp;
        }
    }

}
