/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Voxels
 * FILE:        Voxels/Slice.cs
 * PURPOSE:     Slice we render
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Drawing;

namespace Voxels
{
    /// <summary>
    ///     Slice of the rendered Image
    /// </summary>
    public sealed class Slice
    {
        /// <summary>
        ///     Gets the shade.
        /// </summary>
        /// <value>
        ///     The shade.
        /// </value>
        public Color Shade { get; init; }

        /// <summary>
        ///     Gets the x1.
        /// </summary>
        /// <value>
        ///     The x1.
        /// </value>
        public int X1 { get; init; }

        /// <summary>
        ///     Gets the y1.
        /// </summary>
        /// <value>
        ///     The y1.
        /// </value>
        public int Y1 { get; init; }

        /// <summary>
        ///     Gets the y2.
        /// </summary>
        /// <value>
        ///     The y2.
        /// </value>
        public int Y2 { get; init; }
    }
}