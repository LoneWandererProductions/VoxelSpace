/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageExtension.cs
 * PURPOSE:     Image Extensions, I think that are helpful and should be there from the beginning
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Drawing;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace Imaging
{
    /// <summary>
    ///     Image Extension Methods
    /// </summary>
    public static class ImageExtension
    {
        /// <summary>
        ///     Extension Method
        ///     Converts Bitmap to BitmapImage.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <returns>
        ///     A BitmapImage
        /// </returns>
        public static BitmapImage ToBitmapImage(this Bitmap bmp)
        {
            return ImageStreamMedia.BitmapToBitmapImage(bmp);
        }

        /// <summary>
        ///     Extension Method
        ///     Converts Bitmap to BitmapImage.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <returns>
        ///     A BitmapImage
        /// </returns>
        public static Bitmap ToBitmap(this BitmapImage bmp)
        {
            return ImageStreamMedia.BitmapImageToBitmap(bmp);
        }

        /// <summary>
        ///     Extension Method
        ///     Converts Image to BitmapImage.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>
        ///     A BitmapImage
        /// </returns>
        public static BitmapImage ToBitmapImage(this Image image)
        {
            var bitmap = new Bitmap(image);
            return bitmap.ToBitmapImage();
        }

        /// <summary>
        ///     Converts to argb.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>Id of the color</returns>
        public static int ToArgb(this Color color)
        {
            // ARGB format: (Alpha << 24) | (Red << 16) | (Green << 8) | Blue
            return (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
        }
    }
}
