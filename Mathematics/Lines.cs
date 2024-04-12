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
        ///     Create Coordinates for a line.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public static List<Coordinate2D> LinearLine(Coordinate2D from, Coordinate2D to)
        {
            var lst = new List<Coordinate2D>();

            if (from.Equals(to))
            {
                lst.Add(from);
                return lst;
            }

            if (to.Y == from.Y)
            {
                for (var x = from.X; x <= to.X; x++)
                {
                    var point = new Coordinate2D { X = x, Y = to.Y };
                    lst.Add(point);
                }

                return lst;
            }

            if (to.X == from.X)
            {
                lst = new List<Coordinate2D>();

                for (var y = from.Y; y <= to.Y; y++)
                {
                    var point = new Coordinate2D { X = from.X, Y = y };
                    lst.Add(point);
                }

                return lst;
            }

            lst = new List<Coordinate2D>();

            var m = (to.Y - from.Y) / (to.X - from.X);
            var n = from.Y - (m * from.X);

            for (var x = from.X; x <= to.X; x++)
            {
                var y = (m * x) + n;
                var point = new Coordinate2D { X = x, Y = y };
                lst.Add(point);
            }

            return lst;
        }

        /// <summary>
        ///     Create Coordinates for a line.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public static List<Vector3D> LinearLine(Vector3D from, Vector3D to)
        {
            var lst = new List<Vector3D>();

            if (from.Equals(to))
            {
                lst.Add(from);
                return lst;
            }

            if (Math.Abs(to.Y - from.Y) < MathResources.Tolerance)
            {
                for (var x = from.X; x <= to.X; x++)
                {
                    var point = new Vector3D { X = x, Y = to.Y };
                    lst.Add(point);
                }

                return lst;
            }

            if (Math.Abs(to.X - from.X) < MathResources.Tolerance)
            {
                for (var y = from.Y; y <= from.Y; y++)
                {
                    var point = new Vector3D { X = from.X, Y = y };
                    lst.Add(point);
                }

                return lst;
            }

            var m = (to.Y - from.Y) / (to.X - from.X);
            var n = from.Y - (m * from.X);

            for (var x = from.X; x <= to.X; x++)
            {
                var y = (m * x) + n;
                var point = new Vector3D { X = x, Y = y };
                lst.Add(point);
            }

            return lst;
        }
    }
}
