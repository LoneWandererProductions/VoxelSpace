/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/ExtendedDouble.cs
 * PURPOSE:     Some Extensions for double
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global

using System;

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     double class extension
    /// </summary>
    public static class ExtendedDouble
    {
        /// <summary>
        ///     Determines whether [is equal to] [the specified second].
        /// </summary>
        /// <param name="first">The first Parameter.</param>
        /// <param name="second">The second Parameter.</param>
        /// <param name="margin">The margin as int, 10^(-margin). Margin is always positive</param>
        /// <returns>
        ///     <c>true</c> if first is equal to the specified second; currentSequence-wise, <c>false</c>.
        /// </returns>
        public static bool IsEqualTo(this double first, double second, int margin)
        {
            var variance = Math.Pow(10, -Math.Abs(margin));
            return Math.Abs(first - second) < variance;
        }

        /// <summary>
        ///     Determines whether [is equal to] [the specified second].
        /// </summary>
        /// <param name="first">The first Parameter.</param>
        /// <param name="second">The second Parameter.</param>
        /// <returns>
        ///     <c>true</c> if first is equal to the specified second; currentSequence-wise, <c>false</c>.
        /// </returns>
        public static bool IsEqualTo(this double first, double second)
        {
            return Math.Abs(first - second) < double.Epsilon;
        }
    }
}
