/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/CifImageData.cs
 * PURPOSE:     The Parser Object that will hold the actual Image Data
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Imaging
{
    internal sealed class CifImageData
    {
        /// <summary>
        ///     Define a converter delegate for image data parsing
        ///     The converter
        /// </summary>
        internal static readonly Func<List<string>, CifImageData> Converter = parts =>
        {
            var hex = parts[0];
            if (!int.TryParse(parts[1], out var a))
            {
                return null;
            }

            var converter = new ColorHsv(hex, a);
            var color = Color.FromArgb((byte)converter.A, (byte)converter.R, (byte)converter.G, (byte)converter.B);
            var coordinates = new List<int>();

            for (var i = 2; i < parts.Count; i++)
            {
                if (parts[i].Contains(ImagingResources.IntervalSplitter))
                {
                    var lst = parts[i].Split(ImagingResources.CifSeparator).ToList();
                    var sequence = GetStartEndPoint(lst);

                    if (sequence == null)
                    {
                        continue;
                    }

                    // Ensure Start and End are not null before casting
                    if (sequence.Value.Start.HasValue && sequence.Value.End.HasValue)
                    {
                        for (var id = sequence.Value.Start.Value; id <= sequence.Value.End.Value; id++)
                        {
                            coordinates.Add(id);
                        }
                    }
                }
                else if (int.TryParse(parts[i], out var idMaster))
                {
                    coordinates.Add(idMaster);
                }
            }

            return new CifImageData { Color = color, Coordinates = coordinates };
        };

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        /// <value>
        ///     The color.
        /// </value>
        internal Color Color { get; init; }

        /// <summary>
        ///     Gets or sets the coordinates.
        /// </summary>
        /// <value>
        ///     The coordinates.
        /// </value>
        internal List<int>? Coordinates { get; init; }

        /// <returns>start and End Point as Tuple</returns>
        /// <summary>
        ///     Gets the start and end points from a list of strings.
        /// </summary>
        /// <returns>A tuple representing the start and end points, or null if parsing fails.</returns>
        private static (int? Start, int? End)? GetStartEndPoint(IReadOnlyList<string> lst)
        {
            if (lst.Count < 2)
            {
                return null;
            }

            var checkStart = int.TryParse(lst[0], out var start);
            var checkEnd = int.TryParse(lst[1], out var end);

            return checkStart && checkEnd ? (start, end) : null;
        }
    }
}
