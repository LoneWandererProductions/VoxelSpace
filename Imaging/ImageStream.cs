/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageStream.cs
 * PURPOSE:     Does all the leg work for the Image operations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCE:      https://lodev.org/cgtutor/floodfill.html
 *              https://www.csharphelper.com/howtos/howto_colorize2.html
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable MissingSpace

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ExtendedSystemObjects;
using Mathematics;

namespace Imaging
{
    /// <summary>
    ///     Loads a BitMapImage out of a specific path
    ///     Can Combine two Images and returns a new one
    /// </summary>
    public static class ImageStream
    {
        /// <summary>
        ///     Get the bitmap file.
        ///     Will  leak like crazy. Only use it if we load Icons or something.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     The Image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="IOException">File not Found</exception>
        internal static Bitmap LoadBitmapFromFile(string path)
        {
            ImageHelper.ValidateFilePath(path);

            // Load the bitmap from the file
            using var originalBitmap = new Bitmap(path);
            // Return a defensive copy
            return new Bitmap(path, true);
        }

        /// <summary>
        ///     Gets the original bitmap file.
        ///     Takes longer but produces a higher quality
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     The Image in Original size
        /// </returns>
        /// <exception cref="IOException">
        /// </exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        internal static Bitmap GetOriginalBitmap(string path)
        {
            ImageHelper.ValidateFilePath(path);

            try
            {
                using var flStream = new FileStream(path, FileMode.Open);
                // Original picture information
                var original = new Bitmap(flStream);

                var bmp = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppPArgb);

                using var graph = Graphics.FromImage(bmp);
                graph.Clear(Color.Transparent);
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graph.DrawImage(original, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0,
                    original.Width,
                    original.Height, GraphicsUnit.Pixel);

                return bmp;
            }
            catch (Exception ex)
            {
                ImageHelper.HandleException(ex);
                // Optionally, rethrow or handle further as needed
                throw; // This will preserve the original stack trace and exception details
            }
        }

        /// <summary>
        ///     Resizes an image
        /// </summary>
        /// <param name="image">The image to resize</param>
        /// <param name="width">The new width in pixels</param>
        /// <param name="height">The new height in pixels</param>
        /// <returns>
        ///     A resized version of the original image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="InsufficientMemoryException"></exception>
        internal static Bitmap BitmapScaling(Bitmap image, int width, int height)
        {
            ImageHelper.ValidateImage(nameof(BitmapScaling), image);

            var btm = new Bitmap(width, height);
            //fix Resolution
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            try
            {
                using var graph = Graphics.FromImage(btm);
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graph.DrawImage(image, 0, 0, width, height);
                return new Bitmap(btm);
            }

            catch (InsufficientMemoryException ex)
            {
                Trace.WriteLine(ex);
                throw new InsufficientMemoryException(ex.Message);
            }
            catch (OutOfMemoryException ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
            catch (Exception ex)
            {
                ImageHelper.HandleException(ex);
                // Optionally, rethrow or handle further as needed
                throw; // This will preserve the original stack trace and exception details
            }
        }

        /// <summary>
        ///     Bitmaps the scaling.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="scaling">The scaling.</param>
        /// <returns>
        ///     A resized version of the original image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        internal static Bitmap BitmapScaling(Bitmap image, float scaling)
        {
            ImageHelper.ValidateImage(nameof(BitmapScaling), image);

            var width = (int)(image.Width * scaling);
            var height = (int)(image.Height * scaling);

            //needed because of: A Graphics object cannot be created from an image that has an indexed pixel format
            var btm = new Bitmap(width, height);
            //fix Resolution
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graph = Graphics.FromImage(btm))
            {
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var rect = new Rectangle(0, 0, width, height);

                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graph.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }

            return new Bitmap(btm);
        }

