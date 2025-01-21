/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonLibraryTests
 * FILE:        CommonLibraryTests/DirectBitmapValidation.cs
 * PURPOSE:     Validate some functions in my   DirectBitmap
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */


using System.Collections.Generic;
using System.Drawing;
using Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpeedTests
{
    /// <summary>
    ///     Validate Vertical lines mostly
    /// </summary>
    [TestClass]
    public class DirectBitmapValidation
    {
        /// <summary>
        ///     Draws the single vertical line should modify bits correctly.
        /// </summary>
        [TestMethod]
<<<<<<< HEAD
        public void DrawSingleVerticalLineShouldModifyBitsCorrectly()
=======
        public void DrawSingleVerticalLine_ShouldModifyBitsCorrectly()
>>>>>>> 91575aab8f9579eb20506704ca4c5cefc545c9db
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
<<<<<<< HEAD
                Assert.AreEqual(color.ToArgb(), target.Bits[0 + y * width]);
=======
                Assert.AreEqual(color.ToArgb(), target.Bits[0 + (y * width)]);
>>>>>>> 91575aab8f9579eb20506704ca4c5cefc545c9db
            }
        }


<<<<<<< HEAD
        /// <summary>
        ///     Draws the single vertical line within bounds should modify bits correctly.
        /// </summary>
        [TestMethod]
        public void DrawSingleVerticalLineWithinBoundsShouldModifyBitsCorrectly()
=======
        [TestMethod]
        public void DrawSingleVerticalLine_WithinBounds_ShouldModifyBitsCorrectly()
>>>>>>> 91575aab8f9579eb20506704ca4c5cefc545c9db
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
<<<<<<< HEAD
                var bitIndex = 5 + y * (width + 1); // Correct bit index calculation for the given coordinates
=======
                var bitIndex = 5 + (y * (width + 1)); // Correct bit index calculation for the given coordinates
>>>>>>> 91575aab8f9579eb20506704ca4c5cefc545c9db
                Assert.AreEqual(color.ToArgb(), target.Bits[bitIndex], $"Pixel at ({5}, {y}) was not drawn correctly.");
            }

            // Verify no unexpected modifications
            for (var x = 0; x <= width; x++)
            {
                for (var y = 0; y <= height; y++)
                {
<<<<<<< HEAD
                    if (x == 5 && y >= 2 && y <= 8)
                    {
                        continue; // Skip the drawn vertical line
                    }

                    Assert.AreEqual(0, target.Bits[x + y * (width + 1)],
                        $"Pixel at ({x}, {y}) was unexpectedly modified.");
=======
                    if (x == 5 && y >= 2 && y <= 8) continue; // Skip the drawn vertical line
                    Assert.AreEqual(0, target.Bits[x + y * (width + 1)], $"Pixel at ({x}, {y}) was unexpectedly modified.");
>>>>>>> 91575aab8f9579eb20506704ca4c5cefc545c9db
                }
            }
        }

        /// <summary>
        ///     Draws the vertical lines simd multiple lines should handle correctly.
        /// </summary>
        [TestMethod]
        public void DrawVerticalLinesSimdMultipleLinesShouldHandleCorrectly()
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
<<<<<<< HEAD
            for (var y = 1; y < 5; y++)
            {
                Assert.AreEqual(red, target.Bits[1 + y * width]);
            }

            for (var y = 3; y < 8; y++)
            {
                Assert.AreEqual(blue, target.Bits[2 + y * width]);
            }
=======
            for (var y = 1; y < 5; y++) Assert.AreEqual(red, target.Bits[1 + (y * width)]);
            for (var y = 3; y < 8; y++) Assert.AreEqual(blue, target.Bits[2 + (y * width)]);
>>>>>>> 91575aab8f9579eb20506704ca4c5cefc545c9db
        }
    }
}
