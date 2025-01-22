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
        /// <summary>
        /// Fills gaps (zeros) in a column slice with the last known non-zero value.
        /// </summary>
        private static int[] FillGaps(int[] columnSlice)
        {
            var filledColumnSlice = new int[columnSlice.Length];
            var previousColor = 0;

            for (var y = 0; y < columnSlice.Length; y++)
            {
                if (columnSlice[y] != 0)
                {
                    previousColor = columnSlice[y];
                }

                filledColumnSlice[y] = previousColor;
            }

            return filledColumnSlice;
        }

        internal static IEnumerable<(int x, int y, Color color)> FillMissingColorsOld(List<int[]> columnSlices, ImmutableDictionary<int, Color> colorDictionary)
        {
            var filledPixelTuples = new ConcurrentBag<(int x, int y, Color color)>();

            _ = Parallel.For(0, columnSlices.Count, x =>
            {
                var filledColumnSlice = FillGaps(columnSlices[x]);

                for (var y = 0; y < filledColumnSlice.Length; y++)
                {
                    var colorId = filledColumnSlice[y];
                    if (colorId == 0) continue;

                    if (colorDictionary.TryGetValue(colorId, out var color))
                        filledPixelTuples.Add((x, y, color));
                }
            });

            return filledPixelTuples;
        }

        internal static IEnumerable<(int x, int y, Color color)> FillMissingColorsPoints(List<int[]> columnSlices, ImmutableDictionary<int, Color> colorDictionary)
        {
            var filledPixelTuples = new ConcurrentBag<(int x, int y, Color color)>();

            _ = Parallel.For(0, columnSlices.Count, x =>
            {
                var filledColumnSlice = FillGaps(columnSlices[x]);

                for (var y = 0; y < filledColumnSlice.Length; y++)
                {
                    var colorId = filledColumnSlice[y];
                    if (colorId == 0) continue;

                    if (colorDictionary.TryGetValue(colorId, out var color))
                        filledPixelTuples.Add((x, y, color));
                }
            });

            return filledPixelTuples;
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

        internal static (List<(int x, int y, Color color)> points, List<(int x, int y, int finalY, Color color)> lines)
            GetPointsAndLines(
                List<int[]> columnSlices, ImmutableDictionary<int, Color> colorDictionary)
        {
            var points = new List<(int x, int y, Color color)>();
            var lines = new List<(int x, int y, int finalY, Color color)>();

            for (var x = 0; x < columnSlices.Count; x++)
            {
                var columnSlice = columnSlices[x];
                var startY = -1;
                var currentColor = 0;

                for (var y = 0; y < columnSlice.Length; y++)
                {
                    var colorId = columnSlice[y];

                    if (colorId != currentColor)
                    {
                        // Handle the end of a line or point when transitioning away from the current color.
                        if (currentColor != 0)
                        {
                            var mappedColor = colorDictionary.GetValueOrDefault(currentColor, Color.Empty);
                            if (startY == y - 1)
                            {
                                // Single point.
                                points.Add((x, startY, mappedColor));
                            }
                            else
                            {
                                // Line.
                                lines.Add((x, startY, y - 1, mappedColor));
                            }
                        }

                        // Start a new segment if the new color is non-zero.
                        if (colorId != 0)
                        {
                            startY = y;
                        }
                        else
                        {
                            startY = -1; // Reset for gaps.
                        }

                        currentColor = colorId;
                    }
                }

                // Handle the final segment if it ends at the last pixel.
                if (currentColor != 0)
                {
                    var mappedColor = colorDictionary.GetValueOrDefault(currentColor, Color.Empty);
                    if (startY == columnSlice.Length - 1)
                    {
                        // Single point.
                        points.Add((x, startY, mappedColor));
                    }
                    else
                    {
                        // Line.
                        lines.Add((x, startY, columnSlice.Length - 1, mappedColor));
                    }
                }
            }

            return (points, lines);
        }
    }
}
