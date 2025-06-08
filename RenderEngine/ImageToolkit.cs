/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        ImageToolkit.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;
using System.Linq;

namespace RenderEngine
{
    using System;

    namespace RenderEngine
    {
        /// <inheritdoc />
        /// <summary>
        /// Provides utilities for converting raw image data to and from UnmanagedImageBuffer,
        /// and managing multi-layered compositions.
        /// </summary>
        public sealed class ImageToolkit : IDisposable
        {
            /// <summary>
            /// The container
            /// </summary>
            private readonly LayeredImageContainer _container;

            /// <summary>
            /// Gets the width.
            /// </summary>
            /// <value>
            /// The width.
            /// </value>
            public int Width { get; }

            /// <summary>
            /// Gets the height.
            /// </summary>
            /// <value>
            /// The height.
            /// </value>
            public int Height { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ImageToolkit"/> class.
            /// </summary>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            public ImageToolkit(int width, int height)
            {
                Width = width;
                Height = height;
                _container = new LayeredImageContainer(width, height);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() => _container.Dispose();

            /// <summary>
            /// Converts a raw int[] BGRA bitmap to an UnmanagedImageBuffer.
            /// </summary>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="bits">The bits.</param>
            /// <returns>Our UnmanagedImageBuffer Image Format</returns>
            public static UnmanagedImageBuffer FromBits(int width, int height, int[] bits)
            {
                var buffer = new UnmanagedImageBuffer(width, height);
                var span = buffer.BufferSpan;

                for (int i = 0; i < bits.Length; i++)
                {
                    int color = bits[i];
                    int offset = i * 4;

                    span[offset + 0] = (byte)(color & 0xFF);         // B
                    span[offset + 1] = (byte)((color >> 8) & 0xFF);  // G
                    span[offset + 2] = (byte)((color >> 16) & 0xFF); // R
                    span[offset + 3] = (byte)((color >> 24) & 0xFF); // A
                }

                return buffer;
            }

            /// <summary>
            /// Converts an UnmanagedImageBuffer to a raw int[] BGRA bitmap.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            /// <returns>Image in Bits Format</returns>
            public static int[] ToBits(UnmanagedImageBuffer buffer)
            {
                var span = buffer.BufferSpan;
                var result = new int[buffer.Width * buffer.Height];

                for (int i = 0; i < result.Length; i++)
                {
                    int offset = i * 4;

                    int b = span[offset + 0];
                    int g = span[offset + 1];
                    int r = span[offset + 2];
                    int a = span[offset + 3];

                    result[i] = (a << 24) | (r << 16) | (g << 8) | b;
                }

                return result;
            }

            /// <summary>
            /// Adds an existing image layer.
            /// </summary>
            /// <param name="image">The image.</param>
            public void AddLayer(UnmanagedImageBuffer image) => _container.AddLayer(image);

            /// <summary>
            /// Adds a layer from raw int[] BGRA data.
            /// </summary>
            /// <param name="bits">The bits.</param>
            public void AddLayer(int[] bits)
            {
                var image = FromBits(Width, Height, bits);
                _container.AddLayer(image);
            }

            public void MergeAndInsert(IEnumerable<int> mergeIndices, int insertIndex, bool removeOriginals = true)
            {
                var merged = _container.CompositeLayers(mergeIndices);
                _container.InsertLayer(insertIndex, merged);

                if (!removeOriginals)
                {
                    return;
                }

                foreach (var index in mergeIndices.OrderByDescending(i => i))
                    _container.RemoveLayer(index);
            }


            /// <summary>
            /// Adds an empty transparent layer.
            /// </summary>
            /// <returns>Our UnmanagedImageBuffer Image Format</returns>
            public UnmanagedImageBuffer AddEmptyLayer() => _container.AddEmptyLayer();

            /// <summary>
            /// Composites all layers and returns the result as an UnmanagedImageBuffer.
            /// </summary>
            /// <returns>Our UnmanagedImageBuffer Image Format</returns>
            public UnmanagedImageBuffer Composite() => _container.Composite();

            /// <summary>
            /// Composites all layers and returns the result as raw int[] BGRA bits.
            /// </summary>
            /// <returns>Image in Bits Format</returns>
            public int[] CompositeToBits() => ToBits(_container.Composite());
        }
    }
}
