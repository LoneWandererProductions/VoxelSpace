using NUnit.Framework;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;
using CommonViewer;

namespace BitmapComparisonTests
{
    [TestFixture]
    public class BitmapRenderingTests
    {
        private Bitmap _testBitmap;

        [SetUp]
        public void Setup()
        {
            // Load a test bitmap (adjust the path to your test image)
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Terrain", "Landscape.png");
            _testBitmap = new Bitmap(path);
        }

        [TearDown]
        public void Cleanup()
        {
            _testBitmap.Dispose();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CompareRenderingSpeeds()
        {
            // Measure time for Media.Image with conversion
            var mediaImageTime = MeasureMediaImageRendering();

            // Measure time for NativeBitmapDisplay
            var nativeDisplayTime = MeasureNativeBitmapRendering();

            Debug.WriteLine($"Media.Image Time: {mediaImageTime}ms");
            Debug.WriteLine($"NativeBitmapDisplay Time: {nativeDisplayTime}ms");

            Assert.IsTrue(nativeDisplayTime <= mediaImageTime, "NativeBitmapDisplay should be as fast or faster than Media.Image rendering.");
        }

        private long MeasureMediaImageRendering()
        {
            var stopwatch = Stopwatch.StartNew();

            // Convert Bitmap to BitmapSource
            BitmapSource bitmapSource = BitmapToBitmapSource(_testBitmap);

            // Simulate rendering in Media.Image
            // Normally, we'd add this to a visual tree in WPF, but here we're testing only the conversion/rendering logic.
            var imageControl = new System.Windows.Controls.Image { Source = bitmapSource };

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private long MeasureNativeBitmapRendering()
        {
            var stopwatch = Stopwatch.StartNew();

            // Simulate rendering in NativeBitmapDisplay
            var nativeControl = new NativeBitmapDisplay { Bitmap = _testBitmap };

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private static BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
    }
}
