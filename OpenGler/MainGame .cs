using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using MinimalRender;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Rays;
using Viewer;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

// your Raycaster namespace

namespace OpenGler
{
    public class MainGame : GameWindow
    {
        private RvCamera _camera;
        private Raycaster _raycaster;
        private RasterRenderer _renderer;

        public MainGame(GameWindowSettings gws, NativeWindowSettings nws)
            : base(gws, nws)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color.Black);

            var screenWidth = 320;
            var screenHeight = 200;

            _renderer = new RasterRenderer();
            _renderer.Initialize(screenWidth, screenHeight);

            // Basic test map
            // Simple map where 1 is a wall and 0 is empty space
            var map = new int[10, 10]
            {
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0, 0, 1 },
                { 1, 0, 0, 0, 1, 0, 1, 0, 1, 1 },
                { 1, 0, 1, 0, 0, 0, 1, 1, 1, 1 },
                { 1, 0, 1, 1, 1, 1, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 1, 0, 1, 0, 1, 1 },
                { 1, 1, 1, 1, 1, 0, 1, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            // Set up a camera
            var camera = new RvCamera(96, 96, 0); // Position and angle of the camera
            //setup the context
            CameraContext context = new(64, 600, 800);

            // Create Raycaster and render
            _raycaster = new Raycaster(map, context);
            _camera = camera;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            _camera.Angle += 2;

            var result = _raycaster.Render(_camera);
            var pixelData = result.Bytes;

            _renderer.UpdateTexture(pixelData, result.Bitmap.Width, result.Bitmap.Height);
            _renderer.Render();
            SwapBuffers();

            // Save just once (optional, use a bool flag if needed)
            SaveDebugImage(pixelData, result.Bitmap.Width, result.Bitmap.Height, "debug/raycast_output.png");
        }

        private void SaveDebugImage(byte[] pixels, int width, int height, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var data = bmp.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                bmp.PixelFormat);

            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY); // OpenGL is flipped

            bmp.Save(path);
            Console.WriteLine($"Saved image to {Path.GetFullPath(path)}");
        }
    }
}