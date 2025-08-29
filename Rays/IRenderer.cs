using RenderEngine;
using System.Drawing;

namespace Rays
{
    /// <summary>
    /// Defines a generic 2D/3D renderer interface.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Gets the width of the raster canvas.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the raster canvas.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Clears the canvas to a single color.
        /// </summary>
        /// <param name="color">The color to clear the canvas with.</param>
        void Clear(Color color);

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="p0">Starting point.</param>
        /// <param name="p1">Ending point.</param>
        /// <param name="color">Line color.</param>
        void DrawLine(Point p0, Point p1, Color color);

        /// <summary>
        /// Draws a filled rectangle.
        /// </summary>
        /// <param name="x">Top-left X coordinate.</param>
        /// <param name="y">Top-left Y coordinate.</param>
        /// <param name="width">Rectangle width.</param>
        /// <param name="height">Rectangle height.</param>
        /// <param name="color">Fill color.</param>
        void DrawFilledRect(int x, int y, int width, int height, Color color);

        /// <summary>
        /// Draws a textured quad defined by four points.
        /// </summary>
        /// <param name="p0">Top-left point.</param>
        /// <param name="p1">Top-right point.</param>
        /// <param name="p2">Bottom-right point.</param>
        /// <param name="p3">Bottom-left point.</param>
        /// <param name="texture">Texture to map onto the quad.</param>
        void DrawTexturedQuad(Point p0, Point p1, Point p2, Point p3, UnmanagedImageBuffer? texture = null);

        /// <summary>
        /// Draws a filled quad defined by four points.
        /// </summary>
        void DrawSolidQuad(Point p0, Point p1, Point p2, Point p3, Color fill);

        /// <summary>
        /// Copies a rectangular region from a source buffer to the raster canvas.
        /// </summary>
        /// <param name="src">Source image buffer.</param>
        /// <param name="srcX">Source X coordinate.</param>
        /// <param name="srcY">Source Y coordinate.</param>
        /// <param name="width">Region width.</param>
        /// <param name="height">Region height.</param>
        /// <param name="destX">Destination X coordinate.</param>
        /// <param name="destY">Destination Y coordinate.</param>
        void BlitRegion(UnmanagedImageBuffer src, int srcX, int srcY, int width, int height, int destX, int destY);

        /// <summary>
        /// Returns the current frame as a <see cref="Bitmap"/>.
        /// </summary>
        /// <returns>A bitmap representing the current frame.</returns>
        Bitmap GetFrame();
    }
}
