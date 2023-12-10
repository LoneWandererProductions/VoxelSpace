/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileHandleSort.cs
 * PURPOSE:     Extension for File Sort
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.Linq;

// ReSharper disable MemberCanBeInternal, we use it external
// ReSharper disable UnusedMember.Global, it is a library
// ReSharper disable UnusedType.Global

namespace FileHandler
{
    /// <summary>
    ///     Some Extensions for Search results
    /// </summary>
    /// <summary>
    ///     Some Extensions for Search results
    /// </summary>
    public static class FileHandleSort
    {
        /// <summary>
        ///     Path Sort. Sorts a list of strings in a more sane way.
        /// </summary>
        /// <param name="value">The list.</param>
        /// <returns>The sorted list.</returns>
        public static List<string> PathSort(this List<string> value)
        {
            var lst = new List<FilePathStruct>(value.Count);

            lst.AddRange(value.Select(element => new FilePathStruct(element)));

            lst.Sort();

            return lst.ConvertAll(element => element.Path);
        }
    }
}