/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/FilterType.cs
 * PURPOSE:     Provide filters for certain areas
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Drawing;

namespace Imaging
{
    /// <summary>
    ///     Apply textures to certain areas
    /// </summary>
    internal static class FiltersAreas
    {
        /// <summary>
        ///     Generates the filter.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="imageSettings">The image settings.</param>
        /// <param name="shapeParams">The shape parameters.</param>
        /// <param name="startPoint">The optional starting point (top-left corner) of the rectangle. Defaults to (0, 0).</param>
        /// <returns>
        ///     Generates a filter for a certain area
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     filter - null
        ///     or
        ///     shape - null
        /// </exception>
        internal static Bitmap GenerateFilter(Bitmap image,
            int? width,
            int? height,
            FiltersType filter,
            MaskShape shape,
            ImageRegister imageSettings,
            object shapeParams = null,
            Point? startPoint = null)
        {
            // Validate input
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (filter == FiltersType.None)
            {
                return image; // No filtering required
            }

            // Default start point
            var actualStartPoint = startPoint ?? new Point(0, 0);

            // Determine dimensions
            var actualWidth = width ?? image.Width;
            var actualHeight = height ?? image.Height;

            // Apply filter
            var filterBitmap = FiltersStream.FilterImage(image, filter, imageSettings);

            // Apply mask
            filterBitmap = shape switch
            {
                MaskShape.Rectangle => ImageMask.ApplyRectangleMask(filterBitmap, actualWidth, actualHeight,
                    actualStartPoint),
                MaskShape.Circle => ImageMask.ApplyCircleMask(filterBitmap, actualWidth, actualHeight,
                    actualStartPoint),
                MaskShape.Polygon => shapeParams is Point[] points
                    ? ImageMask.ApplyPolygonMask(filterBitmap, points)
                    : throw new ArgumentException(ImagingResources.ErrorWithShapePolygon, nameof(shapeParams)),
                _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
            };

            // Combine original and filtered images
            return ImageStream.CombineBitmap(image, filterBitmap, 0, 0);
        }
    }
}
