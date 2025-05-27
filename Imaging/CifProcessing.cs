/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/CifProcessing.cs
 * PURPOSE:     Processing of our custom Image Format, with custom compression
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DataFormatter;
using ExtendedSystemObjects;

namespace Imaging
{
    //TODO add checksum Fun and custom image Exceptions

    /// <summary>
    ///     Basic Cif Processing
    /// </summary>
    internal static class CifProcessing
    {
        /// <summary>
        ///     Converts to cif.
        /// </summary>
        /// <param name="image">The image.</param>
        internal static Dictionary<Color, SortedSet<int>> ConvertToCifFromBitmap(Bitmap image)
        {
            var imageFormat = new Dictionary<Color, SortedSet<int>>();

            var dbm = DirectBitmap.GetInstance(image);

            var colorMap = dbm.GetColors();

            for (var i = 0; i < image.Height * image.Width; i++)
            {
                var color = colorMap[i];

                if (!imageFormat.ContainsKey(color)) imageFormat[color] = new SortedSet<int>();

                imageFormat[color].Add(i);
            }

            return imageFormat;
        }

        /// <summary>
        ///     Cifs to image.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        internal static Bitmap? CifFileToImage(string path)
        {
            var cif = CifFromFile(path);
            if (cif == null) return null;

            var image = new Bitmap(cif.Width, cif.Height);

            var dbm = DirectBitmap.GetInstance(image);

            foreach (var (color, ids) in cif.CifImage) dbm.SetArea(ids, color);

            return dbm.Bitmap;
        }

        /// <summary>
        ///     Gets the cif from file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     Cif Image
        /// </returns>
        public static Cif? CifFromFile(string path)
        {
            var ranges = new List<(Func<List<string>, object> converter, int startLine, int endLine)>
            {
                (CifMetadata.Converter, 0, 0), // First line for metadata
                (CifImageData.Converter, 1, int.MaxValue) // Rest for image data
            };

            var csvData = new List<object>();

            foreach (var (converter, startLine, endLine) in ranges)
                csvData.AddRange(CsvHandler.ReadCsvRange(path, ImagingResources.Separator, converter, startLine,
                    endLine));

            var meta = csvData.OfType<CifMetadata>().FirstOrDefault();
            var imageData = csvData.OfType<CifImageData>().ToList();

            if (meta == null) return null;

            var cif = new Cif
            {
                Height = meta.Height,
                Width = meta.Width,
                Compressed = meta.Compressed,
                CifImage = new Dictionary<Color, SortedSet<int>>()
            };

            foreach (var data in imageData)
            {
                if (!cif.CifImage.ContainsKey(data.Color)) cif.CifImage[data.Color] = new SortedSet<int>();

                foreach (var coordinates in data.Coordinates) cif.CifImage[data.Color].Add(coordinates);
            }

            return cif;
        }

        /// <summary>
        ///     Generates the CSV.
        /// </summary>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>Cif Format ready to be saved as csv.</returns>
        internal static List<List<string>> GenerateCsv(int imageHeight, int imageWidth,
            Dictionary<Color, SortedSet<int>> imageFormat)
        {
            var master = new List<List<string>>();
            //first line is size of the image, compression, Number of Ids, used for checking and lines and number of Colors, added later
            var child = new List<string>(4)
            {
                imageHeight.ToString(),
                imageWidth.ToString(),
                ImagingResources.CifUnCompressed,
                (imageHeight * imageWidth).ToString()
            };

            master.Add(child);

            //add colors and Points
            foreach (var (key, value) in imageFormat)
            {
                //Possible error here
                var converter = new ColorHsv(key.R, key.G, key.B, key.A);
                //First two keys are color and Hue
                var subChild = new List<string>(2) { converter.Hex, key.A.ToString() };

                subChild.AddRange(value.Select(id => id.ToString()));

                master.Add(subChild);
            }

            //add number of Colors as CheckSum
            master[0].Add(master.Count.ToString());

            return master;
        }

        /// <summary>
        ///     Generates the CSV compressed.
        /// </summary>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>Compressed image</returns>
        internal static List<List<string>> GenerateCsvCompressed(int imageHeight, int imageWidth,
            Dictionary<Color, SortedSet<int>> imageFormat)
        {
            var master = new List<List<string>>();
            //first line is size of the image, compression, Number of Ids, used for checking and lines and number of Colors, added later
            var child = new List<string>(4)
            {
                imageHeight.ToString(),
                imageWidth.ToString(),
                ImagingResources.CifCompressed,
                (imageHeight * imageWidth).ToString()
            };

            master.Add(child);

            //add colors and Points
            foreach (var (key, value) in imageFormat)
            {
                var converter = new ColorHsv(key.R, key.G, key.B, key.A);
                //First two keys are color and Hue
                var subChild = new List<string>(2) { converter.Hex, key.A.ToString() };

                var sequence = Utility.Sequencer(value, 3);

                var compressed = new List<int>();

                if (sequence == null) continue;

                var sortedList = new List<int>(value);

                foreach (var (startS, endS) in sequence)
                {
                    var start = sortedList[startS];
                    var end = sortedList[endS];
                    var cache = string.Concat(start, ImagingResources.CifSeparator, end);
                    subChild.Add(cache);

                    var range = new List<int>(sortedList.GetRange(startS, endS - startS));
                    compressed.AddRange(range);
                }

                var intersect = compressed.Except(value).ToList();
                var modulo = intersect.ConvertAll(i => i.ToString());

                subChild.AddRange(modulo);

                master.Add(subChild);
            }

            //add number of Colors as CheckSum
            master[0].Add(master.Count.ToString());

            return master;
        }
    }
}