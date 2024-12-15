using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Input;
using Mathematics;

namespace Voxels
{
    public sealed class Raycaster
    {
        private const int MovementSpeed = 10; // Movement speed (units per second)
        private const int RotationSpeed = 50; // Rotation speed (degrees per second)

        /// <summary>
        ///     The last update time
        /// </summary>
        private DateTime _lastUpdateTime;

        /// <summary>
        ///     Time elapsed since the last frame
        /// </summary>
        private float _elapsedTime;

        /// <summary>
        ///     Gets the camera.
        /// </summary>
        /// <value>
        ///     The camera.
        /// </value>
        public Camera Camera { get; set; }

        private int[,] _map;
        private readonly RasterRaycast _raster;

        public Raycaster(Camera camera, int[,] map)
        {
            Camera = camera;
            _map = map;
            _raster= new RasterRaycast(Camera, _map);
        }

        public Bitmap Render()
        {
            return _raster.Render();
        }

        public Bitmap Render(Key key)
        {
            Camera = SimulateCameraMovement(key, Camera);
            _raster.Camera = Camera;
            return _raster.Render();
        }


        /// <summary>
        /// Simulates the camera movement.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="camera">The camera.</param>
        /// <returns>New Camera Values</returns>
        private Camera SimulateCameraMovement(Key key, Camera camera)
        {
            UpdateDeltaTime(); // Update deltaTime based on frame time

            // Log the key pressed
            Trace.WriteLine($"Key: {key}");

            // Log the old camera state
            Trace.WriteLine($"Before: {camera}");

            // Update the actual camera object directly based on key presses
            switch (key)
            {
                case Key.W:
                    // Moving forward: adjust X and Y based on the camera's angle
                    camera.X += (int)Math.Round(MovementSpeed * _elapsedTime * Math.Cos(camera.Angle * Math.PI / 180));  // Convert angle to radians and cast to int
                    camera.Y += (int)Math.Round(MovementSpeed * _elapsedTime * Math.Sin(camera.Angle * Math.PI / 180));  // Convert angle to radians and cast to int
                    break;
                case Key.S:
                    // Moving backward: adjust X and Y based on the camera's angle
                    camera.X -= (int)Math.Round(MovementSpeed * _elapsedTime * Math.Cos(camera.Angle * Math.PI / 180));  // Convert angle to radians and cast to int
                    camera.Y -= (int)Math.Round(MovementSpeed * _elapsedTime * Math.Sin(camera.Angle * Math.PI / 180));  // Convert angle to radians and cast to int
                    break;
                case Key.A:
                    // Turn left: rotate the camera's angle counter-clockwise (increase angle)
                    camera.Angle += (int)(RotationSpeed * _elapsedTime); // Angle stays in degrees, cast to int
                    if (camera.Angle >= 360) camera.Angle -= 360;  // Keep angle within 0-360 degrees
                    break;
                case Key.D:
                    // Turn right: rotate the camera's angle clockwise (decrease angle)
                    camera.Angle -= (int)(RotationSpeed * _elapsedTime); // Angle stays in degrees, cast to int
                    if (camera.Angle < 0) camera.Angle += 360; // Keep angle within 0-360 degrees
                    break;
                case Key.O:
                    // Move up (horizon change)
                    camera.Horizon += (int)(RotationSpeed * _elapsedTime); // This can change the horizon or vertical view
                    break;
                case Key.P:
                    // Move down (horizon change)
                    camera.Horizon -= (int)(RotationSpeed * _elapsedTime);
                    break;
                case Key.X:
                    // Look down (pitch control)
                    camera.Pitch = Math.Max(camera.Pitch - (int)(RotationSpeed * _elapsedTime), -90); // Cap between -90 and 90 degrees
                    break;
                case Key.Y:
                    // Look up (pitch control)
                    camera.Pitch = Math.Min(camera.Pitch + (int)(RotationSpeed * _elapsedTime), 90); // Cap between -90 and 90 degrees
                    break;
            }

            // Log the new camera state
            Trace.WriteLine($"After: {camera}");

            return camera;
        }

        /// <summary>
        ///     Update method to calculate deltaTime
        /// </summary>
        private void UpdateDeltaTime()
        {
            var currentTime = DateTime.Now;
            _elapsedTime = (float)(currentTime - _lastUpdateTime).TotalSeconds;

            // If no time has elapsed, use a default small value to avoid zero movement on startup
            if (_elapsedTime == 0) _elapsedTime = 0.016f; // Assuming ~60 FPS, 1 frame = ~0.016 seconds

            // Optional: Cap delta time to avoid large jumps
            _elapsedTime = Math.Min(_elapsedTime, 0.1f); // 0.1s cap to prevent large frame gaps
            _lastUpdateTime = currentTime;
        }
    }
}