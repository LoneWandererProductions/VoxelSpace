/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        LayeredImageContainer.cs
 * PURPOSE:     Layered Image Container to overlay Images in a quick way.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;

namespace RenderEngine
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides a container for multiple image layers stored as unmanaged buffers,
    ///     allowing fast compositing and alpha blending of layered images.
    /// </summary>
    public sealed class LayeredImageContainer : IDisposable
    {
        /// <summary>
        ///     The height
        /// </summary>
        private readonly int _height;

        /// <summary>
        ///     The layers
        /// </summary>
        private readonly List<UnmanagedImageBuffer> _layers = new();

        /// <summary>
        ///     The width
        /// </summary>
        private readonly int _width;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LayeredImageContainer" /> class
        ///     with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the container and all layers.</param>
        /// <param name="height">The height of the container and all layers.</param>
        public LayeredImageContainer(int width, int height)
        {
            _width = width;
            _height = height;
        }

        /// <summary>
        ///     Gets the layer count.
        /// </summary>
        /// <value>
        ///     The layer count.
        /// </value>
        public int LayerCount => _layers.Count;

        /// <inheritdoc />
        /// <summary>
        ///     Releases all resources used by the <see cref="T:RenderEngine.LayeredImageContainer" />,
        ///     including all contained <see cref="T:RenderEngine.UnmanagedImageBuffer" /> layers.
        /// </summary>
        public void Dispose()
        {
            foreach (var layer in _layers) layer.Dispose();

            _layers.Clear();
        }

        /// <summary>
        ///     Adds an existing unmanaged image buffer as a layer.
        /// </summary>
        /// <param name="layer">The <see cref="UnmanagedImageBuffer" /> to add as a layer.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown if the layer's dimensions do not match the container's size.
        /// </exception>
        public void AddLayer(UnmanagedImageBuffer layer)
        {
            if (layer.Width != _width || layer.Height != _height)
                throw new ArgumentException(RenderResource.ErrorLayerSize);

            _layers.Add(layer);
        }

        /// <summary>
        ///     Adds a new empty (fully transparent) layer to the container.
        /// </summary>
        /// <returns>
        ///     The newly created <see cref="UnmanagedImageBuffer" /> representing the blank layer.
        /// </returns>
        public UnmanagedImageBuffer AddEmptyLayer()
        {
            var newLayer = new UnmanagedImageBuffer(_width, _height);
            newLayer.Clear(0, 0, 0, 0); // transparent clear
            _layers.Add(newLayer);
            return newLayer;
        }

        /// <summary>
        ///     Composites all layers in the container using alpha blending,
        ///     producing a single combined <see cref="UnmanagedImageBuffer" />.
        /// </summary>
        /// <returns>
        ///     A new <see cref="UnmanagedImageBuffer" /> representing the composited image.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if no layers exist to composite.</exception>
        public UnmanagedImageBuffer Composite()
        {
            if (_layers.Count == 0) throw new InvalidOperationException(RenderResource.ErrorNoLayers);

            var result = new UnmanagedImageBuffer(_width, _height);
            result.Clear(0, 0, 0, 0); // start transparent

            var targetSpan = result.BufferSpan;

            foreach (var layer in _layers) AlphaBlend(targetSpan, layer.BufferSpan);

            return result;
        }

        public UnmanagedImageBuffer CompositeLayers(IEnumerable<int> layerIndices)
        {
            var result = new UnmanagedImageBuffer(_width, _height);
            result.Clear(0, 0, 0, 0);

            var targetSpan = result.BufferSpan;
            foreach (var index in layerIndices)
            {
                if (index < 0 || index >= _layers.Count)
                    throw new ArgumentOutOfRangeException(nameof(layerIndices),
                        string.Format(RenderResource.ErrorInvalidLayerIndex, index));

                var layerSpan = _layers[index].BufferSpan;
                AlphaBlend(targetSpan, layerSpan);
            }

            return result;
        }

        public void InsertLayer(int index, UnmanagedImageBuffer layer)
        {
            if (layer.Width != _width || layer.Height != _height)
                throw new ArgumentException(RenderResource.ErrorLayerSizeMismatch);

            _layers.Insert(index, layer);
        }

        public void RemoveLayer(int index)
        {
            if (index < 0 || index >= _layers.Count)
                throw new ArgumentOutOfRangeException(nameof(index),
                    string.Format(RenderResource.ErrorInvalidLayerIndex, index));

            _layers.RemoveAt(index);
        }

        /// <summary>
        ///     Performs alpha blending of an overlay image onto a base image buffer.
        ///     Both buffers must be in BGRA format with 4 bytes per pixel.
        /// </summary>
        /// <param name="baseSpan">The span of bytes representing the base image buffer.</param>
        /// <param name="overlaySpan">The span of bytes representing the overlay image buffer.</param>
        private static void AlphaBlend(Span<byte> baseSpan, Span<byte> overlaySpan)
        {
            const int bytesPerPixel = 4;
            var length = baseSpan.Length;

            for (var i = 0; i < length; i += bytesPerPixel)
            {
                var srcB = overlaySpan[i];
                var srcG = overlaySpan[i + 1];
                var srcR = overlaySpan[i + 2];
                var srcA = overlaySpan[i + 3];

                if (srcA == 0)
                    continue;

                var dstB = baseSpan[i];
                var dstG = baseSpan[i + 1];
                var dstR = baseSpan[i + 2];
                var dstA = baseSpan[i + 3];

                // Calculate output alpha: outA = srcA + dstA * (255 - srcA) / 255
                var outA = srcA + (dstA * (255 - srcA) + 127) / 255;
                if (outA == 0)
                {
                    baseSpan[i] = 0;
                    baseSpan[i + 1] = 0;
                    baseSpan[i + 2] = 0;
                    baseSpan[i + 3] = 0;
                    continue;
                }

                // Blend channels: (src * srcA + dst * dstA * (255 - srcA) / 255) / outA
                baseSpan[i] = (byte)((srcB * srcA + dstB * dstA * (255 - srcA) / 255 + outA / 2) / outA);
                baseSpan[i + 1] = (byte)((srcG * srcA + dstG * dstA * (255 - srcA) / 255 + outA / 2) / outA);
                baseSpan[i + 2] = (byte)((srcR * srcA + dstR * dstA * (255 - srcA) / 255 + outA / 2) / outA);
                baseSpan[i + 3] = (byte)outA;
            }
        }
    }
}