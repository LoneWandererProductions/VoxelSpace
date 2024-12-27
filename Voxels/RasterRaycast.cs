using System.Drawing;
using System.Windows.Input;

namespace Voxels
{
    public class RasterRaycast
    {
        public Camera Camera { get; set; }
        private readonly Raycaster _ray;

        public RasterRaycast(int[,] map, Camera camera, CameraContext context)
        {
            Camera = camera;

            _ray = new Raycaster(map, context);
        }

        public Bitmap Render(Key eKey)
        {
            Camera = Helper.SimulateCameraMovementRay(eKey, Camera);
            return _ray.Render(Camera);
        }

        public Bitmap Render()
        {
            return _ray.Render(Camera);
        }
    }
}