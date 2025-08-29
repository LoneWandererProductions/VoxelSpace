/*
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

namespace RenderEngine;

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
    /// Gets the stride (number of bytes per row).
    /// </summary>
    public int Stride { get; }

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
        Stride = width * 4; // assuming 32bpp ARGB

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
        if (_bufferPtr != IntPtr.Zero) Marshal.FreeHGlobal(_bufferPtr);
    }

    /// <summary>
    ///     Gets the color of the pixel at the specified coordinates.
    /// </summary>
    /// <param name="x">X coordinate (0-based)</param>
    /// <param name="y">Y coordinate (0-based)</param>
    /// <returns>The pixel color as a System.Drawing.Color</returns>
    public Color GetPixel(int x, int y)
    {
        var offset = GetPixelOffset(x, y);
        var buffer = BufferSpan;
        var b = buffer[offset];
        var g = buffer[offset + 1];
        var r = buffer[offset + 2];
        var a = buffer[offset + 3];
        return Color.FromArgb(a, r, g, b);
    }

    /// <summary>
    ///     Sets the pixel color at the specified coordinates using a System.Drawing.Color.
    /// </summary>
    /// <param name="x">X coordinate (0-based)</param>
    /// <param name="y">Y coordinate (0-based)</param>
    /// <param name="color">The color to set</param>
    public void SetPixel(int x, int y, Color color)
    {
        SetPixel(x, y, color.A, color.R, color.G, color.B);
    }

    /// <summary>
    ///     Sets the pixel with alpha blend.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="a">a.</param>
    /// <param name="r">The r.</param>
    /// <param name="g">The g.</param>
    /// <param name="b">The b.</param>
    public void SetPixelAlphaBlend(int x, int y, byte a, byte r, byte g, byte b)
    {
        var offset = GetPixelOffset(x, y);
        var buffer = BufferSpan;

        // Old pixel BGRA
        var oldB = buffer[offset];
        var oldG = buffer[offset + 1];
        var oldR = buffer[offset + 2];
        var oldA = buffer[offset + 3];

        var alpha = a / 255f;

        var newR = (byte)(r * alpha + oldR * (1 - alpha));
        var newG = (byte)(g * alpha + oldG * (1 - alpha));
        var newB = (byte)(b * alpha + oldB * (1 - alpha));
        var newA = (byte)Math.Clamp((int)(a + oldA * (1 - alpha)), 0, 255); // Approximate new alpha

        buffer[offset] = newB;
        buffer[offset + 1] = newG;
        buffer[offset + 2] = newR;
        buffer[offset + 3] = newA;
    }


    /// <summary>
    ///     Calculates the byte offset in the buffer for the pixel at coordinates (x, y).
    /// </summary>
    /// <param name="x">The horizontal pixel coordinate (0-based).</param>
    /// <param name="y">The vertical pixel coordinate (0-based).</param>
    /// <returns>The byte offset of the pixel in the buffer.</returns>
    public int GetPixelOffset(int x, int y)
    {
        return (y * Width + x) * _bytesPerPixel;
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
    ///     Clears the entire buffer by setting every pixel to the specified color in BGRA order.
    ///     Uses SIMD vectorized operations for performance when available.
    /// </summary>
    /// <param name="a">Alpha channel byte value.</param>
    /// <param name="r">Red channel byte value.</param>
    /// <param name="g">Green channel byte value.</param>
    /// <param name="b">Blue channel byte value.</param>
    public void Clear(byte a, byte r, byte g, byte b)
    {
        var buffer = BufferSpan; // Span<byte> representing the pixel buffer

        int vectorSize = Vector<byte>.Count;

        // Ensure vectorSize is multiple of 4 (pixels)
        if (vectorSize % 4 != 0)
            vectorSize -= vectorSize % 4;

        // Build a temporary array containing repeated pixels to fill one Vector
        byte[] pixelArray = new byte[vectorSize];
        for (int j = 0; j < vectorSize; j += 4)
        {
            pixelArray[j] = b;
            pixelArray[j + 1] = g;
            pixelArray[j + 2] = r;
            pixelArray[j + 3] = a;
        }

        var pixelVector = new Vector<byte>(pixelArray);

        int i = 0;

        // SIMD loop
        for (; i <= buffer.Length - vectorSize; i += vectorSize)
        {
            pixelVector.CopyTo(buffer.Slice(i, vectorSize));
        }

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
            const int vectorSize = 32; // 256 bits / 8

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
    ///     Converts to bitmap.
    /// </summary>
    /// <returns>ImageBufferManager to BitmapBuffer</returns>
    public Bitmap ToBitmap()
    {
        var bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
        var bmpData = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
        try
        {
            Buffer.MemoryCopy(
                (void*)_bufferPtr,
                bmpData.Scan0.ToPointer(),
                _bufferSize,
                _bufferSize);
        }
        finally
        {
            bmp.UnlockBits(bmpData);
        }

        return bmp;
    }

    /// <summary>
    ///     Froms the bitmap.
    /// </summary>
    /// <param name="bmp">The BMP.</param>
    /// <returns>Bitmap converted to ImageBufferManager</returns>
    public static UnmanagedImageBuffer FromBitmap(Bitmap bmp)
    {
        if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            throw new ArgumentException("Bitmap must be Format32bppArgb", nameof(bmp));

        var buffer = new UnmanagedImageBuffer(bmp.Width, bmp.Height);
        var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly, bmp.PixelFormat);

        try
        {
            int rowBytes = bmp.Width * 4; // 4 bytes per pixel
            var dst = buffer.BufferSpan;

            for (int y = 0; y < bmp.Height; y++)
            {
                var srcRow = new Span<byte>((byte*)data.Scan0 + y * data.Stride, rowBytes);
                var dstRow = dst.Slice(y * rowBytes, rowBytes);
                srcRow.CopyTo(dstRow);
            }
        }
        finally
        {
            bmp.UnlockBits(data);
        }

        return buffer;
    }


    /// <summary>
    ///     Blits the specified source.
    /// </summary>
    /// <param name="src">The source.</param>
    /// <param name="destX">The dest x.</param>
    /// <param name="destY">The dest y.</param>
    public void Blit(UnmanagedImageBuffer src, int destX, int destY)
    {
        for (var y = 0; y < src.Height; y++)
        {
            if (y + destY >= Height) break;

            var srcRow = src.GetPixelSpan(0, y, src.Width);
            var dstRow = GetPixelSpan(destX, destY + y, src.Width);
            srcRow.CopyTo(dstRow);
        }
    }
    /// <summary>
    /// Draws a line using Bresenham’s integer algorithm (no allocations, fast).
    /// </summary>
    public void DrawLine(int x0, int y0, int x1, int y1, byte a, byte r, byte g, byte b)
    {
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if ((uint)x0 < (uint)Width && (uint)y0 < (uint)Height)
            {
                SetPixel(x0, y0, a, r, g, b);
            }
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }

    /// <summary>
    /// Fills an axis-aligned rectangle, using SIMD for full rows when possible.
    /// </summary>
    public void FillRect(int x, int y, int width, int height, byte a, byte r, byte g, byte b)
    {
        if (x < 0) { width += x; x = 0; }
        if (y < 0) { height += y; y = 0; }
        if (x + width > Width) width = Width - x;
        if (y + height > Height) height = Height - y;
        if (width <= 0 || height <= 0) return;

        var rowVector = CreatePixelVector(a, r, g, b);
        int vectorSize = Vector<byte>.Count;

        for (int j = 0; j < height; j++)
        {
            var rowSpan = GetPixelSpan(x, y + j, width);
            int i = 0;
            for (; i <= rowSpan.Length - vectorSize; i += vectorSize)
                rowVector.CopyTo(rowSpan.Slice(i, vectorSize));
            for (; i < rowSpan.Length; i += 4)
            {
                rowSpan[i] = b;
                rowSpan[i + 1] = g;
                rowSpan[i + 2] = r;
                rowSpan[i + 3] = a;
            }
        }
    }

    /// <summary>
    /// Fills a triangle using barycentric coordinates (optional texture sampling).
    /// </summary>
    public void FillTriangle(Point p0, Point p1, Point p2,
                             UnmanagedImageBuffer texture = null,
                             Vector2? uv0 = null, Vector2? uv1 = null, Vector2? uv2 = null)
    {
        int minX = Math.Max(0, Math.Min(p0.X, Math.Min(p1.X, p2.X)));
        int maxX = Math.Min(Width - 1, Math.Max(p0.X, Math.Max(p1.X, p2.X)));
        int minY = Math.Max(0, Math.Min(p0.Y, Math.Min(p1.Y, p2.Y)));
        int maxY = Math.Min(Height - 1, Math.Max(p0.Y, Math.Max(p1.Y, p2.Y)));

        float denom = (p1.Y - p2.Y) * (p0.X - p2.X) + (p2.X - p1.X) * (p0.Y - p2.Y);
        if (denom == 0) return;

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float w0 = ((p1.Y - p2.Y) * (x - p2.X) + (p2.X - p1.X) * (y - p2.Y)) / denom;
                float w1 = ((p2.Y - p0.Y) * (x - p2.X) + (p0.X - p2.X) * (y - p2.Y)) / denom;
                float w2 = 1 - w0 - w1;

                if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                {
                    if (texture != null && uv0.HasValue && uv1.HasValue && uv2.HasValue)
                    {
                        int tx = (int)(w0 * uv0.Value.X + w1 * uv1.Value.X + w2 * uv2.Value.X);
                        int ty = (int)(w0 * uv0.Value.Y + w1 * uv1.Value.Y + w2 * uv2.Value.Y);

                        tx = Math.Clamp(tx, 0, texture.Width - 1);
                        ty = Math.Clamp(ty, 0, texture.Height - 1);

                        var c = texture.GetPixel(tx, ty);
                        SetPixel(x, y, c.A, c.R, c.G, c.B);
                    }
                    else
                    {
                        SetPixel(x, y, 255, 255, 255, 255); // Default white
                    }
                }
            }
        }
    }


    /// <summary>
    /// Draws a quad by splitting into two triangles (optionally textured).
    /// </summary>
    /// <summary>
    /// Draws a textured quad by splitting it into two triangles with proper texture mapping.
    /// </summary>
    /// <param name="p0">Top-left vertex in screen space.</param>
    /// <param name="p1">Top-right vertex in screen space.</param>
    /// <param name="p2">Bottom-right vertex in screen space.</param>
    /// <param name="p3">Bottom-left vertex in screen space.</param>
    /// <param name="texture">The texture to map onto the quad.</param>
    public void DrawTexturedQuad(Point p0, Point p1, Point p2, Point p3, UnmanagedImageBuffer? texture = null)
    {
        if (texture == null) throw new ArgumentNullException(nameof(texture));

        // Define per-vertex UVs for full quad
        var uv0 = new Vector2(0, 0);                       // top-left
        var uv1 = new Vector2(texture.Width - 1, 0);       // top-right
        var uv2 = new Vector2(texture.Width - 1, texture.Height - 1); // bottom-right
        var uv3 = new Vector2(0, texture.Height - 1);     // bottom-left

        // Draw two triangles forming the quad
        FillTriangle(p0, p1, p2, texture, uv0, uv1, uv2);
        FillTriangle(p0, p2, p3, texture, uv0, uv2, uv3);
    }


    /// <summary>
    /// Draws a filled triangle using integer coordinates and a solid color.
    /// </summary>
    /// <param name="p0">First vertex.</param>
    /// <param name="p1">Second vertex.</param>
    /// <param name="p2">Third vertex.</param>
    /// <param name="color">Fill color.</param>
    public void DrawFilledTriangle(Point p0, Point p1, Point p2, Color color)
    {
        // Bounding box
        int minX = Math.Max(0, Math.Min(p0.X, Math.Min(p1.X, p2.X)));
        int minY = Math.Max(0, Math.Min(p0.Y, Math.Min(p1.Y, p2.Y)));
        int maxX = Math.Min(Width - 1, Math.Max(p0.X, Math.Max(p1.X, p2.X)));
        int maxY = Math.Min(Height - 1, Math.Max(p0.Y, Math.Max(p1.Y, p2.Y)));

        // Precompute edge vectors
        var v0 = new Vector2(p1.X - p0.X, p1.Y - p0.Y);
        var v1 = new Vector2(p2.X - p0.X, p2.Y - p0.Y);
        float denom = v0.X * v1.Y - v1.X * v0.Y;
        if (denom == 0) return; // Degenerate triangle

        // Iterate bounding box
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                var v2 = new Vector2(x - p0.X, y - p0.Y);

                // Barycentric coordinates
                float u = (v2.X * v1.Y - v1.X * v2.Y) / denom;
                float v = (v0.X * v2.Y - v2.X * v0.Y) / denom;
                float w = 1 - u - v;

                if (u >= 0 && v >= 0 && w >= 0)
                {
                    SetPixel(x, y, color.A, color.R, color.G, color.B);
                }
            }
        }
    }

    /// <summary>
    ///     Blits a rectangular region from the source buffer to this buffer.
    /// </summary>
    /// <param name="src">The source.</param>
    /// <param name="srcX">X-coordinate of the top-left corner in the source.</param>
    /// <param name="srcY">Y-coordinate of the top-left corner in the source.</param>
    /// <param name="width">Width of the region to copy.</param>
    /// <param name="height">Height of the region to copy.</param>
    /// <param name="destX">The dest x.</param>
    /// <param name="destY">The dest y.</param>
    public void BlitRegion(UnmanagedImageBuffer src, int srcX, int srcY, int width, int height, int destX, int destY)
    {
        for (var y = 0; y < height; y++)
        {
            var srcRow = src.GetPixelSpan(srcX, srcY + y, width);
            var dstRow = GetPixelSpan(destX, destY + y, width);
            srcRow.CopyTo(dstRow);
        }
    }

    /// <summary>
    /// Creates a deep copy of this <see cref="UnmanagedImageBuffer"/>.
    /// </summary>
    public UnmanagedImageBuffer Clone()
    {
        var clone = new UnmanagedImageBuffer(Width, Height);
        long byteCount = (long)Stride * Height;

        unsafe
        {
            Buffer.MemoryCopy(
                source: (void*)_bufferPtr,
                destination: (void*)clone._bufferPtr,
                destinationSizeInBytes: byteCount,
                sourceBytesToCopy: byteCount
            );
        }

        return clone;
    }

    /// <summary>
    ///     Packs the bgra.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="r">The r.</param>
    /// <param name="g">The g.</param>
    /// <param name="b">The b.</param>
    /// <returns>Converts color to an unsigned Int</returns>
    public static uint PackBgra(byte a, byte r, byte g, byte b)
    {
        return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
    }
}