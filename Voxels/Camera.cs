/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Voxels
 * FILE:        Voxels/Camera.cs
 * PURPOSE:     All possible options
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Drawing;

namespace Voxels
{
    public sealed class Camera
    {
        /// <summary>
        ///     Gets or sets the color of the background.
        /// </summary>
        /// <value>
        ///     The color of the background.
        /// </value>
        public Color BackgroundColor { get; set; } = Color.Cyan;

        /// <summary>
        ///     Gets or sets the x.
        ///     x position on the map
        /// </summary>
        /// <value>
        ///     The x.
        /// </value>
        public int X { get; set; }

        /// <summary>
        ///     Gets or sets the y.
        ///     y position on the map
        /// </summary>
        /// <value>
        ///     The y.
        /// </value>
        public int Y { get; set; }

        /// <summary>
        ///     Gets or sets the height.
        ///     height of the camera
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height { get; init; }

        /// <summary>
        ///     Gets or sets the horizon.
        ///     offset of the horizon position (looking up-down)
        /// </summary>
        /// <value>
        ///     The horizon.
        /// </value>
        public int Horizon { get; set; }

        /// <summary>
        ///     Gets or sets the z far.
        ///     distance of the camera looking forward
        /// </summary>
        /// <value>
        ///     The z far.
        /// </value>
        public float ZFar { get; init; }

        /// <summary>
        ///     Gets or sets the angle.
        ///     camera angle (radians, clockwise)
        /// </summary>
        /// <value>
        ///     The angle.
        /// </value>
        public int Angle { get; set; }

        /// <summary>
        ///     Gets the scale.
        /// </summary>
        /// <value>
        ///     The scale.
        /// </value>
        public int Scale { get; init; }

        /// <summary>
        ///     Gets or sets the height of the screen.
        ///     The higher the better the resolution, at the cost of speed
        /// </summary>
        /// <value>
        ///     The height of the screen.
        /// </value>
        public int ScreenHeight { get; set; } = 200;

        /// <summary>
        ///     Gets or sets the width of the screen.
        ///     The higher the better the resolution, at the cost of speed
        /// </summary>
        /// <value>
        ///     The width of the screen.
        /// </value>
        public int ScreenWidth { get; set; } = 300;
    }
}