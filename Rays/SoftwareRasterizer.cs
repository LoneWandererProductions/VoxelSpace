using System.Drawing;
using System.Drawing.Imaging;

namespace Rays;

public class SoftwareRasterizer
{
    private readonly Graphics _gfx;
    private readonly int _height;
    private readonly Bitmap _target;
    private readonly int _width;

    public SoftwareRasterizer(int width, int height)
    {
        _width = width;
        _height = height;
        _target = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        _gfx = Graphics.FromImage(_target);
        _gfx.Clear(Color.Black);
    }

    /// <summary>
    ///     Get the rendered frame as a bitmap.
    /// </summary>
    public Bitmap GetFrame()
    {
        return _target;
    }

    /// <summary>
    ///     Clear the frame.
    /// </summary>
    public void Clear(Color color)
    {
        _gfx.Clear(color);
    }

    /// <summary>
    ///     Draw a line in screen space.
    /// </summary>
    public void DrawLine(PointF p0, PointF p1, Color color)
    {
        using var pen = new Pen(color, 1);
        _gfx.DrawLine(pen, p0, p1);
    }

    /// <summary>
    ///     Draw a filled quad with optional wireframe outline.
    /// </summary>
    public void DrawSolidQuad(PointF p0, PointF p1, PointF p2, PointF p3, Color fill)
    {
        PointF[] pts = { p0, p1, p2, p3 };
        using var brush = new SolidBrush(fill);
        _gfx.FillPolygon(brush, pts);

        using var pen = new Pen(Color.Black, 1);
        _gfx.DrawPolygon(pen, pts);
    }

    /// <summary>
    ///     Draw a textured quad in screen space.
    /// </summary>
    public void DrawTexturedQuad(PointF p0, PointF p1, PointF p2, PointF p3, Bitmap texture)
    {
        // Split quad into two triangles because GDI+ can only map parallelograms
        PointF[] tri1 = { p0, p1, p2 };
        PointF[] tri2 = { p2, p3, p0 };

        var rect = new RectangleF(0, 0, texture.Width, texture.Height);
        using var brush = new TextureBrush(texture, rect);

        _gfx.FillPolygon(brush, tri1);
        brush.ResetTransform();
        _gfx.FillPolygon(brush, tri2);
    }
}