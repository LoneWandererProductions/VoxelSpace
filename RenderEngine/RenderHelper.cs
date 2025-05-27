/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/RenderHelper.cs
 * PURPOSE:     Helper Methods
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using Mathematics;
using SkiaSharp;

namespace RenderEngine
{
    /// <summary>
    ///     Some generic stuff, that is used everywhere
    /// </summary>
    internal static class RenderHelper
    {
        /// <summary>
        ///     Draws the point.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        /// <param name="skPaint">The sk paint.</param>
        internal static void DrawPoint(SKCanvas canvas, Coordinate2D point, SKPaint skPaint)
        {
            canvas.DrawPoint(point.X, point.Y, skPaint);
        }

        /// <summary>
        ///     Creates the path.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="path">The path.</param>
        /// <returns>Converted Path</returns>
        internal static SKPath CreatePath(Coordinate2D start, List<Coordinate2D> path)
        {
            var skPath = new SKPath();

            skPath.MoveTo(start.X, start.Y); // Move to the starting point

            foreach (var line in path) skPath.LineTo(line.X, line.Y); // Line to the points

            skPath.Close(); // Close the path to complete the polygon

            return skPath;
        }
    }
}