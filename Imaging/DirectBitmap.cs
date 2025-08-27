/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/DirectBitmap.cs
 * PURPOSE:     Custom Image Class, speeds up Get and Set Pixel
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * Sources:     https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.gchandle.addrofpinnedobject?view=net-7.0
 *              https://learn.microsoft.com/en-us/dotnet/api/system.drawing.imaging.pixelformat?view=dotnet-plat-ext-8.0
 *              https://learn.microsoft.com/en-us/dotnet/api/system.drawing.bitmap.-ctor?view=dotnet-plat-ext-8.0#system-drawing-bitmap-ctor(system-int32-system-int32-system-int32-system-drawing-imaging-pixelformat-system-intptr)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Imaging;

/// <inheritdoc />
/// <summary>
///     Simple elegant Solution to get Color of an pixel, for more information look into Source.
/// </summary>
/// <seealso cref="T:System.IDisposable" />
public sealed class DirectBitmap : IDisposable
{
    /// <summary>
    ///     The synchronize lock
    /// </summary>
    private readonly object _syncLock = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="DirectBitmap" /> class.
    ///     Bitmap which references pixel data directly
    ///     PixelFormat, Specifies the format of the color data for each pixel in the image.
    ///     AddrOfPinnedObject, reference to address of pinned object
    ///     GCHandleType, Retrieves the address of object data in a Pinned handle.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="color">The color. Totally optional.</param>
    public DirectBitmap(int width, int height, Color color = default)
    {
        Width = width;
        Height = height;
        Initiate(color);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DirectBitmap" /> class.
    ///     Bitmap which references pixel data directly
    ///     PixelFormat, Specifies the format of the color data for each pixel in the image.
    ///     AddrOfPinnedObject, reference to address of pinned object
    ///     GCHandleType, Retrieves the address of object data in a Pinned handle.
    /// </summary>
    /// <param name="btm">The in question.</param>
    public DirectBitmap(Image btm)
    {
        Width = btm.Width;
        Height = btm.Height;
        Initiate();

        using var graph = Graphics.FromImage(Bitmap);
        graph.DrawImage(btm, new Rectangle(0, 0, btm.Width, btm.Height), 0, 0, btm.Width, btm.Height,
            GraphicsUnit.Pixel);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DirectBitmap" /> class from a file path.
    ///     Loads an image from the specified file path and initializes the DirectBitmap instance.
    /// </summary>
    /// <param name="filePath">The file path to the image.</param>
    /// <exception cref="ArgumentException">Thrown if the file path is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
    /// <exception cref="Exception">Thrown if the file could not be loaded as an image.</exception>
    public DirectBitmap(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException(ImagingResources.ErrorPath, nameof(filePath));

        if (!File.Exists(filePath)) throw new FileNotFoundException(ImagingResources.ErrorFileNotFound, filePath);

        try
        {
            using var image = Image.FromFile(filePath);
            Width = image.Width;
            Height = image.Height;
            Initiate();

            using var graphics = Graphics.FromImage(Bitmap);
            graphics.DrawImage(image, new Rectangle(0, 0, Width, Height), 0, 0, Width, Height, GraphicsUnit.Pixel);
        }
        catch (Exception ex)
        {
            throw new Exception(ImagingResources.ErrorWrongParameters, ex);
        }
    }

    /// <summary>
    ///     Gets the bits.
    /// </summary>
    /// <value>
    ///     The bits.
    /// </value>
    public int[] Bits { get; private set; }

    /// <summary>
    ///     Gets the bitmap.
    /// </summary>
    /// <value>
    ///     The bitmap.
    /// </value>
    public Bitmap Bitmap { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="DirectBitmap" /> is disposed.
    /// </summary>
    /// <value>
    ///     <c>true</c> if disposed; otherwise, <c>false</c>.
    /// </value>
    private bool Disposed { get; set; }

    /// <summary>
    ///     Gets the height.
    /// </summary>
    /// <value>
    ///     The height.
    /// </value>
    public int Height { get; }

    /// <summary>
    ///     Gets the width.
    /// </summary>
    /// <value>
    ///     The width.
    /// </value>
    public int Width { get; }

    /// <summary>
    ///     Gets the bits handle.
    /// </summary>
    /// <value>
    ///     The bits handle.
    /// </value>
    private GCHandle BitsHandle { get; set; }

    /// <inheritdoc />
    /// <summary>
    ///     Free up all the Memory.
    ///     See:
    ///     https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1063?view=vs-2019
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Byte data for this instance.
    /// </summary>
    /// <returns>Image data as Bytes</returns>
    public byte[] Bytes()
    {
        lock (_syncLock)
        {
            if (Bits == null) return null;

            // The resulting byte array will have 4 bytes per int (32-bit)
            var byteArray = new byte[Bits.Length * 4];

            for (var i = 0; i < Bits.Length; i++)
            {
                var color = Bits[i];

                // Extract the ARGB components from the int and pack them as RGBA
                byteArray[i * 4 + 0] = (byte)((color >> 16) & 0xFF); // Red
                byteArray[i * 4 + 1] = (byte)((color >> 8) & 0xFF); // Green
                byteArray[i * 4 + 2] = (byte)(color & 0xFF); // Blue
                byteArray[i * 4 + 3] = (byte)((color >> 24) & 0xFF); // Alpha
            }

            return byteArray;
        }
    }

    /// <summary>
    ///     Initiates this instance and sets all Helper Variables.
    /// </summary>
    private void Initiate(Color color = default)
    {
        Bits = new int[Width * Height];
        BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
        Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb,
            BitsHandle.AddrOfPinnedObject());

        // Make the background transparent
        using var g = Graphics.FromImage(Bitmap);
        g.Clear(color);
    }

    /// <summary>
    ///     Gets the instance.
    /// </summary>
    /// <param name="btm">The custom Bitmap.</param>
    public static DirectBitmap GetInstance(Bitmap btm)
    {
        var dbm = new DirectBitmap(btm.Width, btm.Height);

        using var graph = Graphics.FromImage(dbm.Bitmap);
        graph.DrawImage(btm, new Rectangle(0, 0, btm.Width, btm.Height), 0, 0, btm.Width, btm.Height,
            GraphicsUnit.Pixel);

        return dbm;
    }

    /// <summary>
    ///     Draws a vertical line with a specified color.
    ///     For now Microsoft's Rectangle Method is faster in certain circumstances
    /// </summary>
    /// <param name="x">The x Coordinate.</param>
    /// <param name="y">The y Coordinate.</param>
    /// <param name="height">The height.</param>
    /// <param name="color">The color.</param>
    public void DrawVerticalLine(int x, int y, int height, Color color)
    {
        lock (_syncLock)
        {
            var colorArgb = color.ToArgb();
            for (var i = y; i < y + height && i < Height; i++) Bits[x + i * Width] = colorArgb;
        }
    }

    /// <summary>
    ///     Draws a horizontal line with a specified color.
    ///     For now Microsoft's Rectangle Method is faster in certain circumstances
    /// </summary>
    /// <param name="x">The x Coordinate.</param>
    /// <param name="y">The y Coordinate.</param>
    /// <param name="length">The length.</param>
    /// <param name="color">The color.</param>
    public void DrawHorizontalLine(int x, int y, int length, Color color)
    {
        lock (_syncLock)
        {
            if (y < 0 || y >= Height || length <= 0) return;

            var colorArgb = color.ToArgb();
            var vectorCount = Vector<int>.Count;
            var colorVector = new Vector<int>(colorArgb);

            var endX = Math.Min(x + length, Width);

            // Align start position to the next multiple of vectorCount if possible
            var startX = (x + vectorCount - 1) & ~(vectorCount - 1);

            // Fill initial non-aligned part
            for (var i = x; i < startX && i < endX; i++) Bits[i + y * Width] = colorArgb;

            // Fill aligned part with SIMD
            for (var xPos = startX; xPos + vectorCount <= endX; xPos += vectorCount)
                colorVector.CopyTo(Bits, xPos + y * Width);

            // Fill final non-aligned part
            for (var i = endX - vectorCount; i < endX; i++) Bits[i + y * Width] = colorArgb;
        }
    }

    /// <summary>
    ///     Draws the rectangle.
    ///     For now Microsoft's Rectangle Method is faster
    /// </summary>
    /// <param name="x1">The x Coordinate.</param>
    /// <param name="y2">The y Coordinate.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="color">The color.</param>
    public void DrawRectangle(int x1, int y2, int width, int height, Color color)
    {
        lock (_syncLock)
        {
            var colorArgb = color.ToArgb();
            var vectorCount = Vector<int>.Count;
            var colorVector = new Vector<int>(colorArgb);

            // Loop over the rectangle's rows.
            for (var y = y2; y < y2 + height && y < Height; y++)
                // Loop over the rectangle's columns with SIMD optimizations.
            for (var x = x1; x < x1 + width && x < Width; x += vectorCount)
            {
                var startIndex = x + y * Width;
                // Ensure we don't go out of bounds.
                if (startIndex + vectorCount <= Bits.Length)
                    colorVector.CopyTo(Bits, startIndex);
                else
                    // Handle remainder if not divisible by vectorCount
                    for (var j = 0; j < vectorCount && startIndex + j < Bits.Length; j++)
                        Bits[startIndex + j] = colorArgb;
            }
        }
    }

    /// <summary>
    ///     Sets the area.
    /// </summary>
    /// <param name="idList">The identifier list.</param>
    /// <param name="color">The color.</param>
    public void SetArea(IEnumerable<int> idList, Color color)
    {
        lock (_syncLock)
        {
            var colorArgb = color.ToArgb();
            var indices = idList.ToArray();
            var vectorCount = Vector<int>.Count;

            // Process the indices in chunks that fit within a Vector<int>
            for (var i = 0; i < indices.Length; i += vectorCount)
            {
                var chunkSize = Math.Min(vectorCount, indices.Length - i);

                // If the chunk size is equal to vectorCount, we can process the entire chunk in parallel using SIMD
                if (chunkSize == vectorCount)
                {
                    var indexVector = new Vector<int>(indices, i);

                    // Use SIMD to write the color to multiple positions in the Bits array at once
                    for (var j = 0; j < vectorCount; j++)
                    {
                        var index = indexVector[j];
                        if (index < Bits.Length) Bits[index] = colorArgb;
                    }
                }
                else
                {
                    // For the remainder of the indices, process them individually
                    for (var j = i; j < i + chunkSize; j++)
                    {
                        var index = indices[j];
                        if (index < Bits.Length) Bits[index] = colorArgb;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Sets the pixel.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="color">The color.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPixel(int x, int y, Color color)
    {
        lock (_syncLock)
        {
            var index = x + y * Width;
            Bits[index] = color.ToArgb();
        }
    }

    /// <summary>
    ///     Sets the pixels using SIMD for performance improvement.
    ///     Be careful when to return values. The Bitmap might be premature disposed.
    /// </summary>
    /// <param name="pixels">
    ///     An IEnumerable of pixels, each defined by x, y coordinates and a Color.
    /// </param>
    /// <summary>
    ///     Sets the pixels using SIMD for performance improvement.
    ///     Be careful when to return values. The Bitmap might be premature disposed.
    /// </summary>
    public void SetPixelsSimd(IEnumerable<(int x, int y, Color color)> pixels)
    {
        lock (_syncLock)
        {
            // Convert pixels to array for efficient indexing
            var pixelArray = pixels.ToArray();
            var vectorCount = Vector<int>.Count;

            // Ensure Bits array is properly initialized
            if (Bits == null || Bits.Length < Width * Height)
                throw new InvalidOperationException(ImagingResources.ErrorInvalidOperation);

            // Convert the Bits array to a Span for more efficient access
            var bitsSpan = new Span<int>(Bits);

            // Process pixels in blocks using Span
            for (var i = 0; i < pixelArray.Length; i += vectorCount)
            {
                // Create slices for indices and colors
                var indices = new int[vectorCount];
                var colors = new int[vectorCount];

                // Load data into vectors (use Span slicing)
                for (var j = 0; j < vectorCount; j++)
                    if (i + j < pixelArray.Length)
                    {
                        var (x, y, color) = pixelArray[i + j];
                        indices[j] = x + y * Width;
                        colors[j] = color.ToArgb();
                    }
                    else
                    {
                        // Handle cases where the remaining elements are less than vectorCount
                        indices[j] = 0;
                        colors[j] = Color.Transparent.ToArgb(); // Default color
                    }

                // Write data to Bits array via Span slice
                for (var j = 0; j < vectorCount; j++)
                    if (i + j < pixelArray.Length)
                        Bits[indices[j]] = colors[j];
            }
        }
    }

    /// <summary>
    ///     Draws the vertical lines.
    /// </summary>
    /// <param name="verticalLines">The vertical lines.</param>
    public void DrawVerticalLines(IEnumerable<(int x, int y, int finalY, Color color)> verticalLines)
    {
        _ = Parallel.ForEach(verticalLines, line =>
        {
            var (x, y, finalY, color) = line;
            var colorArgb = color.ToArgb();

            // Starting position in the Bits array
            var position = x + y * Width;

            // Calculate the number of rows in the vertical line
            var rowCount = finalY - y + 1;

            // Create a span over the Bits array
            var bitsSpan = new Span<int>(Bits);

            // Set the color for each pixel in the vertical line
            for (var i = 0; i < rowCount; i++) bitsSpan[position + i * Width] = colorArgb;
        });
    }

    /// <summary>
    ///     Gets the pixel.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>Color of the Pixel</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Color GetPixel(int x, int y)
    {
        var index = x + y * Width;
        var col = Bits[index];
        return Color.FromArgb(col);
    }

    /// <summary>
    ///     Gets the column of pixels at a given x-coordinate.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <returns>Array of Colors in the column.</returns>
    public Color[] GetColumn(int x)
    {
        var column = new Color[Height];
        Span<int> bitsSpan = Bits;

        for (var y = 0; y < Height; y++)
        {
            var index = x + y * Width;
            column[y] = Color.FromArgb(bitsSpan[index]);
        }

        return column;
    }

    /// <summary>
    ///     Gets the row of pixels at a given y-coordinate.
    /// </summary>
    /// <param name="y">The y-coordinate.</param>
    /// <returns>Array of Colors in the row.</returns>
    public Color[] GetRow(int y)
    {
        var bitsSpan = Bits.AsSpan(y * Width, Width);
        var row = new Color[Width];
        for (var i = 0; i < Width; i++) row[i] = Color.FromArgb(bitsSpan[i]);

        return row;
    }

    /// <summary>
    ///     Gets the color list.
    /// </summary>
    /// <returns>The Image as a list of Colors</returns>
    public Span<Color> GetColors()
    {
        if (Bits == null) return null;

        var length = Height * Width;
        var array = new Color[length];
        var span = new Span<Color>(array, 0, length);

        for (var i = 0; i < length; i++)
        {
            var col = Bits[i];
            span[i] = Color.FromArgb(col);
        }

        return span;
    }

    /// <summary>
    ///     Converts the Bits into bitmapImage.
    /// </summary>
    /// <returns>BitmapImage image data</returns>
    public BitmapImage ToBitmapImage()
    {
        // Ensure this method runs on the UI thread.
        BitmapImage bitmapImage = null;
        Application.Current.Dispatcher.Invoke(() =>
        {
            // Create a WriteableBitmap with the same dimensions as the DirectBitmap
            var writeableBitmap = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgra32, null);

            // Write the pixel data (from DirectBitmap's Bits) into the WriteableBitmap
            writeableBitmap.WritePixels(
                new Int32Rect(0, 0, Width, Height),
                Bits,
                Width * 4, // Each pixel has 4 bytes (BGRA)
                0);

            // Create a BitmapImage and set the WriteableBitmap's pixel data
            bitmapImage = new BitmapImage();
            using var stream = new MemoryStream();
            // Encode the WriteableBitmap to a MemoryStream
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(writeableBitmap));
            encoder.Save(stream);

            // Set the stream as the source for the BitmapImage
            stream.Seek(0, SeekOrigin.Begin);
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
        });

        return bitmapImage;
    }


    /// <summary>
    ///     Converts to string.
    /// </summary>
    /// <returns>
    ///     A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        var info = string.Empty;

        for (var i = 0; i < Bits.Length - 1; i++) info = string.Concat(info, Bits[i], ImagingResources.Indexer);

        return string.Concat(info, ImagingResources.Spacing, Bits[Bits.Length]);
    }

    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Height, Width);
    }

    /// <summary>
    ///     Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    ///     unmanaged resources.
    /// </param>
    private void Dispose(bool disposing)
    {
        if (Disposed) return;

        if (disposing)
        {
            // free managed resources
            Bitmap?.Dispose();

            // Free the GCHandle if it is allocated
            if (BitsHandle.IsAllocated) BitsHandle.Free();
        }

        Disposed = true;
    }

    /// <summary>
    ///     NOTE: Leave out the finalizer altogether if this class doesn't
    ///     own unmanaged resources, but leave the other methods
    ///     exactly as they are.
    ///     Finalizes an instance of the <see cref="DirectBitmap" /> class.
    /// </summary>
    ~DirectBitmap()
    {
        // Finalizer calls Dispose(false)
        Dispose(false);
    }
}