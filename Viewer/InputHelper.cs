using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mathematics;
using System.Windows.Input;

namespace Viewer
{
    //TODO Performance Bottleneck is here
    public static class InputHelper
    {
        /// <summary>
        ///     Time elapsed since the last frame
        /// </summary>
        private static float _elapsedTime;

        public static float RotationSpeed { get; set; } = 30f;

        public static float MovementSpeed { get; set; } = 30f;

        /// <summary>
        ///     The last update time
        /// </summary>
        public static DateTime LastUpdateTime { get; set; }

        /// <summary>
        ///     Simulates the camera movement.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cam">The camera.</param>
        /// <returns>New Camera Values</returns>
        public static RvCamera SimulateCameraMovementVoxel(Key key, RvCamera cam)
        {
            UpdateDeltaTime(); // Update deltaTime based on frame time

            //the key
            Trace.WriteLine($"Key: {key}");

            // Log the old camera state
            Trace.WriteLine($"Before: {cam}");

            // Update the actual camera object directly
            float angle;
            switch (key)
            {
                case Key.W:
                    cam.X -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSinF(cam.Angle));
                    cam.Y -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCos(cam.Angle));
                    break;
                case Key.S:
                    cam.X += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSinF(cam.Angle));
                    cam.Y += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCosF(cam.Angle));
                    break;
                case Key.A:
                    angle = RotationSpeed * _elapsedTime;
                    cam.Angle += NormalizeAngle(angle); // Turn left
                    break;
                case Key.D:
                    angle = RotationSpeed * _elapsedTime;
                    cam.Angle -= NormalizeAngle(angle); // Turn right
                    break;
                case Key.O:
                    cam.Horizon += (int)(RotationSpeed * _elapsedTime); // Move up
                    break;
                case Key.P:
                    cam.Horizon -= (int)(RotationSpeed * _elapsedTime); // Move down
                    break;
                case Key.X:
                    cam.Pitch = Math.Max(cam.Pitch - (int)(RotationSpeed * _elapsedTime), -90); // Look down
                    break;
                case Key.Y:
                    cam.Pitch = Math.Min(cam.Pitch + (int)(RotationSpeed * _elapsedTime), 90); // Look up
                    break;
            }

            // Log the new camera state
            Trace.WriteLine($"After: {cam}");

            return cam;
        }

        /// <summary>
        ///     Simulates the camera movement.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cam">The camera.</param>
        /// <returns>New Camera Values</returns>
        public static RvCamera SimulateCameraMovementRay(Key key, RvCamera cam)
        {
            UpdateDeltaTime(); // Update deltaTime based on frame time

            //the key
            Trace.WriteLine($"Key: {key}");

            // Log the old camera state
            Trace.WriteLine($"Before: {cam}");

            // Update the actual camera object directly
            float angle;
            switch (key)
            {
                case Key.W:
                    // Move forward along the camera's angle direction
                    cam.X += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCosF(cam.Angle));
                    cam.Y += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSinF(cam.Angle));
                    break;
                case Key.S:
                    // Move backward opposite to the camera's angle direction
                    cam.X -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCosF(cam.Angle));
                    cam.Y -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSinF(cam.Angle));
                    break;
                case Key.A:
                    angle = RotationSpeed * _elapsedTime;
                    cam.Angle -= NormalizeAngle(angle); // Turn left
                    break;
                case Key.D:
                    angle = RotationSpeed * _elapsedTime;
                    cam.Angle += NormalizeAngle(angle); // Turn right
                    break;
                case Key.O:
                    cam.Horizon += (int)(RotationSpeed * _elapsedTime); // Move up
                    break;
                case Key.P:
                    cam.Horizon -= (int)(RotationSpeed * _elapsedTime); // Move down
                    break;
                case Key.X:
                    cam.Pitch = Math.Max(cam.Pitch - (int)(RotationSpeed * _elapsedTime), -90); // Look down
                    break;
                case Key.Y:
                    cam.Pitch = Math.Min(cam.Pitch + (int)(RotationSpeed * _elapsedTime), 90); // Look up
                    break;
            }

            // Log the new camera state
            Trace.WriteLine($"After: {cam}");

            return cam;
        }

        /// <summary>
        ///     Normalizes an angle to the range [0, 360).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int NormalizeAngle(float angle)
        {
            var normalizedAngle = (int)(angle % 360);
            return normalizedAngle < 0 ? normalizedAngle + 360 : normalizedAngle;
        }


        /// <summary>
        ///     Update method to calculate deltaTime
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateDeltaTime()
        {
            var currentTime = DateTime.Now;
            _elapsedTime = (float)(currentTime - LastUpdateTime).TotalSeconds;

            // If no time has elapsed, use a default small value to avoid zero movement on startup
            if (_elapsedTime == 0) _elapsedTime = 0.016f; // Assuming ~60 FPS, 1 frame = ~0.016 seconds

            // Optional: Cap delta time to avoid large jumps
            _elapsedTime = Math.Min(_elapsedTime, 0.1f); // 0.1s cap to prevent large frame gaps
            LastUpdateTime = currentTime;
        }
    }
}