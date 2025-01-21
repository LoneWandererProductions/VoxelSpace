using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Threading.Tasks;

namespace Voxels
{
    internal static class RasterHelper
    {

        internal static IEnumerable<(int x, int y, Color color)> FillMissingColorsPoints(List<int[]> columnSlices, ImmutableDictionary<int, Color> colorDictionary)
        {
            var filledPixelTuples = new ConcurrentBag<(int x, int y, Color color)>();

            _ = Parallel.For(0, columnSlices.Count, x =>
            {
                Span<int> columnSlice = columnSlices[x]; // Span used for performance
                var lastKnownColorId = 0;

                for (var y = 0; y < columnSlice.Length; y++)
                {
                    var colorId = columnSlice[y]; // Local variable to store the value

                    if (colorId != 0)
                    {
                        lastKnownColorId = colorId;
                    }
                    else if (lastKnownColorId != 0)
                    {
                        colorId = lastKnownColorId; // Continue using the last known color
                    }

                    if (colorId == 0) continue;

                    if (colorDictionary.TryGetValue(colorId, out var color))
                        filledPixelTuples.Add((x, y, color));
                    else continue;
                    //Trace.WriteLine($"Warning: Color ID {colorId} not found in the dictionary.");
                }
            });

            return filledPixelTuples;
        }

        internal static List<(int columnIndex, int y, int finalY, Color color)> FillMissingColorsLines(List<int[]> columnSlices, ImmutableDictionary<int, Color> colorDictionary)
        {
            var lines = new List<(int columnIndex, int y, int finalY, Color color)>();

            for (int columnIndex = 0; columnIndex < columnSlices.Count; columnIndex++)
            {
                var columnSlice = columnSlices[columnIndex];

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

                // Step 2: Iterate over the filled column slice to identify vertical lines
                var currentColor = 0;
                var startY = -1; // Start of the current line

                for (int y = 0; y < filledColumnSlice.Length; y++)
                {
                    var color = filledColumnSlice[y];
                    if (color != currentColor)
                    {
                        // End the current line if a sequence is detected
                        if (currentColor != 0 && startY != -1)
                        {
                            var mappedColor = colorDictionary.GetValueOrDefault(currentColor, Color.Empty);
                            lines.Add((columnIndex, startY, y - 1, mappedColor));
                        }

                        // Start a new line if the current color is not zero
                        currentColor = color;
                        startY = color != 0 ? y : -1;
                    }
                }

                // Handle the last line after the loop ends
                if (currentColor != 0 && startY != -1)
                {
                    var mappedColor = colorDictionary.GetValueOrDefault(currentColor, Color.Empty);
                    lines.Add((columnIndex, startY, columnSlice.Length - 1, mappedColor));
                }
            }

            return lines;
        }

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
