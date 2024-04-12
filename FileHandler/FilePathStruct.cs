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
    ///     Sort File Paths
    /// </summary>
    internal readonly struct FilePathStruct : IComparable<FilePathStruct>
    {
        /// <summary>
        ///     Gets the path.
        /// </summary>
        /// <value>
        ///     The path.
        /// </value>
        internal string Path { get; }

        /// <summary>
        ///     Gets the file.
        /// </summary>
        /// <value>
        ///     The file.
        /// </value>
        internal string File { get; }

        /// <summary>
        ///     Gets the directory.
        ///     Not yet in use
        /// </summary>
        /// <value>
        ///     The directory.
        /// </value>
        internal string Directory { get; }

        /// <summary>
        ///     Gets or sets the table.
        /// </summary>
        /// <value>
        ///     The table.
        /// </value>
        private Dictionary<string, string[]> Table { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FilePathStruct" /> struct.
        /// </summary>
        /// <param name="path">The path.</param>
        public FilePathStruct(string path)
        {
            Path = path;
            File = System.IO.Path.GetFileName(Path);
            Directory = System.IO.Path.GetDirectoryName(Path);
            Table = new Dictionary<string, string[]>();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
        ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings:
        ///     <list type="table">
        ///         <listheader>
        ///             <term> Value</term><description> Meaning</description>
        ///         </listheader>
        ///         <item>
        ///             <term> Less than zero</term>
        ///             <description> This instance precedes <paramref name="other" /> in the sort order.</description>
        ///         </item>
        ///         <item>
        ///             <term> Zero</term>
        ///             <description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description>
        ///         </item>
        ///         <item>
        ///             <term> Greater than zero</term>
        ///             <description> This instance follows <paramref name="other" /> in the sort order.</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public int CompareTo(FilePathStruct other)
        {
            if (File.Equals(other.File))
            {
                return 0;
            }

            if (string.IsNullOrEmpty(File) || string.IsNullOrEmpty(other.File))
            {
                return 0;
            }

            if (!Table.TryGetValue(File, out var xBase))
            {
                xBase = Regex.Split(File.Replace(FileHandlerResources.Space, string.Empty), "([0-9]+)");
                Table.Add(File, xBase);
            }

            if (!Table.TryGetValue(other.File, out var yBase))
            {
                yBase = Regex.Split(other.File.Replace(FileHandlerResources.Space, string.Empty), "([0-9]+)");
                Table.Add(other.File, yBase);
            }

            for (var i = 0; i < xBase.Length && i < yBase.Length; i++)
            {
                if (xBase[i] != yBase[i])
                {
                    return PartCompare(xBase[i], yBase[i]);
                }
            }

            if (yBase.Length > xBase.Length)
            {
                return 1;
            }

            return -1;
        }

        private static int PartCompare(string left, string right)
        {
            if (!int.TryParse(left, out var x))
            {
                return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
            }

            if (!int.TryParse(right, out var y))
            {
                return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
            }

            return x.CompareTo(y);
        }
    }
}
