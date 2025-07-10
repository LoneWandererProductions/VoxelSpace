/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        RasterRaycast.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Drawing;
using System.Windows.Input;
using Viewer;

namespace Rays
{
    public sealed class RasterRaycast
    {
        private readonly Raycaster _ray;

        public RasterRaycast(int[,] map, RvCamera camera, CameraContext context)
        {
            Camera = camera;

            _ray = new Raycaster(map, context);
        }

        public RvCamera Camera { get; set; }

        public Bitmap Render(Key eKey)
        {
            Camera = InputHelper.SimulateCameraMovementRay(eKey, Camera);
            return _ray.Render(Camera).Bitmap;
        }

        public RenderResult Render()
        {
            return _ray.Render(Camera);
        }
    }
}