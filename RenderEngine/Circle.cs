/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/Circle.cs
 * PURPOSE:     Circle Object
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Diagnostics;
using SkiaSharp;

namespace RenderEngine
{
    /// <inheritdoc cref="Geometry" />
    /// <summary>
    ///     Draw a Circle
    /// </summary>
    /// <seealso cref="Geometry" />
    /// <seealso cref="IDrawable" />
    public sealed class Circle : Geometry, IDrawable
    {
        /// <summary>
        ///     Gets or sets the radius.
        /// </summary>
        /// <value>
        ///     The radius.
        /// </value>
        public int Radius { get; set; }

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
            canvas.DrawCircle(Start.X, Start.Y, Radius, paint);

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
            return string.Concat(str, RenderEngineResources.Radius, Radius);
        }
    }
}