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
        private static readonly double OneOverTwo = 1 / Math.Sqrt(2);
        private static readonly double SixPlusTwo = (Math.Sqrt(6) + Two) / 4;
        private static readonly double SixMinusTwo = (Math.Sqrt(6) - Two) / 4;
        private static readonly double MinusSixMinusTwo = (-Math.Sqrt(6) - Two) / 4;
        private static readonly double MinusSixPlusTwo = (-Math.Sqrt(6) + Two) / 4;

        /// <summary>
        ///     Predefined tangent values for common angles
        ///     The tangents
        /// </summary>
        public static readonly Dictionary<int, double> Tangents = new()
        {
            { 0, 0.0 },
            { 30, Math.Tan(30 * Math.PI / 180.0) }, // √3 / 3
            { 45, 1.0 },
            { 60, Math.Tan(60 * Math.PI / 180.0) }, // √3
            { 90, double.NaN }, // Undefined
            { 120, -Math.Tan(60 * Math.PI / 180.0) },
            { 135, -1.0 },
            { 150, -Math.Tan(30 * Math.PI / 180.0) },
            { 180, 0.0 },
            { 270, double.NaN }, // Undefined
            { 360, 0.0 }
        };

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
            { 45, OneOverTwo },
            { 60, Three },
            { 75, SixPlusTwo },
            { 90, 1 },
            { 105, SixPlusTwo },
            { 120, Three },
            { 135, OneOverTwo },
            { 150, 0.5 },
            { 165, SixMinusTwo },
            { 180, 0 },
            { 195, MinusSixPlusTwo },
            { 210, -0.5 },
            { 225, -OneOverTwo },
            { 240, -Three },
            { 255, MinusSixMinusTwo },
            { 270, -1 },
            { 285, MinusSixMinusTwo },
            { 300, -Three },
            { 315, -OneOverTwo },
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
            { -180, -1 },
            { -165, MinusSixMinusTwo },
            { -150, -Three },
            { -135, -OneOverTwo },
            { -120, -0.5 },
            { -105, MinusSixPlusTwo },
            { -90, 0 },
            { -75, SixMinusTwo },
            { -60, 0.5 },
            { -45, OneOverTwo },
            { -30, Three },
            { -15, SixPlusTwo },
            { 0, 1 },
            { 15, SixPlusTwo },
            { 30, Three },
            { 45, OneOverTwo },
            { 60, 0.5 },
            { 75, SixMinusTwo },
            { 90, 0 },
            { 105, MinusSixPlusTwo },
            { 120, -0.5 },
            { 135, -OneOverTwo },
            { 150, -Three },
            { 165, MinusSixMinusTwo },
            { 180, -1 },
            { 195, MinusSixMinusTwo },
            { 210, -Three },
            { 225, -OneOverTwo },
            { 240, -0.5 },
            { 255, MinusSixPlusTwo },
            { 270, 0 },
            { 285, SixMinusTwo },
            { 300, 0.5 },
            { 315, OneOverTwo },
            { 330, Three },
            { 345, SixPlusTwo },
            { 360, 1 }
        };
    }
}
