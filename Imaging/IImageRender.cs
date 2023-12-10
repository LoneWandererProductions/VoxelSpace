/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/IImageRender.cs
 * PURPOSE:     Image Interface for most Image Operations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Imaging
{
    /// <summary>
    ///     The IImageRender interface.
    /// </summary>
    internal interface IImageRender
    {
        /// <summary>
        ///     Get the bitmap file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     The Image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="IOException"></exception>
        Bitmap GetBitmapFile(string path);

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
        Bitmap GetOriginalBitmap(string path);

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
        Bitmap BitmapScaling(Bitmap image, int width, int height);

        /// <summary>
        ///     Bitmaps the scaling.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="scaling">The scaling.</param>
        /// <returns>
        ///     A resized version of the original image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        Bitmap BitmapScaling(Bitmap image, float scaling);

        /// <summary>
        ///     Converts an image to greyscale
        ///     Source:
        ///     https://web.archive.org/web/20110525014754/http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
        /// </summary>
        /// <param name="image">The image to filter</param>
        /// <param name="filter">The Image Filter</param>
        /// <returns>
        ///     A filtered version of the image
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        /// <exception cref="OutOfMemoryException">Memory Exceeded</exception>
        Bitmap FilterImage(Bitmap image, ImageFilter filter);

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
        Bitmap CombineBitmap(List<string> files);

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
        Bitmap CutBitmap(Bitmap image, int x, int y, int height, int width);

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
        List<Bitmap> CutBitmaps(Bitmap image, int x, int y, int height, int width);

        /// <summary>
        ///     Loads File one Time
        ///     Can only used to load an Image Once
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <returns>An Image as <see cref="BitmapImage" />.</returns>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="IOException">Could not find the File</exception>
        BitmapImage GetBitmapImage(string path);

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
        BitmapImage GetBitmapImage(string path, int width, int height);

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
        BitmapImage GetBitmapImageFileStream(string path);

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
        BitmapImage GetBitmapImageFileStream(string path, int width, int height);

        /// <summary>
        ///     Converts to bitmap image.
        /// </summary>
        /// <param name="image">The bitmap.</param>
        /// The Image as
        /// <see cref="BitmapImage" />
        /// .
        /// <exception cref="ArgumentNullException"></exception>
        BitmapImage BitmapToBitmapImage(Bitmap image);

        /// <summary>
        ///     Bitmaps the image  bitmap.
        /// </summary>
        /// <param name="image">The bitmap image.</param>
        /// <returns>
        ///     The Image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        Bitmap BitmapImageToBitmap(BitmapImage image);

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
        Bitmap RotateImage(Bitmap image, int degree);

        /// <summary>
        ///     Crops the image.
        ///     Non Edges are defined as transparent
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Cropped Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        Bitmap CropImage(Bitmap image);

        /// <summary>
        ///     Saves the bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="path">The path where we want to save it.</param>
        /// <param name="format">The Image format.</param>
        /// <exception cref="ArgumentNullException">Wrong parameters</exception>
        /// <exception cref="IOException">File already exists</exception>
        /// <exception cref="ExternalException">Errors with the Path</exception>
        void SaveBitmap(Bitmap image, string path, ImageFormat format);

        /// <summary>
        ///     Converts White to Transparent.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="threshold">The threshold when the color is still white.</param>
        /// <returns>The Transparent Image</returns>
        /// <exception cref="ArgumentNullException">Wrong parameters</exception>
        Bitmap ConvertWhiteToTransparent(Bitmap image, int threshold);

        /// <summary>
        ///     Gets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The Color at the point
        /// </returns>
        /// <exception cref="ArgumentNullException">nameof(image)</exception>
        Color GetPixel(Bitmap image, Point point);

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
        Color GetPixel(Bitmap image, Point point, int radius);

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
        Bitmap SetPixel(Bitmap image, Point point, Color color);

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
        Bitmap SetPixel(Bitmap image, Point point, Color color, int radius);

        /// <summary>
        ///     Splits the GIF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of Images from gif</returns>
        /// <exception cref="IOException">Could not find the File</exception>
        List<Bitmap> SplitGif(string path);

        /// <summary>
        ///     Loads the GIF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of Images from gif as ImageSource</returns>
        /// <exception cref="IOException">Could not find the File</exception>
        List<ImageSource> LoadGif(string path);

        /// <summary>
        ///     Initializes a new instance of the <see cref="IImageRender" /> interface.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="target">The target.</param>
        void CreateGif(string path, string target);
    }
}