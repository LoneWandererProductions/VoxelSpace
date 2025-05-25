using System;
using MinimalRender;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenGler
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            var gameSettings = new GameWindowSettings
            {
                UpdateFrequency = 60.0 // Logic updates per second
            };

            var nativeSettings = new NativeWindowSettings
            {
                ClientSize = new Vector2i(800, 600),
                Title = "Manual Texture Update Example"
            };

            using var window = new GameWindow(gameSettings, nativeSettings);
            RasterRenderer renderer = null;

            //stretch the image to the window size
            //int width = window.Size.X;   // 800
            //int height = window.Size.Y;  // 600
            //byte[] pixels = new byte[width * height * 4];


            var width = 256;
            var height = 256;
            var pixels = new byte[width * height * 4];
            var frameCounter = 0;

            window.Load += () =>
            {
                renderer = new RasterRenderer();
                renderer.Initialize(window.Size.X, window.Size.Y);
            };


            // Game logic runs here — this is where you update the pixel buffer
            window.UpdateFrame += (FrameEventArgs args) =>
            {
                UpdateImageFromGameLogic(height, width, pixels, ref frameCounter); // Call your method manually
                renderer.UpdateTexture(pixels, width, height);
            };

            //window.Load += () =>
            //{
            //    renderer = new RasterRenderer();
            //    renderer.Initialize(window.Size.X, window.Size.Y);

            //    // Initialize pixels to window size
            //    width = window.Size.X;
            //    height = window.Size.Y;
            //    pixels = new byte[width * height * 4];
            //};

            //window.UpdateFrame += (args) =>
            //{
            //    UpdateImageFromGameLogic();
            //    renderer.UpdateTexture(pixels, width, height);
            //};

            // Render just draws whatever texture is current
            window.RenderFrame += (FrameEventArgs args) =>
            {
                renderer.Render();
                window.SwapBuffers();
            };

            window.Unload += () =>
            {
                renderer?.Dispose();
            };



            window.Run();
        }

        /// <summary>
        /// Updates the image from game logic.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="pixels">The pixels.</param>
        /// <param name="frameCounter">The frame counter.</param>
        private static void UpdateImageFromGameLogic(int height, int width, byte[] pixels, ref int frameCounter)
        {
            // Example: red-green animation you can replace
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = (y * width + x) * 4;
                    pixels[i + 0] = (byte) ((x + frameCounter) % 256); // Blue
                    pixels[i + 1] = (byte) ((y + frameCounter) % 256); // Green
                    pixels[i + 2] = 0; // Red
                    pixels[i + 3] = 255; // Alpha
                }
            }

            frameCounter++;
        }
    }
}