        /// <summary>
        ///     Combines the bitmaps overlays them and merges them into one Image
        ///     Can and will throw Exceptions as part of nameof(ToBitmapImage)
        ///     The size will be trimmed to the size of the first Image
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>
        ///     a new Bitmap with all combined Images
        /// </returns>
        /// <exception cref="ArgumentNullException">if file is null or Empty</exception>
        [return: MaybeNull]
        internal static Bitmap CombineBitmap(List<string> files)
        {
            if (files.IsNullOrEmpty())
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(CombineBitmap), ImagingResources.Spacing,
                        nameof(files)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //read all images into memory
            var images = new List<Bitmap>();

            foreach (var image in files)
            {
                //create a Bitmap from the file and add it to the list
                if (!File.Exists(image))
                {
                    Trace.WriteLine(string.Concat(ImagingResources.ErrorFileNotFound, image));
                    continue;
                }

                var bitmap = new Bitmap(image);

                images.Add(bitmap);
            }

            if (images.IsNullOrEmpty())
            {
                Trace.WriteLine(ImagingResources.ErrorFileNotFound);
                return null;
            }

            //get the correct size of the Final Image
            var bmp = images[0];

            //create a bitmap to hold the combined image
            var btm = new Bitmap(bmp.Width, bmp.Height);

            //get a graphics object from the image so we can draw on it
            using (var graph = Graphics.FromImage(btm))
            {
                //go through each image and draw it on the final image
                foreach (var image in images)
                {
                    graph.DrawImage(image,
                        new Rectangle(0, 0, image.Width, image.Height));
                }
            }

            foreach (var image in images)
            {
                image.Dispose();
            }

            //before return please Convert
            return btm;
        }

        /// <summary>
        ///     Combines the bitmaps.
        /// </summary>
        /// <param name="image">The original image.</param>
        /// <param name="overlay">The overlay image.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>Combined Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Bitmap CombineBitmap(Bitmap image, Bitmap overlay, int x, int y)
        {
            ImageHelper.ValidateImage(nameof(CombineBitmap), image);

            if (overlay == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(CombineBitmap),
                    ImagingResources.Spacing, nameof(overlay)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //get a graphics object from the image so we can draw on it
            using var graph = Graphics.FromImage(image);

            graph.DrawImage(overlay,
                new Rectangle(x, y, overlay.Width, overlay.Height));

            return image;
        }

        /// <summary>
        ///     Cuts a piece out of a bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>The cut Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Bitmap CutBitmap(Bitmap image, int x, int y, int height, int width)
        {
            ImageHelper.ValidateImage(nameof(CutBitmap), image);

            var btm = new Bitmap(width, height);

            using var graph = Graphics.FromImage(btm);
            graph.CompositingMode = CompositingMode.SourceCopy;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graph.Clear(Color.White);
            var rect = new Rectangle(x, y, width, height);

            graph.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);

            return btm;
        }

        /// <summary>
        ///     Cuts a bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>List of cut Images</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static List<Bitmap> CutBitmaps(Bitmap image, int x, int y, int height, int width)
        {
            ImageHelper.ValidateImage(nameof(CutBitmaps), image);

            //read all images into memory
            var images = new List<Bitmap>(x * y);

            for (var j = 0; j < y; j++)
            for (var i = 0; i < x; i++)
            {
                var img = CutBitmap(image, i * width, j * height, height, width);
                images.Add(img);
            }

            return images;
        }

        /// <summary>
        ///     Erases the rectangle from an Image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>Original Image with the erased area</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Bitmap EraseRectangle(Bitmap image, int x, int y, int height, int width)
        {
            ImageHelper.ValidateImage(nameof(EraseRectangle), image);

            using var graph = Graphics.FromImage(image);

            graph.CompositingMode = CompositingMode.SourceCopy;

            using var br = new SolidBrush(Color.FromArgb(0, 255, 255, 255));

            graph.FillRectangle(br, new Rectangle(x, y, width, height));

            return image;
        }

