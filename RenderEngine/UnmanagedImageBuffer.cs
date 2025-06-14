﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        UnmanagedImageBuffer.cs
 * PURPOSE:     A way to store images in a fast way.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace RenderEngine
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents an unmanaged memory buffer for storing image pixel data with direct memory access,
    ///     optimized for fast pixel manipulation and bulk operations using SIMD acceleration where available.
    /// </summary>
    /// <remarks>
    ///     This class allocates unmanaged memory of size Width * Height * BytesPerPixel to store image data in BGRA format by
    ///     default.
    ///     It supports setting individual pixels, clearing the buffer to a uniform color,
    ///     applying multiple pixel changes at once, and replacing the entire buffer efficiently.
    /// </remarks>
    public sealed unsafe class UnmanagedImageBuffer : IDisposable
    {
        /// <summary>
        ///     The buffer PTR
        /// </summary>
        private readonly IntPtr _bufferPtr;

        /// <summary>
        ///     The buffer size
        /// </summary>
        private readonly int _bufferSize;

        /// <summary>
        ///     The bytes per pixel
        /// </summary>
        private readonly int _bytesPerPixel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UnmanagedImageBuffer" /> class with specified dimensions and bytes per
        ///     pixel.
        ///     The buffer is allocated in unmanaged memory and initially cleared to transparent black.
        /// </summary>
        /// <param name="width">The width of the image in pixels. Must be positive.</param>
        /// <param name="height">The height of the image in pixels. Must be positive.</param>
        /// <param name="bytesPerPixel">The number of bytes per pixel (default is 4 for BGRA).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if width or height is less than or equal to zero.</exception>
        public UnmanagedImageBuffer(int width, int height, int bytesPerPixel = 4)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            if (bytesPerPixel <= 0) throw new ArgumentOutOfRangeException(nameof(bytesPerPixel));

            Width = width;
            Height = height;
            _bytesPerPixel = bytesPerPixel;
            _bufferSize = width * height * bytesPerPixel;

            _bufferPtr = Marshal.AllocHGlobal(_bufferSize);
            Clear(0, 0, 0, 0);
        }

        /// <summary>
        ///     Gets a span representing the entire unmanaged buffer memory as a byte sequence.
        ///     Modifications to this span directly update the unmanaged image data.
        /// </summary>
        public Span<byte> BufferSpan => new(_bufferPtr.ToPointer(), _bufferSize);

        /// <summary>
        ///     Gets the width of the image in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        ///     Gets the height of the image in pixels.
        /// </summary>
        public int Height { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Frees the unmanaged buffer memory.
        /// </summary>
        public void Dispose()
        {
            if (_bufferPtr == IntPtr.Zero) return;

            Marshal.FreeHGlobal(_bufferPtr);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Converts to bitmap.
        /// </summary>
        /// <returns>The Buffer as bitmapImage</returns>
        public Bitmap ToBitmap()
        {
            var bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            var bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                bitmap.PixelFormat);

            var length = BufferSpan.Length;

            fixed (byte* srcPtr = BufferSpan)
            {
                Buffer.MemoryCopy(srcPtr, (void*)bmpData.Scan0, length, length);
            }

            bitmap.UnlockBits(bmpData);
            return bitmap;
        }

        /// <summary>
        ///     Sets the color of a single pixel at coordinates (x, y) in BGRA order.
        /// </summary>
        /// <param name="x">The horizontal pixel coordinate (0-based).</param>
        /// <param name="y">The vertical pixel coordinate (0-based).</param>
        /// <param name="a">Alpha channel byte value.</param>
        /// <param name="r">Red channel byte value.</param>
        /// <param name="g">Green channel byte value.</param>
        /// <param name="b">Blue channel byte value.</param>
        public void SetPixel(int x, int y, byte a, byte r, byte g, byte b)
        {
            var offset = GetPixelOffset(x, y);
            var buffer = BufferSpan;
            buffer[offset] = b;
            buffer[offset + 1] = g;
            buffer[offset + 2] = r;
            buffer[offset + 3] = a;
        }

        /// <summary>
        ///     Sets a horizontal sequence of pixels starting at (x, y) using the provided byte data.
        /// </summary>
        /// <param name="x">The starting horizontal coordinate.</param>
        /// <param name="y">The vertical coordinate.</param>
        /// <param name="pixelData">A span of pixel data to set, must be a multiple of bytes per pixel.</param>
        /// <exception cref="ArgumentException">Thrown if the data does not align with bytes per pixel or is out of bounds.</exception>
        public void SetPixelSpan(int x, int y, ReadOnlySpan<byte> pixelData)
        {
            var count = pixelData.Length / _bytesPerPixel;
            var target = GetPixelSpan(x, y, count);
            pixelData.CopyTo(target);
        }

        /// <summary>
        ///     Sets a vertical span of pixels at the specified x-coordinate, starting from yStart, using the provided pixel data.
        /// </summary>
        /// <param name="x">The x-coordinate (column) where pixels will be written vertically.</param>
        /// <param name="yStart">The starting y-coordinate (row) where writing begins.</param>
        /// <param name="pixelData">
        ///     A span of bytes representing consecutive pixels to write vertically.
        ///     Must be a multiple of the pixel size in bytes (_bytesPerPixel).
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when x or yStart are out of bounds, or when the span would exceed the image height.
        /// </exception>
        public void SetVerticalPixelSpan(int x, int yStart, ReadOnlySpan<byte> pixelData)
        {
            // Calculate the number of vertical pixels from the byte length
            var count = pixelData.Length / _bytesPerPixel;

            // Ensure coordinates are within bounds and the span fits vertically
            if (x < 0 || x >= Width || yStart < 0 || yStart + count > Height)
                throw new ArgumentOutOfRangeException();

            // Copy each pixel row-by-row into the target buffer
            for (var i = 0; i < count; i++)
            {
                // Get destination slice for this pixel
                var dest = BufferSpan.Slice(GetPixelOffset(x, yStart + i), _bytesPerPixel);

                // Copy pixel data from the source span to the destination
                pixelData.Slice(i * _bytesPerPixel, _bytesPerPixel).CopyTo(dest);
            }
        }

        /// <summary>
        ///     Retrieves a span representing a horizontal sequence of pixels starting at (x, y).
        ///     The span length is equal to count pixels, each containing bytes per pixel.
        /// </summary>
        /// <param name="x">The starting horizontal pixel coordinate (0-based).</param>
        /// <param name="y">The vertical pixel coordinate (0-based).</param>
        /// <param name="count">The number of consecutive pixels to retrieve.</param>
        /// <returns>A span of bytes representing the requested pixels.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if the requested pixel range is out of bounds of the image dimensions.
        /// </exception>
        public Span<byte> GetPixelSpan(int x, int y, int count)
        {
            if (x < 0 || y < 0 || x + count > Width || y >= Height) throw new ArgumentOutOfRangeException();

            var offset = GetPixelOffset(x, y);
            var length = count * _bytesPerPixel;
            return BufferSpan.Slice(offset, length);
        }

        /// <summary>
        ///     Retrieves a vertical column of pixels starting from (x, yStart) over a specified number of pixels.
        /// </summary>
        /// <param name="x">Horizontal coordinate (column index).</param>
        /// <param name="yStart">Starting row.</param>
        /// <param name="count">Number of pixels to retrieve.</param>
        /// <param name="buffer">The destination buffer span. Must be large enough.</param>
        /// <exception cref="ArgumentOutOfRangeException">If coordinates are invalid.</exception>
        /// <exception cref="ArgumentException">If buffer is too small.</exception>
        public void GetColumnSpan(int x, int yStart, int count, Span<byte> buffer)
        {
            if (x < 0 || x >= Width || yStart < 0 || yStart + count > Height)
                throw new ArgumentOutOfRangeException();

            if (buffer.Length < count * _bytesPerPixel)
                throw new ArgumentException("Buffer too small");

            for (var i = 0; i < count; i++)
            {
                var offset = GetPixelOffset(x, yStart + i);
                BufferSpan.Slice(offset, _bytesPerPixel).CopyTo(
                    buffer.Slice(i * _bytesPerPixel, _bytesPerPixel));
            }
        }

        /// <summary>
        ///     Clears the entire buffer by setting every pixel to the specified color in BGRA order.
        ///     Uses SIMD vectorized operations for performance when available.
        /// </summary>
        /// <param name="a">Alpha channel byte value.</param>
        /// <param name="r">Red channel byte value.</param>
        /// <param name="g">Green channel byte value.</param>
        /// <param name="b">Blue channel byte value.</param>
        public void Clear(byte a, byte r, byte g, byte b)
        {
            var buffer = BufferSpan;

            var pixelVector = CreatePixelVector(a, r, g, b);

            var vectorSize = Vector<byte>.Count;
            var i = 0;

            for (; i <= buffer.Length - vectorSize; i += vectorSize) pixelVector.CopyTo(buffer.Slice(i, vectorSize));

            // Fill any remaining bytes one pixel at a time
            for (; i < buffer.Length; i += 4)
            {
                buffer[i] = b;
                buffer[i + 1] = g;
                buffer[i + 2] = r;
                buffer[i + 3] = a;
            }
        }

        /// <summary>
        ///     Applies multiple pixel changes to the buffer in-place, given a span of coordinate-color tuples.
        ///     Each tuple contains the x and y pixel coordinates and a packed 32-bit BGRA color.
        ///     Pixels outside the valid image bounds are ignored.
        /// </summary>
        /// <param name="changes">A read-only span of pixel changes, each specified as (x, y, BGRA color).</param>
        public void ApplyChanges(ReadOnlySpan<(int x, int y, uint bgra)> changes)
        {
            var buffer = BufferSpan;

            foreach (var (x, y, bgra) in changes)
            {
                if ((uint)x >= (uint)Width || (uint)y >= (uint)Height) continue;

                var offset = GetPixelOffset(x, y);

                // Decompose packed uint BGRA color into bytes:
                buffer[offset] = (byte)(bgra & 0xFF); // Blue
                buffer[offset + 1] = (byte)((bgra >> 8) & 0xFF); // Green
                buffer[offset + 2] = (byte)((bgra >> 16) & 0xFF); // Red
                buffer[offset + 3] = (byte)((bgra >> 24) & 0xFF); // Alpha
            }
        }

        /// <summary>
        ///     Replaces the entire unmanaged buffer with a new byte span.
        ///     The input buffer must match the internal buffer size exactly.
        ///     Uses hardware-accelerated AVX2 instructions for bulk copy if supported.
        /// </summary>
        /// <param name="fullBuffer">The source byte span representing the full image buffer to copy.</param>
        /// <exception cref="ArgumentException">Thrown if the input buffer length does not match the internal buffer size.</exception>
        public void ReplaceBuffer(ReadOnlySpan<byte> fullBuffer)
        {
            if (fullBuffer.Length != _bufferSize) throw new ArgumentException(RenderResource.ErrorInputBuffer);

            var buffer = BufferSpan;

            if (Avx2.IsSupported)
            {
                var vectorSize = 32; // 256 bits / 8
                var simdCount = _bufferSize / vectorSize;
                var remainder = _bufferSize % vectorSize;

                fixed (byte* srcPtr = fullBuffer)
                {
                    var dstPtr = (byte*)_bufferPtr;

                    for (var i = 0; i < simdCount; i++)
                    {
                        var vec = Avx.LoadVector256(srcPtr + i * vectorSize);
                        Avx.Store(dstPtr + i * vectorSize, vec);
                    }

                    // Copy any remaining bytes one by one
                    for (var i = _bufferSize - remainder; i < _bufferSize; i++) buffer[i] = fullBuffer[i];
                }
            }
            else
            {
                // Fallback: simple managed copy
                fullBuffer.CopyTo(buffer);
            }
        }

        /// <summary>
        ///     Calculates the byte offset in the buffer for the pixel at coordinates (x, y).
        /// </summary>
        /// <param name="x">The horizontal pixel coordinate (0-based).</param>
        /// <param name="y">The vertical pixel coordinate (0-based).</param>
        /// <returns>The byte offset of the pixel in the buffer.</returns>
        private int GetPixelOffset(int x, int y)
        {
            return (y * Width + x) * _bytesPerPixel;
        }

        /// <summary>
        ///     Creates a SIMD vector filled with the specified BGRA pixel color repeated to fill the vector.
        /// </summary>
        /// <param name="a">Alpha channel byte value.</param>
        /// <param name="r">Red channel byte value.</param>
        /// <param name="g">Green channel byte value.</param>
        /// <param name="b">Blue channel byte value.</param>
        /// <returns>A <see cref="Vector{Byte}" /> filled with the repeated pixel color pattern.</returns>
        private static Vector<byte> CreatePixelVector(byte a, byte r, byte g, byte b)
        {
            var pixelBytes = new byte[Vector<byte>.Count];
            for (var i = 0; i < pixelBytes.Length; i += 4)
            {
                pixelBytes[i] = b;
                pixelBytes[i + 1] = g;
                pixelBytes[i + 2] = r;
                pixelBytes[i + 3] = a;
            }

            return new Vector<byte>(pixelBytes);
        }
    }
}