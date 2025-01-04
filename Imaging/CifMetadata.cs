/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/CifMetadata.cs
 * PURPOSE:     The Parser Object that will hold the actual meta Image Data
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;

namespace Imaging
{
    /// <summary>
    ///     Meta Data of Cif Images
    /// </summary>
    public sealed class CifMetadata
    {
        /// <summary>
        ///     Define a converter delegate for metadata parsing
        ///     The converter
        /// </summary>
        internal static readonly Func<List<string>, CifMetadata> Converter = parts =>
        {
            if (int.TryParse(parts[0], out var height) && int.TryParse(parts[1], out var width) &&
                int.TryParse(parts[3], out var checkSum))
            {
                return new CifMetadata
                {
                    Height = height,
                    Width = width,
                    Compressed = parts[2] == ImagingResources.CifCompressed,
                    CheckSum = checkSum
                };
            }

            return null;
        };

        public int Height { get; init; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public int Width { get; init; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="CifMetadata" /> is compressed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if compressed; otherwise, <c>false</c>.
        /// </value>
        public bool Compressed { get; init; }

        /// <summary>
        ///     Gets or sets the check sum.
        /// </summary>
        /// <value>
        ///     The check sum.
        /// </value>
        public int CheckSum { get; set; }
    }
}
