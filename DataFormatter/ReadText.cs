/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/ReadTxtFile.cs
 * PURPOSE:     Read txt Files
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace DataFormatter
{
    /// <summary>
    ///     Read and write a File
    /// </summary>
    public static class ReadText
    {
        /// <summary>
        ///     Reads a file line for line
        /// </summary>
        /// <param name="filePath">path of the file</param>
        /// <returns>the values as String[]. Can return null.</returns>
        [return: MaybeNull]
        public static List<string> ReadFile(string filePath)
        {
            var parts = new List<string>();
            try
            {
                var encoding = DataHelper.GetFileEncoding(filePath);
                using var reader = new StreamReader(filePath, encoding);
                while (reader.ReadLine() is { } line) parts.Add(line);
            }
            catch (IOException ex)
            {
                Trace.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex.Message);
            }

            return parts;
        }

        /// <summary>
        ///     Write a File async
        /// </summary>
        /// <param name="filepath">path of the File, the method doesn't care about the extension</param>
        /// <param name="content">The contents we want to write</param>
        public static async Task WriteFile(string filepath, IEnumerable<string> content)
        {
            try
            {
                await File.WriteAllLinesAsync(filepath, content).ConfigureAwait(false);
            }
            catch (IOException ex)
            {
                Trace.WriteLine(ex.Message);
                throw new IOException(string.Empty, ex);
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex.Message);
                throw new ArgumentException(string.Empty, ex);
            }
        }
    }
}