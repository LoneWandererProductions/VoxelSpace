/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/CsvHandler.cs
 * PURPOSE:     Basic CSV Reader and Writer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace DataFormatter
{
    /// <summary>
    ///     Basic Cvs Reader/Writer
    /// </summary>
    public static class CsvHandler
    {
        /// <summary>
        ///     Try to read an csv File
        /// </summary>
        /// <param name="filepath">path of the csv file</param>
        /// <param name="separator">What separator is in use</param>
        /// <returns>IEnumerable if something wrong, we return null</returns>
        [return: MaybeNull]
        public static List<List<string>> ReadCsv(string filepath, char separator)
        {
            var lst = ReadText.ReadFile(filepath);
            if (lst == null)
            {
                return null;
            }

            var enums = new List<List<string>>(lst.Count);

            enums.AddRange(lst.Select(item => DataHelper.GetParts(item, separator))
                .Select(subs => subs.ConvertAll(s => s.Trim())));

            return enums;
        }

        /// <summary>
        ///     Writes the CSV into a file
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <param name="csv">The Csv data.</param>
        public static void WriteCsv(string filepath, List<List<string>> csv)
        {
            var file = new StringBuilder();

            for (var i = 0; i < csv.Count; i++)
            {
                var row = csv[i];

                var line = string.Empty;

                for (var j = 0; j < row.Count; j++)
                {
                    var cache = row[j];

                    line = j != row.Count - 1
                        ? string.Concat(line, cache, DataFormatterResources.Splitter)
                        : string.Concat(line, cache);
                }

                _ = i != row.Count - 1 ? file.Append(line).Append(Environment.NewLine) : file.Append(line);
            }

            File.WriteAllText(filepath, file.ToString());
        }
    }
}
