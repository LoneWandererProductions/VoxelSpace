using System;
using System.Drawing;

namespace RenderEngine;

/// <summary>
/// Software rasterizer using <see cref="UnmanagedImageBuffer"/> for fast pixel rendering.
/// </summary>
public class CPURasterizer : IDisposable
{
    private readonly UnmanagedImageBuffer _buffer;

    public int Width => _buffer.Width;
    public int Height => _buffer.Height;

    /// <summary>
    /// Creates a new rasterizer with a fixed canvas size.
    /// </summary>
    public CPURasterizer(int width, int height)
    {
        _buffer = new UnmanagedImageBuffer(width, height);
    }

    /// <summary>
    /// Clears the canvas to a single color.
    /// </summary>
    public void Clear(Color color)
    {
        _buffer.Clear(color.A, color.R, color.G, color.B);
    }

    public void DrawLine(Point p0, Point p1, Color color)
    {
        _buffer.DrawLine(p0.X, p0.Y, p1.X, p1.Y, color.A, color.R, color.G, color.B);
    }

    public void DrawFilledRect(int x, int y, int width, int height, Color color)
    {
        _buffer.FillRect(x, y, width, height, color.A, color.R, color.G, color.B);
    }

    public void DrawTexturedQuad(Point p0, Point p1, Point p2, Point p3, UnmanagedImageBuffer texture)
    {
        _buffer.DrawTexturedQuad(p0, p1, p2, p3, texture);
    }

    public void BlitRegion(UnmanagedImageBuffer src, int srcX, int srcY, int width, int height, int destX, int destY)
    {
        _buffer.BlitRegion(src, srcX, srcY, width, height, destX, destY);
    }

    /// <summary>
    /// Returns the current frame as a bitmap (slow, only for presenting on screen).
    /// </summary>
    public Bitmap GetFrame()
    {
        return _buffer.ToBitmap();
    }

    public void Dispose()
    {
        _buffer.Dispose();
    }
}
