/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     Rays
* FILE:        RasterRaycastV3.cs
* PURPOSE:     Wrapper for RaycasterV3, handles camera state and delegates rendering.
* PROGRAMMER:  Peter Geinitz (Wayfarer)
*/

using System;
using System.Drawing;
using System.Windows.Input;
using Imaging;
using Viewer;

namespace Rays
{
    /// <summary>
    /// Wrapper for the <see cref="RaycasterV3"/> class. 
    /// Maintains the camera state, applies movement from input, and delegates rendering.
    /// </summary>
    public sealed class RasterRaycastV3
    {
        private readonly RaycasterV3 _ray;

        /// <summary>
        /// Initializes a new instance of the <see cref="RasterRaycastV3"/> class.
        /// </summary>
        /// <param name="map">The cell-based map data.</param>
        /// <param name="camera">The camera object (position, direction, etc.).</param>
        /// <param name="context">Rendering context (resolution, viewport size, etc.).</param>
        /// <param name="wallTextures">Wall textures used by the raycaster.</param>
        /// <exception cref="ArgumentNullException">Thrown if a required argument is null.</exception>
        public RasterRaycastV3(MapCell[,] map, RvCamera camera, CameraContext context, DirectBitmap[] wallTextures)
        {
            Camera = camera ?? throw new ArgumentNullException(nameof(camera));

            _ray = new RaycasterV3(map, context, wallTextures);
        }

        /// <summary>
        /// Gets or sets the current camera.
        /// </summary>
        public RvCamera Camera { get; set; }

        /// <summary>
        /// Render a frame, applying camera movement from input.
        /// </summary>
        /// <param name="eKey">The key input that may affect camera movement.</param>
        /// <returns>The rendered frame as a <see cref="Bitmap"/>.</returns>
        public Bitmap Render(Key eKey)
        {
            Camera = InputHelper.SimulateCameraMovementRay(eKey, Camera);
            return _ray.Render(Camera).Bitmap;
        }

        /// <summary>
        /// Render a frame with the current camera, without any new input.
        /// </summary>
        /// <returns>A <see cref="RenderResult"/> containing both the final bitmap and metadata.</returns>
        public RenderResult Render()
        {
            return _ray.Render(Camera);
        }
    }
}
