/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/Projection3DCamera.cs
 * PURPOSE:     Does the heavy liftign for the 3D Display and joins all the Matrices
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://learn.microsoft.com/en-us/windows/win32/direct3d9/transforms
 *              https://www.brainvoyager.com/bv/doc/UsersGuide/CoordsAndTransforms/SpatialTransformationMatrices.html
 *              https://github.com/OneLoneCoder/Javidx9/blob/master/ConsoleGameEngine/BiggerProjects/Engine3D/OneLoneCoder_olcEngine3D_Part3.cpp
 *              https://www.3dgep.com/understanding-the-view-matrix/#Arcball_Orbit_Camera
 */

using System;

namespace Mathematics
{
    /// <summary>
    ///     3D Projection
    /// </summary>
    public static class Projection3DCamera
    {
        /// <summary>
        ///     Converts to 3d.
        ///     We need norm vectors for the camera if we use a plane
        ///     than we need a translation matrix to increase the values and perhaps position of the point, might as well use the
        ///     directX components in the future
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns>Transformed Coordinates</returns>
        public static Vector3D ProjectionTo3D(Vector3D start)
        {
            double[,] matrix = { { start.X, start.Y, start.Z, 1 } };

            var m1 = new BaseMatrix(matrix);
            var projection = Projection3DConstants.ProjectionTo3DMatrix();

            var result = m1 * projection;
            var x = result[0, 0];
            var y = result[0, 1];
            var z = result[0, 2];
            var w = result[0, 3];

            var check = Math.Abs(Math.Round(w, 2));

            if (check == 0.0d)
            {
                return new Vector3D(x, y, z);
            }

            x /= w;
            y /= w;
            z /= w;
            return new Vector3D(x, y, z);
        }

        /// <summary>
        ///     Orthographic projection to 3d.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns>Transformed Coordinates</returns>
        public static Vector3D OrthographicProjectionTo3D(Vector3D start)
        {
            double[,] matrix = { { start.X, start.Y, start.Z, 1 } };

            var m1 = new BaseMatrix(matrix);
            var projection = OrthographicProjectionTo3DMatrix();

            var result = m1 * projection;
            var x = result[0, 0];
            var y = result[0, 1];
            var z = result[0, 2];

            return new Vector3D(x, y, z);
        }

        /// <returns>
        ///     World Transformation
        ///     ModelViewProjection mvp = Projection * View * Model
        ///     Use LEFT-Handed rotation matrices (as seen in DirectX)
        ///     https://docs.microsoft.com/en-us/windows/win32/direct3d9/transforms#rotate
        ///     https://www.opengl-tutorial.org/beginners-tutorials/tutorial-3-matrices/#cumulating-transformations
        /// </returns>
        /// <summary>
        ///     Gets the model matrix.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <returns>The Model Matrix</returns>
        public static BaseMatrix ModelMatrix(Transform transform)
        {
            var rotationX = Projection3DConstants.RotateX(transform.Rotation.X);
            var rotationY = Projection3DConstants.RotateY(transform.Rotation.Y);
            var rotationZ = Projection3DConstants.RotateZ(transform.Rotation.Z);

            // XYZ rotation = (((Z × Y) × X) × Vector3D) or (Z×Y×X)×V
            var rotation = rotationZ * rotationY * rotationX;

            var translation = Projection3DConstants.Translate(transform.Translation);

            var scaling = Projection3DConstants.Scale(transform.Scale);

            // Model Matrix = T × R × S (right to left order)
            return scaling * rotation * translation;
        }

        /// <summary>
        ///     Orthographic projection to 3d matrix.
        /// </summary>
        /// <returns>Projection Matrix</returns>
        private static BaseMatrix OrthographicProjectionTo3DMatrix()
        {
            double[,] translation =
            {
                { Projection3DRegister.A, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 1 }
            };
            return new BaseMatrix(translation);
        }

        /// <summary>
        ///     Generates the view from the Camera onto the world.
        /// </summary>
        /// <param name="transform">The transform object.</param>
        /// <returns>
        ///     transform
        ///     View on the Object from the Camera perspective
        /// </returns>
        internal static BaseMatrix PointAt(Transform transform)
        {
            var matCameraRot = Projection3DConstants.RotateY(transform.Yaw);

            var vLookDir = transform.Target * matCameraRot;
            var vTarget = transform.Position + vLookDir;

            return Projection3DConstants.LookAt(transform, vTarget);
        }

        /// <summary>
        ///     The Orbit camera.
        ///     M = T r t -> Inverse(M) = V, return value is V
        ///     t,  t translation, moves the camera away from the object
        ///     r, rotation quaternion, rotates the camera around the object
        ///     T, to move the pivot point of the camera to center on the object, since the Object is always the center
        ///     t will be identy matrix
        ///     https://www.3dgep.com/understanding-the-view-matrix/#Arcball_Orbit_Camera
        /// </summary>
        /// <param name="transform">The transform object.</param>
        /// <returns>
        ///     transform
        ///     View on the Object from the Camera perspective
        /// </returns>
        internal static BaseMatrix OrbitCamera(Transform transform)
        {
            // LEFT-Handed Coordinate System
            // Rotation in X = Positive when 'looking down'
            // Rotation in Y = Positive when 'looking right'
            // Rotation in Z = Positive when 'tilting left'

            //r Matrix
            var cosPitch = Math.Cos(transform.Pitch * Projection3DConstants.Rad);
            var sinPitch = Math.Sin(transform.Pitch * Projection3DConstants.Rad);

            var cosYaw = Math.Cos(transform.Yaw * Projection3DConstants.Rad);
            var sinYaw = Math.Sin(transform.Yaw * Projection3DConstants.Rad);

            //converted r matrix
            transform.Right = new Vector3D(cosYaw, 0, -sinYaw);

            transform.Up = new Vector3D(sinYaw * sinPitch, cosPitch, cosYaw * sinPitch);

            transform.Forward = new Vector3D(sinYaw * cosPitch, -sinPitch, cosPitch * cosYaw);

            // The inverse camera's translation
            var transl = new Vector3D(-(transform.Right * transform.Position),
                -(transform.Up * transform.Position),
                -(transform.Forward * transform.Position));

            //{ 1 0 0 0  0 1 - 0 0 - 0 0 1 0 - 0 - 0 - 0 1  }

            // Join rotation and translation in a single matrix
            // instead of calculating their multiplication
            double[,] viewMatrix =
            {
                { transform.Right.X, transform.Up.X, transform.Forward.X, 0 },
                { transform.Right.Y, transform.Up.Y, transform.Forward.Y, 0 },
                { transform.Right.Z, transform.Up.Z, transform.Forward.Z, 0 }, { transl.X, transl.Y, transl.Z, 1 }
            };

            return new BaseMatrix { Matrix = viewMatrix };
        }
    }
}
