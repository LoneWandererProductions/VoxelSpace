using Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing;

namespace SpeedTests
{
    [TestClass]
    public class DirectBitmapValidation
    {
        [TestMethod]
        public void DrawSingleVerticalLine_ShouldModifyBitsCorrectly()
        {
            // Arrange
            const int width = 1, height = 10;
            var target = new DirectBitmap(width, height);
            var color = Color.Red;
            var verticalLines = new List<(int x, int y, int finalY, Color color)>
            {
                (0, 2, 8, color) // Line from (5, 2) to (5, 8)
            };

            // Act
            target.DrawVerticalLinesSimd(verticalLines);

            // Assert
            for (var y = 2; y <= 8; y++) // Include the pixel at finalY
            {
                Assert.AreEqual(color.ToArgb(), target.Bits[0 + (y * width)]);
            }
        }


        [TestMethod]
        public void DrawSingleVerticalLine_WithinBounds_ShouldModifyBitsCorrectly()
        {
            // Arrange
            const int width = 10, height = 10; // Adjusted to reflect valid indices from 0 to 8
            var target = new DirectBitmap(width + 1, height + 1); // Create bitmap with size 10x10
            var color = Color.Red;
            var verticalLines = new List<(int x, int y, int finalY, Color color)>
            {
                (5, 2, 8, color) // Line from (5, 2) to (5, 8) inclusive
            };

            // Act
            target.DrawVerticalLinesSimd(verticalLines);

            // Assert
            for (var y = 2; y <= 8; y++) // Ensure all expected pixels are tested
            {
                var bitIndex = 5 + (y * (width + 1)); // Correct bit index calculation for the given coordinates
                Assert.AreEqual(color.ToArgb(), target.Bits[bitIndex], $"Pixel at ({5}, {y}) was not drawn correctly.");
            }

            // Verify no unexpected modifications
            for (var x = 0; x <= width; x++)
            {
                for (var y = 0; y <= height; y++)
                {
                    if (x == 5 && y >= 2 && y <= 8) continue; // Skip the drawn vertical line
                    Assert.AreEqual(0, target.Bits[x + y * (width + 1)], $"Pixel at ({x}, {y}) was unexpectedly modified.");
                }
            }
        }

        [TestMethod]
        public void DrawVerticalLinesSimd_MultipleLines_ShouldHandleCorrectly()
        {
            // Arrange
            const int width = 10, height = 10;
            var target = new DirectBitmap(width, height);
            var red = Color.Red.ToArgb();
            var blue = Color.Blue.ToArgb();

            var verticalLines = new List<(int x, int y, int finalY, Color color)>
            {
                (1, 1, 5, Color.Red), // Vertical line in red
                (2, 3, 8, Color.Blue) // Vertical line in blue
            };

            // Act
            target.DrawVerticalLinesSimd(verticalLines);

            // Assert
            for (var y = 1; y < 5; y++) Assert.AreEqual(red, target.Bits[1 + (y * width)]);
            for (var y = 3; y < 8; y++) Assert.AreEqual(blue, target.Bits[2 + (y * width)]);
        }
    }
}
