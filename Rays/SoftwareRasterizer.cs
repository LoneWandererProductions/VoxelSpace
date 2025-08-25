using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace Rays
{
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
        /// Draws a quad in either wireframe or solid mode.
        /// </summary>
        public void DrawQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Matrix4x4 vp, RenderMode mode, Color color)
        {
            // Transform vertices to clip space
            Vector3[] verts = { v0, v1, v2, v3 };
            PointF[] points = new PointF[4];

            for (int i = 0; i < verts.Length; i++)
            {
                Vector4 hv = Vector4.Transform(new Vector4(verts[i], 1), vp);

                if (hv.W <= 0) return; // behind camera → skip

                // Perspective divide
                hv /= hv.W;

                points[i] = ToScreen(new Vector3(hv.X, hv.Y, hv.Z));
            }

            if (mode == RenderMode.Wireframe)
            {
                using var pen = new Pen(color, 1);
                _gfx.DrawPolygon(pen, points);
            }
            else
            {
                using var brush = new SolidBrush(color);
                _gfx.FillPolygon(brush, points);
                using var pen = new Pen(Color.Black, 1);
                _gfx.DrawPolygon(pen, points);
            }
        }

        public void Clear(Color color)
        {
            _gfx.Clear(color);
        }
    }
}
