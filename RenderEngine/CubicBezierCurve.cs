/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/CubicBezierCurves.cs
 * PURPOSE:     Curve objects to form a long curvy line
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mathematics;
using SkiaSharp;

namespace RenderEngine
{
    /// <inheritdoc cref="Geometry" />
    /// <summary>
    ///     Draw a Bezier Curve
    /// </summary>
    /// <seealso cref="Geometry" />
    /// <seealso cref="IDrawable" />
    public sealed class CubicBezierCurve : Geometry, IDrawable
    {
        /// <summary>
        ///     Gets or sets the first control point.
        /// </summary>
        /// <value>
        ///     The first.
        /// </value>
        public List<Coordinate2D> Path { get; set; } = new();

        /// <inheritdoc />
        /// <summary>
        ///     Draws the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="paint">The paint.</param>
        /// <param name="style">The style.</param>
        /// <returns>
        ///     Success Status.
        /// </returns>
        public bool Draw(SKCanvas canvas, SKPaint paint, GraphicStyle style)
        {
            //check if it division is possible
            if (Path.Count % 3 != 0) return false;

            using var path = RenderHelper.CreatePath(Start, Path);

            // Move to the starting point
            path.MoveTo(Start.X, Start.Y);

            // Draw a cubic Bezier curve, define Start Point
            path.MoveTo(Start.X, Start.Y);

            for (var i = 0; i < Path.Count; i += 3)
                // Draw a cubic Bezier curve
                if (i + 2 < Path.Count)
                    path.CubicTo(path[i], path[i + 1], path[i + 2]);
                // Draw a quadratic Bezier curve (if there are only three control points left)
                else if (i + 1 < Path.Count) path.QuadTo(path[i], path[i + 1]);

            switch (style)
            {
                // Choose drawing style
                case GraphicStyle.Mesh:
                    paint.Style = SKPaintStyle.Stroke;
                    paint.StrokeWidth = StrokeWidth;
                    break;
                case GraphicStyle.Fill:
                {
                    using var fillPaint = new SKPaint { Style = SKPaintStyle.Fill };
                    canvas.DrawPath(path, fillPaint);
                    return true; // No need to draw the stroke in the Fill style
                }
                case GraphicStyle.Plot:
                    paint.StrokeWidth = StrokeWidth;
                    foreach (var plot in Path) RenderHelper.DrawPoint(canvas, plot, paint);

                    break;
            }

            if (RenderRegister.Debug) Trace.WriteLine(ToString());

            // Draw the path
            canvas.DrawPath(path, paint);

            return true;
        }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var str = string.Concat(Start.ToString(), Environment.NewLine);
            var last = Path.Last();
            Path.Remove(last);

            str = Path.Aggregate(str, (current, plot) => string.Concat(current, plot.ToString(), Environment.NewLine));

            str = string.Concat(str, last.ToString());

            return str;
        }
    }
}