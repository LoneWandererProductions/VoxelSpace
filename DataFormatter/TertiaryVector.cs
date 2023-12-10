/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/TertiaryVector.cs
 * PURPOSE:     A really basic obj that holds three double values, needed for obj Files
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace DataFormatter
{
    /// <summary>
    ///     Three Numbers, here it will describe a 3dimensional Vector
    /// </summary>
    public sealed class TertiaryVector
    {
        /// <summary>
        ///     Gets the x.
        /// </summary>
        /// <value>
        ///     The x.
        /// </value>
        public double X { get; init; }

        /// <summary>
        ///     Gets the y.
        /// </summary>
        /// <value>
        ///     The y.
        /// </value>
        public double Y { get; init; }

        /// <summary>
        ///     Gets the z.
        /// </summary>
        /// <value>
        ///     The z.
        /// </value>
        public double Z { get; init; }
    }
}