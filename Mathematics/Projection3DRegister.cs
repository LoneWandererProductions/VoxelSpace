/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/Projection3DRegister.cs
 * PURPOSE:     Basic Config for the 3D Camera
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System;

namespace Mathematics
{
    /// <summary>
    ///     3D Config, mostly the camera
    /// </summary>
    public static class Projection3DRegister
    {
        /// <summary>
        ///     Gets or sets the z near. Can't be 0
        /// </summary>
        /// <value>
        ///     The z near.
        /// </value>
        public static double ZNear { get; set; } = 0.1d;

        /// <summary>
        ///     Gets or sets the z, how far we can see
        /// </summary>
        /// <value>
        ///     The z far.
        /// </value>
        public static double ZFar { get; set; } = 1000.0d;

        /// <summary>
        ///     Gets or sets the angle for the view
        /// </summary>
        /// <value>
        ///     The allowed angle.
        /// </value>
        public static double Angle { get; set; } = 90.0d;

        /// <summary>
        ///     Gets or sets the height of the display area.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public static int Height { get; set; } = 480;

        /// <summary>
        ///     Gets or sets the width of the display area.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public static int Width { get; set; } = 640;

        /// <summary>
        ///     Gets the calculated Aspect Ration
        /// </summary>
        /// <value>
        ///     A, as Aspect Ratio
        /// </value>
        internal static double A => (double)Width / Height;

        /// <summary>
        ///     field of view, degree
        ///     var f = 1 / Math.Tan(angle / 2), base, here converted into rad
        /// </summary>
        /// <value>
        ///     The field of view in Radial value.
        /// </value>
        internal static double F => 1 / Math.Tan(Angle / 2 * Math.PI / 180);

        /// <summary>
        ///     Gets the distance distortion
        /// </summary>
        /// <value>
        ///     The distance distortion, based on zNear and zFar
        /// </value>
        internal static double Q => ZFar / (ZFar - ZNear);
    }
}