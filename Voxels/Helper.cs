using System;
using System.Diagnostics;
using System.Windows.Input;
using Mathematics;

namespace Voxels
{
    public static class Helper
    {
        public static float RotationSpeed { get; set; } = 10f;

        public static float MovementSpeed { get; set; } = 10f;

        /// <summary>
        ///     The last update time
        /// </summary>
        public static DateTime LastUpdateTime { get; set; }

        /// <summary>
        ///     Time elapsed since the last frame
        /// </summary>
        private static float _elapsedTime;

        /// <summary>
        /// Simulates the camera movement.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cam">The camera.</param>
        /// <returns>New Camera Values</returns>
        public static Camera SimulateCameraMovement(Key key, Camera cam)
        {
            UpdateDeltaTime(); // Update deltaTime based on frame time

            //the key
            Trace.WriteLine($"Key: {key}");

            // Log the old camera state
            Trace.WriteLine($"Before: {cam}");

            // Update the actual camera object directly
            switch (key)
            {
                case Key.W:
                    cam.X -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSin(cam.Angle));
                    cam.Y -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCos(cam.Angle));
                    break;
                case Key.S:
                    cam.X += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSin(cam.Angle));
                    cam.Y += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCos(cam.Angle));
                    break;
                case Key.A:
                    cam.Angle += (int)(RotationSpeed * _elapsedTime); // Turn left
                    break;
                case Key.D:
                    cam.Angle -= (int)(RotationSpeed * _elapsedTime); // Turn right
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
        /// Simulates the camera movement.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cam">The camera.</param>
        /// <returns>New Camera Values</returns>
        public static Camera SimulateCameraMovementNew(Key key, Camera cam)
        {
            UpdateDeltaTime(); // Update deltaTime based on frame time

            //the key
            Trace.WriteLine($"Key: {key}");

            // Log the old camera state
            Trace.WriteLine($"Before: {cam}");

            // Update the actual camera object directly
            switch (key)
            {
                case Key.W:
                    cam.X -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSin(cam.Angle));
                    cam.Y -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCos(cam.Angle));
                    break;
                case Key.S:
                    cam.X += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSin(cam.Angle));
                    cam.Y += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCos(cam.Angle));
                    break;
                case Key.A:
                    cam.Angle += (int)(RotationSpeed * _elapsedTime); // Turn left
                    break;
                case Key.D:
                    cam.Angle -= (int)(RotationSpeed * _elapsedTime); // Turn right
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
        ///     Update method to calculate deltaTime
        /// </summary>
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