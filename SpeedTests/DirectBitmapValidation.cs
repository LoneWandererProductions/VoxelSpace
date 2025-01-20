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
        public void DrawSingleVerticalLine_WithinBounds_ShouldModifyBitsCorrectly()
        {
            // Arrange
            int width = 10, height = 10;
            var target = new DirectBitmap(width, height);
            var color = Color.Red;
            var verticalLines = new List<(int x, int y, int finalY, Color color)>
            {
                (5, 2, 8, color) // Line from (5, 2) to (5, 8)
            };

            // Act
            target.DrawVerticalLinesSimd(verticalLines);

            // Assert
            for (int y = 2; y < 8; y++)
            {
                Assert.AreEqual(color.ToArgb(), target.Bits[5 + (y * width)]);
            }
        }

        [TestMethod]
        public void DrawVerticalLine_PartiallyOutOfBounds_ShouldModifyValidPixelsOnly()
        {
            // Arrange
            int width = 10, height = 10;
            var target = new DirectBitmap(width, height);
            var color = Color.Green;
            var verticalLines = new List<(int x, int y, int finalY, Color color)>
            {
                (5, -3, 8, color) // Line starting out of bounds
            };

            // Act
            target.DrawVerticalLinesSimd(verticalLines);

            // Assert
            // Assert that only in-bounds pixels are updated
            for (int y = 0; y < 8; y++) // Only the in-bounds part
            {
                if (y >= 0) // Ensure only in-bounds part is modified
                {
                    Assert.AreEqual(color.ToArgb(), target.Bits[5 + (y * width)]);
                }
            }
        }

        [TestMethod]
        public void DrawVerticalLinesSimd_MultipleLines_ShouldHandleCorrectly()
        {
            // Arrange
            int width = 10, height = 10;
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
            for (int y = 1; y < 5; y++) Assert.AreEqual(red, target.Bits[1 + (y * width)]);
            for (int y = 3; y < 8; y++) Assert.AreEqual(blue, target.Bits[2 + (y * width)]);
        }
    }
}
