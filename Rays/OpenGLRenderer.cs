using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using RenderEngine; // For IRenderer

namespace Rays
{
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

        public int Width => _width;
        public int Height => _height;

        public OpenGLRenderer(int width, int height)
        {
            _width = width;
            _height = height;
            _frame = new Bitmap(width, height);

            // Initialize OpenGL context here if not already done
            // This depends on how you embed OpenTK in WPF
            InitGL();
        }

        private void InitGL()
        {
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Enable(EnableCap.DepthTest);
        }

        public void Clear(Color color)
        {
            GL.ClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void DrawLine(Point p0, Point p1, Color color)
        {
            // TODO: Implement OpenGL line drawing
            // Stub: convert to PointF if needed and draw GL.Lines
        }

        public void DrawFilledRect(int x, int y, int width, int height, Color color)
        {
            // Convert rectangle to quad points
            var p0 = new Point(x, y);
            var p1 = new Point(x + width, y);
            var p2 = new Point(x + width, y + height);
            var p3 = new Point(x, y + height);
            DrawSolidQuad(p0, p1, p2, p3, color);
        }

        private void DrawSolidQuad(Point p0, Point p1, Point p2, Point p3, Color fill)
        {
            // TODO: Implement OpenGL quad rendering
        }

        public void DrawTexturedQuad(Point p0, Point p1, Point p2, Point p3, UnmanagedImageBuffer texture)
        {
            // TODO: Implement OpenGL textured quad using texture data
        }

        public void BlitRegion(UnmanagedImageBuffer src, int srcX, int srcY, int width, int height, int destX, int destY)
        {
            // TODO: Convert source buffer to OpenGL texture and render a quad
        }

        public Bitmap GetFrame()
        {
            // TODO: Read pixels from GL framebuffer if needed
            return _frame;
        }

        public void Dispose()
        {
            // TODO: Dispose OpenGL resources if any
            _frame.Dispose();
        }

        void IRenderer.DrawSolidQuad(Point p0, Point p1, Point p2, Point p3, Color fill)
        {
            DrawSolidQuad(p0, p1, p2, p3, fill);
        }

        public void DrawSprite(Point topLeft, UnmanagedImageBuffer sprite)
        {
            // TODO: Implement OpenGL Draw Sprite
        }

        public void DrawSprite(Point topLeft, UnmanagedImageBuffer sprite, bool opaqueFastPath)
        {
            // TODO: Implement OpenGL Draw Sprite
        }
    }
}
