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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Imaging
{
    /// <inheritdoc />
    /// <summary>
    ///     Simple elegant Solution to get Color of an pixel, for more information look into Source.
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    public sealed class DirectBitmap : IDisposable
    {
        /// <summary>
        ///     Gets the bits.
        /// </summary>
        /// <value>
        ///     The bits.
        /// </value>
        private readonly int[] _bits;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectBitmap" /> class.
        ///     Bitmap which references pixel data directly
        ///     PixelFormat, Specifies the format of the color data for each pixel in the image.
        ///     AddrOfPinnedObject, reference to address of pinned object
        ///     GCHandleType, Retrieves the address of object data in a Pinned handle.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            _bits = new int[width * height];
            BitsHandle = GCHandle.Alloc(_bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb,
                BitsHandle.AddrOfPinnedObject());
        }

        /// <summary>
        ///     Gets the bitmap.
        /// </summary>
        /// <value>
        ///     The bitmap.
        /// </value>
        public Bitmap Bitmap { get; }

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
        private GCHandle BitsHandle { get; }

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
        /// Draws a vertical line with a specified color.
        /// For now Microsoft's Rectangle Method is faster in certain circumstances
        /// </summary>
        /// <param name="x">The x Coordinate.</param>
        /// <param name="y">The y Coordinate.</param>
        /// <param name="height">The height.</param>
        /// <param name="color">The color.</param>
        public void DrawVerticalLine(int x, int y, int height, Color color)
        {
            for (int i = y; i < height; i++)
                SetPixel(x, i, color);
        }

        /// <summary>
        /// Draws a horizontal line with a specified color.
        /// For now Microsoft's Rectangle Method is faster in certain circumstances
        /// /// </summary>
        /// <param name="x">The x Coordinate.</param>
        /// <param name="y">The y Coordinate.</param>
        /// <param name="length">The length.</param>
        /// <param name="color">The color.</param>
        public void DrawHorizontalLine(int x, int y, int length, Color color)
        {
            for (int i = x; i < length; i++)
                SetPixel(i, y, color);
        }

        /// <summary>
        /// Draws the rectangle.
        /// For now Microsoft's Rectangle Method is faster
        /// </summary>
        /// <param name="x">The x Coordinate.</param>
        /// <param name="y">The y Coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="color">The color.</param>
        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            if(width > height)
                Parallel.For(x, height,
                    index => DrawVerticalLine(index, y, width, color));
            else
                Parallel.For(y, width,
                    index => DrawHorizontalLine(x, index, height, color));
        }

        /// <summary>
        ///     Sets the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        public void SetPixel(int x, int y, Color color)
        {
            var index = x + (y * Width);
            _bits[index] = color.ToArgb();
        }

        /// <summary>
        ///     Gets the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Color of the Pixel</returns>
        public Color GetPixel(int x, int y)
        {
            var index = x + (y * Width);
            var col = _bits[index];
            return Color.FromArgb(col);
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
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                // free managed resources
                Bitmap.Dispose();
                BitsHandle.Free();
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
}
