/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/TertiaryFace.cs
 * PURPOSE:     Basic Object that holds three int values, used for obj File
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeInternal

using System;

namespace DataFormatter
{
    /// <summary>
    ///     Plot file for the vectors
    /// </summary>
    public sealed class TertiaryFace
    {
        /// <summary>
        ///     Gets the x.
        /// </summary>
        /// <value>
        ///     The x.
        /// </value>
        public int X { get; init; }

        /// <summary>
        ///     Gets the y.
        /// </summary>
        /// <value>
        ///     The y.
        /// </value>
        public int Y { get; init; }

        /// <summary>
        ///     Gets the z.
        /// </summary>
        /// <value>
        ///     The z.
        /// </value>
        public int Z { get; init; }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Concat(DataFormatterResources.StrX, X, DataFormatterResources.StrY, Y,
                DataFormatterResources.StrZ, Z);
        }
    }
}