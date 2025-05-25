using System;
using MinimalRender;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Voxels;

namespace OpenGler
{
    internal static class Program
    {
        private static RasterRaycast _raycaster;
        private static RenderHost _renderHost;
        private static RenderMode _renderMode = RenderMode.Manual;
        private static byte[] pixels;

        private static int width;
        private static int height;

        private enum RenderMode
        {
            Manual,
            Raycaster,
            RenderHost
        }

        [STAThread]
        private static void Main()
        {
            var gameSettings = new GameWindowSettings
            {
                UpdateFrequency = 60.0
            };

            var nativeSettings = new NativeWindowSettings
            {
                ClientSize = new Vector2i(800, 600),
                Title = "Manual Texture Update Example"
            };

            using var window = new GameWindow(gameSettings, nativeSettings);

            RasterRenderer renderer = null;

            width = window.Size.X;
            height = window.Size.Y;
            pixels = new byte[width * height * 4];

            var frameCounter = 0;

            window.Load += () =>
            {
                switch (_renderMode)
                {
                    case RenderMode.Manual:
                    case RenderMode.Raycaster:
                        renderer = new RasterRenderer();
                        renderer.Initialize(width, height);
                        if (_renderMode == RenderMode.Raycaster)
                            InitiateVRaycaster(width, height);
                        break;

                    case RenderMode.RenderHost:
                        _renderHost = new RenderHost(width, height);
                        // Start RenderHost on a separate thread or run directly
                        // We'll handle that later in this example
                        break;
                }
            };

            window.Resize += (ResizeEventArgs e) =>
            {
                width = e.Width;
                height = e.Height;
                pixels = new byte[width * height * 4];

                switch (_renderMode)
                {
                    case RenderMode.Manual:
                        // nothing special
                        break;

                    case RenderMode.Raycaster:
                        InitiateVRaycaster(width, height);
                        break;

                    case RenderMode.RenderHost:
                        _renderHost?.Dispose();
                        _renderHost = new RenderHost(width, height);
                        break;
                }
            };

            window.UpdateFrame += (FrameEventArgs args) =>
            {
                switch (_renderMode)
                {
                    case RenderMode.Manual:
                        UpdateImageFromGameLogic(height, width, pixels, ref frameCounter);
                        break;

                    case RenderMode.Raycaster:
                        Array.Copy(_raycaster.Render().Bytes, pixels, pixels.Length);
                        break;

                    case RenderMode.RenderHost:
                        UpdateImageFromGameLogic(height, width, pixels, ref frameCounter);
                        _renderHost?.SetFrame(pixels);
                        break;
                }

                if (_renderMode != RenderMode.RenderHost && renderer != null)
                {
                    renderer.UpdateTexture(pixels, width, height);
                }
            };

            window.RenderFrame += (FrameEventArgs args) =>
            {
                if (_renderMode != RenderMode.RenderHost && renderer != null)
                {
                    renderer.Render();
                    window.SwapBuffers();
                }
            };

            window.Unload += () =>
            {
                renderer?.Dispose();
                _renderHost?.Dispose();
            };

            if (_renderMode == RenderMode.RenderHost)
            {
                // Run the RenderHost loop instead of this GameWindow's loop
                _renderHost?.Run();
            }
            else
            {
                window.Run();
            }
        }


        private static void InitiateVRaycaster(int width, int height)
        {
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

            var camera = new RvCamera(96, 96, 0);
            var context = new CameraContext(64, height, width);
            _raycaster = new RasterRaycast(map, camera, context);
        }

        private static void UpdateImageFromGameLogic(int height, int width, byte[] pixels, ref int frameCounter)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = (y * width + x) * 4;
                    pixels[i + 0] = (byte)((x + frameCounter) % 256); // Blue
                    pixels[i + 1] = (byte)((y + frameCounter) % 256); // Green
                    pixels[i + 2] = 0; // Red
                    pixels[i + 3] = 255; // Alpha
                }
            }

            frameCounter++;
        }
    }
}