        /// <summary>
        ///     Rotates the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="degree">The angle in degrees</param>
        /// <returns>
        ///     Rotated Image
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        /// <exception cref="OverflowException">Degrees have a certain allowed radius</exception>
        internal static Bitmap RotateImage(Bitmap image, int degree)
        {
            ImageHelper.ValidateImage(nameof(RotateImage), image);

            //no need to do anything
            if (degree is 360 or 0)
            {
                return image;
            }

            if (degree is > 360 or < -360)
            {
                var innerException = new OverflowException(nameof(degree));
                throw new OverflowException(string.Concat(ImagingResources.ErrorWrongParameters, degree),
                    innerException);
            }

            //Calculate the size of the new Bitmap because if we rotate the Image it will be bigger
            var wOver = image.Width / 2.0f;
            var hOver = image.Height / 2.0f;

            // Get the coordinates of the corners, taking the origin to be the centre of the bitmap.
            PointF[] corners = { new(-wOver, -hOver), new(+wOver, -hOver), new(+wOver, +hOver), new(-wOver, +hOver) };

            for (var i = 0; i < 4; i++)
            {
                var point = corners[i];
                corners[i] =
                    new PointF(
                        (float)((point.X * ExtendedMath.CalcCos(degree)) - (point.Y * ExtendedMath.CalcSin(degree))),
                        (float)((point.X * ExtendedMath.CalcSin(degree)) + (point.Y * ExtendedMath.CalcCos(degree))));
            }

            // Find the min and max x and y coordinates.
            var minX = corners[0].X;
            var maxX = minX;
            var minY = corners[0].Y;
            var maxY = minY;

            for (var i = 1; i < 4; i++)
            {
                var p = corners[i];
                minX = Math.Min(minX, p.X);
                maxX = Math.Max(maxX, p.X);
                minY = Math.Min(minY, p.Y);
                maxY = Math.Max(maxY, p.Y);
            }

            // Get the size of the new bitmap.
            var newSize = new SizeF(maxX - minX, maxY - minY);
            // create it.
            var btm = new Bitmap((int)Math.Ceiling(newSize.Width), (int)Math.Ceiling(newSize.Height));
            //fix Resolution
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //  draw the old bitmap on it.
            using var graph = Graphics.FromImage(btm);
            graph.TranslateTransform(newSize.Width / 2.0f, newSize.Height / 2.0f);
            graph.RotateTransform(degree);
            graph.TranslateTransform(-image.Width / 2.0f, -image.Height / 2.0f);
            graph.DrawImage(image, 0, 0);

            return btm;
        }

        /// <summary>
        ///     Crops the image.
        ///     Non Edges are defined as transparent
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Coped Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Bitmap CropImage(Bitmap image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var bounds = ImageHelper.GetNonTransparentBounds(image);

            if (bounds.Width <= 0 || bounds.Height <= 0)
                // Return an empty image or handle this case as needed
            {
                return new Bitmap(1, 1);
            }

            var croppedBitmap = new Bitmap(bounds.Width, bounds.Height);
            using var graphics = Graphics.FromImage(croppedBitmap);
            graphics.DrawImage(image, 0, 0, bounds, GraphicsUnit.Pixel);

            return croppedBitmap;
        }

