/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageRender.cs
 * PURPOSE:     Interface that handles all Image Interactions
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace Imaging
{
    /// <inheritdoc />
    /// <summary>
    ///     The image render class.
    /// </summary>
    public sealed class ImageRender : IImageRender
    {
        /// <inheritdoc />
        /// <summary>
        ///     Get the bitmap file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     The Image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="IOException"></exception>
        public Bitmap GetBitmapFile(string path)
        {
            return ImageStream.GetBitmapFile(path);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the original bitmap file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The Image in Original size</returns>
        /// <exception cref="IOException">
        /// </exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public Bitmap GetOriginalBitmap(string path)
        {
            return ImageStream.GetOriginalBitmap(path);
        }

        /// <inheritdoc />
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
        public Bitmap BitmapScaling(Bitmap image, int width, int height)
        {
            return ImageStream.BitmapScaling(image, width, height);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Bitmaps the scaling.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="scaling">The scaling.</param>
        /// <returns>
        ///     A resized version of the original image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        public Bitmap BitmapScaling(Bitmap image, float scaling)
        {
            return ImageStream.BitmapScaling(image, scaling);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Converts an image to greyscale
        ///     Source:
        ///     https://web.archive.org/web/20110525014754/http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
        /// </summary>
        /// <param name="image">The image to // ReSharper disable UnusedType.Global</param>
        /// <param name="filter">The filter for the image</param>
        /// <returns>
        ///     A filtered version of the image
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        /// <exception cref="OutOfMemoryException">Memory Exceeded</exception>
        [return: MaybeNull]
        public Bitmap FilterImage(Bitmap image, ImageFilter filter)
        {
            return ImageStream.FilterImage(image, filter);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Combines the bitmaps overlays them and merges them into one Image
        ///     Can and will throw Exceptions as part of nameof(ToBitmapImage)
        ///     The size will be trimmed to the size of the first Image
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>
        ///     a new Bitmap with all combined Images
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        [return: MaybeNull]
        public Bitmap CombineBitmap(List<string> files)
        {
            return ImageStream.CombineBitmap(files);
        }

        /// <inheritdoc />
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
        public Bitmap CutBitmap(Bitmap image, int x, int y, int height, int width)
        {
            return ImageStream.CutBitmap(image, x, y, height, width);
        }

        /// <inheritdoc />
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
        public List<Bitmap> CutBitmaps(Bitmap image, int x, int y, int height, int width)
        {
            return ImageStream.CutBitmaps(image, x, y, height, width);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Loads File one Time
        ///     Can only used to load an Image Once
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <returns>An Image as <see cref="BitmapImage" />.</returns>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="IOException">Could not find the File</exception>
        public BitmapImage GetBitmapImage(string path)
        {
            return ImageStream.GetBitmapImage(path);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Loads File one Time
        ///     Can only used to load an Image Once
        ///     Use this for huge amounts of image that will be resized, else we will break memory limits
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>
        ///     An Image as <see cref="BitmapImage" />.
        /// </returns>
        /// <exception cref="IOException">Could not find the File</exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        public BitmapImage GetBitmapImage(string path, int width, int height)
        {
            return ImageStream.GetBitmapImage(path, width, height);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Loads File in a Stream
        ///     takes longer but can be changed on Runtime
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <returns>An Image as <see cref="BitmapImage" />.</returns>
        /// <exception cref="ArgumentException">No Correct Argument were provided</exception>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        /// <exception cref="IOException">Error while we try to access the File</exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="IOException">Could not find the File</exception>
        public BitmapImage GetBitmapImageFileStream(string path)
        {
            return ImageStream.GetBitmapImageFileStream(path);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Loads File in a Stream
        ///     takes longer but can be changed on Runtime
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>
        ///     An Image as <see cref="BitmapImage" />.
        /// </returns>
        /// <exception cref="IOException">Error while we try to access the File</exception>
        /// <exception cref="ArgumentException">No Correct Argument were provided</exception>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="IOException">Error while we try to access the File</exception>
        public BitmapImage GetBitmapImageFileStream(string path, int width, int height)
        {
            return ImageStream.GetBitmapImageFileStream(path, width, height);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Converts to bitmap image.
        /// </summary>
        /// <param name="image">The bitmap.</param>
        /// The Image as
        /// <see cref="BitmapImage" />
        /// .
        /// <exception cref="ArgumentNullException"></exception>
        public BitmapImage BitmapToBitmapImage(Bitmap image)
        {
            return ImageStream.BitmapToBitmapImage(image);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Bitmaps the image2 bitmap.
        /// </summary>
        /// <param name="image">The bitmap image.</param>
        /// <returns>
        ///     The Image as <see cref="T:System.Drawing.Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Bitmap BitmapImageToBitmap(BitmapImage image)
        {
            return ImageStream.BitmapImageToBitmap(image);
        }

        /// <inheritdoc />
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
        public Bitmap RotateImage(Bitmap image, int degree)
        {
            return ImageStream.RotateImage(image, degree);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Crops the image.
        ///     Non Edges are defined as transparent
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Coped Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Bitmap CropImage(Bitmap image)
        {
            return ImageStream.CropImage(image);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Saves the bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="path">The path where we want to save it.</param>
        /// <param name="format">The Image format.</param>
        /// <exception cref="ArgumentNullException">Wrong parameters</exception>
        /// <exception cref="IOException">File already exists</exception>
        /// <exception cref="ExternalException">Errors with the Path</exception>
        public void SaveBitmap(Bitmap image, string path, ImageFormat format)
        {
            ImageStream.SaveBitmap(image, path, format);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Converts White to Transparent.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="threshold">The threshold when the color is still white.</param>
        /// <returns>The Transparent Image</returns>
        /// <exception cref="ArgumentNullException">Wrong parameters</exception>
        public Bitmap ConvertWhiteToTransparent(Bitmap image, int threshold)
        {
            return ImageStream.ConvertWhiteToTransparent(image, threshold);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The Color at the point
        /// </returns>
        /// <exception cref="ArgumentNullException">nameof(image)</exception>
        public Color GetPixel(Bitmap image, Point point)
        {
            return ImageStream.GetPixel(image, point);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>The Color at the Point</returns>
        /// <exception cref="ArgumentNullException">
        ///     nameof(image)
        ///     or
        ///     nameof(image)
        /// </exception>
        public Color GetPixel(Bitmap image, Point point, int radius)
        {
            return ImageStream.GetPixel(image, point, radius);
        }

        /// <inheritdoc />
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
        public Bitmap SetPixel(Bitmap image, Point point, Color color)
        {
            return ImageStream.SetPixel(image, point, color);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="color">The color.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>
        ///     The Changed Image
        /// </returns>
        /// <exception cref="ArgumentNullException">nameof(image)</exception>
        public Bitmap SetPixel(Bitmap image, Point point, Color color, int radius)
        {
            return ImageStream.SetPixel(image, point, color, radius);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Splits the GIF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of Images from gif</returns>
        /// <exception cref="IOException">Could not find the File</exception>
        public List<Bitmap> SplitGif(string path)
        {
            return ImageGifHandler.SplitGif(path);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Loads a gif.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of Images from gif as ImageSource</returns>
        /// <exception cref="IOException">Could not find the File</exception>
        public List<ImageSource> LoadGif(string path)
        {
            return ImageGifHandler.LoadGif(path);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="IImageRender" /> interface.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="target">The target.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CreateGif(string path, string target)
        {
            ImageGifHandler.CreateGif(path, target);
        }
    }
}
