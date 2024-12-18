using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Voxels;
using PixelData = Voxels.PixelData;

namespace SpeedTests
{
    [TestClass]
    public class Speed
    {
        private VoxelRaster _raster;

        private PixelData[,] _rasterData;
        private VoxelRasterTest _voxel;

        private static readonly int[,] Map =
        {
                { 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1 }
        };

        /// <summary>
        ///     Initializes this instance.
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

            ProcessMaps(heightMap, colorMap);

            _voxel = new VoxelRasterTest(100, 100, 0, 100, 120, 120, 300, colorMap, heightMap);

            _raster = new VoxelRaster(100, 100, 0, 100, 120, 120, 300, colorMap, heightMap);
        }

        /// <summary>
        ///     Speeds the and result comparison tests.
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

            stopwatch.Restart();
            var depthBitmap = _voxel.RenderDepth();
            stopwatch.Stop();
            Trace.WriteLine($"Depth rendering time: {stopwatch.ElapsedMilliseconds} ms");

            Assert.IsNotNull(depthBitmap, "Container rendering produced a null Bitmap.");

            // Compare the two images
            //Assert.IsTrue(AreBitmapsEqual(depthBitmap, containerBitmap), "The images rendered by the two methods are not identical. (depth, container)");

            // Compare the two images (depth, direct) is not possible since they are different
        }

        /// <summary>
        /// Speeds the and result comparison raycast tests.
        /// </summary>
        [TestMethod]
        public void SpeedAndResultComparisonRaycastTests()
        {
            var stopwatch = new Stopwatch();

            // Define the map
            var map = new int[10, 10]
            {
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 1, 1, 1, 1, 1, 1, 0, 1},
                {1, 0, 1, 0, 0, 0, 0, 1, 0, 1},
                {1, 0, 1, 0, 1, 1, 1, 1, 0, 1},
                {1, 0, 1, 0, 1, 0, 0, 1, 0, 1},
                {1, 0, 1, 1, 1, 1, 0, 1, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
            };

            // Initialize and test Raycaster
            stopwatch.Start();
            var camera1 = new Camera { X = 3, Y = 3, Angle = 0, Height = 2, ScreenWidth = 300, ScreenHeight = 200, ZFar = 20 };
            var raycaster1 = new RasterRaycast(camera1, map);
            var bmp1 = raycaster1.Render();
            bmp1.Save("raycaster_output.png");
            stopwatch.Stop();
            Trace.WriteLine($"Raycaster rendering time: {stopwatch.ElapsedMilliseconds} ms");

            Assert.IsNotNull(bmp1, "Raycaster produced a null Bitmap.");

            // Initialize and test Raycaster2
            stopwatch.Restart();
            var camera2 = new Camera2(3.5, 3.5, 0);
            var raycaster2 = new Raycaster2(map);
            var bmp2 = raycaster2.RenderBitmap(camera2);
            bmp2.Save("raycaster2_output.png");
            stopwatch.Stop();
            Trace.WriteLine($"Raycaster2 rendering time: {stopwatch.ElapsedMilliseconds} ms");

            Assert.IsNotNull(bmp2, "Raycaster2 produced a null Bitmap.");

            // Initialize and test Raycaster3
            stopwatch.Restart();
            var camera3 = new Camera3(2.0, 2.0, 1.0, 0.0, 0.66, 0.0);
            var raycaster3 = new Raycaster3(map);
            var bmp3 = raycaster3.Render(camera3);
            bmp3.Save("raycaster3_output.png");
            stopwatch.Stop();
            Trace.WriteLine($"Raycaster3 rendering time: {stopwatch.ElapsedMilliseconds} ms");

            Assert.IsNotNull(bmp3, "Raycaster3 produced a null Bitmap.");

            // Initialize and test Raycaster4
            stopwatch.Restart();
            var camera4 = new Camera4(2.5, 2.5, 45); // Example position and angle
            var raycaster4 = new Raycaster4(map);
            var bmp4 = raycaster4.Render(camera4);
            bmp4.Save("raycaster4_output.png");
            stopwatch.Stop();
            Trace.WriteLine($"Raycaster4 rendering time: {stopwatch.ElapsedMilliseconds} ms");

            Assert.IsNotNull(bmp4, "Raycaster4 produced a null Bitmap.");

            // Initialize and test Raycaster5
            stopwatch.Restart();
            var camera5 = new Camera5(3.5, 3.5, 0) // Match position and angle with Camera2
            {
                FieldOfView = Math.PI / 2, // Match Camera2's field of view
                CellSize = 64 // Match Raycaster2's default cell size
            };

            var raycaster5 = new Raycaster5(map, camera5.CellSize);
            var bmp5 = raycaster5.RenderBitmap(camera5);
            bmp5.Save("raycaster5_output.png");
            stopwatch.Stop();
            Trace.WriteLine($"Raycaster5 rendering time: {stopwatch.ElapsedMilliseconds} ms");


            Assert.IsNotNull(bmp5, "Raycaster5 produced a null Bitmap.");

            // Initialize and test Raycaster5
            stopwatch.Restart();
            map = new int[,]
            {
                { 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1 }
            };

            int cellSize = 64;
            Raycaster6 raycaster = new(map, cellSize);

            Camera6 camera = new(96, 96, 60, 0); // Initial position and direction.

            Bitmap rendered = raycaster.Render(camera, 800, 600);
            rendered.Save("raycaster6_output.png");
            stopwatch.Stop();

            // Compare results
            //Assert.IsTrue(AreBitmapsEqual(bmp3, bmp4), "Raycaster and Raycaster4 produced different outputs.");
        }


        [TestMethod]
        public void ValidateRaycaster()
        {
             var map = new int[,]
             {
                    { 1, 1, 1, 1, 1 },
                    { 1, 0, 0, 0, 1 },
                    { 1, 0, 1, 0, 1 },
                    { 1, 0, 0, 0, 1 },
                    { 1, 1, 1, 1, 1 }
             };

            int cellSize = 64;

            Raycaster6 raycaster = new(map, cellSize);

            double startX = 96; // Camera position X.
            double startY = 96; // Camera position Y.
            double[] testAngles = { 0, 45, 90, 135, 180 }; // Test angles in degrees.

            foreach (var angle in testAngles)
            {
                double expected = Maths.CalculateExpectedDistance(startX, startY, angle, cellSize, 5, 5, map);
                double actual = raycaster.CastRay(startX, startY, Math.Cos(angle * Math.PI / 180.0), Math.Sin(angle * Math.PI / 180.0));

                Console.WriteLine($"Angle: {angle}° | Expected: {expected:F2} | Actual: {actual:F2}");
                Debug.Assert(Math.Abs(expected - actual) < 0.1, $"Mismatch at angle {angle}°");
            }
        }

        [TestMethod]
        public void Resultsets()
        {
            var stopwatch = new Stopwatch();

            // Define the map
            var map = new int[10, 10]
            {
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 1, 1, 1, 1, 1, 1, 0, 1},
                {1, 0, 1, 0, 0, 0, 0, 1, 0, 1},
                {1, 0, 1, 0, 1, 1, 1, 1, 0, 1},
                {1, 0, 1, 0, 1, 0, 0, 1, 0, 1},
                {1, 0, 1, 1, 1, 1, 0, 1, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
            };

            //1 Initialize and test Raycaster5
            stopwatch.Restart();
            var camera2 = new Camera2(5, 5, 0) // Match position and angle with Camera2
            {
                FieldOfView = Math.PI / 2, // Match Camera2's field of view
                CellSize = 64 // Match Raycaster2's default cell size
            };

            var raycaster2 = new Raycaster2(map);
            var bmp5 = raycaster2.RenderBitmap(camera2);
            bmp5.Save("raycaster5_1_output.png");
            stopwatch.Stop();
            Trace.WriteLine($"Raycaster5 rendering time: {stopwatch.ElapsedMilliseconds} ms");

            //2 Initialize and test Raycaster5
            stopwatch.Restart();
            camera2 = new Camera2(6, 6, 0) // Match position and angle with Camera2
            {
                FieldOfView = Math.PI / 2, // Match Camera2's field of view
                CellSize = 64 // Match Raycaster2's default cell size
            };

            raycaster2 = new Raycaster2(map);
            bmp5 = raycaster2.RenderBitmap(camera2);
            bmp5.Save("raycaster5_2_output.png");
            stopwatch.Stop();
            Trace.WriteLine($"Raycaster5 rendering time: {stopwatch.ElapsedMilliseconds} ms");

            //3 Initialize and test Raycaster5
            stopwatch.Restart();
            camera2 = new Camera2(7, 7, 0) // Match position and angle with Camera2
            {
                FieldOfView = Math.PI / 2, // Match Camera2's field of view
                CellSize = 64 // Match Raycaster2's default cell size
            };

            raycaster2 = new Raycaster2(map);
            bmp5 = raycaster2.RenderBitmap(camera2);
            bmp5.Save("raycaster5_3_output.png");
            stopwatch.Stop();
            Trace.WriteLine($"Raycaster5 rendering time: {stopwatch.ElapsedMilliseconds} ms");

            //4 Initialize and test Raycaster5
            stopwatch.Restart();
            camera2 = new Camera2(8, 8, 0) // Match position and angle with Camera2
            {
                FieldOfView = Math.PI / 2, // Match Camera2's field of view
                CellSize = 64 // Match Raycaster2's default cell size
            };

            raycaster2 = new Raycaster2(map);
            bmp5 = raycaster2.RenderBitmap(camera2);
            bmp5.Save("raycaster5_4_output.png");
            stopwatch.Stop();
            Trace.WriteLine($"Raycaster5 rendering time: {stopwatch.ElapsedMilliseconds} ms");
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

            for (var x = 0; x < bmp1.Width; x++)
            for (var y = 0; y < bmp1.Height; y++)
                if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                    return false;

            return true;
        }

        /// <summary>
        ///     Processes the height map and color map, combining them into a single container.
        /// </summary>
        /// <param name="bmpHeight">The BMP for the height map.</param>
        /// <param name="bmpColor">The BMP for the color map.</param>
        private void ProcessMaps(Bitmap bmpHeight, Bitmap bmpColor)
        {
            if (bmpHeight == null || bmpColor == null || bmpHeight.Width != bmpColor.Width ||
                bmpHeight.Height != bmpColor.Height)
                return;


            var dbmHeight = DirectBitmap.GetInstance(bmpHeight);
            var dbmColor = DirectBitmap.GetInstance(bmpColor);

            _rasterData = new PixelData[bmpHeight.Width, bmpHeight.Height];

            for (var i = 0; i < bmpHeight.Width; i++)
                for (var j = 0; j < bmpHeight.Height; j++)
                {
                    // Get height from height map (assuming it uses the red channel)
                    int height = dbmHeight.GetPixel(i, j).R;

                    // Get color from color map
                    var color = dbmColor.GetPixel(i, j);

                    // Store both color and height in the same location
                    _rasterData[i, j] = new PixelData
                    {
                        Color = color,
                        Height = height
                    };
                }
        }
    }
}