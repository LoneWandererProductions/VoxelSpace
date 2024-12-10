/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/CsvHandler.cs
 * PURPOSE:     Simple Csv reader/writer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataFormatter
{
    /// <summary>
    ///     Handle all operations related to CSV reading and writing.
    /// </summary>
    public static class CsvHandler
    {
        /// <summary>
        ///     Reads a CSV file and splits it by separator.
        /// </summary>
        /// <param name="filepath">Path of the CSV file.</param>
        /// <param name="separator">The separator in use.</param>
        /// <returns>List of lines split into parts if successful; otherwise, null.</returns>
        public static List<List<string>> ReadCsv(string filepath, char separator)
        {
            var lst = CsvHelper.ReadFileContent(filepath);
            return lst?.ConvertAll(item => CsvHelper.SplitLine(item, separator));
        }

        /// <summary>
        ///     Reads a CSV file and maps the data to objects of type T using a converter.
        /// </summary>
        /// <typeparam name="T">The type of objects to parse.</typeparam>
        /// <param name="filepath">The path of the CSV file.</param>
        /// <param name="separator">The separator character.</param>
        /// <param name="converter">The converter function that maps CSV data to an object.</param>
        /// <returns>A list of objects of type T.</returns>
        public static List<T> ReadCsv<T>(string filepath, char separator, Func<List<string>, T> converter)
            where T : new()
        {
            return ReadCsvRange(filepath, separator, converter).ToList();
        }

        /// <summary>
        ///     Reads a specific range of lines from a CSV file and maps them to objects using a converter function.
        /// </summary>
        /// <typeparam name="T">The type of objects to parse.</typeparam>
        /// <param name="filepath">The path of the CSV file.</param>
        /// <param name="separator">The separator character.</param>
        /// <param name="converter">The converter function to map each line to an object.</param>
        /// <param name="startLine">The starting line (inclusive). Optional, defaults to 0.</param>
        /// <param name="endLine">The ending line (inclusive). Optional, defaults to all lines.</param>
        /// <returns>A list of parsed objects of the specified type.</returns>
        public static List<T> ReadCsvRange<T>(string filepath, char separator, Func<List<string>, T> converter,
            int startLine = 0, int endLine = int.MaxValue)
        {
            var lst = ReadText.ReadFile(filepath);
            if (lst == null || lst.Count == 0 || startLine > endLine || startLine >= lst.Count) return null;

            var result = new List<T>();

            for (var i = startLine; i <= Math.Min(endLine, lst.Count - 1); i++)
            {
                var parts = DataHelper.GetParts(lst[i], separator).ConvertAll(s => s.Trim());
                var obj = converter(parts);
                if (obj != null) result.Add(obj);
            }

            return result;
        }

        /// <summary>
        ///     Writes the CSV data into a file.
        /// </summary>
        /// <param name="filepath">The file path.</param>
        /// <param name="csv">The CSV data.</param>
        /// <param name="separator">The separator character. Defaults to comma.</param>
        public static void WriteCsv(string filepath, IEnumerable<List<string>> csv, string separator = ",")
        {
            var file = new StringBuilder();

            foreach (var line in csv.Select(row => string.Join(separator, row))) file.AppendLine(line);

            CsvHelper.WriteContentToFile(filepath, file);
        }
    }
}