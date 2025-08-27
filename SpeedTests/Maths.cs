/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SpeedTests
 * FILE:        Maths.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;

namespace SpeedTests;

internal static class Maths
{
    internal static double CalculateExpectedDistance(double startX, double startY, double angle, int cellsize,
        int mapWidth, int mapHeight, int[,] map)
    {
        // Convert angle to radians.
        var rad = angle * Math.PI / 180.0;
        var rayDirX = Math.Cos(rad);
        var rayDirY = Math.Sin(rad);

        var x = startX;
        var y = startY;

        while (true)
        {
            var mapX = (int)(x / cellsize);
            var mapY = (int)(y / cellsize);

            if (mapX < 0 || mapX >= mapWidth || mapY < 0 || mapY >= mapHeight)
                return double.MaxValue; // Out of bounds.

            if (map[mapY, mapX] > 0) // Wall hit.
                return Math.Sqrt((x - startX) * (x - startX) + (y - startY) * (y - startY));

            x += rayDirX * 0.1; // Small step.
            y += rayDirY * 0.1;
        }
    }
}