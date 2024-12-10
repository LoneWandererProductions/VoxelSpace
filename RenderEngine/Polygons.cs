/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/Polygons.cs
 * PURPOSE:     Polygons object
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable ClassNeverInstantiated.Global

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
    ///     Our Polygon Object
    /// </summary>
    /// <seealso cref="Geometry" />
    /// <seealso cref="IDrawable" />
    public sealed class Polygons : Geometry, IDrawable
    {
        /// <summary>
        ///     Gets or sets the path.
        /// </summary>
        /// <value>
        ///     The path.
        /// </value>
        public List<Coordinate2D> Path { get; set; } = new();

        /// <inheritdoc />
        /// <summary>
        ///     Draws the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="paint">The paint.</param>
        /// <param name="style">The style.</param>
        /// <exception cref="ArgumentOutOfRangeException">style - null</exception>
        /// <returns>
        ///     Success status of the drawing
        /// </returns>
        public bool Draw(SKCanvas canvas, SKPaint paint, GraphicStyle style)
        {
            using var path = RenderHelper.CreatePath(Start, Path);

            // Fill or stroke the polygon
            switch (style)
            {
                case GraphicStyle.Mesh:
                    paint.StrokeWidth = StrokeWidth;
                    canvas.DrawPath(path, paint);
                    break;
                case GraphicStyle.Fill:
                {
                    //using var fillPaint = new SKPaint { Style = SKPaintStyle.Fill };
                    var colors = new[] { SKColors.Red, SKColors.Blue };
                    var positions = new float[] { 0, 1 };

                    // Create a linear gradient shader
                    var shader = SKShader.CreateLinearGradient(
                        new SKPoint(0, 0), // Start point
                        new SKPoint(0, RenderRegister.Height), // End point
                        colors, // Colors
                        positions, // Color positions
                        SKShaderTileMode.Clamp); // Tile mode

                    // Create a paint with the gradient shader
                    var shade = new SKPaint { Shader = shader };

                    canvas.DrawPath(path, shade);
                    return true; // No need to draw the stroke in the Fill style
                }
                case GraphicStyle.Plot:
                    foreach (var plot in Path) RenderHelper.DrawPoint(canvas, plot, paint);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }

            if (RenderRegister.Debug) Trace.WriteLine(ToString());

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
            _ = Path.Remove(last);

            str = Path.Aggregate(str, (current, plot) => string.Concat(current, plot.ToString(), Environment.NewLine));

            str = string.Concat(str, last.ToString());

            return str;
        }
    }
}