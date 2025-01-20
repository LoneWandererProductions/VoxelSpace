using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;

namespace Voxels
{
    internal static class RasterHelper
    {
        internal static (List<(int x, int y, Color color)>, ConcurrentBag<(int x, int y, int finalY, Color color)>) Raster(
            int x,
            int[] columnSlice,
            ImmutableDictionary<int, Color> colorDictionary)
        {
            var singlePixels = new List<(int x, int y, Color color)>();
            var verticalLines = new ConcurrentBag<(int x, int y, int finalY, Color color)>();

            // Step 1: Fill the gaps (zeros) with the previous color
            var filledColumnSlice = new int[columnSlice.Length];
            var previousColor = 0; // Start with no color

            for (int y = 0; y < columnSlice.Length; y++)
            {
                if (columnSlice[y] != 0)
                {
                    previousColor = columnSlice[y]; // Update the previous color if it's not zero
                }
                filledColumnSlice[y] = previousColor; // Fill the current pixel with the previous color
            }

            // Step 2: Iterate over the filled column slice to identify points and vertical lines
            var currentColor = 0;
            var colorYList = new List<int>(); // List to store Y values for the same color

            for (int y = 0; y < filledColumnSlice.Length; y++)
            {
                var color = filledColumnSlice[y];
                Color mappedColor = colorDictionary.GetValueOrDefault(color, Color.Empty);

                // Detect if we're in a new color sequence
                if (color != currentColor)
                {
                    // If a sequence exists, handle the previous one
                    if (currentColor != 0 && colorYList.Count > 0)
                    {
                        if (colorYList.Count == 1) // Single pixel
                        {
                            singlePixels.Add((x, colorYList[0], mappedColor)); // Add as single point
                        }
                        else // Vertical line (more than 1 pixel)
                        {
                            verticalLines.Add((x, colorYList[0], colorYList[^1], mappedColor)); // Add as vertical line
                        }
                    }

                    // Start a new sequence with the current color
                    currentColor = color;
                    colorYList.Clear(); // Reset the Y list for the new color
                }

                // Add the current Y value to the list of Y's for the current color
                colorYList.Add(y);
            }

            // Handle the last sequence after the loop ends
            if (currentColor != 0 && colorYList.Count > 0)
            {
                Color lastColor = colorDictionary.GetValueOrDefault(currentColor, Color.Empty);
                if (colorYList.Count == 1) // Single pixel
                {
                    singlePixels.Add((x, colorYList[0], lastColor)); // Add as single point
                }
                else // Vertical line (more than 1 pixel)
                {
                    verticalLines.Add((x, colorYList[0], colorYList[^1], lastColor)); // Add as vertical line
                }
            }

            return (singlePixels, verticalLines);
        }
    }
}
