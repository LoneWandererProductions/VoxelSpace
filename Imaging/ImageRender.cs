/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageRender.cs
 * PURPOSE:     Interface that handles all Image Interactions
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://www.csharphelper.com/howtos/howto_colorize2.html
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
using System.Threading.Tasks;
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
        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageRender" /> class.
        /// </summary>
        public ImageRender()
        {
            ImageSettings = ImageRegister.Instance; // Ensure singleton instance is available
        }

        /// <summary>
        ///     The image Settings
        /// </summary>
        /// <value>
        ///     The image settings.
        /// </value>
        public ImageRegister ImageSettings { get; set; }

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
            return ImageStream.LoadBitmapFromFile(path);
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
        public Bitmap FilterImage(Bitmap image, ImageFilters filter)
        {
            return ImageFilterStream.FilterImage(image, filter, ImageSettings);
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
        ///     Combines the bitmaps.
        /// </summary>
        /// <param name="original">The original image.</param>
        /// <param name="overlay">The overlay image.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>Combined Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [return: MaybeNull]
        public Bitmap CombineBitmap(Bitmap original, Bitmap overlay, int x, int y)
        {
            return ImageStream.CombineBitmap(original, overlay, x, y);
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
        ///     Cuts the bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="shapeParams">The shape parameters.</param>
        /// <param name="startPoint">The start point.</param>
        /// <returns>The selected Image area.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">shape - null</exception>
        public Bitmap CutBitmap(Bitmap image, int width, int height, MaskShape shape, object shapeParams = null,
            Point? startPoint = null)
        {
            var btm = ImageStream.CutBitmap(image, 0, 0, image.Height, image.Width);

            // If no start point is provided, default to (0, 0)
            var actualStartPoint = startPoint ?? new Point(0, 0);

            switch (shape)
            {
                case MaskShape.Rectangle:
                    return ImageMask.ApplyRectangleMask(btm, width, height, actualStartPoint);

                case MaskShape.Circle:
                    return ImageMask.ApplyCircleMask(btm, width, height, actualStartPoint);

                case MaskShape.Polygon:
                    return ImageMask.ApplyPolygonMask(btm, (Point[])shapeParams);

                default:
                    throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
            }
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
        ///     Erases the rectangle from an Image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>Original Image with the erased area</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Bitmap EraseRectangle(Bitmap image, int x, int y, int height, int width)
        {
            return ImageStream.EraseRectangle(image, x, y, height, width);
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
            return ImageStreamMedia.GetBitmapImage(path);
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
            return ImageStreamMedia.GetBitmapImage(path, width, height);
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
            return ImageStreamMedia.GetBitmapImageFileStream(path);
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
            return ImageStreamMedia.GetBitmapImageFileStream(path, width, height);
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
            return ImageStreamMedia.BitmapToBitmapImage(image);
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
            return ImageStreamMedia.BitmapImageToBitmap(image);
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
        ///     Pixelate the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="stepWidth">Width of the step.</param>
        /// <returns>
        ///     Pixelated Image
        /// </returns>
        public Bitmap Pixelate(Bitmap image, int stepWidth = 2)
        {
            return ImageFilterStream.Pixelate(image, stepWidth);
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
        ///     Floods the fill scan line stack.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="newColor">The new color.</param>
        /// <returns>Bitmap with filled area</returns>
        public Bitmap FloodFillScanLineStack(Bitmap image, int x, int y, Color newColor)
        {
            return ImageStream.FloodFillScanLineStack(image, x, y, newColor);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Adjusts the brightness.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="brightnessFactor">The brightness factor.</param>
        /// <returns>
        ///     The changed image as Bitmap
        /// </returns>
        public Bitmap AdjustBrightness(Bitmap image, float brightnessFactor)
        {
            return ImageStream.AdjustBrightness(image, brightnessFactor);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Combines two images by averaging their pixel values.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>
        ///     A bitmap resulting from the average of the two images, or null if an error occurs.
        /// </returns>
        public Bitmap AverageImages(Image imgOne, Image imgTwo)
        {
            return ImageOverlays.AverageImages(imgOne, imgTwo);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Combines two images by adding their pixel values.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>
        ///     A bitmap resulting from the addition of the two images, or null if an error occurs.
        /// </returns>
        public Bitmap AddImages(Image imgOne, Image imgTwo)
        {
            return ImageOverlays.AddImages(imgOne, imgTwo);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Combines two images by subtracting the pixel values of the first image from the second image.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>
        ///     A bitmap resulting from the subtraction of the two images, or null if an error occurs.
        /// </returns>
        public Bitmap SubtractImages(Image imgOne, Image imgTwo)
        {
            return ImageOverlays.SubtractImages(imgOne, imgTwo);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Combines two images by multiplying their pixel values.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>
        ///     A bitmap resulting from the multiplication of the two images, or null if an error occurs.
        /// </returns>
        public Bitmap MultiplyImages(Image imgOne, Image imgTwo)
        {
            return ImageOverlays.MultiplyImages(imgOne, imgTwo);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Cross-fades between two images based on the given factor.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <param name="factor">The blending factor (0.0 to 1.0).</param>
        /// <returns>
        ///     A bitmap resulting from the cross-fading of the two images, or null if an error occurs.
        /// </returns>
        public Bitmap CrossFadeImages(Image imgOne, Image imgTwo, float factor)
        {
            return ImageOverlays.CrossFadeImages(imgOne, imgTwo, factor);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Finds the minimum color values from two images.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>
        ///     A bitmap resulting from the minimum values of the two images, or null if an error occurs.
        /// </returns>
        public Bitmap MinImages(Image imgOne, Image imgTwo)
        {
            return ImageOverlays.MinImages(imgOne, imgTwo);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Finds the maximum color values from two images.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>
        ///     A bitmap resulting from the maximum values of the two images, or null if an error occurs.
        /// </returns>
        public Bitmap MaxImages(Image imgOne, Image imgTwo)
        {
            return ImageOverlays.MaxImages(imgOne, imgTwo);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Calculates the amplitude of the pixel values between two images.
        /// </summary>
        /// <param name="imgOne">The first image.</param>
        /// <param name="imgTwo">The second image.</param>
        /// <returns>
        ///     A bitmap resulting from the amplitude of the two images, or null if an error occurs.
        /// </returns>
        public Bitmap AmplitudeImages(Image imgOne, Image imgTwo)
        {
            return ImageOverlays.AmplitudeImages(imgOne, imgTwo);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Adjusts the hue.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="hueShift">The hue shift.</param>
        /// <returns>
        ///     Bitmap with adjusted Hue.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Bitmap AdjustHue(Bitmap image, double hueShift)
        {
            return ImageStreamHsv.AdjustHue(image, hueShift);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Adjusts the saturation.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="saturationFactor">The saturation factor.</param>
        /// <returns>
        ///     Bitmap with adjusted Saturation.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Bitmap AdjustSaturation(Bitmap image, double saturationFactor)
        {
            return ImageStreamHsv.AdjustSaturation(image, saturationFactor);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Adjusts the brightness.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="brightnessFactor">The brightness factor.</param>
        /// <returns>
        ///     Bitmap with adjusted brightness.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Bitmap AdjustBrightness(Bitmap image, double brightnessFactor)
        {
            return ImageStreamHsv.AdjustBrightness(image, brightnessFactor);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Applies the gamma correction.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="gamma">The gamma.</param>
        /// <returns>
        ///     Bitmap with adjusted Gamma.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Bitmap ApplyGammaCorrection(Bitmap image, double gamma)
        {
            return ImageStreamHsv.ApplyGammaCorrection(image, gamma);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Adjusts the color.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="sourceColor">Color of the source.</param>
        /// <param name="targetColor">Color of the target.</param>
        /// <returns>
        ///     Color adjusted Bitmap
        /// </returns>
        public Bitmap AdjustColor(Bitmap image, Color sourceColor, Color targetColor)
        {
            return ImageStream.AdjustColor(image, sourceColor, targetColor);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Splits the GIF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of Images from gif</returns>
        /// <exception cref="IOException">Could not find the File</exception>
        public async Task<List<Bitmap>> SplitGif(string path)
        {
            return await ImageGifHandler.SplitGifAsync(path);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Loads a gif.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of Images from gif as ImageSource</returns>
        /// <exception cref="IOException">Could not find the File</exception>
        public async Task<List<ImageSource>> LoadGifAsync(string path)
        {
            return await ImageGifHandler.LoadGif(path);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Creates a gif.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="target">The target.</param>
        public void CreateGif(string path, string target)
        {
            ImageGifHandler.CreateGif(path, target);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Creates a gif.
        /// </summary>
        /// <param name="path">The paths of the images.</param>
        /// <param name="target">The target File.</param>
        public void CreateGif(List<string> path, string target)
        {
            ImageGifHandler.CreateGif(path, target);
        }

        /// <inheritdoc />
        /// <summary>
        /// Creates a gif.
        /// </summary>
        /// <param name="images">List off bitmaps and timer data</param>
        /// <param name="target">The target File.</param>
        public void CreateGif(List<FrameInfo> images, string target)
        {
            ImageGifHandler.CreateGif(images, target);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Bitmaps to base64.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>
        ///     Image as string
        /// </returns>
        public string BitmapToBase64(Bitmap bitmap)
        {
            return ImageConverter.BitmapToBase64(bitmap);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Bitmaps the image to base64.
        /// </summary>
        /// <param name="bitmapImage">The bitmap image.</param>
        /// <returns>
        ///     Image as string
        /// </returns>
        public string BitmapImageToBase64(BitmapImage bitmapImage)
        {
            return ImageConverter.BitmapImageToBase64(bitmapImage);
        }
    }
}