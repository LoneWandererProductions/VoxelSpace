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

        /// <summary>
        /// Gets or sets the height of the screen.
        /// Voxel is different here, it is more like a "Texture Size.
        /// </summary>
        /// <value>
        /// The height of the screen.
        /// </value>
        public int ScreenHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the screen.
        /// Voxel is different here, it is more like a "Texture Size.
        /// </summary>
        /// <value>
        /// The width of the screen.
        /// </value>
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

        /// <summary>
        /// Gets or sets the fov.
        /// </summary>
        /// <value>
        /// The fov.
        /// </value>
        public double Fov { get; set; } = 90;

        /// <summary>
        ///     Gets the scale.
        ///     Voxel only
        /// </summary>
        /// <value>
        ///     The scale.
        /// </value>
        public int Scale { get; init; } = 120;
    }
}