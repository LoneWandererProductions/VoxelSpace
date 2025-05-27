/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageMask.cs
 * PURPOSE:     Helper class to handle some shape operations on an Image
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Drawing;

namespace Imaging
{
    /// <summary>
    ///     Handle all the possible selections on an Image
    /// </summary>
    internal static class ImageMask
    {
        /// <summary>
        ///     Applies the rectangle mask.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="startPoint">The starting point (top-left corner) of the rectangle, optional.</param>
        /// <returns>Rectangle Bitmap</returns>
        internal static Bitmap ApplyRectangleMask(Image bitmap, int width, int height, Point? startPoint = null)
        {
            // Default start point
            var actualStartPoint = startPoint ?? new Point(0, 0);

            // Create a new bitmap to work on
            var rectBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            // Use graphics to apply the mask
            using var g = Graphics.FromImage(rectBitmap);
            // Clear the background to transparent
            g.Clear(Color.Transparent);

            // Create a texture brush with the original bitmap
            using var brush = new TextureBrush(bitmap);
            // Fill a rectangle starting from the given start point
            g.FillRectangle(brush, new Rectangle(actualStartPoint.X, actualStartPoint.Y, width, height));

            return rectBitmap;
        }

        /// <summary>
        ///     Applies the circle mask.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startPoint">The start point, optional.</param>
        /// <returns>
        ///     Circle Bitmap
        /// </returns>
        internal static Bitmap ApplyCircleMask(Image bitmap, int width, int height, Point? startPoint = null)
        {
            // Default start point
            var actualStartPoint = startPoint ?? new Point(0, 0);

            var circleBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            using var g = Graphics.FromImage(circleBitmap);
            g.Clear(Color.Transparent);
            using var brush = new TextureBrush(bitmap);

            // Fill the ellipse starting at the specified start point
            g.FillEllipse(brush, actualStartPoint.X, actualStartPoint.Y, width, height);

            return circleBitmap;
        }

        /// <summary>
        ///     Applies the polygon mask.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="points">The points.</param>
        /// <returns>Polygon Bitmap</returns>
        internal static Bitmap ApplyPolygonMask(Image bitmap, Point[] points)
        {
            var polyBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            using var g = Graphics.FromImage(polyBitmap);
            g.Clear(Color.Transparent);
            using var brush = new TextureBrush(bitmap);
            g.FillPolygon(brush, points);

            return polyBitmap;
        }
    }
}