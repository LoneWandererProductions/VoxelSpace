using System;
using MinimalRender;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenGler
{
    public class RenderHost : IDisposable
    {
        private readonly byte[] _currentPixels;
        private readonly int _height;
        private readonly object _lock = new();

        private readonly int _width;
        private readonly GameWindow _window;
        private RasterRenderer _renderer;

        public RenderHost(int width, int height)
        {
            _width = width;
            _height = height;
            _currentPixels = new byte[width * height * 4];

            var gameSettings = new GameWindowSettings { UpdateFrequency = 60 };
            var nativeSettings = new NativeWindowSettings
            {
                ClientSize = new Vector2i(width, height),
                Title = "Renderer"
            };

            _window = new GameWindow(gameSettings, nativeSettings);
            _window.Load += OnLoad;
            _window.RenderFrame += OnRender;
            _window.UpdateFrame += OnUpdate;
            _window.Unload += OnUnload;
        }

        public Action UpdateLoop { get; set; }

        public bool IsRunning { get; set; }

        public void Dispose()
        {
            _window?.Close();
            _window?.Dispose();
        }

        private void OnLoad()
        {
            IsRunning = true;
            _renderer = new RasterRenderer();
            _renderer.Initialize(_width, _height);
        }

        private void OnRender(FrameEventArgs args)
        {
            lock (_lock)
            {
                _renderer.UpdateTexture(_currentPixels, _width, _height);
            }

            _renderer.Render();
            _window.SwapBuffers();
        }

        private void OnUpdate(FrameEventArgs args)
        {
            UpdateLoop?.Invoke();
        }

        private void OnUnload()
        {
            _renderer?.Dispose();
        }

        public void SetFrame(byte[] pixels)
        {
            lock (_lock)
            {
                if (pixels.Length != _currentPixels.Length) return;

                Buffer.BlockCopy(pixels, 0, _currentPixels, 0, pixels.Length);
            }
        }

        public void Run()
        {
            _window.Run();
        }
    }
}