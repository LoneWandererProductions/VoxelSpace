/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Voxels
 * FILE:        Voxels/Camera.cs
 * PURPOSE:     All possible options
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Drawing;

namespace SpeedTests
{
    public sealed class Camera
    {
        public Camera(int x, int y, double fov, int direction)
        {
            X = x;
            Y = y;
            Fov = fov;
            Angle = direction;
        }

        public Camera()
        {
        }

        /// <summary>
        /// Gets or sets the fov.
        /// </summary>
        /// <value>
        /// The fov.
        /// </value>
        public double Fov { get; set; } = 90;

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
        /// Gets the z.
        /// Z = − CellSize / 2: Bottom of the cell.
        /// Z = + CellSize / 2: Top of the cell.
        /// </summary>
        /// <value>
        /// The z.
        /// </value>
        public int Z { get; internal set; }

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
        ///     Gets or sets the pitch.
        ///     Camera pitch (vertical angle, radians, looking up-down)
        /// </summary>
        /// <value>
        ///     The pitch.
        /// </value>
        public int Pitch { get; set; }

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

        /// <summary>
        ///     Creates a deep copy of the current Camera instance.
        /// </summary>
        /// <returns>A new Camera instance with the same property values.</returns>
        internal Camera Clone()
        {
            return new Camera
            {
                BackgroundColor = BackgroundColor,
                X = X,
                Y = Y,
                Z = Z,
                Height = Height,
                Horizon = Horizon,
                ZFar = ZFar,
                Angle = Angle,
                Pitch = Pitch,
                Scale = Scale,
                Fov = Fov,
                ScreenHeight = ScreenHeight,
                ScreenWidth = ScreenWidth
            };
        }

        /// <summary>
        ///     Restores the current Camera instance with the values from another Camera.
        /// </summary>
        /// <param name="original">The Camera instance to copy values from.</param>
        internal void Restore(Camera original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            BackgroundColor = original.BackgroundColor;
            X = original.X;
            Y = original.Y;
            Z = original.Z;
            Horizon = original.Horizon;
            Angle = original.Angle;
            ScreenHeight = original.ScreenHeight;
            ScreenWidth = original.ScreenWidth;
            Pitch = Pitch;
            Fov = Fov;

            // Immutable properties are skipped in restoration:
            // this.Height = original.Height;
            // this.ZFar = original.ZFar;
            // this.Scale = original.Scale;
        }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Camera [X={X}, Y={Y}, Z={Z}, Angle={Angle}, Horizon={Horizon}, Height={Height}, ZFar={ZFar}], Pitch= {Pitch}";
        }
    }
}