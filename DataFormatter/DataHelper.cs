/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/DataHelper.cs
 * PURPOSE:     Basic stuff shared over all operations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.Linq;

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
        /// <param name="separator"></param>
        /// <returns>split Parts</returns>
        internal static List<string> GetParts(string str, char separator)
        {
            return str.Split(separator).ToList();
        }
    }
}
