using System;
using MinimalRender;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenGler
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var nativeSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "Minimal Raster Renderer"
            };

            using (var window = new GameWindow(GameWindowSettings.Default, nativeSettings))
            {
                RasterRenderer renderer = null;

                window.Load += () =>
                {
                    renderer = new RasterRenderer();
                    renderer.Initialize(window.Size.X, window.Size.Y);

                    // Create a solid red texture in BGRA format
                    int w = window.Size.X;
                    int h = window.Size.Y;
                    byte[] pixels = new byte[w * h * 4];

                    for (int i = 0; i < pixels.Length; i += 4)
                    {
                        pixels[i + 0] = 0; // B
                        pixels[i + 1] = 0; // G
                        pixels[i + 2] = 255; // R
                        pixels[i + 3] = 255; // A
                    }

                    renderer.UpdateTexture(pixels, w, h);
                };

                window.RenderFrame += (FrameEventArgs args) =>
                {
                    renderer.Render();
                    window.SwapBuffers();
                };

                window.Unload += () => { renderer?.Dispose(); };

                window.Run();
            }
        }
    }
}