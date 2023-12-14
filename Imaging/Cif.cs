/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/Cif.cs
 * PURPOSE:     Custom Image Format object, that contains all attributes and basic information
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Imaging
{
    /// <summary>
    ///     Image in Cif format, with various further Tools
    /// </summary>
    public sealed class Cif
    {
        /// <summary>
        ///     The cif image
        /// </summary>
        public Dictionary<Color, List<int>> cifImage = new();

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Cif" /> is compressed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if compressed; otherwise, <c>false</c>.
        /// </value>
        public bool Compressed { get; init; }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height { get; init; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public int Width { get; init; }

        /// <summary>
        ///     Gets the check sum.
        /// </summary>
        /// <value>
        ///     The check sum.
        /// </value>
        public int CheckSum => Height * Width;

        /// <summary>
        ///     Gets the number of colors.
        /// </summary>
        /// <value>
        ///     The number of colors.
        /// </value>
        public int NumberOfColors { get; init; }

        /// <summary>
        ///     Changes the color.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        /// <returns>Success Status</returns>
        public bool ChangeColor(int x, int y, Color color)
        {
            var id = CifProcessing.CalculateId(x, y, Width);

            if (id > CheckSum)
            {
                return false;
            }

            foreach (var (key, value) in cifImage)
            {
                if (!value.Contains(id))
                {
                    continue;
                }

                if (key == color)
                {
                    return false;
                }

                cifImage[key].Remove(id);

                if (cifImage.ContainsKey(color))
                {
                    cifImage[color].Add(id);
                }
                else
                {
                    var cache = new List<int> { id };
                    cifImage.Add(color, cache);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Changes the color.
        /// </summary>
        /// <param name="oldColor">The old color.</param>
        /// <param name="newColor">The new color.</param>
        /// <returns>Success Status</returns>
        public bool ChangeColor(Color oldColor, Color newColor)
        {
            if (!cifImage.ContainsKey(oldColor))
            {
                return false;
            }

            var cache = cifImage[oldColor];
            cifImage.Remove(oldColor);

            if (cifImage.ContainsKey(newColor))
            {
                cifImage[newColor].AddRange(cache);
            }
            else
            {
                cifImage.Add(newColor, cache);
            }

            return true;
        }

        /// <summary>
        ///     Gets the image.
        /// </summary>
        /// <returns>Cif Converted to Image</returns>
        [return: MaybeNull]
        public Image GetImage()
        {
            if (cifImage == null)
            {
                return null;
            }

            var image = new Bitmap(Height, Width);
            var dbm = DirectBitmap.GetInstance(image);

            foreach (var (key, value) in cifImage)
            {
                foreach (var id in value)
                {
                    var x = CifProcessing.IdToX(id, Width);
                    var y = CifProcessing.IdToY(id, Width);
                    dbm.SetPixel(x, y, key);
                }
            }

            return null;
        }
    }
}
