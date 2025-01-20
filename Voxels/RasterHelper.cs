using System;
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
            var previousColor = 0; // Start with no color
            var colorYList = new List<int>(); // To store Y positions of pixels of the same color
            var currentColor = 0;

            for (int y = 0; y < columnSlice.Length; y++)
            {
                int color = columnSlice[y];
                Color mappedColor = colorDictionary.GetValueOrDefault(color, Color.Empty);

                // Handle gap filling for zero values (fill with the last known color)
                if (color == 0 && previousColor != 0)
                {
                    color = previousColor; // Continue with the last known color
                    mappedColor = colorDictionary.GetValueOrDefault(color, Color.Empty); // Get the mapped color
                }

                // Detect color change
                if (color != currentColor)
                {
                    // If the current sequence has any Y-values, process it
                    if (currentColor != 0 && colorYList.Count > 0)
                    {
                        if (colorYList.Count == 1) // Single pixel
                        {
                            singlePixels.Add((x, colorYList[0], mappedColor)); // Add as a single pixel
                        }
                        else // Vertical line (more than 1 pixel)
                        {
                            verticalLines.Add((x, colorYList[0], colorYList[^1], mappedColor)); // Add as vertical line
                        }
                    }

                    // Start a new sequence with the current color
                    currentColor = color;
                    colorYList.Clear(); // Reset Y-list for the new color
                }

                // Add the current Y to the list
                colorYList.Add(y);

                // Update the previous color
                if (color != 0)
                {
                    previousColor = color;
                }

                // Debugging: Track the current state
                Console.WriteLine($"y={y}, color={color}, previousColor={previousColor}, currentColor={currentColor}, filledColor={mappedColor}");
            }

            // Handle the last sequence after the loop ends
            if (currentColor != 0 && colorYList.Count > 0)
            {
                Color lastColor = colorDictionary.GetValueOrDefault(currentColor, Color.Empty);
                if (colorYList.Count == 1) // Single pixel
                {
                    singlePixels.Add((x, colorYList[0], lastColor)); // Add as a single pixel
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
