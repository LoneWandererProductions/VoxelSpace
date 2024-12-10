/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/Projection3DConstants.cs
 * PURPOSE:     Holds the basic 3D Matrices
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://learn.microsoft.com/en-us/windows/win32/direct3d9/transforms
 *              https://www.brainvoyager.com/bv/doc/UsersGuide/CoordsAndTransforms/SpatialTransformationMatrices.html
 *              https://github.com/OneLoneCoder/Javidx9/blob/master/ConsoleGameEngine/BiggerProjects/Engine3D/OneLoneCoder_olcEngine3D_Part3.cpp
 */

using System;

namespace Mathematics
{
    /// <summary>
    ///     Some Matrices that can be used outside of 3D Projection
    /// </summary>
    internal static class Projection3DConstants
    {
        /// <summary>
        ///     Convert Degree to radial
        /// </summary>
        internal const double Rad = Math.PI / 180.0d;

        /// <summary>
        ///     Projections the to 3d matrix.
        /// </summary>
        /// <returns>Projection Matrix</returns>
        internal static BaseMatrix ProjectionTo3DMatrix()
        {
            double[,] translation =
            {
                { Projection3DRegister.A * Projection3DRegister.F, 0, 0, 0 }, { 0, Projection3DRegister.F, 0, 0 },
                { 0, 0, Projection3DRegister.Q, 1 },
                { 0, 0, -Projection3DRegister.ZNear * Projection3DRegister.Q, 0 }
            };

            //now lacks /w, has to be done at the end!
            return new BaseMatrix(translation);
        }

        /// <summary>
        ///     Converts Coordinates based on the Camera.
        ///     https://ksimek.github.io/2012/08/22/extrinsic/
        ///     https://www.youtube.com/watch?v=HXSuNxpCzdM
        ///     https://stackoverflow.com/questions/74233166/custom-lookat-and-whats-the-math-behind-it
        ///     https://medium.com/@carmencincotti/lets-look-at-magic-lookat-matrices-c77e53ebdf78
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="target">Target Vector.</param>
        /// <returns>
        ///     matrix for Transforming the Coordinate
        /// </returns>
        internal static BaseMatrix LookAt(Transform transform, Vector3D target)
        {
            var forward = (target - transform.Position).Normalize(); // Z axis

            var right = transform.Up.CrossProduct(forward).Normalize(); // X axis

            var up = forward.CrossProduct(right); // Y axis

            // The inverse camera's translation
            var transl = new Vector3D(-(right * transform.Position),
                -(up * transform.Position),
                -(forward * transform.Position));

            double[,] viewMatrix =
            {
                { right.X, up.X, forward.X, 0 }, { right.Y, up.Y, forward.Y, 0 }, { right.Z, up.Z, forward.Z, 0 },
                { transl.X, transl.Y, transl.Z, 1 }
            };

            return new BaseMatrix { Matrix = viewMatrix };
        }

        /// <summary>
        ///     Rotates x.
        /// </summary>
        /// <param name="angleD">The angle d.</param>
        /// <returns>Rotation Matrix X</returns>
        public static BaseMatrix RotateX(double angleD)
        {
            //convert to Rad
            var angle = angleD * Rad;

            double[,] rotation =
            {
                { 1, 0, 0, 0 }, { 0, Math.Cos(angle), Math.Sin(angle), 0 },
                { 0, -Math.Sin(angle), Math.Cos(angle), 0 }, { 0, 0, 0, 1 }
            };

            return new BaseMatrix { Matrix = rotation };
        }

        /// <summary>
        ///     Rotates y.
        /// </summary>
        /// <param name="angleD">The angle d.</param>
        /// <returns>Rotation Matrix Y</returns>
        public static BaseMatrix RotateY(double angleD)
        {
            //convert to Rad
            var angle = angleD * Rad;

            double[,] rotation =
            {
                { Math.Cos(angle), 0, -Math.Sin(angle), 0 }, { 0, 1, 0, 0 },
                { Math.Sin(angle), 0, Math.Cos(angle), 0 }, { 0, 0, 0, 1 }
            };

            return new BaseMatrix { Matrix = rotation };
        }

        /// <summary>
        ///     Rotates z.
        /// </summary>
        /// <param name="angleD">The angle d.</param>
        /// <returns>Rotation Matrix Z</returns>
        public static BaseMatrix RotateZ(double angleD)
        {
            //convert to Rad
            var angle = angleD * Rad;

            double[,] rotation =
            {
                { Math.Cos(angle), Math.Sin(angle), 0, 0 }, { -Math.Sin(angle), Math.Cos(angle), 0, 0 },
                { 0, 0, 1, 0 }, { 0, 0, 0, 1 }
            };

            return new BaseMatrix { Matrix = rotation };
        }

        /// <summary>
        ///     Scale Matrix.
        /// </summary>
        /// <param name="value">The scale value.</param>
        /// <returns>Scale Matrix.</returns>
        public static BaseMatrix Scale(int value)
        {
            double[,] scale = { { value, 0, 0, 0 }, { 0, value, 0, 0 }, { 0, 0, value, 0 }, { 0, 0, 0, 1 } };

            return new BaseMatrix { Matrix = scale };
        }

        /// <summary>
        ///     Scales the specified vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Scale Matrix.</returns>
        public static BaseMatrix Scale(Vector3D vector)
        {
            double[,] scale = { { vector.X, 0, 0, 0 }, { 0, vector.Y, 0, 0 }, { 0, 0, vector.Z, 0 }, { 0, 0, 0, 1 } };

            return new BaseMatrix { Matrix = scale };
        }

        /// <summary>
        ///     Scale Matrix.
        /// </summary>
        /// <param name="one">The x value.</param>
        /// <param name="two">The y value.</param>
        /// <param name="three">The z value.</param>
        /// <returns>Scale Matrix.</returns>
        public static BaseMatrix Scale(double one, double two, double three)
        {
            double[,] scale = { { one, 0, 0, 0 }, { 0, two, 0, 0 }, { 0, 0, three, 0 }, { 0, 0, 0, 1 } };

            return new BaseMatrix { Matrix = scale };
        }

        /// <summary>
        ///     Translates the specified vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Translation Matrix</returns>
        public static BaseMatrix Translate(Vector3D vector)
        {
            double[,] translate =
            {
                { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { vector.X, vector.Y, vector.Z, 1 }
            };

            return new BaseMatrix { Matrix = translate };
        }
    }
}