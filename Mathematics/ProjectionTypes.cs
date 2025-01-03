/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/ProjectionTypes.cs
 * PURPOSE:     Config enums for the 3d Projection.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace Mathematics
{
    /// <summary>
    ///     How to convert 2d to 3d
    /// </summary>
    public enum Display
    {
        /// <summary>
        ///     The normal Projection
        /// </summary>
        Normal = 0,

        /// <summary>
        ///     The orthographic Projection
        /// </summary>
        Orthographic = 1
    }

    /// <summary>
    ///     Type of Camera
    /// </summary>
    public enum Cameras
    {
        /// <summary>
        ///     The orbit camera
        /// </summary>
        Orbit = 0,

        /// <summary>
        ///     The point at camera
        /// </summary>
        PointAt = 1
    }
}
