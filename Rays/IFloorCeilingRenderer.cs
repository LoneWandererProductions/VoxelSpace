/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        IFloorCeilingRenderer.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using Imaging;
using Viewer;

namespace Rays
{
    public interface IFloorCeilingRenderer
    {
        void Render(DirectBitmap dbm, RvCamera camera, CameraContext context);
    }
}