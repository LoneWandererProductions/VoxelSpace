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

namespace Imaging
{
    /// <inheritdoc />
    /// <summary>
    ///     Similar to DirectBitmap, generate a pixel image, should be slightly faster
    /// </summary>
    public sealed class DirectBitmapImage : IDisposable
    {
        /// <summary>
        ///     The bitmap
        /// </summary>
        private readonly WriteableBitmap _bitmap;

        /// <summary>
        ///     GCHandle to manage the memory of the bits array
        /// </summary>
        private GCHandle _bitsHandle;

        /// <summary>
        ///     Indicates if the instance has been disposed
        /// </summary>
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

            // Initialize the Bits array and pin it for direct access
            Bits = new uint[width * height];
            _bitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);

            // Initialize WriteableBitmap
            _bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        }

        /// <summary>
        ///     The height
        /// </summary>
        public int Height { get; }

        /// <summary>
        ///     The width
        /// </summary>
        public int Width { get; }

        /// <summary>
        ///     Gets the bitmap Image.
        /// </summary>
        /// <value>
        ///     The bitmap Image.
        /// </value>
        public BitmapImage BitmapImage => ConvertImage();

        /// <summary>
        ///     Gets the bits.
        /// </summary>
        public uint[] Bits { get; }


        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Sets the pixels from an enumerable source of pixel data.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        public void SetPixels(IEnumerable<PixelData> pixels)
        {
            _bitmap.Lock(); // Lock the bitmap for writing

            unsafe
            {
                // Get a pointer to the back buffer
                var dataPointer = (byte*)_bitmap.BackBuffer.ToPointer();

                foreach (var pixel in pixels)
                {
                    // Validate pixel bounds
                    if (pixel.X < 0 || pixel.X >= Width || pixel.Y < 0 || pixel.Y >= Height)
                        continue; // Skip invalid pixels

                    // Calculate the index in the back buffer
                    var pixelIndex = (pixel.Y * Width + pixel.X) * 4; // 4 bytes per pixel (BGRA)

                    // Set the pixel data in the back buffer
                    dataPointer[pixelIndex + 0] = pixel.B; // Blue
                    dataPointer[pixelIndex + 1] = pixel.G; // Green
                    dataPointer[pixelIndex + 2] = pixel.R; // Red
                    dataPointer[pixelIndex + 3] = pixel.A; // Alpha

                    // Store the pixel data as ARGB in the Bits array
                    Bits[pixel.Y * Width + pixel.X] = Bits[pixel.Y * Width + pixel.X] =
                        (uint)((pixel.A << 24) | (pixel.R << 16) | (pixel.G << 8) | pixel.B);
                    // This will be fine as long as A, R, G, B are 0-255
                }
            }

            // Mark the area of the bitmap that was changed
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
            _bitmap.Unlock(); // Unlock the bitmap after writing
        }

        /// <summary>
        ///     Converts the image.
        /// </summary>
        /// <returns>The BitmapImage from our WriteableBitmap.</returns>
        private BitmapImage ConvertImage()
        {
            // Create a new WriteableBitmap
            var bitmap = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgra32, null);

            // Create a byte array to hold the byte representation of the Bits
            var byteArray = new byte[Bits.Length * sizeof(uint)];

            // Fill the byte array with data from the Bits array
            Buffer.BlockCopy(Bits, 0, byteArray, 0, byteArray.Length);

            // Copy the byte array to the WriteableBitmap's back buffer
            bitmap.Lock();
            Marshal.Copy(byteArray, 0, bitmap.BackBuffer, byteArray.Length);
            bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
            bitmap.Unlock();

            // Create a new BitmapImage from the WriteableBitmap
            var bitmapImage = new BitmapImage();
            using var memoryStream = new MemoryStream();
            // Encode the WriteableBitmap as a PNG and write it to a MemoryStream
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memoryStream);

            // Reset the stream's position to the beginning
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Load the BitmapImage from the MemoryStream
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze(); // Make it immutable and thread-safe

            return bitmapImage;
        }

        /// <summary>
        ///     Applies the color matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void ApplyColorMatrix(float[][] matrix)
        {
            // Prepare an array to hold transformed pixel data
            var transformedPixels = new (int x, int y, Color color)[Bits.Length];

            for (var i = 0; i < Bits.Length; i++)
            {
                // Original color data
                var color = Bits[i];
                var a = (byte)((color >> 24) & 0xFF);
                var r = (byte)((color >> 16) & 0xFF);
                var g = (byte)((color >> 8) & 0xFF);
                var b = (byte)(color & 0xFF);

                // Color vector, ensure it is float
                var colorVector = new float[] { r, g, b, a };
                var result = new float[4];

                // Apply matrix transformation
                for (var j = 0; j < 4; j++)
                {
                    result[j] = 0; // Initialize to zero before summation
                    for (var k = 0; k < 4; k++) // Ensure we only sum over valid indices
                        result[j] += matrix[j][k] * colorVector[k];
                }

                // Clamp result to [0, 255] and convert to bytes
                var newColor = Color.FromArgb(
                    (byte)Math.Clamp(result[3], 0, 255),
                    (byte)Math.Clamp(result[0], 0, 255),
                    (byte)Math.Clamp(result[1], 0, 255),
                    (byte)Math.Clamp(result[2], 0, 255)
                );

                // Calculate x and y for the pixel
                var x = i % Width;
                var y = i / Width;

                // Store the transformed pixel color
                transformedPixels[i] = (x, y, newColor);
            }

            // Apply transformed pixels
            SetPixelsSimd(transformedPixels);
        }

        /// <summary>
        ///     Sets the pixels simd.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <exception cref="InvalidOperationException">ImagingResources.ErrorInvalidOperation</exception>
        public void SetPixelsSimd(IEnumerable<(int x, int y, Color color)> pixels)
        {
            var pixelArray = pixels.ToArray();
            var vectorCount = Vector<int>.Count;

            // Ensure Bits array is properly initialized
            if (Bits == null || Bits.Length < Width * Height)
                throw new InvalidOperationException(ImagingResources.ErrorInvalidOperation);

            for (var i = 0; i < pixelArray.Length; i += vectorCount)
            {
                var indices = new int[vectorCount];
                var colors = new int[vectorCount];

                // Load data into vectors
                for (var j = 0; j < vectorCount; j++)
                    if (i + j < pixelArray.Length)
                    {
                        var (x, y, color) = pixelArray[i + j];

                        // Check for valid pixel bounds
                        if (x >= 0 && x < Width && y >= 0 && y < Height)
                        {
                            indices[j] = x + y * Width;
                            colors[j] = (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
                        }
                        else
                        {
                            // Assign default values if the pixel is out of bounds
                            indices[j] = 0; // Or a suitable default index if required
                            colors[j] = 0; // Default color
                        }
                    }
                    else
                    {
                        // Handle cases where the remaining elements are less than vectorCount
                        indices[j] = 0; // Default index (can also be an invalid one)
                        colors[j] = 0; // Default color
                    }

                // Write data to Bits array
                for (var j = 0; j < vectorCount; j++)
                    // Write only valid indices
                    if (i + j < pixelArray.Length)
                        Bits[indices[j]] = (uint)colors[j];
            }
        }


        /// <summary>
        ///     Updates the bitmap from the Bits array.
        /// </summary>
        public void UpdateBitmapFromBits()
        {
            _bitmap.Lock();

            // Create a byte array to hold the byte representation of the Bits
            var byteArray = new byte[Bits.Length * sizeof(uint)];

            // Fill the byte array with data from the Bits array
            Buffer.BlockCopy(Bits, 0, byteArray, 0, byteArray.Length);

            // Copy the byte array to the WriteableBitmap's back buffer
            Marshal.Copy(byteArray, 0, _bitmap.BackBuffer, byteArray.Length);
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
            _bitmap.Unlock();
        }

        /// <summary>
        ///     Releases unmanaged and managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
                // Free the GCHandle if allocated
                if (_bitsHandle.IsAllocated)
                    _bitsHandle.Free();

            _disposed = true;
        }

        /// <summary>
        ///     Finalizes the instance.
        /// </summary>
        ~DirectBitmapImage()
        {
            Dispose(false);
        }
    }
}