        /// <summary>
        ///     Saves the bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="path">The path where we want to save it.</param>
        /// <param name="format">The Image format.</param>
        /// <exception cref="ArgumentNullException">Wrong parameters</exception>
        /// <exception cref="IOException">File already exists</exception>
        /// <exception cref="ExternalException">Errors with the Path</exception>
        internal static void SaveBitmap(Bitmap image, string path, ImageFormat format)
        {
            ImageHelper.ValidateImage(nameof(SaveBitmap), image);

            if (format == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(SaveBitmap), ImagingResources.Spacing,
                        nameof(format)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (string.IsNullOrEmpty(path))
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(SaveBitmap), ImagingResources.Spacing,
                        nameof(path)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (File.Exists(path))
            {
                var count = 1;

                var fileNameOnly = Path.GetFileNameWithoutExtension(path);
                var extension = Path.GetExtension(path);
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    return;
                }

                var newPath = path;

                while (File.Exists(newPath))
                {
                    var tempFileName = $"{fileNameOnly}({count++})";
                    newPath = Path.Combine(directory!, tempFileName + extension);
                }

                SaveBitmap(image, newPath, format);
            }

            try
            {
                image.Save(path, format);
            }
            catch (ExternalException ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        ///     Converts White to Transparent.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="threshold">The threshold when the color is still white.</param>
        /// <returns>The Transparent Image</returns>
        /// <exception cref="ArgumentNullException">Wrong parameters</exception>
        internal static Bitmap ConvertWhiteToTransparent(Bitmap image, int threshold)
        {
            ImageHelper.ValidateImage(nameof(ConvertWhiteToTransparent), image);

            //use our new Format
            var result = DirectBitmap.GetInstance(image);

            //255,255,255 is White
            var replacementColor = Color.FromArgb(255, 255, 255);
            var pixelsToSet = new List<(int x, int y, Color color)>();

            for (var x = 0; x < result.Width; x++)
            for (var y = 0; y < result.Height; y++)
            {
                var color = result.GetPixel(x, y);

                //not in the area? continue, 255 is White
                if (255 - color.R >= threshold || 255 - color.G >= threshold || 255 - color.B >= threshold)
                {
                    continue;
                }

                //replace Value under the threshold with pure White
                pixelsToSet.Add((x, y, replacementColor));
            }

            try
            {
                result.SetPixelsSimd(pixelsToSet);

                //get the Bitmap
                var btm = new Bitmap(result.Bitmap);
                //make Transparent
                btm.MakeTransparent(replacementColor);
                //cleanup
                result.Dispose();
                return btm;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{ImagingResources.ErrorPixel} {ex.Message}");
                return null;
            }
        }

        /// <summary>
        ///     Gets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The Color at the point
        /// </returns>
        /// <exception cref="ArgumentNullException">Image was null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Point was out of bound.</exception>
        internal static Color GetPixel(Bitmap image, Point point)
        {
            ImageHelper.ValidateImage(nameof(GetPixel), image);

            if (point.X < 0 || point.X >= image.Width || point.Y < 0 || point.Y >= image.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(point), ImagingResources.ErrorOutOfBounds);
            }

            //use our new Format
            var dbm = DirectBitmap.GetInstance(image);
            return dbm.GetPixel(point.X, point.Y);
        }

        /// <summary>
        ///     Gets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>
        ///     The Color at the Point
        /// </returns>
        /// <exception cref="ArgumentNullException">image was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     radius or point is out of bounds.
        /// </exception>
        internal static Color GetPixel(Bitmap image, Point point, int radius)
        {
            ImageHelper.ValidateImage(nameof(GetPixel), image);

            if (radius < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), ImagingResources.ErrorRadius);
            }

