/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileSearchExtension.cs
 * PURPOSE:     Extension for FileSearch
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
    public static class FileSearchExtension
    {
        /// <summary>
        ///     The regex
        /// </summary>
        private static readonly Regex Regex = new(@"(\d+)|(\D+)");

        /// <summary>
        ///     Sorts the numbers first.
        /// </summary>
        /// <param name="lst">The list we want to sort.</param>
        /// <returns>Ordered FileNames</returns>
        public static List<string> PathSortAlphaNumerical(this IEnumerable<string> lst)
        {
            return lst?.OrderBy(Path.GetFileName).CustomSort().ToList();
        }

        /// <summary>
        ///     Custom Sort, internal helper only avalable though extension
        /// </summary>
        /// <param name="lst">The list of files.</param>
        /// <returns>Ordered List of files.</returns>
        private static IEnumerable<string> CustomSort(this IEnumerable<string> lst)
        {
            if (!lst.Any())
            {
                return lst;
            }

            var enumerable = lst.ToList();
            var maxLen = enumerable.Max(s => s.Length);

            return enumerable.Select(s => new
                {
                    OrgStr = s,
                    SortStr = Regex.Replace(s,
                        m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
                })
                .OrderBy(x => x.SortStr)
                .Select(x => x.OrgStr);
        }
    }
}
