/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Voxels
 * FILE:        RasterHelper.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;

namespace Voxels
{
    internal static class RasterHelper
    {
        /// <summary>
        ///     Fills gaps (zeros) in a column slice with the last known non-zero value.
        /// </summary>
        private static int[] FillGaps(int[] columnSlice)
        {
            var filledColumnSlice = new int[columnSlice.Length];
            var previousColor = 0;

            for (var y = 0; y < columnSlice.Length; y++)
            {
                if (columnSlice[y] != 0) previousColor = columnSlice[y];

                filledColumnSlice[y] = previousColor;
            }

            return filledColumnSlice;
        }

        internal static IEnumerable<(int columnIndex, int y, int finalY, Color color)> FillMissingColorsLines(
            List<int[]> columnSlices, ImmutableDictionary<int, Color> colorDictionary)
        {
            var lines = new List<(int columnIndex, int y, int finalY, Color color)>();

            for (var columnIndex = 0; columnIndex < columnSlices.Count; columnIndex++)
            {
                var filledColumnSlice = FillGaps(columnSlices[columnIndex]);
                var startY = -1;
                var currentColor = 0;

                for (var y = 0; y < filledColumnSlice.Length; y++)
                {
                    var color = filledColumnSlice[y];

                    if (color != currentColor)
                    {
                        // If we're ending a segment, add it to the lines list.
                        if (currentColor != 0 && startY != -1)
                        {
                            var mappedColor = colorDictionary.GetValueOrDefault(currentColor, Color.Empty);
                            lines.Add((columnIndex, startY, y - 1, mappedColor));
                        }

                        // Start a new segment for the new color.
                        currentColor = color;
                        startY = color != 0 ? y : -1;
                    }
                }

                // Handle the final segment if it ends at the last pixel.
                if (currentColor != 0 && startY != -1)
                {
                    var mappedColor = colorDictionary.GetValueOrDefault(currentColor, Color.Empty);
                    lines.Add((columnIndex, startY, filledColumnSlice.Length - 1, mappedColor));
                }
            }

            return lines;
        }
    }
}