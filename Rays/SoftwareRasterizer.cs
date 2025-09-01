using Rays;
using System;
using System.Drawing;

namespace RenderEngine;

/// <summary>
/// Software rasterizer using <see cref="UnmanagedImageBuffer"/> for fast pixel rendering.
/// </summary>
public class SoftwareRasterizer : IRenderer , IDisposable
{
    private readonly UnmanagedImageBuffer _buffer;

    private UnmanagedImageBuffer _imageBuffer;

    public int Width => _buffer.Width;
    public int Height => _buffer.Height;

    /// <summary>
    /// Creates a new rasterizer with a fixed canvas size.
    /// </summary>
    public SoftwareRasterizer(int width, int height)
    {
        _buffer = new UnmanagedImageBuffer(width, height);
        _imageBuffer = _buffer.Clone();
    }

    /// <summary>
    /// Clears the canvas to a single color.
    /// </summary>
    public void Clear(Color color)
    {
        _imageBuffer = _buffer.Clone();
        _imageBuffer.Clear(color.A, color.R, color.G, color.B);
    }

    public void DrawLine(Point p0, Point p1, Color color)
    {
        _imageBuffer.DrawLine(p0.X, p0.Y, p1.X, p1.Y, color.A, color.R, color.G, color.B);
    }

    public void DrawFilledRect(int x, int y, int width, int height, Color color)
    {
        _imageBuffer.FillRect(x, y, width, height, color.A, color.R, color.G, color.B);
    }

    public void DrawTexturedQuad(Point p0, Point p1, Point p2, Point p3, UnmanagedImageBuffer? texture = null)
    {
        _imageBuffer.DrawTexturedQuad(p0, p1, p2, p3, texture);
    }

    public void BlitRegion(UnmanagedImageBuffer src, int srcX, int srcY, int width, int height, int destX, int destY)
    {
        _imageBuffer.BlitRegion(src, srcX, srcY, width, height, destX, destY);
    }

    public void DrawSolidQuad(Point p0, Point p1, Point p2, Point p3, Color fill)
    {
        // CPU: Fill by splitting into 2 triangles or using scanline
        _imageBuffer.DrawFilledTriangle(p0, p1, p2, fill);
        _imageBuffer.DrawFilledTriangle(p0, p2, p3, fill);
    }

    /// <summary>
    /// Returns the current frame as a bitmap (slow, only for presenting on screen).
    /// </summary>
    public Bitmap GetFrame()
    {
        return _imageBuffer.ToBitmap();
    }

    public void Dispose()
    {
        _buffer.Dispose();
        _imageBuffer.Dispose();
    }

    public void DrawSprite(Point topLeft, UnmanagedImageBuffer sprite)
    {
        int srcW = sprite.Width;
        int srcH = sprite.Height;

        int dstW = this.Width;
        int dstH = this.Height;

        for (int sy = 0; sy < srcH; sy++)
        {
            int dy = topLeft.Y + sy;
            if (dy < 0 || dy >= dstH)
                continue;

            for (int sx = 0; sx < srcW; sx++)
            {
                int dx = topLeft.X + sx;
                if (dx < 0 || dx >= dstW)
                    continue;

                var c = sprite.GetPixel(sx, sy);
                if (c.A == 0)
                    continue; // skip fully transparent

                if (c.A == 255)
                {
                    // fast opaque set
                    _imageBuffer.SetPixel(dx, dy, c);
                }
                else
                {
                    // alpha blend with background
                    _imageBuffer.SetPixelAlphaBlend(dx, dy, c.A, c.R, c.G, c.B);
                }
            }
        }
    }

    public void DrawSprite(Point topLeft, UnmanagedImageBuffer sprite, bool opaqueFastPath)
    {
        if (!opaqueFastPath)
        {
            DrawSprite(topLeft, sprite);
            return;
        }

        int copyW = Math.Min(sprite.Width, Width - topLeft.X);
        int copyH = Math.Min(sprite.Height, Height - topLeft.Y);

        for (int y = 0; y < copyH; y++)
        {
            var srcRow = sprite.GetPixelSpan(0, y, copyW);
            var dstRow = _imageBuffer.GetPixelSpan(topLeft.X, topLeft.Y + y, copyW);
            srcRow.CopyTo(dstRow);
        }
    }
}
