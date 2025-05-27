/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/Line.cs
 * PURPOSE:     Line Object
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using Mathematics;

namespace RenderEngine
{
    /// <inheritdoc cref="Geometry" />
    /// <summary>
    ///     Draw a Circle
    /// </summary>
    /// <seealso cref="Geometry" />
    /// <seealso cref="IDrawable" />
    public sealed class Line : Geometry
    {
        /// <summary>
        ///     Gets or sets the end point.
        /// </summary>
        /// <value>
        ///     The end point.
        /// </value>
        public Coordinate2D EndPoint { get; set; }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var str = string.Concat(Start.ToString(), Environment.NewLine);
            return string.Concat(str, EndPoint.ToString());
        }
    }
}