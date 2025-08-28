using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rays
{
    /// <summary>
    /// Wrapper for DungeonRenderer that handles camera movement and rendering.
    /// </summary>
    public sealed class DungeonViewport
    {
        private readonly DungeonRenderer _renderer;
        private readonly IRenderer _rast;
        private readonly DungeonMap _map;
        private readonly int _width;
        private readonly int _height;

        public DungeonViewport(DungeonMap map, IRenderer renderer, int width, int height)
        {
            _map = map;
            _renderer = new DungeonRenderer(map, new Dictionary<int, Bitmap?>());
            _rast = renderer;
            _width = width;
            _height = height;
            Camera = new Camera3D { Position = new Vector3(1.5f, 1.7f, 1.5f) };
        }

        public Camera3D Camera { get; set; }

        /// <summary>
        /// Render a frame, optionally applying a single key input.
        /// </summary>
        public Bitmap Render(Key? eKey = null)
        {
            // Apply movement
            if (eKey.HasValue)
                ApplyKeyMovement(eKey.Value);

            // Render using current camera and raster
            _renderer.Render(_rast, Camera, _width, _height);
            return _rast.GetFrame();
        }

        public void ApplyKeyMovement(Key key)
        {
            var moveSpeed = 0.2f;
            var rotSpeed = 0.05f;

            if (key == Key.W) Camera.Position += Vector3.Transform(new Vector3(0, 0, moveSpeed), Matrix4x4.CreateRotationY(Camera.Yaw));
            if (key == Key.S) Camera.Position += Vector3.Transform(new Vector3(0, 0, -moveSpeed), Matrix4x4.CreateRotationY(Camera.Yaw));
            if (key == Key.A) Camera.Position += Vector3.Transform(new Vector3(-moveSpeed, 0, 0), Matrix4x4.CreateRotationY(Camera.Yaw));
            if (key == Key.D) Camera.Position += Vector3.Transform(new Vector3(moveSpeed, 0, 0), Matrix4x4.CreateRotationY(Camera.Yaw));
            if (key == Key.Left) Camera.Yaw -= rotSpeed;
            if (key == Key.Right) Camera.Yaw += rotSpeed;
            if (key == Key.Up) Camera.Pitch = MathF.Min(Camera.Pitch + rotSpeed, MathF.PI / 2);
            if (key == Key.Down) Camera.Pitch = MathF.Max(Camera.Pitch - rotSpeed, -MathF.PI / 2);
        }
    }
}
