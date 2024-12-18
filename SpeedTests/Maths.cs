using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedTests
{
    internal static class Maths
    {
        internal static double CalculateExpectedDistance(double startX, double startY, double angle, int cellsize, int mapWidth, int mapHeight, int[,] map)
        {
            // Convert angle to radians.
            double rad = angle * Math.PI / 180.0;
            double rayDirX = Math.Cos(rad);
            double rayDirY = Math.Sin(rad);

            double x = startX;
            double y = startY;

            while (true)
            {
                int mapX = (int)(x / cellsize);
                int mapY = (int)(y / cellsize);

                if (mapX < 0 || mapX >= mapWidth || mapY < 0 || mapY >= mapHeight)
                    return double.MaxValue; // Out of bounds.

                if (map[mapY, mapX] > 0) // Wall hit.
                {
                    return Math.Sqrt((x - startX) * (x - startX) + (y - startY) * (y - startY));
                }

                x += rayDirX * 0.1; // Small step.
                y += rayDirY * 0.1;
            }
        }

    }
}
