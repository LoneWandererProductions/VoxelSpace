using System.Drawing;

namespace Voxels
{
    public sealed class Raycaster
    {
        private Camera _camera;
        private int[,] _map;
        private readonly RasterRaycast _raster;

        public Raycaster(Camera camera, int[,] map)
        {
            _camera = camera;
            _map = map;
            _raster= new RasterRaycast(_camera, _map);
        }

        public Bitmap Render()
        {
            return _raster.Render();
        }
    }
}