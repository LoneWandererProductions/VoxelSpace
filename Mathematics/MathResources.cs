/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/MathResources.cs
 * PURPOSE:     Some basic string Resources
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace Mathematics
{
    /// <summary>
    ///     Some basic strings
    /// </summary>
    internal static class MathResources
    {
        /// <summary>
        ///     The Tolerance (const). Value: 0.000001.
        /// </summary>
        internal const double Tolerance = 1e-6;

        /// <summary>
        ///     The matrix error inverse (const). Value: "Unable to compute inverse.".
        /// </summary>
        internal const string MatrixErrorInverse = "Unable to compute inverse.";

        /// <summary>
        ///     The matrix error inverse not cubic (const). Value: "Unable to compute inverse Matrices that are not cubic.".
        /// </summary>
        internal const string MatrixErrorInverseNotCubic = "Unable to compute inverse Matrices that are not cubic.";

        /// <summary>
        ///     The matrix error Determinant (const). Value: "Unable to compute Matrix Determinant.".
        /// </summary>
        internal const string MatrixErrorDeterminant = "Unable to compute Matrix Determinant.";

        /// <summary>
        ///     The matrix error Doolittle (const). Value: "Cannot use Doolittle's method.".
        /// </summary>
        internal const string MatrixErrorDoolittle = "Cannot use Doolittle's method.";

        /// <summary>
        ///     The matrix error Doolittle (const). Value: "Number of Columns of first are not equal to the number of rows in the
        ///     second Matrix.".
        /// </summary>
        internal const string MatrixErrorColumns =
            "Number of Columns of first are not equal to the number of rows in the second Matrix.";

        /// <summary>
        ///     The string X (const). Value: "X: ".
        /// </summary>
        internal const string StrX = "X: ";

        /// <summary>
        ///     The string Y (const). Value: " Y: ".
        /// </summary>
        internal const string StrY = " Y: ";

        /// <summary>
        ///     The string Z (const). Value: " Z: ".
        /// </summary>
        internal const string StrZ = " Z: ";

        /// <summary>
        ///     The string Separator (const). Value:" : ";.
        /// </summary>
        internal const string Separator = " : ";

        /// <summary>
        ///     The string Id (const). Value: " Id: ".
        /// </summary>
        internal const string StrId = " Id: ";

        /// <summary>
        ///     The Camera Vector (const). Value: "Camera Position: ".
        /// </summary>
        internal const string CameraPosition = "Camera Position: ";

        /// <summary>
        ///     The Target Vector (const). Value: "Target Position: ".
        /// </summary>
        internal const string Target = "Target Position: ";

        /// <summary>
        ///     The Up Vector (const). Value: "Up Position: ".
        /// </summary>
        internal const string Up = "Up Position: ";

        /// <summary>
        ///     The Right Vector (const). Value: "Right Position: ".
        /// </summary>
        internal const string Right = "Right Position: ";

        /// <summary>
        ///     The Forward Vector (const). Value: "Forward Position: ".
        /// </summary>
        internal const string Forward = "Forward Position: ";

        /// <summary>
        ///     The Pitch (const). Value: "Pitch: ".
        /// </summary>
        internal const string Pitch = "Pitch: ";

        /// <summary>
        ///     The Yaw (const). Value: "Yaw: ".
        /// </summary>
        internal const string Yaw = "Yaw: ";

        /// <summary>
        ///     The World (const). Value: "World Transformations.".
        /// </summary>
        internal const string World = "World Transformations.";

        /// <summary>
        ///     The Translation Vector (const). Value: "Translation Vector: ".
        /// </summary>
        internal const string Translation = "Translation Vector: ";

        /// <summary>
        ///     The Rotation Vector (const). Value: "Rotation Vector: ".
        /// </summary>
        internal const string Rotation = "Rotation Vector: ";

        /// <summary>
        ///     The Scale Vector (const). Value: "Scale Vector: ".
        /// </summary>
        internal const string Scale = "Scale Vector: ";

        /// <summary>
        ///     The Camera Type (const). Value: ""Camera Type: ".
        /// </summary>
        internal const string CameraType = "Camera Type: ";

        /// <summary>
        ///     The Display Type(const). Value: "Display Type: ".
        /// </summary>
        internal const string DisplayType = "Display Type: ";

        /// <summary>
        ///     The Debug 3D World Transformation Message(const). Value: "World Transformation".
        /// </summary>
        internal const string Debug3DWorld = "World Transformation";

        /// <summary>
        ///     The Debug 3D Camera Message(const). Value: "Camera Transformation".
        /// </summary>
        internal const string Debug3DCamera = "Camera Transformation";

        /// <summary>
        ///     The Debug 3D Clipping Message(const). Value: "Display Type: ".
        /// </summary>
        internal const string Debug3DClipping = "Clipping Transformation";

        /// <summary>
        ///     The Debug 3D Transformation Message(const). Value: "3D Transformation".
        /// </summary>
        internal const string Debug3D = "3D Transformation";

        /// <summary>
        ///     The Debug 3D Transformation Setting Message(const). Value: "Transformation Settings".
        /// </summary>
        internal const string Debug3DTransformation = "Transformation Settings";
    }
}
