using System;
using System.Drawing;

namespace Voxels
{
    public sealed class RasterRaycast
    {
        internal Camera Camera { get; set; }

        private int[,] _map;

        internal RasterRaycast(Camera camera, int[,] map)
        {
            Camera = camera;
            _map = map;
        }
        public Bitmap Render(bool drawOutlines = true)
        {
            var width = 800; // Screen width
            var height = 600; // Screen height
            var bitmap = new Bitmap(width, height);

            using var g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black); // Clear the screen to black

            var fov = Math.PI / 4; // Field of view (45 degrees)
            var halfFov = fov / 2;
            var step = fov / width;

            for (var x = 0; x < width; x++)
            {
                var rayAngle = (Camera.Angle - halfFov + x * step) * Math.PI / 180;

                // Calculate ray direction
                var rayDirX = Math.Cos(rayAngle);
                var rayDirY = Math.Sin(rayAngle);

                // Starting position of the ray
                double posX = Camera.X;
                double posY = Camera.Y;

                // Delta distances
                var deltaDistX = Math.Abs(1 / rayDirX);
                var deltaDistY = Math.Abs(1 / rayDirY);

                var mapX = (int)posX;
                var mapY = (int)posY;

                double sideDistX;
                double sideDistY;

                // Step direction
                int stepX;
                int stepY;

                if (rayDirX < 0)
                {
                    stepX = -1;
                    sideDistX = (posX - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0 - posX) * deltaDistX;
                }

                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = (posY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0 - posY) * deltaDistY;
                }

                // Perform DDA
                var hit = false;
                var side = 0;
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

                    if (_map[mapX, mapY] == 1) hit = true;
                }

                // Calculate distance to wall
                double perpWallDist;
                if (side == 0)
                    perpWallDist = (mapX - posX + (1 - stepX) / 2) / rayDirX;
                else
                    perpWallDist = (mapY - posY + (1 - stepY) / 2) / rayDirY;

                // Log for debugging
                if (perpWallDist <= 0)
                {
                    Console.WriteLine($"Warning: Invalid perpWallDist at x={x}, perpWallDist={perpWallDist}");
                    perpWallDist = 0.01; // Avoid division by zero or negative distances
                }

                // Calculate line height and check for extreme values
                var lineHeight = (int)(height / perpWallDist);
                if (lineHeight > height)
                    lineHeight = height;

                var drawStart = -lineHeight / 2 + height / 2;
                var drawEnd = lineHeight / 2 + height / 2;

                // Clamp drawStart and drawEnd within screen bounds
                drawStart = Math.Max(0, drawStart);
                drawEnd = Math.Min(height - 1, drawEnd);

                // Log values for debugging
                Console.WriteLine($"x={x}, perpWallDist={perpWallDist}, lineHeight={lineHeight}, drawStart={drawStart}, drawEnd={drawEnd}");

                // Use a solid color to test drawing
                Color color = Color.Gray;

                // Draw the wall itself
                for (var y = drawStart; y <= drawEnd; y++)
                {
                    bitmap.SetPixel(x, y, color); // Fill the wall with color
                }

                if (drawOutlines)
                {
                    // Draw only the top and bottom pixels of the wall (outline)
                    bitmap.SetPixel(x, drawStart, Color.White); // Top of the wall
                    bitmap.SetPixel(x, drawEnd, Color.White);   // Bottom of the wall

                    // Draw vertical separators only between adjacent walls
                    if (x > 0) // Draw left separator if not the first column
                    {
                        for (var y = drawStart; y <= drawEnd; y++)
                        {
                            bitmap.SetPixel(x - 1, y, Color.White); // Left separator
                        }
                    }
                    if (x < width - 1) // Draw right separator if not the last column
                    {
                        for (var y = drawStart; y <= drawEnd; y++)
                        {
                            bitmap.SetPixel(x + 1, y, Color.White); // Right separator
                        }
                    }
                }
            }

            return bitmap;
        }






    }
}
