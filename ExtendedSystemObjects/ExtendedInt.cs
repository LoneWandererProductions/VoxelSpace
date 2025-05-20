/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/ExtendedInt.cs
 * PURPOSE:     Some Extensions for int
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global

using System;

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     Some extensions for int
    /// </summary>
    public static class ExtendedInt
    {
        /// <summary>
        ///     Intervals the specified value.
        /// </summary>
        /// <param name="i">The int we want to check.</param>
        /// <param name="value">The value we are comparing.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>If value is in the interval</returns>
        public static bool Interval(this int i, int value, int interval)
        {
            return i - interval <= value && value <= i + interval;
        }

        /// <summary>
        ///     Converts to binary.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns>int as binary string</returns>
        public static string ToBinary(this int i)
        {
            //2 is the new base, in our case binary, from decimal
            return Convert.ToString(i, 2);
        }

        /// <summary>
        ///     Binaries to int.
        /// </summary>
        /// <param name="binary">The binary.</param>
        /// <returns>a binary string to decimal</returns>
        public static int BinaryToInt(this string binary)
        {
            //2 was the base, convert back to decimal
            return Convert.ToInt32(binary, 2);
        }

        /// <summary>
        ///     Converts to base.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="toBase">To base.</param>
        /// <returns>int as target number string</returns>
        public static string ToBase(this int i, int toBase)
        {
            //the toBase is the new base for our number system, from decimal
            return Convert.ToString(i, toBase);
        }

        /// <summary>
        ///     Binaries to base.
        /// </summary>
        /// <param name="binary">The binary.</param>
        /// <param name="toBase">To base.</param>
        /// <returns>a binary string to target number system</returns>
        public static int BinaryToBase(this string binary, int toBase)
        {
            //2 was the base, convert back to base
            return Convert.ToInt32(binary, toBase);
        }

        /// <summary>
        ///     Rounds a float to int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Rounded  up float to int value.</returns>
        public static int RoundToInt(this float value)
        {
            if (value < 0)
            {
                return (int)(value - 0.5f); // round down for negative values
            }

            return (int)(value + 0.5f); // round up for positive values
        }
    }
}
