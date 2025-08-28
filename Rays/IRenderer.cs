using System.Drawing;

namespace Rays
{
    /// <summary>
    /// Defines a generic 2D/3D renderer interface.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Clears the current frame with the given color.
        /// </summary>
        void Clear(Color color);

        /// <summary>
        /// Draw a line in screen space.
        /// </summary>
        void DrawLine(PointF p0, PointF p1, Color color);

        /// <summary>
        /// Draw a filled quad with optional wireframe outline.
        /// </summary>
        void DrawSolidQuad(PointF p0, PointF p1, PointF p2, PointF p3, Color fill);

        /// <summary>
        /// Draw a textured quad in screen space.
        /// </summary>
        void DrawTexturedQuad(PointF p0, PointF p1, PointF p2, PointF p3, Bitmap texture);

        /// <summary>
        /// Get the rendered frame as a Bitmap (optional for software rasterizers).
        /// </summary>
        Bitmap GetFrame();
    }
}
