/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/Bresenham.cs
 * PURPOSE:     Implementation of the Bresenham Algorithm, Helps drawing lines.
 *              Alternative Wu's Algorithm TODO
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * Source:      https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
 *              https://en.wikipedia.org/wiki/Xiaolin_Wu%27s_line_algorithm
 */

using System;
using System.Collections.Generic;

namespace Mathematics
{
    /// <summary>
    ///     Bresenham Implementation
    /// </summary>
    public static class Lines
    {
        /// <summary>
        ///     Create Coordinates for a line using Bresenham's algorithm (2D).
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns>List of Coordinates as line</returns>
        public static List<Coordinate2D> BresenhamLine(Coordinate2D from, Coordinate2D to)
        {
            var lst = new List<Coordinate2D>();

            var dx = to.X - from.X;
            var dy = to.Y - from.Y;
            var dx2 = 2 * Math.Abs(dx);
            var dy2 = 2 * Math.Abs(dy);
            var x = from.X;
            var y = from.Y;
            var xEnd = to.X;
            var yEnd = to.Y;

            var signX = Math.Sign(dx);
            var signY = Math.Sign(dy);

            if (Math.Abs(dy) < Math.Abs(dx))
            {
                // Line is mostly horizontal
                var error = dy2 - Math.Abs(dx);
                while (x != xEnd)
                {
                    lst.Add(new Coordinate2D { X = x, Y = y });
                    if (error > 0)
                    {
                        y += signY;
                        error -= dx2;
                    }

                    x += signX;
                    error += dy2;
                }
            }
            else
            {
                // Line is mostly vertical
                var error = dx2 - Math.Abs(dy);
                while (y != yEnd)
                {
                    lst.Add(new Coordinate2D { X = x, Y = y });
                    if (error > 0)
                    {
                        x += signX;
                        error -= dy2;
                    }

                    y += signY;
                    error += dx2;
                }
            }

            lst.Add(new Coordinate2D { X = x, Y = y }); // Add the last point
            return lst;
        }

        /// <summary>
        ///     Create Coordinates for a line using Bresenham's algorithm (3D).
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns>List of Coordinates as line</returns>
        public static List<Vector3D> BresenhamLine(Vector3D from, Vector3D to)
        {
            var lst = new List<Vector3D>();

            var dx = (int)(to.X - from.X);
            var dy = (int)(to.Y - from.Y);
            var dz = (int)(to.Z - from.Z);
            var dx2 = 2 * Math.Abs(dx);
            var dy2 = 2 * Math.Abs(dy);
            var dz2 = 2 * Math.Abs(dz);
            var x = (int)from.X;
            var y = (int)from.Y;
            var z = (int)from.Z;
            var xEnd = (int)to.X;
            var yEnd = (int)to.Y;
            var zEnd = (int)to.Z;

            var signX = Math.Sign(dx);
            var signY = Math.Sign(dy);
            var signZ = Math.Sign(dz);

            if (Math.Abs(dy) < Math.Abs(dx) && Math.Abs(dy) < Math.Abs(dz))
            {
                // Line is mostly horizontal
                var error1 = dy2 - Math.Abs(dx);
                var error2 = dz2 - Math.Abs(dx);
                while (x != xEnd)
                {
                    lst.Add(new Vector3D { X = x, Y = y, Z = z });
                    if (error1 > 0)
                    {
                        y += signY;
                        error1 -= dx2;
                    }

                    if (error2 > 0)
                    {
                        z += signZ;
                        error2 -= dx2;
                    }

                    x += signX;
                    error1 += dy2;
                    error2 += dz2;
                }
            }
            else if (Math.Abs(dz) < Math.Abs(dx) && Math.Abs(dz) < Math.Abs(dy))
            {
                // Line is mostly in the Z direction
                var error1 = dy2 - Math.Abs(dz);
                var error2 = dx2 - Math.Abs(dz);
                while (z != zEnd)
                {
                    lst.Add(new Vector3D { X = x, Y = y, Z = z });
                    if (error1 > 0)
                    {
                        y += signY;
                        error1 -= dz2;
                    }

                    if (error2 > 0)
                    {
                        x += signX;
                        error2 -= dz2;
                    }

                    z += signZ;
                    error1 += dy2;
                    error2 += dx2;
                }
            }
            else
            {
                // Line is mostly vertical
                var error1 = dx2 - Math.Abs(dy);
                var error2 = dz2 - Math.Abs(dy);
                while (y != yEnd)
                {
                    lst.Add(new Vector3D { X = x, Y = y, Z = z });
                    if (error1 > 0)
                    {
                        x += signX;
                        error1 -= dy2;
                    }

                    if (error2 > 0)
                    {
                        z += signZ;
                        error2 -= dy2;
                    }

                    y += signY;
                    error1 += dx2;
                    error2 += dz2;
                }
            }

            lst.Add(new Vector3D { X = x, Y = y, Z = z }); // Add the last point
            return lst;
        }
    }
}
