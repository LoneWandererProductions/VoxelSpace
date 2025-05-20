/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/DataHelper.cs
 * PURPOSE:     Basic stuff shared over all operations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataFormatter
{
    /// <summary>
    ///     Basic helper Operations
    /// </summary>
    internal static class DataHelper
    {
        /// <summary>
        ///     Gets the parts.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param separator="separator"></param>
        /// <param name="separator">the splitter used in the csv</param>
        /// <returns>split Parts</returns>
        internal static List<string> GetParts(string str, char separator)
        {
            return str.Split(separator).ToList();
        }

        /// <summary>
        ///     Helper function to detect the encoding of a file based on BOM
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Type of Encoding</returns>
        internal static Encoding GetFileEncoding(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // Read the first few bytes to detect the encoding
            var buffer = new byte[5];
            var bytesRead = fs.Read(buffer, 0, 5);

            // Check if the file is smaller than the BOM sizes we are checking
            if (bytesRead < 2)
            {
                return Encoding.Default; // Default ANSI code page if not enough bytes for BOM
            }

            switch (buffer[0])
            {
                // Check for UTF-8 BOM
                // Check for UTF-16 LE BOM
                case 0xef when buffer[1] == 0xbb && buffer[2] == 0xbf:
                    return Encoding.UTF8;
                // Check for UTF-16 BE BOM
                case 0xff when buffer[1] == 0xfe:
                    return Encoding.Unicode; // UTF-16 LE
                // Check for UTF-32 LE BOM
                case 0xfe when buffer[1] == 0xff:
                    return Encoding.BigEndianUnicode; // UTF-16 BE
                // Check for UTF-32 BE BOM
                case 0xff when buffer[1] == 0xfe && buffer[2] == 0x00 && buffer[3] == 0x00:
                    return Encoding.UTF32; // UTF-32 LE
                case 0x00 when buffer[1] == 0x00 && buffer[2] == 0xfe && buffer[3] == 0xff:
                    return Encoding.GetEncoding(DataFormatterResources.EncodingUtf32); // UTF-32 BE

                default:
                    // Default encoding (change this as per your requirements)
                    return Encoding.Default; // Default ANSI code page
            }
        }
    }
}
