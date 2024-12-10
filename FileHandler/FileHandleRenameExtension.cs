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
        /// <returns>string with the removed appendage</returns>
        public static string RemoveAppendage(this string str, string appendage)
        {
            return !str.StartsWith(appendage, StringComparison.OrdinalIgnoreCase)
                ? str
                : str.Remove(0, appendage.Length);
        }

        /// <summary>
        ///     Adds the appendage.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="appendage">The appendage.</param>
        /// <returns>string with added appendage</returns>
        public static string AddAppendage(this string str, string appendage)
        {
            return str.StartsWith(appendage, StringComparison.OrdinalIgnoreCase) ? str : string.Concat(appendage, str);
        }

        /// <summary>
        ///     Replaces the part.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="targetStr">The target string.</param>
        /// <param name="update">The update string.</param>
        /// <returns>string with replaced substring</returns>
        public static string ReplacePart(this string str, string targetStr, string update)
        {
            if (string.IsNullOrEmpty(targetStr)) return str;

            return !str.Contains(targetStr) ? str : str.Replace(targetStr, update);
        }

        /// <summary>
        ///     Reorders Numbers in a string and appends them.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>New string</returns>
        public static string ReOrderNumbers(this string str)
        {
            var charsToRemove = Regex.Split(str);
            var numbers = string.Concat(charsToRemove);

            return string.Concat(
                charsToRemove.Where(c => !string.IsNullOrEmpty(c))
                    .Aggregate(str, (current, c) => current.Replace(c, string.Empty)), "_", numbers);
        }
    }
}