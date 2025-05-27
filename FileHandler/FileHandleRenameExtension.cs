/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileHandleRenameExtension.cs
 * PURPOSE:     Extension for FileHandleRename
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileHandler
{
    /// <summary>
    ///     Some string Extensions
    /// </summary>
    public static class FileHandleRenameExtension
    {
        /// <summary>
        ///     The regex Instance
        /// </summary>
        private static readonly Regex Regex = new(@"\D+");

        /// <summary>
        ///     Removes the appendage.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="appendage">The appendage.</param>
        /// <param name="comparison">The string comparison option.</param>
        /// <returns>
        ///     string with the removed appendage
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     str or appendage was empty
        /// </exception>
        public static string RemoveAppendage(this string str, string appendage,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            if (appendage == null) throw new ArgumentNullException(nameof(appendage));

            return !str.StartsWith(appendage, comparison)
                ? str
                : str.Remove(0, appendage.Length);
        }

        /// <summary>
        ///     Adds the appendage.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="appendage">The appendage.</param>
        /// <param name="comparison">The string comparison option.</param>
        /// <returns>
        ///     string with added appendage
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     str or appendage was empty
        /// </exception>
        public static string AddAppendage(this string str, string appendage,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            if (appendage == null) throw new ArgumentNullException(nameof(appendage));

            return str.StartsWith(appendage, comparison) ? str : string.Concat(appendage, str);
        }

        /// <summary>
        ///     Replaces the part.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="targetStr">The target string.</param>
        /// <param name="update">The update string.</param>
        /// <param name="comparison">The string comparison option.</param>
        /// <returns>
        ///     string with replaced substring
        /// </returns>
        /// <exception cref="ArgumentNullException">str was empty</exception>
        public static string ReplacePart(this string str, string targetStr, string update,
            StringComparison comparison = StringComparison.Ordinal)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            if (string.IsNullOrEmpty(targetStr)) return str;

            return !str.Contains(targetStr, comparison) ? str : str.Replace(targetStr, update);
        }

        /// <summary>
        ///     Reorders Numbers in a string and appends them.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>New string</returns>
        public static string ReOrderNumbers(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            var charsToRemove = Regex.Split(str);
            var numbers = string.Concat(charsToRemove);

            return string.Concat(
                charsToRemove.Where(c => !string.IsNullOrEmpty(c))
                    .Aggregate(str, (current, c) => current.Replace(c, string.Empty)), FileHandlerResources.Append,
                numbers);
        }
    }
}