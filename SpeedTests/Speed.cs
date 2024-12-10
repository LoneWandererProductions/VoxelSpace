using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using Voxels;
using Imaging;

namespace SpeedTests
{
    [TestClass]
    public class Speed
    {
        private VoxelRaster _voxel;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var colorMapPath = Path.Combine(Directory.GetCurrentDirectory(), "Terrain", "C1W.png");
            var heightMapPath = Path.Combine(Directory.GetCurrentDirectory(), "Terrain", "D1.png");

            Assert.IsTrue(File.Exists(colorMapPath), "Color map file not found.");
            Assert.IsTrue(File.Exists(heightMapPath), "Height map file not found.");

            var colorMap = new Bitmap(Image.FromFile(colorMapPath));
            var heightMap = new Bitmap(Image.FromFile(heightMapPath));

            _voxel = new VoxelRaster(100, 100, 0, 100, 120, 120, 300, colorMap, heightMap);
        }

        /// <summary>
        /// Speeds the and result comparison tests.
        /// </summary>
        [TestMethod]
        public void SpeedAndResultComparisonTests()
        {
            var stopwatch = new Stopwatch();

            // Measure the performance of RenderDirect
            stopwatch.Start();
            var directBitmap = _voxel.RenderDirect();
            stopwatch.Stop();
            Trace.WriteLine($"Direct rendering time: {stopwatch.ElapsedMilliseconds} ms");

            Assert.IsNotNull(directBitmap, "Direct rendering produced a null Bitmap.");

            // Measure the performance of RenderWithContainer
            stopwatch.Restart();
            var containerBitmap = _voxel.RenderWithContainer();
            stopwatch.Stop();
            Trace.WriteLine($"Container rendering time: {stopwatch.ElapsedMilliseconds} ms");

            Assert.IsNotNull(containerBitmap, "Container rendering produced a null Bitmap.");

            // Compare the two images
            Assert.IsTrue(AreBitmapsEqual(directBitmap, containerBitmap), "The images rendered by the two methods are not identical.");
        }

        /// <summary>
        /// Checks the speed panorama.
        /// </summary>
        [TestMethod]
        public void CheckSpeedPanorama()
        {
            var stopwatch = new Stopwatch();
            // Measure the performance of RenderPanoramic Parallel
            stopwatch.Start();
            var bmp = _voxel.RenderPanoramic(30);
            stopwatch.Stop();
            Trace.WriteLine($"Panoramic rendering time: {stopwatch.ElapsedMilliseconds} ms");

            Assert.IsNotNull(bmp, "Panoramic produced a null Bitmap.");

            stopwatch.Start();
            var raster = _voxel.RenderPanoramicParallel(30);
            stopwatch.Stop();

            Trace.WriteLine($"Panoramic trask rendering time: {stopwatch.ElapsedMilliseconds} ms");

            Assert.IsNotNull(raster, "Panoramic trask produced a null Bitmap.");
        }

        /// <summary>
        /// Compares two Bitmaps pixel by pixel.
        /// </summary>
        /// <param name="bmp1">The first Bitmap.</param>
        /// <param name="bmp2">The second Bitmap.</param>
        /// <returns>True if the images are identical; otherwise, false.</returns>
        private bool AreBitmapsEqual(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height)
                return false;

            for (int x = 0; x < bmp1.Width; x++)
            {
                for (int y = 0; y < bmp1.Height; y++)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                        return false;
                }
            }

            return true;
        }
    }
}
