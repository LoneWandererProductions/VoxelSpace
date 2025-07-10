/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Viewer
 * FILE:        CameraContext.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace Viewer
{
    public sealed class CameraContext
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

        public int CellSize { get; init; }

        /// <summary>
        ///     Gets or sets the height of the screen.
        ///     Voxel is different here, it is more like a "Texture Size.
        /// </summary>
        /// <value>
        ///     The height of the screen.
        /// </value>
        public int ScreenHeight { get; init; }

        /// <summary>
        ///     Gets or sets the width of the screen.
        ///     Voxel is different here, it is more like a "Texture Size.
        /// </summary>
        /// <value>
        ///     The width of the screen.
        /// </value>
        public int ScreenWidth { get; init; }

        /// <summary>
        /// Gets the aspect ratio.
        /// </summary>
        /// <value>
        /// The aspect ratio.
        /// </value>
        public float AspectRatio => (float) ScreenWidth / ScreenHeight;

        /// <summary>
        ///     Gets or sets the height.
        ///     For Voxel only, is similar to CellSize, it handles the max height on Screen
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height { get; init; }

        /// <summary>
        ///     Gets or sets the distance.
        ///     Max cells that are visible. In this case 15
        /// </summary>
        /// <value>
        ///     The distance.
        /// </value>
        public int Distance { get; init; } = 15;

        /// <summary>
        ///     Gets or sets the fov.
        /// </summary>
        /// <value>
        ///     The fov.
        /// </value>
        public double Fov { get; init; } = 90;

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