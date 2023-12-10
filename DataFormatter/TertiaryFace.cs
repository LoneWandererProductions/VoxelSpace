/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/TertiaryFace.cs
 * PURPOSE:     Basic Object that holds three int values, used for obj File
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeInternal

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
    }
}