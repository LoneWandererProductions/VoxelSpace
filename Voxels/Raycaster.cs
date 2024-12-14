using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Input;
using Mathematics;

namespace Voxels
{
    public sealed class Raycaster
    {
        private const float MovementSpeed = 10f; // Movement speed (units per second)
        private const float RotationSpeed = 10f; // Rotation speed (degrees per second)

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

            //the key
            Trace.WriteLine($"Key: {key}");

            // Log the old camera state
            Trace.WriteLine($"Before: {Camera}");

            // Update the actual camera object directly
            switch (key)
            {
                case Key.W:
                    camera.X -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSin(Camera.Angle));
                    camera.Y -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCos(Camera.Angle));
                    break;
                case Key.S:
                    camera.X += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSin(Camera.Angle));
                    camera.Y += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCos(Camera.Angle));
                    break;
                case Key.A:
                    camera.Angle += (int)(RotationSpeed * _elapsedTime); // Turn left
                    break;
                case Key.D:
                    camera.Angle -= (int)(RotationSpeed * _elapsedTime); // Turn right
                    break;
                case Key.O:
                    camera.Horizon += (int)(RotationSpeed * _elapsedTime); // Move up
                    break;
                case Key.P:
                    camera.Horizon -= (int)(RotationSpeed * _elapsedTime); // Move down
                    break;
                case Key.X:
                    camera.Pitch = Math.Max(camera.Pitch - (int)(RotationSpeed * _elapsedTime), -90); // Look down
                    break;
                case Key.Y:
                    camera.Pitch = Math.Min(camera.Pitch + (int)(RotationSpeed * _elapsedTime), 90); // Look up
                    break;
            }

            // Log the new camera state
            Trace.WriteLine($"After: {Camera}");

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