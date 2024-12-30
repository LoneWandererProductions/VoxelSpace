namespace Voxels
{
    public class CameraContext
    {
        public CameraContext()
        {
        }

        public CameraContext(int cellSize, int screenHeight, int screenWidth)
        {
            CellSize = cellSize;
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;
        }

        public int CellSize { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenWidth { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// For Voxel only, is similar to CellSize, it handles the max height on Screen
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the distance.
        /// Max cells that are visible. In this case 15
        /// </summary>
        /// <value>
        /// The distance.
        /// </value>
        public int Distance { get; set; } = 15;
    }
}