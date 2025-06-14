﻿using Imaging;
using Viewer;

namespace Rays
{
    public interface IFloorCeilingRenderer
    {
        void Render(DirectBitmap dbm, RvCamera camera, CameraContext context);
    }
}