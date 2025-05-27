/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/CvsLayeredHandler.cs
 * PURPOSE:     My custom format, it is a collection of csv files separated with an keyword. Mostly needed for my Lif file format
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.Text;

namespace DataFormatter
{
    public static class CvsLayeredHandler
    {
        /// <summary>
        ///     Writes the CSV with layer keywords.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="csvLayers">The CSV layers.</param>
        /// <param name="layerKeyword">The layer keyword.</param>
        public static void WriteCsvWithLayerKeywords(string filepath, char separator, List<List<string>> csvLayers,
            string layerKeyword)
        {
            var file = new StringBuilder();

            for (var layerIndex = 0; layerIndex < csvLayers.Count; layerIndex++)
            {
                foreach (var row in csvLayers[layerIndex])
                {
                    var line = string.Join(separator, row);
                    file.AppendLine(line);
                }

                // Add the layer keyword at the end
                file.AppendLine($"{layerKeyword}{layerIndex}");
            }

            CsvHelper.WriteContentToFile(filepath, file);
        }

        /// <summary>
        ///     Reads the CSV with layer keywords.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="layerKeyword">The layer keyword.</param>
        /// <returns>Content of our special format file</returns>
        public static List<string> ReadCsvWithLayerKeywords(string filepath, char separator, string layerKeyword)
        {
            var lst = CsvHelper.ReadFileContent(filepath);
            if (lst == null) return null;

            var layers = new List<string>();
            var currentLayer = new StringBuilder(); // Use StringBuilder to accumulate lines for each layer

            foreach (var line in lst)
                // When the layer keyword is encountered, store the current layer
                if (line.StartsWith(layerKeyword))
                {
                    if (currentLayer.Length > 0)
                        layers.Add(currentLayer.ToString()
                            .TrimEnd()); // Add the current layer string and trim the last newline

                    currentLayer = new StringBuilder(); // Start a new layer
                }
                else
                {
                    currentLayer.AppendLine(line); // Append the line to the current layer with a newline
                }

            // Add the last layer if there are any remaining lines
            if (currentLayer.Length > 0) layers.Add(currentLayer.ToString().TrimEnd()); // Trim the trailing newline

            return layers;
        }
    }
}