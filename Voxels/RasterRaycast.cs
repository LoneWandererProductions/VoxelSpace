using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Input;
using Mathematics;

namespace Voxels
{
    public class RasterRaycast
    {
        public Camera6 Camera { get; set; } = null;
        private readonly Raycaster _ray;
        private readonly CameraContext _context;

        public float RotationSpeed { get; set; } = 10f;

        public float MovementSpeed { get; set; } =  10f;

        /// <summary>
        ///     The last update time
        /// </summary>
        private DateTime _lastUpdateTime;

        /// <summary>
        ///     Time elapsed since the last frame
        /// </summary>
        private float _elapsedTime;


        public RasterRaycast(int[,] map, Camera6 camera, CameraContext context)
        {
            _context = context;
            Camera = camera;

            _ray = new Raycaster(map, context);
        }

        public Bitmap Render(Key eKey)
        {
            Camera = SimulateCameraMovement(eKey, Camera);
            return _ray.Render(Camera);
        }

        public Bitmap Render()
        {
            return _ray.Render(Camera);
        }

        private Camera6 SimulateCameraMovement(Key key, Camera6 cam)
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
                    cam.Angle += (int) (RotationSpeed * _elapsedTime); // Turn left
                    break;
                case Key.D:
                    cam.Angle -= (int) (RotationSpeed * _elapsedTime); // Turn right
                    break;
                case Key.O:
                    cam.Angle += (int) (RotationSpeed * _elapsedTime); // Move up
                    break;
                case Key.P:
                    cam.Angle -= (int) (RotationSpeed * _elapsedTime); // Move down
                    break;
                case Key.X:
                    //camera.Pitch = Math.Max(camera.Pitch - (int) (RotationSpeed * _elapsedTime), -90); // Look down
                    break;
                case Key.Y:
                    //camera.Pitch = Math.Min(camera.Pitch + (int) (RotationSpeed * _elapsedTime), 90); // Look up
                    break;
            }

            // Log the new camera state
            Trace.WriteLine($"After: {cam}");

            return cam;
        }

        /// <summary>
        ///     Update method to calculate deltaTime
        /// </summary>
        private void UpdateDeltaTime()
        {
            var currentTime = DateTime.Now;
            _elapsedTime = (float) (currentTime - _lastUpdateTime).TotalSeconds;

            // If no time has elapsed, use a default small value to avoid zero movement on startup
            if (_elapsedTime == 0) _elapsedTime = 0.016f; // Assuming ~60 FPS, 1 frame = ~0.016 seconds

            // Optional: Cap delta time to avoid large jumps
            _elapsedTime = Math.Min(_elapsedTime, 0.1f); // 0.1s cap to prevent large frame gaps
            _lastUpdateTime = currentTime;
        }
    }
}