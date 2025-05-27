using System;
using System.Collections.Generic;

namespace RenderEngine
{
    // Layered image container:
    public sealed class LayeredImageContainer : IDisposable
    {
        private readonly int _height;
        private readonly List<UnmanagedImageBuffer> _layers = new();
        private readonly int _width;

        public LayeredImageContainer(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void Dispose()
        {
            foreach (var layer in _layers) layer.Dispose();

            _layers.Clear();
        }

        public void AddLayer(UnmanagedImageBuffer layer)
        {
            if (layer.Width != _width || layer.Height != _height)
                throw new ArgumentException("Layer size does not match container size.");

            _layers.Add(layer);
        }

        // New method to add a blank layer (all transparent)
        public UnmanagedImageBuffer AddEmptyLayer()
        {
            var newLayer = new UnmanagedImageBuffer(_width, _height);
            newLayer.Clear(0, 0, 0, 0); // transparent clear
            _layers.Add(newLayer);
            return newLayer;
        }

        public UnmanagedImageBuffer Composite()
        {
            if (_layers.Count == 0) throw new InvalidOperationException("No layers to composite.");

            var result = new UnmanagedImageBuffer(_width, _height);
            result.Clear(0, 0, 0, 0); // start transparent

            var targetSpan = result.BufferSpan;

            foreach (var layer in _layers) AlphaBlend(targetSpan, layer.BufferSpan);

            return result;
        }

        private static void AlphaBlend(Span<byte> baseSpan, Span<byte> overlaySpan)
        {
            var length = baseSpan.Length;
            var bytesPerPixel = 4;

            for (var i = 0; i < length; i += bytesPerPixel)
            {
                var srcB = overlaySpan[i];
                var srcG = overlaySpan[i + 1];
                var srcR = overlaySpan[i + 2];
                var srcAByte = overlaySpan[i + 3];
                var srcA = srcAByte / 255f;

                if (srcA <= 0)
                    continue;

                var dstB = baseSpan[i];
                var dstG = baseSpan[i + 1];
                var dstR = baseSpan[i + 2];
                var dstAByte = baseSpan[i + 3];
                var dstA = dstAByte / 255f;

                var outA = srcA + dstA * (1 - srcA);

                if (outA <= 0)
                {
                    baseSpan[i] = 0;
                    baseSpan[i + 1] = 0;
                    baseSpan[i + 2] = 0;
                    baseSpan[i + 3] = 0;
                    continue;
                }

                baseSpan[i] = (byte)Math.Round((srcB * srcA + dstB * dstA * (1 - srcA)) / outA);
                baseSpan[i + 1] = (byte)Math.Round((srcG * srcA + dstG * dstA * (1 - srcA)) / outA);
                baseSpan[i + 2] = (byte)Math.Round((srcR * srcA + dstR * dstA * (1 - srcA)) / outA);
                baseSpan[i + 3] = (byte)Math.Round(outA * 255);
            }
        }
    }
}