namespace Voxels
{
    public class CameraContext
    {
        public CameraContext(int cellSize, int screenHeight, int screenWidth)
        {
            CellSize = cellSize;
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;
        }

        public int CellSize { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenWidth { get; set; }
    }
}