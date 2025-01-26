/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/RenderRegister.cs
 * PURPOSE:     Basic Configuration methods
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace RenderEngine
{
    /// <summary>
    ///     Some config stuff for the whole render Engine
    /// </summary>
    internal static class RenderRegister
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="RenderRegister" /> is debug.
        /// </summary>
        /// <value>
        ///     <c>true</c> if debug; otherwise, <c>false</c>.
        /// </value>
        public static bool Debug { get; set; }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public static int Height { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public static int Width { get; set; }
    }
}
