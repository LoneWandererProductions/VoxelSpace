using System.Drawing;
using OpenTK.Graphics.OpenGL4;

namespace Rays;

/// <summary>
/// Minimal OpenGL renderer implementing IRenderer.
/// Swap this in for GPU rendering later.
/// </summary>
public class OpenGLRenderer : IRenderer
{
    private readonly int _width;
    private readonly int _height;

    /// <summary>
    /// Framebuffer bitmap placeholder (for WPF ImageView)
    /// </summary>
    private Bitmap _frame;

    public OpenGLRenderer(int width, int height)
    {
        _width = width;
        _height = height;
        _frame = new Bitmap(width, height);

        // Initialize OpenGL context here if not already done
        // This depends on how you embed OpenTK in WPF
        // For now, just a stub
        InitGL();
    }

    private void InitGL()
    {
        GL.ClearColor(0f, 0f, 0f, 1f);
        GL.Enable(EnableCap.DepthTest);
    }

    public Bitmap GetFrame()
    {
        // TODO: Read pixels from GL framebuffer if needed
        // For now, just return placeholder
        return _frame;
    }

    public void Clear(Color color)
    {
        GL.ClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void DrawLine(PointF p0, PointF p1, Color color)
    {
        // TODO: Implement OpenGL line drawing (or use GL.Lines)
        // Currently stub does nothing
    }

    public void DrawSolidQuad(PointF p0, PointF p1,
        PointF p2, PointF p3, Color fill)
    {
        // TODO: Implement OpenGL quad rendering
    }

    public void DrawTexturedQuad(PointF p0, PointF p1,
        PointF p2, PointF p3, Bitmap texture)
    {
        // TODO: Implement OpenGL textured quad
    }
}
