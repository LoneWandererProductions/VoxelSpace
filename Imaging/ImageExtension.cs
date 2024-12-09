using System.Drawing;
using System.Windows.Media.Imaging;

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
    }
}