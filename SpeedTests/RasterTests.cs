using System;
using System.Drawing;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Voxels;
using System.Diagnostics;

namespace SpeedTests
{
    [TestClass]
    public class RasterTests
    {
        // Helper function to create an ImmutableDictionary from a color map
        private ImmutableDictionary<int, Color> CreateColorDictionary(Color[,] colorMap, int width, int height)
        {
            var dictionaryBuilder = ImmutableDictionary.CreateBuilder<int, Color>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var color = colorMap[x, y];
                    dictionaryBuilder[color.ToArgb()] = color; // Store ARGB as the key
                }
            }
            return dictionaryBuilder.ToImmutable();
        }

        // Test the Raster function with different inputs
        [TestMethod]
        public void TestRasterWithColorSequence()
        {
            // Define a simple color map (e.g., 3x3)
            Color[,] colorMap = new Color[3, 3]
            {
                { Color.Red, Color.Red, Color.Green },
                { Color.Blue, Color.Red, Color.Green },
                { Color.Red, Color.Green, Color.Green }
            };

            // Create the ImmutableDictionary using the color map
            var colorDictionary = CreateColorDictionary(colorMap, 3, 3);

            // Define a column slice with a sequence of colors, including 0 for placeholder
            int[] columnSlice = new int[] {
                0, // Placeholder (no color before, should stay 0)
                colorMap[1, 0].ToArgb(), // Blue
                colorMap[2, 0].ToArgb(), // Red
                0, // Placeholder (should be replaced with Red)
                colorMap[2, 1].ToArgb(), // Green
                0  // Placeholder (should be replaced with Green)
            };

            // Call Raster function
            var (singlePixels, verticalLines) = RasterHelper.Raster(0, columnSlice, colorDictionary);

            // Debug Output
            Trace.WriteLine("TestRasterWithColorSequence - Column Slice: ");
            foreach (var pixel in columnSlice)
            {
                Trace.WriteLine(pixel);
            }
            Trace.WriteLine("Single Pixels: ");
            foreach (var pixel in singlePixels)
            {
                Trace.WriteLine($"X: {pixel.x}, Y: {pixel.y}, Color: {pixel.color}");
            }
            Trace.WriteLine("Vertical Lines: ");
            foreach (var line in verticalLines)
            {
                Trace.WriteLine($"X: {line.x}, Y: {line.y}, Final Y: {line.finalY}, Color: {line.color}");
            }

            // Verify the results
            Assert.AreEqual(1, singlePixels.Count, $"Pixels {singlePixels.Count}"); // Expect 1 single pixels (after filling placeholders)
            Assert.AreEqual(2, verticalLines.Count, $"Pixels {verticalLines.Count}"); // Expect 2 vertical lines (one for Red and one for Green)
        }



        // Test the case with no color changes (just single pixels)
        [TestMethod]
        public void TestRasterWithSinglePixels()
        {
            // Define a simple color map (e.g., 2x2)
            Color[,] colorMap = new Color[2, 2]
            {
                { Color.Red, Color.Green },
                { Color.Blue, Color.Yellow }
            };

            // Create the ImmutableDictionary using the color map
            var colorDictionary = CreateColorDictionary(colorMap, 2, 2);

            // Define a column slice with single pixels
            int[] columnSlice = new int[] { colorMap[0, 0].ToArgb(), 0, colorMap[1, 1].ToArgb() };

            // Call Raster function
            var (singlePixels, verticalLines) = RasterHelper.Raster(0, columnSlice, colorDictionary);

            // Debug Output
            Console.WriteLine("TestRasterWithSinglePixels - Column Slice: ");
            foreach (var pixel in columnSlice)
            {
                Trace.WriteLine(pixel);
            }
            Console.WriteLine("Single Pixels: ");
            foreach (var pixel in singlePixels)
            {
                Trace.WriteLine($"X: {pixel.x}, Y: {pixel.y}, Color: {pixel.color}");
            }
            Console.WriteLine("Vertical Lines: ");
            foreach (var line in verticalLines)
            {
                Trace.WriteLine($"X: {line.x}, Y: {line.y}, Final Y: {line.finalY}, Color: {line.color}");
            }

            // Verify the results
            Assert.AreEqual(1, singlePixels.Count);
            Assert.AreEqual(1, verticalLines.Count); // No vertical lines in this case
        }

        // Test the case with alternating colors and vertical lines
        [TestMethod]
        public void TestRasterWithAlternatingColors()
        {
            // Define a simple color map (e.g., 3x3)
            Color[,] colorMap = new Color[3, 3]
            {
                { Color.Red, Color.Green, Color.Red },
                { Color.Green, Color.Red, Color.Green },
                { Color.Red, Color.Green, Color.Red }
            };

            // Create the ImmutableDictionary using the color map
            var colorDictionary = CreateColorDictionary(colorMap, 3, 3);

            // Define a column slice with alternating colors
            int[] columnSlice = new int[] { colorMap[0, 0].ToArgb(), colorMap[1, 0].ToArgb(), colorMap[2, 0].ToArgb() };

            // Call Raster function
            var (singlePixels, verticalLines) = RasterHelper.Raster(0, columnSlice, colorDictionary);

            // Debug Output
            Console.WriteLine("TestRasterWithAlternatingColors - Column Slice: ");
            foreach (var pixel in columnSlice)
            {
                Console.WriteLine(pixel);
            }
            Console.WriteLine("Single Pixels: ");
            foreach (var pixel in singlePixels)
            {
                Console.WriteLine($"X: {pixel.x}, Y: {pixel.y}, Color: {pixel.color}");
            }
            Console.WriteLine("Vertical Lines: ");
            foreach (var line in verticalLines)
            {
                Console.WriteLine($"X: {line.x}, Y: {line.y}, Final Y: {line.finalY}, Color: {line.color}");
            }

            // Verify the results
            Assert.AreEqual(3, singlePixels.Count, "Points");
            Assert.AreEqual(0, verticalLines.Count, "Lines"); // One vertical line
        }

        // Test with all same color pixels (no vertical lines)
        [TestMethod]
        public void TestRasterWithSameColorPixels()
        {
            // Define a simple color map (e.g., 3x3)
            Color[,] colorMap = new Color[3, 3]
            {
        { Color.Red, Color.Red, Color.Red },
        { Color.Red, Color.Red, Color.Red },
        { Color.Red, Color.Red, Color.Red }
            };

            // Create the ImmutableDictionary using the color map
            var colorDictionary = CreateColorDictionary(colorMap, 3, 3);

            // Define a column slice with the same color for all pixels
            int[] columnSlice = new int[] { colorMap[0, 0].ToArgb(), colorMap[1, 0].ToArgb(), colorMap[2, 0].ToArgb() };

            // Call Raster function
            var (singlePixels, verticalLines) = RasterHelper.Raster(0, columnSlice, colorDictionary);

            // Debug Output
            Trace.WriteLine("TestRasterWithSameColorPixels - Column Slice: ");
            foreach (var pixel in columnSlice)
            {
                Trace.WriteLine(pixel);
            }
            Trace.WriteLine("Single Pixels: ");
            foreach (var pixel in singlePixels)
            {
                Trace.WriteLine($"X: {pixel.x}, Y: {pixel.y}, Color: {pixel.color}");
            }
            Trace.WriteLine("Vertical Lines: ");
            foreach (var line in verticalLines)
            {
                Trace.WriteLine($"X: {line.x}, Y: {line.y}, Final Y: {line.finalY}, Color: {line.color}");
            }

            // Verify the results
            Assert.AreEqual(0, singlePixels.Count); // No single pixels
            Assert.AreEqual(1, verticalLines.Count); // One vertical line detected
        }

    }
}
