/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/Rectangular.cs
 * PURPOSE:     Rectangular object
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mathematics;
using SkiaSharp;

namespace RenderEngine
{
    /// <inheritdoc cref="Geometry" />
    /// <summary>
    ///     Basic Rectangular Object
    /// </summary>
    /// <seealso cref="Geometry" />
    /// <seealso cref="IDrawable" />
    public sealed class Rectangular : Geometry, IDrawable
    {
        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public int Width { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Draws the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="paint">The paint.</param>
        /// <param name="style">The style.</param>
        /// <exception cref="ArgumentOutOfRangeException">style - null</exception>
        /// <returns>
        ///     Success Status.
        /// </returns>
        public bool Draw(SKCanvas canvas, SKPaint paint, GraphicStyle style)
        {
            var c1 = new Coordinate2D(Start.X + Width, Start.Y);
            var c2 = new Coordinate2D(Start.X, Start.Y - Height);
            var c3 = new Coordinate2D(Start.X + Width, Start.Y - Height);
            var path = new List<Coordinate2D> { c1, c2, c3 };
            var skPath = RenderHelper.CreatePath(Start, path);

            if (RenderRegister.Debug) Trace.WriteLine(ToString());

            // Fill or stroke the Rectangle
            switch (style)
            {
                case GraphicStyle.Mesh:
                    paint.StrokeWidth = StrokeWidth;
                    canvas.DrawPath(skPath, paint);
                    break;
                case GraphicStyle.Fill:
                {
                    using var fillPaint = new SKPaint { Style = SKPaintStyle.Fill };
                    canvas.DrawPath(skPath, fillPaint);
                }
                    break;
                case GraphicStyle.Plot:
                    foreach (var plot in path) RenderHelper.DrawPoint(canvas, plot, paint);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }

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
            str = string.Concat(str, RenderEngineResources.Height, Height, Environment.NewLine);
            return string.Concat(str, RenderEngineResources.Width, Width);
        }
    }
}