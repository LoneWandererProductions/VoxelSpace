/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/IDrawable.cs
 * PURPOSE:     The draw Interface
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using Mathematics;
using SkiaSharp;

namespace RenderEngine
{
    /// <summary>
    ///     Generate the basic Interface that is needed for the Objects to be drawn
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        ///     Gets the color.
        /// </summary>
        /// <value>
        ///     The color.
        /// </value>
        SKColor Color { get; }

        /// <summary>
        ///     Gets the start.
        /// </summary>
        /// <value>
        ///     The start.
        /// </value>
        Coordinate2D Start { get; }

        /// <summary>
        ///     Draws the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="paint">The paint.</param>
        /// <param name="style">The style.</param>
        bool Draw(SKCanvas canvas, SKPaint paint, GraphicStyle style);
    }
}
