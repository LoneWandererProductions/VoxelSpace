using System.Drawing;
using System.Windows.Input;

namespace Voxels
{
    public class RasterRaycast
    {
        public RVCamera Camera { get; set; }
        private readonly Raycaster _ray;

        public RasterRaycast(int[,] map, RVCamera camera, CameraContext context)
        {
            Camera = camera;

            _ray = new Raycaster(map, context);
        }

        public Bitmap Render(Key eKey)
        {
            Camera = InputHelper.SimulateCameraMovementRay(eKey, Camera);
            return _ray.Render(Camera);
        }

        public Bitmap Render()
        {
            return _ray.Render(Camera);
        }
    }
}