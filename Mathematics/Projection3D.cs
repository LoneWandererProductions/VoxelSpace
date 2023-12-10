/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/Projection3D.cs
 * PURPOSE:     3D Matrix transformations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://learn.microsoft.com/en-us/windows/win32/direct3d9/transforms
 *              https://www.brainvoyager.com/bv/doc/UsersGuide/CoordsAndTransforms/SpatialTransformationMatrices.html
 */

using System;

namespace Mathematics
{
    /// <summary>
    ///     Helper Class that handles some 3D Transformations
    /// </summary>
    public static class Projection3D
    {
        private const double Rad = Math.PI / 180.0;

        /// <summary>
        ///     Rotates the x axis.
        ///     Source: https://learn.microsoft.com/en-us/windows/win32/direct3d9/transforms
        ///     https://en.wikipedia.org/wiki/Rotation_matrix
        ///     Use DirectX
        /// </summary>
        /// <param name="vector">The start vector.</param>
        /// <param name="angleD">The Angle degree.</param>
        /// <returns>Rotated Vector</returns>
        public static BaseMatrix RotateX(Vector3D vector, int angleD)
        {
            //convert to Rad
            var angle = angleD * Rad;

            double[,] matrix = {{vector.X, vector.Y, vector.Z, 1}};

            var m1 = new BaseMatrix {Matrix = matrix};

            double[,] rotation =
            {
                {1, 0, 0, 0}, {0, Math.Cos(angle), Math.Sin(angle), 0},
                {0, -Math.Sin(angle), Math.Cos(angle), 0}, {0, 0, 0, 1}
            };

            var m2 = new BaseMatrix {Matrix = rotation};

            return m1 * m2;
        }

        /// <summary>
        ///     Rotates the y axis.
        ///     Source: https://learn.microsoft.com/en-us/windows/win32/direct3d9/transforms
        ///     https://en.wikipedia.org/wiki/Rotation_matrix
        ///     Use DirectX
        /// </summary>
        /// <param name="vector">The start vector.</param>
        /// <param name="angleD">The Angle degree.</param>
        /// <returns>Rotated Vector</returns>
        public static BaseMatrix RotateY(Vector3D vector, int angleD)
        {
            //convert to Rad
            var angle = angleD * Rad;

            double[,] matrix = {{vector.X, vector.Y, vector.Z, 1}};

            var m1 = new BaseMatrix {Matrix = matrix};

            double[,] rotation =
            {
                {Math.Cos(angle), 0, -Math.Sin(angle), 0}, {0, 1, 0, 0},
                {Math.Sin(angle), 0, Math.Cos(angle), 0}, {0, 0, 0, 1}
            };

            var m2 = new BaseMatrix {Matrix = rotation};

            return m1 * m2;
        }

        /// <summary>
        ///     Rotates the z axis.
        ///     Source: https://learn.microsoft.com/en-us/windows/win32/direct3d9/transforms
        ///     https://en.wikipedia.org/wiki/Rotation_matrix
        ///     https://en.wikipedia.org/wiki/Rotation_formalisms_in_three_dimensions
        ///     Use DirectX
        /// </summary>
        /// <param name="vector">The start vector.</param>
        /// <param name="angleD">The Angle degree.</param>
        /// <returns>Rotated Vector</returns>
        public static BaseMatrix RotateZ(Vector3D vector, int angleD)
        {
            //convert to Rad
            var angle = angleD * Rad;

            double[,] matrix = {{vector.X, vector.Y, vector.Z, 1}};

            var m1 = new BaseMatrix {Matrix = matrix};

            double[,] rotation =
            {
                {Math.Cos(angle), Math.Sin(angle), 0, 0}, {-Math.Sin(angle), Math.Cos(angle), 0, 0},
                {0, 0, 1, 0}, {0, 0, 0, 1}
            };

            var m2 = new BaseMatrix {Matrix = rotation};

            return m1 * m2;
        }

        /// <summary>
        ///     Scales the specified start.
        ///     Source: https://learn.microsoft.com/en-us/windows/win32/direct3d9/transforms
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="value">The value.</param>
        /// <returns>Scale Matrix</returns>
        public static BaseMatrix Scale(Vector3D start, int value)
        {
            double[,] matrix = {{start.X, start.Y, start.Z, 1}};
            var m1 = new BaseMatrix(matrix);

            double[,] scale = {{value, 0, 0, 0}, {0, value, 0, 0}, {0, 0, value, 0}, {0, 0, 0, 1}};

            var m2 = new BaseMatrix {Matrix = scale};

            return m1 * m2;
        }

        /// <summary>
        ///     Scales the specified start.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <param name="three">The three.</param>
        /// <returns>Translation Matrix</returns>
        public static BaseMatrix Scale(Vector3D start, double one, double two, double three)
        {
            double[,] matrix = {{start.X, start.Y, start.Z, 1}};
            var m1 = new BaseMatrix(matrix);

            double[,] scale = {{one, 0, 0, 0}, {0, two, 0, 0}, {0, 0, three, 0}, {0, 0, 0, 1}};
            var m2 = new BaseMatrix {Matrix = scale};

            return m1 * m2;
        }

        /// <summary>
        ///     Translates the specified start.
        ///     Source: https://learn.microsoft.com/en-us/windows/win32/direct3d9/transforms
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Transplanted Vector</returns>
        public static BaseMatrix Translate(Vector3D start, Vector3D end)
        {
            double[,] matrix = {{end.X, end.Y, end.Z, 1}};
            var m1 = new BaseMatrix {Matrix = matrix};

            double[,] translate = {{1, 0, 0, 0}, {0, 1, 0, 0}, {0, 0, 1, 0}, {start.X, start.Y, start.Z, 1}};

            var m2 = new BaseMatrix {Matrix = translate};

            return m1 * m2;
        }

        /// <summary>
        ///     Gets the vector.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>Vector from Matrix</returns>
        public static Vector3D GetVector(BaseMatrix matrix)
        {
            //TODO higly untested!
            return new(matrix[0, 0], matrix[0, 1], matrix[0, 2]);
        }
    }
}