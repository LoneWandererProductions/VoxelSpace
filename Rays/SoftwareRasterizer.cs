using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace Rays
{
    public enum RenderMode
    {
        Wireframe,
        Solid,
        Textured
    }

    public class SoftwareRasterizer
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Bitmap _target;
        private readonly Graphics _gfx;

        public SoftwareRasterizer(int width, int height)
        {
            _width = width;
            _height = height;
            _target = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            _gfx = Graphics.FromImage(_target);
            _gfx.Clear(Color.Black);
        }

        /// <summary>
        /// Get the rendered frame as a bitmap.
        /// </summary>
        public Bitmap GetFrame() => _target;

        /// <summary>
        /// Converts from normalized device coordinates (−1..+1) to screen pixels.
        /// </summary>
        private PointF ToScreen(Vector3 v)
        {
            float x = (v.X + 1) * 0.5f * _width;
            float y = (1 - (v.Y + 1) * 0.5f) * _height;
            return new PointF(x, y);
        }

        /// <summary>
        /// Helper: Transforms a 3D vertex into screen space.
        /// </summary>
        private PointF TransformToScreen(Vector3 v, Matrix4x4 vp)
        {
            Vector4 hv = Vector4.Transform(new Vector4(v, 1), vp);
            if (hv.W <= 0) return new PointF(-9999, -9999); // sentinel for behind camera
            hv /= hv.W;
            return ToScreen(new Vector3(hv.X, hv.Y, hv.Z));
        }

        public void Clear(Color color) => _gfx.Clear(color);

        internal void DrawLine(Vector3 v0, Vector3 v1, Matrix4x4 vp, Color color)
        {
            var p0 = TransformToScreen(v0, vp);
            var p1 = TransformToScreen(v1, vp);
            if (p0.X < 0 || p1.X < 0) return; // skip if behind
            using var pen = new Pen(color, 1);
            _gfx.DrawLine(pen, p0, p1);
        }

        internal void DrawSolidQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Matrix4x4 vp, Color fallbackColor)
        {
            PointF[] pts =
            {
                TransformToScreen(v0, vp),
                TransformToScreen(v1, vp),
                TransformToScreen(v2, vp),
                TransformToScreen(v3, vp)
            };

            using var brush = new SolidBrush(fallbackColor);
            _gfx.FillPolygon(brush, pts);
            using var pen = new Pen(Color.Black, 1);
            _gfx.DrawPolygon(pen, pts);
        }

        /// <summary>
        /// Very naive affine texture mapper (no perspective correction yet).
        /// </summary>
        internal void DrawTexturedQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Matrix4x4 vp, Bitmap texture)
        {
            PointF[] dst =
            {
                TransformToScreen(v0, vp),
                TransformToScreen(v1, vp),
                TransformToScreen(v2, vp),
                TransformToScreen(v3, vp)
            };

            // GDI+ can only map parallelograms, so we split quad into two triangles.
            var rect = new RectangleF(0, 0, texture.Width, texture.Height);

            using var texBrush = new TextureBrush(texture, rect);

            // First triangle
            PointF[] tri1 = { dst[0], dst[1], dst[2] };
            _gfx.FillPolygon(texBrush, tri1);

            // Reset brush transform each time (TextureBrush reuses last transform).
            texBrush.ResetTransform();

            // Second triangle
            PointF[] tri2 = { dst[2], dst[3], dst[0] };
            _gfx.FillPolygon(texBrush, tri2);
        }
    }
}
