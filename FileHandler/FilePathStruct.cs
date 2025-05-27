/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ImageCompare
 * FILE:        ImageCompare/IImageAnalysis.cs
 * PURPOSE:     Search Algorithm
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCE:      https://www.codeproject.com/Articles/22517/Natural-Sort-Comparer
 */

// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FileHandler
{
    /// <inheritdoc />
    /// <summary>
    ///     Struct to represent a file path and provide natural sorting functionality.
    /// </summary>
    internal readonly struct FilePathStruct : IComparable<FilePathStruct>
    {
        /// <summary>
        ///     Gets the full path of the file.
        /// </summary>
        internal string Path { get; }

        /// <summary>
        ///     Gets the file name.
        /// </summary>
        internal string File { get; }

        /// <summary>
        ///     Gets the directory of the file.
        ///     This property is currently not in use.
        /// </summary>
        internal string Directory { get; }

        /// <summary>
        ///     A dictionary for caching split file name components.
        /// </summary>
        private Dictionary<string, string[]> Table { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FilePathStruct" /> struct.
        /// </summary>
        /// <param name="path">The full path of the file.</param>
        public FilePathStruct(string path)
        {
            Path = path;
            File = System.IO.Path.GetFileName(path);
            Directory = System.IO.Path.GetDirectoryName(path);
            Table = new Dictionary<string, string[]>();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Compares this instance with another <see cref="FilePathStruct" /> for sorting purposes.
        /// </summary>
        /// <param name="other">The other instance to compare to.</param>
        /// <returns>
        ///     A negative number if this instance precedes <paramref name="other" />,
        ///     zero if they are equal,
        ///     and a positive number if this instance follows <paramref name="other" />.
        /// </returns>
        public int CompareTo(FilePathStruct other)
        {
            if (File.Equals(other.File, StringComparison.OrdinalIgnoreCase)) return 0;

            if (string.IsNullOrEmpty(File) || string.IsNullOrEmpty(other.File)) return 0;

            if (!Table.TryGetValue(File, out var xBase))
            {
                xBase = Regex.Split(File.Replace(FileHandlerResources.Space, string.Empty), "([0-9]+)");
                Table[File] = xBase;
            }

            if (!Table.TryGetValue(other.File, out var yBase))
            {
                yBase = Regex.Split(other.File.Replace(FileHandlerResources.Space, string.Empty), "([0-9]+)");
                Table[other.File] = yBase;
            }

            for (var i = 0; i < xBase.Length && i < yBase.Length; i++)
                if (xBase[i] != yBase[i])
                    return PartCompare(xBase[i], yBase[i]);

            return yBase.Length > xBase.Length ? 1 : -1;
        }

        /// <summary>
        ///     Compares two parts of file names, treating numeric parts as integers.
        /// </summary>
        private static int PartCompare(string left, string right)
        {
            if (!int.TryParse(left, out var x)) return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);

            if (!int.TryParse(right, out var y)) return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);

            return x.CompareTo(y);
        }
    }
}