            if (point.X < 0 || point.X >= image.Width || point.Y < 0 || point.Y >= image.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(point), ImagingResources.ErrorOutOfBounds);
            }

            var points = ImageHelper.GetCirclePoints(point, radius, image.Height, image.Width);

            if (points.Count == 0)
            {
                return GetPixel(image, point);
            }

            int redSum = 0, greenSum = 0, blueSum = 0;

            foreach (var color in points.Select(pointSingle => GetPixel(image, pointSingle)))
            {
                redSum += color.R;
                greenSum += color.G;
                blueSum += color.B;
            }

            return Color.FromArgb(redSum / points.Count, greenSum / points.Count, blueSum / points.Count);
        }

        /// <summary>
        ///     Sets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="color">The color.</param>
        /// <returns>
        ///     The changed image as Bitmap
        /// </returns>
        /// <exception cref="ArgumentNullException">nameof(image)</exception>
        internal static Bitmap SetPixel(Bitmap image, Point point, Color color)
        {
            ImageHelper.ValidateImage(nameof(SetPixel), image);

            //use our new Format
            var dbm = DirectBitmap.GetInstance(image);
            dbm.SetPixel(point.X, point.Y, color);

            return dbm.Bitmap;
        }

        /// <summary>
        ///     Adjusts the brightness.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="brightnessFactor">The brightness factor.</param>
        /// <returns>
        ///     The changed image as Bitmap
        /// </returns>
        internal static Bitmap AdjustBrightness(Bitmap image, float brightnessFactor)
        {
            ImageHelper.ValidateImage(nameof(GetPixel), image);

            var source = new DirectBitmap(image);
            var result = new DirectBitmap(source.Width, source.Height);

            for (var y = 0; y < source.Height; y++)
            for (var x = 0; x < source.Width; x++)
            {
                var pixelColor = source.GetPixel(x, y);

                // Adjust brightness by multiplying each color component by the brightness factor
                var newRed = ImageHelper.Clamp(pixelColor.R * brightnessFactor);
                var newGreen = ImageHelper.Clamp(pixelColor.G * brightnessFactor);
                var newBlue = ImageHelper.Clamp(pixelColor.B * brightnessFactor);

                result.SetPixel(x, y, Color.FromArgb(newRed, newGreen, newBlue));
            }

            return result.Bitmap;
        }

        /// <summary>
        ///     Sets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="color">The color.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>The Changed Image</returns>
        /// <exception cref="ArgumentNullException">nameof(image)</exception>
        internal static Bitmap SetPixel(Bitmap image, Point point, Color color, int radius)
        {
            ImageHelper.ValidateImage(nameof(SetPixel), image);

            var points = ImageHelper.GetCirclePoints(point, radius, image.Height, image.Width);

            return points.Aggregate(image, (current, pointSingle) => SetPixel(current, pointSingle, color));
        }

        /// <summary>
        ///     Fills the area with color.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="color">The color of the shape</param>
        /// <param name="shape">The shape.</param>
        /// <param name="shapeParams">The shape parameters.</param>
        /// <param name="startPoint">The optional starting point (top-left corner) of the rectangle. Defaults to (0, 0).</param>
        /// <returns>
        ///     Generates a filter for a certain area
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     filter - null
        ///     or
        ///     shape - null
        /// </exception>
        internal static Bitmap FillAreaWithColor(
            Bitmap image,
            int? width,
            int? height,
            Color color,
            MaskShape shape,
            object shapeParams = null,
            Point? startPoint = null)
        {
            // Validate input
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            // Default start point
            var actualStartPoint = startPoint ?? new Point(0, 0);

            // Determine dimensions
            var actualWidth = width ?? image.Width;
            var actualHeight = height ?? image.Height;

            using var g = Graphics.FromImage(image);
            using Brush brush = new SolidBrush(color);

            // Apply mask based on the specified shape
            switch (shape)
            {
                case MaskShape.Rectangle:
                    g.FillRectangle(brush, new Rectangle(actualStartPoint, new Size(actualWidth, actualHeight)));
                    break;

                case MaskShape.Circle:
                    g.FillEllipse(brush, new Rectangle(actualStartPoint, new Size(actualWidth, actualHeight)));
                    break;

                case MaskShape.Polygon:
                    if (shapeParams is Point[] points)
                    {
                        g.FillPolygon(brush, points);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid shape parameters for polygon mask.", nameof(shapeParams));
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(shape), shape, "Unsupported shape type");
            }

            return image;
        }

        /// <summary>
        ///     Floods the fill scan line stack.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="newColor">The new color.</param>
        /// <returns>Bitmap with filled area</returns>
        internal static Bitmap FloodFillScanLineStack(Bitmap image, int x, int y, Color newColor)
        {
            // Create a new bitmap to store the processed image
            var dbm = new DirectBitmap(image);
            var result = new DirectBitmap(image);

            var oldColor = dbm.GetPixel(x, y);
            if (oldColor == newColor)
            {
                return image; // Return original image if the color is the same
            }

            var pixelData = new List<(int x, int y, Color color)>();

            var stack = new Stack<(int, int)>();
            stack.Push((x, y));

            while (stack.Count > 0)
            {
                (x, y) = stack.Pop();
                var x1 = x;

                // Move to the left boundary
                while (x1 >= 0 && dbm.GetPixel(x1, y) == oldColor)
                {
                    x1--;
                }

                x1++;
                bool spanBelow;
                var spanAbove = spanBelow = false;

                // Move to the right boundary
                while (x1 < dbm.Width && dbm.GetPixel(x1, y) == oldColor)
                {
                    pixelData.Add((x1, y, newColor));

                    // Check above
                    if (!spanAbove && y > 0 && dbm.GetPixel(x1, y - 1) == oldColor)
                    {
                        stack.Push((x1, y - 1));
                        spanAbove = true;
                    }
                    else if (spanAbove && y > 0 && dbm.GetPixel(x1, y - 1) != oldColor)
                    {
                        spanAbove = false;
                    }

                    // Check below
                    if (!spanBelow && y < dbm.Height - 1 && dbm.GetPixel(x1, y + 1) == oldColor)
                    {
                        stack.Push((x1, y + 1));
                        spanBelow = true;
                    }
                    else if (spanBelow && y < dbm.Height - 1 && dbm.GetPixel(x1, y + 1) != oldColor)
                    {
                        spanBelow = false;
                    }

                    x1++;
                }
            }

            // Convert list to array for SIMD processing
            var pixelArray = pixelData.ToArray();
            result.SetPixelsSimd(pixelArray);
            pixelData.Clear();

            // Return the modified image as a Bitmap
            return result.Bitmap;
        }

        /// <summary>
        ///     Adjusts the color.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="sourceColor">Color of the source.</param>
        /// <param name="targetColor">Color of the target.</param>
        /// <returns>Color Adjusted Bitmap</returns>
        internal static Bitmap AdjustColor(Bitmap image, Color sourceColor, Color targetColor)
        {
            // Create a ColorMatrix that transforms sourceColor into targetColor
            var cm = CreateColorMatrix(sourceColor, targetColor);
            var attributes = new ImageAttributes();
            attributes.SetColorMatrix(cm);

            // Create a new image to hold the adjusted image.
            var result = new Bitmap(image.Width, image.Height);
            using var g = Graphics.FromImage(result);
            // Draw the original image on the new image using the color matrix.
            var rect = new Rectangle(0, 0, image.Width, image.Height);
            g.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

            return result;
        }

        /// <summary>
        ///     Creates the color matrix.
        /// </summary>
        /// <param name="sourceColor">Color of the source.</param>
        /// <param name="targetColor">Color of the target.</param>
        /// <returns>The color matrix</returns>
        private static ColorMatrix CreateColorMatrix(Color sourceColor, Color targetColor)
        {
            // Calculate the difference between source and target colors for each channel
            var rRatio = (targetColor.R / 255f) - (sourceColor.R / 255f);
            var gRatio = (targetColor.G / 255f) - (sourceColor.G / 255f);
            var bRatio = (targetColor.B / 255f) - (sourceColor.B / 255f);

            return new ColorMatrix(new[]
            {
                new[] { 1 + rRatio, gRatio, bRatio, 0, 0 }, // Red
                new[] { rRatio, 1 + gRatio, bRatio, 0, 0 }, // Green
                new[] { rRatio, gRatio, 1 + bRatio, 0, 0 }, // Blue
                new float[] { 0, 0, 0, 1, 0 }, // Alpha
                new float[] { 0, 0, 0, 0, 1 } // Translation
            });
        }
    }
}
