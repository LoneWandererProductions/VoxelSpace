/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/Constants.cs
 * PURPOSE:     Some Basic Math Constants to ease the pain for the processor
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using System;
using System.Collections.Generic;

namespace Mathematics
{
    /// <summary>
    ///     Some Basic Math Constants
    /// </summary>
    public static class Constants
    {
        private static readonly double Two = Math.Sqrt(2);
        private static readonly double Three = Math.Sqrt(3) / 2;
        private static readonly double SixPlusTwo = (Math.Sqrt(6) + Two) / 4;
        private static readonly double SixMinusTwo = (Math.Sqrt(6) - Two) / 4;
        private static readonly double MinusSixMinusTwo = (-Math.Sqrt(6) - Two) / 4;
        private static readonly double MinusSixPlusTwo = (-Math.Sqrt(6) + Two) / 4;

        /// <summary>
        ///     Gets the sinus.
        ///     http://www2.hs-esslingen.de/~kamelzer/2011WS/Werte_sin_cos.pdf
        ///     https://de.wikipedia.org/wiki/Sinus_und_Kosinus
        ///     Without square Roots for now
        /// </summary>
        /// <value>
        ///     The sinus.
        /// </value>
        public static Dictionary<int, double> Sinus { get; } = new()
        {
            { 0, 0 },
            { 15, SixMinusTwo },
            { 30, 0.5 },
            { 45, 1 / Two },
            { 60, Three },
            { 75, SixPlusTwo },
            { 90, 1 },
            { 105, SixPlusTwo },
            { 120, Three },
            { 135, 1 / Two },
            { 150, 0.5 },
            { 165, SixMinusTwo },
            { 180, 0 },
            { 195, MinusSixPlusTwo },
            { 210, -0.5 },
            { 225, -1 / Two },
            { 240, -Three },
            { 255, MinusSixMinusTwo },
            { 270, -1 },
            { 285, MinusSixMinusTwo },
            { 300, -Three },
            { 315, -1 / Two },
            { 330, -0.5 },
            { 345, MinusSixPlusTwo },
            { 360, 0 }
        };

        /// <summary>
        ///     Gets the co sinus.
        ///     http://www2.hs-esslingen.de/~kamelzer/2011WS/Werte_sin_cos.pdf
        ///     https://de.wikipedia.org/wiki/Sinus_und_Kosinus
        ///     Without square Roots for now
        /// </summary>
        /// <value>
        ///     The co sinus.
        /// </value>
        public static Dictionary<int, double> CoSinus { get; } = new()
        {
            { 0, 1 },
            { 15, SixPlusTwo },
            { 30, Three },
            { 45, 1 / Two },
            { 60, 0.5 },
            { 75, SixMinusTwo },
            { 90, 0 },
            { 105, MinusSixPlusTwo },
            { 120, -0.5 },
            { 135, -1 / Two },
            { 150, -Three / 2 },
            { 165, MinusSixMinusTwo },
            { 180, -1 },
            { 195, MinusSixMinusTwo },
            { 210, -Three },
            { 225, -1 / Two },
            { 240, -0.5 },
            { 255, MinusSixPlusTwo },
            { 270, 0 },
            { 285, SixMinusTwo },
            { 300, 0.5 },
            { 315, 1 / Two },
            { 330, Three },
            { 345, SixPlusTwo },
            { 360, 1 }
        };
    }
}
