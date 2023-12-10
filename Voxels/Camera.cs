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
        public double X { get; set; } = 512.0;

        /// <summary>
        ///     Gets or sets the y.
        ///     y position on the map
        /// </summary>
        /// <value>
        ///     The y.
        /// </value>
        public double Y { get; set; } = 512.0;

        /// <summary>
        ///     Gets or sets the height.
        ///     height of the camera
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public double Height { get; set; } = 70.0;

        /// <summary>
        ///     Gets or sets the horizon.
        ///     offset of the horizon position (looking up-down)
        /// </summary>
        /// <value>
        ///     The horizon.
        /// </value>
        public double Horizon { get; set; } = 60.0;

        /// <summary>
        ///     Gets or sets the z far.
        ///     distance of the camera looking forward
        /// </summary>
        /// <value>
        ///     The z far.
        /// </value>
        public double ZFar { get; set; } = 600.0;

        /// <summary>
        ///     Gets or sets the angle.
        ///     camera angle (radians, clockwise)
        /// </summary>
        /// <value>
        ///     The angle.
        /// </value>
        public double Angle { get; set; } = 90; // 1.5 * 3.141592; // (= 270 deg)
    }
}