/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/IProjection.cs
 * PURPOSE:     Implementation of thhe 3D Projection Interface
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.Diagnostics;

namespace Mathematics
{
    /// <inheritdoc />
    /// <summary>
    ///     The Projection class.
    ///     Handle all 3D Operations in an isolated class.
    /// </summary>
    public sealed class Projection : IProjection
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Projection" /> is debug.
        /// </summary>
        /// <value>
        ///     <c>true</c> if debug; otherwise, <c>false</c>.
        /// </value>
        public bool Debug { get; set; } = true;

        /// <inheritdoc />
        /// <summary>
        ///     Generates the specified triangles.
        /// </summary>
        /// <param name="triangles">The triangles.</param>
        /// <param name="transform">The world transform.</param>
        /// <returns>
        ///     Converted 3d View
        /// </returns>
        public List<PolyTriangle> Generate(List<PolyTriangle> triangles, Transform transform)
        {
            var cache = ProjectionRaster.WorldMatrix(triangles, transform);

            if (Debug)
            {
                Trace.WriteLine(MathResources.Debug3DWorld);
                foreach (var triangle in cache)
                {
                    Trace.WriteLine(triangle.ToString());
                }
            }

            cache = transform.CameraType switch
            {
                Cameras.Orbit => ProjectionRaster.OrbitCamera(cache, transform),
                Cameras.PointAt => ProjectionRaster.PointAt(cache, transform),
                _ => ProjectionRaster.OrbitCamera(cache, transform)
            };

            if (Debug)
            {
                Trace.WriteLine(MathResources.Debug3DCamera);
                foreach (var triangle in cache)
                {
                    Trace.WriteLine(triangle.ToString());
                }
            }

            if (Debug)
            {
                Trace.WriteLine(MathResources.Debug3DClipping);
                foreach (var triangle in cache)
                {
                    Trace.WriteLine(triangle.ToString());
                }
            }

            cache = transform.DisplayType switch
            {
                Display.Normal => ProjectionRaster.Convert2DTo3D(cache),
                Display.Orthographic => ProjectionRaster.Convert2DTo3DOrthographic(cache),
                _ => ProjectionRaster.Convert2DTo3D(cache)
            };

            if (Debug)
            {
                Trace.WriteLine(MathResources.Debug3D);
                foreach (var triangle in cache)
                {
                    Trace.WriteLine(triangle.ToString());
                }
            }

            cache = ProjectionRaster.Clipping(cache);


            if (Debug)
            {
                Trace.WriteLine(MathResources.Debug3DTransformation);
                CreateDump(transform);
            }

            if (transform.DisplayType == Display.Orthographic)
            {
                return ProjectionRaster.MoveIntoViewOrthographic(cache, Projection3DRegister.Width,
                    Projection3DRegister.Height);
            }

            return ProjectionRaster.MoveIntoView(cache, Projection3DRegister.Width, Projection3DRegister.Height);
        }

        /// <summary>
        ///     Creates a debug dump.
        /// </summary>
        /// <param name="transform">The transform.</param>
        private static void CreateDump(Transform transform)
        {
            var matrix = Projection3DConstants.ProjectionTo3DMatrix();
            Trace.WriteLine(matrix.ToString());
            Trace.WriteLine(transform.ToString());
        }
    }
}
