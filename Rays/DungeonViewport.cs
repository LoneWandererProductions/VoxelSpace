using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using static Rays.DungeonRenderer;

namespace Rays
{
    /// <summary>
    /// Wrapper for DungeonRenderer that handles camera movement, player physics, and optional actor rendering.
    /// </summary>
    public sealed class DungeonViewport
    {
        private readonly DungeonRenderer _renderer;
        private readonly IRenderer _rast;
        private readonly DungeonMap _map;
        private readonly int _width;
        private readonly int _height;

        public DungeonViewport(DungeonMap map, IRenderer rasterer, int width, int height, Dictionary<int, Bitmap?>? textures = null)
        {
            _map = map;
            _renderer = new DungeonRenderer(map, textures ?? new());
            _rast = rasterer;
            _width = width;
            _height = height;

            Player = new Actor
            {
                Position = new Vector3(1.5f, 1.7f, 1.5f)
            };

            Camera = new Camera3D
            {
                Position = Player.Position
            };
        }

        public Actor Player { get; }
        public Camera3D Camera { get; set; }

        /// <summary>
        /// Optional list of NPCs, enemies, or other actors to render.
        /// </summary>
        public List<Actor>? Actors { get; set; }

        /// <summary>
        /// Render a frame. Applies key input, updates player, and optionally renders actors.
        /// </summary>
        public Bitmap Render(Key? eKey = null, float deltaTime = 0.016f)
        {
            // Apply input to player
            if (eKey.HasValue)
                ApplyKeyMovement(eKey.Value);

            // Physics update
            Player.Update(deltaTime);

            // Camera follows player
            Camera.Position = Player.Position;

            // Cull visible actors if any
            List<PaperDoll>? dolls = null;
            if (Actors != null && Actors.Count > 0)
            {
                var aspect = (float)_width / _height;
                var vp = Camera.GetViewMatrix() * Camera.GetProjectionMatrix(aspect);

                dolls = Actors.Select(a => new PaperDoll
                {
                    Position = a.Position,
                    Radius = 0.5f // simple radius for frustum culling
                })
                .Where(d => FrustumCulling.IsSphereVisible(vp, d.Position, d.Radius))
                .ToList();
            }

            // Render map + player-following camera + visible actors
            return _renderer.Render(_rast, Camera, _width, _height, dolls);
        }

        public void ApplyKeyMovement(Key key)
        {
            var moveSpeed = 0.2f;
            var rotSpeed = 0.05f;

            // Movement in world-space relative to camera yaw
            var forward = Vector3.Transform(new Vector3(0, 0, moveSpeed), Matrix4x4.CreateRotationY(Camera.Yaw));
            var right = Vector3.Transform(new Vector3(moveSpeed, 0, 0), Matrix4x4.CreateRotationY(Camera.Yaw));

            if (key == Key.W) Player.Position += forward;
            if (key == Key.S) Player.Position -= forward;
            if (key == Key.A) Player.Position -= right;
            if (key == Key.D) Player.Position += right;

            if (key == Key.Space) Player.Jump(); // jump handled by Actor

            // Camera rotation
            if (key == Key.Left) Camera.Yaw -= rotSpeed;
            if (key == Key.Right) Camera.Yaw += rotSpeed;
            if (key == Key.Up) Camera.Pitch = MathF.Min(Camera.Pitch + rotSpeed, MathF.PI / 2);
            if (key == Key.Down) Camera.Pitch = MathF.Max(Camera.Pitch - rotSpeed, -MathF.PI / 2);
        }
    }
}
