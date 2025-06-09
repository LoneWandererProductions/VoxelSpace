using System.Drawing;
using System.Windows.Input;
using Imaging;
using Viewer;
using Voxels;

namespace Rays
{
    public sealed class RasterRaycastV2
    {
        private readonly RaycasterV2 _ray;

        public RasterRaycastV2(MapCell[,] map, RvCamera camera, CameraContext context, DirectBitmap[] wallTextures)
        {
            Camera = camera;

            _ray = new RaycasterV2(map, context, wallTextures);
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