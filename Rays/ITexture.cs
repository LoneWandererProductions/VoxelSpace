/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        ITexture.cs
 * PURPOSE:     3D raycaster with per-cell primitives (cubes, ramps, multi-layered)
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace Rays
{
    /// <summary>
    /// Interface for textures so we can mock or plug in different backends.
    /// </summary>
    public interface ITexture
    {
        int Width { get; }
        int Height { get; }

        /// <summary>
        /// Fetch a vertical column of pixels (ARGB ints) for texture mapping.
        /// </summary>
        int[] GetColumn(int x);

        /// <summary>
        /// Direct pixel access (y * Width + x).
        /// </summary>
        int this[int x, int y] { get; }
    }
}
