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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using Mathematics;

namespace Imaging
{
    /// <summary>
    ///     Image in Cif format, with various further Tools
    /// </summary>
    public sealed class Cif
    {
        /// <summary>
        /// The cif image
        /// </summary>
        private readonly Dictionary<Color, SortedSet<int>> _cifImage = new();

        /// <summary>
        /// The cif sorted
        /// </summary>
        private Dictionary<Color, SortedSet<int>> _cifSorted = new();

        /// <summary>
        /// The sort required
        /// </summary>
        private bool _sortRequired = true;

        /// <summary>
        ///     The cif image
        /// </summary>
        public Dictionary<Color, SortedSet<int>> CifImage
        {
            get => _cifImage;
            init
            {
                _cifImage = value;
                _sortRequired = true;
            }
        }

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
        /// Initializes a new instance of the <see cref="Cif"/> class.
        /// </summary>
        /// <param name="image">The image.</param>
        public Cif(Bitmap image)
        {
            var format = CifProcessing.ConvertToCif(image);

            Compressed = false;
            Height = image.Height;
            Width = image.Width;
            CifImage = format;
            NumberOfColors = format.Count;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cif"/> class.
        /// </summary>
        public Cif()
        {
            Compressed = false;
        }

        /// <summary>
        ///     Changes the color.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        /// <returns>Success Status</returns>
        public bool ChangeColor(int x, int y, Color color)
        {
            var coordinate = new Coordinate2D(x, y, Width);
            var id = coordinate.Id;

            if (id > CheckSum)
            {
                return false;
            }

            foreach (var (key, value) in CifImage)
            {
                if (!value.Contains(id))
                {
                    continue;
                }

                if (key == color)
                {
                    return false;
                }

                CifImage[key].Remove(id);

                if (CifImage.ContainsKey(color))
                {
                    CifImage[color].Add(id);
                }
                else
                {
                    var cache = new SortedSet<int> { id };
                    CifImage.Add(color, cache);
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
            if (!CifImage.ContainsKey(oldColor))
            {
                return false;
            }

            var cache = CifImage[oldColor];
            CifImage.Remove(oldColor);

            if (CifImage.ContainsKey(newColor))
            {
                CifImage[newColor].UnionWith(cache);
            }
            else
            {
                CifImage.Add(newColor, cache);
            }

            return true;
        }

        /// <summary>
        /// Gets the color, it is quite a fast way, if the image is big and the color count is low!
        /// </summary>
        /// 
        /// <param name="id">The identifier.</param>
        /// <returns>Color at this point or null, if id was completely wrong.</returns>
        public Color GetColor(int id)
        {
            if (id < 0 || id > Height * Width) return Color.Transparent;

            // Check if sorting is required and perform lazy loading
            if (_sortRequired)
            {
                _cifSorted = SortDct(_cifImage);
                _sortRequired = false;
            }

            foreach (var (color, value) in _cifSorted)
            {
                if (value.Contains(id)) return color;
            }

            return Color.Transparent;
        }

        /// <summary>
        ///     Gets the image.
        /// </summary>
        /// <returns>Cif Converted to Image</returns>
        [return: MaybeNull]
        public Image GetImage()
        {
            if (CifImage == null)
            {
                return null;
            }

            var image = new Bitmap(Height, Width);
            var dbm = DirectBitmap.GetInstance(image);

            foreach (var (key, value) in CifImage)
            foreach (var coordinate in value.Select(id => Coordinate2D.GetInstance(id, Width)))
            {
                dbm.SetPixel(coordinate.X, coordinate.Y, key);
            }

            return dbm.Bitmap;
        }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var info = string.Empty;

            foreach (var (color, value) in CifImage)
            {
                info = string.Concat(info, ImagingResources.Color, color, ImagingResources.Spacing);

                var sortedList = new List<int>(value);

                for (var i = 0; i < value.Count - 1; i++)
                {
                    info = string.Concat(info, sortedList[i], ImagingResources.Indexer);
                }

                info = string.Concat(info, sortedList[sortedList.Count], Environment.NewLine);
            }

            return info;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Height, Width, NumberOfColors);
        }

        /// <summary>
        /// Sorts the Dictionary.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Sorted Dictionary from biggest Count to lowest</returns>
        private static Dictionary<Color, SortedSet<int>> SortDct(Dictionary<Color, SortedSet<int>> value)
        {
            return value.OrderByDescending(kv => kv.Value.Count)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}
