/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/CifProcessing.cs
 * PURPOSE:     Processing of our custom Image Format, with custom compression
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

#nullable enable
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

                if (!imageFormat.ContainsKey(color))
                {
                    imageFormat[color] = new SortedSet<int>();
                }

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
            if (cif == null)
            {
                return null;
            }

            var image = new Bitmap(cif.Width, cif.Height);

            var dbm = DirectBitmap.GetInstance(image);

            foreach (var (color, ids) in cif.CifImage)
            {
                dbm.SetArea(ids, color);
            }

            return dbm.Bitmap;
        }

        /// <summary>
        ///     Gets the cif from file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     Cif Image
        /// </returns>
        internal static Cif? CifFromFile(string path)
        {
            var csv = CsvHandler.ReadCsv(path, ImagingResources.Separator);

            if (csv == null)
            {
                return null;
            }

            int height = 0, width = 0;

            var compressed = GetInfo(csv[0], ref height, ref width);

            if (compressed == null)
            {
                return null;
            }

            //remove the Height, length csv
            csv.RemoveAt(0);

            var cif = new Cif
            {
                Height = height,
                Width = width,
                Compressed = false,
                CifImage = new Dictionary<Color, SortedSet<int>>()
            };

            if (compressed == true)
            {
                foreach (var line in csv)
                {
                    var hex = line[0];

                    var check = int.TryParse(line[1], out var a);

                    if (!check)
                    {
                        continue;
                    }

                    var converter = new ColorHsv(hex, a);

                    var color = Color.FromArgb((byte)converter.A, (byte)converter.R, (byte)converter.G,
                        (byte)converter.B);

                    //get coordinates
                    for (var i = 2; i < line.Count; i++)
                    {
                        if (line[i].Contains(ImagingResources.IntervalSplitter))
                        {
                            //split get start and end
                            var lst = line[i].Split(ImagingResources.CifSeparator).ToList();

                            var sequence = GetStartEndPoint(lst);

                            if (sequence == null)
                            {
                                continue;
                            }

                            //paint area
                            for (var idMaster = sequence.Start; idMaster <= sequence.End; idMaster++)
                            {
                                cif.CifImage.Add(color, idMaster);
                            }
                        }
                        else
                        {
                            check = int.TryParse(line[i], out var idMaster);

                            if (!check)
                            {
                                continue;
                            }

                            cif.CifImage.Add(color, idMaster);
                        }
                    }
                }
            }
            else
            {
                foreach (var line in csv)
                {
                    var hex = line[0];

                    var check = int.TryParse(line[1], out var a);

                    if (!check)
                    {
                        continue;
                    }

                    var converter = new ColorHsv(hex, a);

                    var color = Color.FromArgb((byte)converter.A, (byte)converter.R, (byte)converter.G,
                        (byte)converter.B);

                    //get coordinates
                    for (var i = 2; i < line.Count; i++)
                    {
                        check = int.TryParse(line[i], out var idMaster);
                        if (!check)
                        {
                            continue;
                        }

                        cif.CifImage.Add(color, idMaster);
                    }
                }
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

                if (sequence == null)
                {
                    continue;
                }

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

        /// <summary>
        ///     Gets the information.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>All needed values for our Image</returns>
        private static bool? GetInfo(IReadOnlyList<string> csv, ref int height, ref int width)
        {
            //get image size
            var check = int.TryParse(csv[0], out var h);
            if (!check)
            {
                return null;
            }

            height = h;

            check = int.TryParse(csv[1], out var w);
            if (!check)
            {
                return null;
            }

            width = w;

            return csv[2] == ImagingResources.CifCompressed;
        }

        /// <summary>
        ///     Gets the start end point.
        /// </summary>
        /// <param name="lst">The LST.</param>
        /// <returns>start and End Point as Tuple</returns>
        private static StartEndPoint? GetStartEndPoint(IReadOnlyList<string> lst)
        {
            var check = int.TryParse(lst[0], out var start);
            if (!check)
            {
                return null;
            }

            check = int.TryParse(lst[1], out var end);

            return !check ? null : new StartEndPoint(start, end);
        }

        /// <summary>
        ///     Start and end points of a Sequence
        /// </summary>
        internal sealed class StartEndPoint
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="StartEndPoint" /> class.
            /// </summary>
            /// <param name="start">The start.</param>
            /// <param name="end">The end.</param>
            internal StartEndPoint(int start, int end)
            {
                Start = start;
                End = end;
            }

            /// <summary>
            ///     Gets the start.
            /// </summary>
            /// <value>
            ///     The start.
            /// </value>
            internal int Start { get; }

            /// <summary>
            ///     Gets the end.
            /// </summary>
            /// <value>
            ///     The end.
            /// </value>
            internal int End { get; }
        }
    }
}
