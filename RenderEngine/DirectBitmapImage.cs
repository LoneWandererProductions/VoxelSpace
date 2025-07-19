/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/DirectBitmap.cs
 * PURPOSE:     Custom BitmapImage Class, speeds up Set Pixel
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedType.Global

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RenderEngine
{
    /// <inheritdoc />
    /// <summary>
    ///     Similar to DirectBitmap, generate a pixel image, should be slightly faster.
    ///     Supports fast SIMD-based pixel operations and unsafe pointer access.
    /// </summary>
    public sealed class DirectBitmapImage : IDisposable
    {
        private readonly WriteableBitmap _bitmap;
        private GCHandle _bitsHandle;
        private BitmapImage? _cachedImage;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectBitmapImage" /> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public DirectBitmapImage(int width, int height)
        {
            Width = width;
            Height = height;

            Bits = new uint[width * height];
            _bitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            _bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        }

        /// <summary>
        ///     The height of the image.
        /// </summary>
        public int Height { get; }

        /// <summary>
        ///     The width of the image.
        /// </summary>
        public int Width { get; }

        /// <summary>
        ///     Gets the raw pixel buffer.
        /// </summary>
        public uint[] Bits { get; }

        /// <summary>
        ///     Gets the cached converted BitmapImage.
        /// </summary>
        public BitmapImage BitmapImage
        {
            get
            {
                if (_cachedImage != null)
                {
                    return _cachedImage;
                }

                _cachedImage = ConvertImage();

                return _cachedImage;
            }
        }

        /// <summary>
        ///     Frees memory and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Sets pixels from a collection of <see cref="PixelData" />.
        ///     Uses unsafe pointer arithmetic for speed.
        /// </summary>
        /// <param name="pixels">The pixels to set.</param>
        public void SetPixelsUnsafe(IEnumerable<PixelData> pixels)
        {
            _bitmap.Lock();
            unsafe
            {
                var buffer = (byte*)_bitmap.BackBuffer.ToPointer();

                foreach (var pixel in pixels)
                {
                    if (pixel.X < 0 || pixel.X >= Width || pixel.Y < 0 || pixel.Y >= Height)
                    {
                        continue;
                    }

                    var offset = ((pixel.Y * Width) + pixel.X) * 4;
                    buffer[offset + 0] = pixel.B;
                    buffer[offset + 1] = pixel.G;
                    buffer[offset + 2] = pixel.R;
                    buffer[offset + 3] = pixel.A;

                    Bits[(pixel.Y * Width) + pixel.X] =
                        (uint)((pixel.A << 24) | (pixel.R << 16) | (pixel.G << 8) | pixel.B);
                }
            }

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
            _bitmap.Unlock();
        }

        /// <summary>
        ///     Fills the bitmap with a uniform color using SIMD.
        /// </summary>
        /// <param name="color">The color to fill with.</param>
        public void FillSimd(Color color)
        {
            var packed = (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
            var vector = new Vector<uint>(packed);
            var i = 0;
            for (; i <= Bits.Length - Vector<uint>.Count; i += Vector<uint>.Count)
            {
                vector.CopyTo(Bits, i);
            }

            for (; i < Bits.Length; i++)
            {
                Bits[i] = packed;
            }

            UpdateBitmapFromBits();
        }

        /// <summary>
        ///     Applies a 4x4 color matrix to all pixels.
        /// </summary>
        /// <param name="matrix">Color transformation matrix.</param>
        public void ApplyColorMatrix(float[][] matrix)
        {
            var transformed = new (int x, int y, Color color)[Bits.Length];

            for (var i = 0; i < Bits.Length; i++)
            {
                var argb = Bits[i];
                var a = (byte)(argb >> 24);
                var r = (byte)(argb >> 16);
                var g = (byte)(argb >> 8);
                var b = (byte)argb;

                var input = new float[] { r, g, b, a };
                var result = new float[4];

                for (var j = 0; j < 4; j++)
                {
                    result[j] = 0;
                    for (var k = 0; k < 4; k++)
                    {
                        result[j] += matrix[j][k] * input[k];
                    }
                }

                var newColor = Color.FromArgb(
                    (byte)Math.Clamp(result[3], 0, 255),
                    (byte)Math.Clamp(result[0], 0, 255),
                    (byte)Math.Clamp(result[1], 0, 255),
                    (byte)Math.Clamp(result[2], 0, 255)
                );

                transformed[i] = (i % Width, i / Width, newColor);
            }

            SetPixelsSimd(transformed);
        }

        /// <summary>
        ///     Sets individual pixels in the image using a collection of <see cref="PixelData" />.
        ///     Each entry defines the X/Y position and RGBA components.
        /// </summary>
        /// <param name="pixels">A collection of <see cref="PixelData" /> describing the pixels to set.</param>
        public void SetPixels(IEnumerable<PixelData> pixels)
        {
            foreach (var pixel in pixels)
            {
                if (pixel.X < 0 || pixel.X >= Width || pixel.Y < 0 || pixel.Y >= Height)
                {
                    continue;
                }

                var index = (pixel.Y * Width) + pixel.X;
                Bits[index] = (uint)((pixel.A << 24) | (pixel.R << 16) | (pixel.G << 8) | pixel.B);
            }

            UpdateBitmapFromBits();
        }

        /// <summary>
        ///     SIMD-based batch pixel update from (x,y,color) triplets.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        public void SetPixelsSimd(IEnumerable<(int x, int y, Color color)> pixels)
        {
            var pixelArray = pixels.ToArray();
            var vectorCount = Vector<int>.Count;

            for (var i = 0; i < pixelArray.Length; i += vectorCount)
            {
                var indices = new int[vectorCount];
                var colors = new int[vectorCount];

                for (var j = 0; j < vectorCount; j++)
                {
                    if (i + j < pixelArray.Length)
                    {
                        var (x, y, c) = pixelArray[i + j];
                        if (x >= 0 && x < Width && y >= 0 && y < Height)
                        {
                            indices[j] = x + (y * Width);
                            colors[j] = (c.A << 24) | (c.R << 16) | (c.G << 8) | c.B;
                        }
                        else
                        {
                            indices[j] = 0;
                            colors[j] = 0;
                        }
                    }
                }

                for (var j = 0; j < vectorCount; j++)
                {
                    if (i + j < pixelArray.Length)
                    {
                        Bits[indices[j]] = (uint)colors[j];
                    }
                }
            }

            UpdateBitmapFromBits();
        }

        /// <summary>
        ///     Converts the internal bitmap to a <see cref="BitmapImage" />.
        /// </summary>
        private BitmapImage ConvertImage()
        {
            var tempBitmap = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgra32, null);
            var byteArray = new byte[Bits.Length * sizeof(uint)];

            Buffer.BlockCopy(Bits, 0, byteArray, 0, byteArray.Length);

            tempBitmap.Lock();
            Marshal.Copy(byteArray, 0, tempBitmap.BackBuffer, byteArray.Length);
            tempBitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
            tempBitmap.Unlock();

            var bitmapImage = new BitmapImage();
            using var stream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(tempBitmap));
            encoder.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);

            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        /// <summary>
        ///     Updates the WPF bitmap from the Bits buffer.
        /// </summary>
        public void UpdateBitmapFromBits()
        {
            _bitmap.Lock();
            var byteArray = new byte[Bits.Length * sizeof(uint)];
            Buffer.BlockCopy(Bits, 0, byteArray, 0, byteArray.Length);
            Marshal.Copy(byteArray, 0, _bitmap.BackBuffer, byteArray.Length);
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
            _bitmap.Unlock();
        }

        /// <summary>
        ///     Releases unmanaged and managed resources.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing && _bitsHandle.IsAllocated)
            {
                _bitsHandle.Free();
            }

            _disposed = true;
        }

        /// <summary>
        ///     Finalizer.
        /// </summary>
        ~DirectBitmapImage()
        {
            Dispose(false);
        }
    }
}
