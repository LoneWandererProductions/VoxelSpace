/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/Geometry.cs
 * PURPOSE:     Graphic Styles we do support
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace RenderEngine
{
    /// <summary>
    ///     Style of the Object
    /// </summary>
    public enum GraphicStyle
    {
        /// <summary>
        ///     The mesh
        /// </summary>
        Mesh = 0,

        /// <summary>
        ///     Fill Graphic
        /// </summary>
        Fill = 1,

        /// <summary>
        ///     Generate only dots
        /// </summary>
        Plot = 2
    }
}
