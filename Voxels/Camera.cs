namespace VoxelEngine
{
    public class Camera
    {
        /// <summary>
        ///     Gets or sets the x.
        ///     x position on the map
        /// </summary>
        /// <value>
        ///     The x.
        /// </value>
        public float X { get; set; }

        /// <summary>
        ///     Gets or sets the y.
        ///     y position on the map
        /// </summary>
        /// <value>
        ///     The y.
        /// </value>
        public float Y { get; set; }

        /// <summary>
        ///     Gets or sets the height.
        ///     height of the camera
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public float Height { get; set; }

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
        public float ZFar { get; set; }

        /// <summary>
        ///     Gets or sets the angle.
        ///     camera angle (radians, clockwise)
        /// </summary>
        /// <value>
        ///     The angle.
        /// </value>
        public int Angle { get; set; } = 90; // 1.5 * 3.141592; // (= 270 deg)

        public int Scale { get; set; }
    }
}