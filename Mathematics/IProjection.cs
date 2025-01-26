/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/IProjection.cs
 * PURPOSE:     3D Projection Interface
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBeInternal
// ReSharper disable once UnusedMemberInSuper.Global
// ReSharper disable UnusedMemberInSuper.Global

using System.Collections.Generic;

namespace Mathematics
{
    /// <summary>
    ///     The Projection Interface.
    ///     Template for all external 3D operations.
    /// </summary>
    public interface IProjection
    {
        /// <summary>
        ///     Generates the specified triangles.
        /// </summary>
        /// <param name="triangles">The triangles.</param>
        /// <param name="transform">The world transform.</param>
        /// <returns>
        ///     Converted 3d View
        /// </returns>
        List<PolyTriangle> Generate(List<PolyTriangle> triangles, Transform transform);
    }
}
