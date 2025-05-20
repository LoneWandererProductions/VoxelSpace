/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/Constants.cs
 * PURPOSE:     Some Basic Math Constants to increase the correct values.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using System;
using System.Collections.Generic;

namespace Mathematics
{
    /// <summary>
    ///     Some Basic Math Constants
    ///     For future events will be extended
    /// </summary>
    public static class Constants
    {
        /// <summary>
        ///     The SQRT2
        /// </summary>
        private static readonly double Sqrt2 = Math.Sqrt(2);

        /// <summary>
        ///     The half SQRT3
        /// </summary>
        private static readonly double HalfSqrt3 = Math.Sqrt(3) / 2;

        /// <summary>
        ///     The reciprocal SQRT2
        /// </summary>
        private static readonly double ReciprocalSqrt2 = 1 / Math.Sqrt(2);

        /// <summary>
        ///     The SQRT6 plus SQRT2
        /// </summary>
        private static readonly double Sqrt6PlusSqrt2 = (Math.Sqrt(6) + Sqrt2) / 4;

        /// <summary>
        ///     The SQRT6 minus SQRT2
        /// </summary>
        private static readonly double Sqrt6MinusSqrt2 = (Math.Sqrt(6) - Sqrt2) / 4;

        /// <summary>
        ///     The negative SQRT6 minus SQRT2
        /// </summary>
        private static readonly double NegativeSqrt6MinusSqrt2 = (-Math.Sqrt(6) - Sqrt2) / 4;

        /// <summary>
        ///     The negative SQRT6 plus SQRT2
        /// </summary>
        private static readonly double NegativeSqrt6PlusSqrt2 = (-Math.Sqrt(6) + Sqrt2) / 4;

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
        ///     Some primes for use
        /// </summary>
        public static readonly int[] SmallPrimes =
        {
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101,
            103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211,
            223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337,
            347, 349, 353, 359, 367, 373, 379, 383, 389, 397
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
            { 15, Sqrt6MinusSqrt2 },
            { 30, 0.5 },
            { 45, ReciprocalSqrt2 },
            { 60, HalfSqrt3 },
            { 75, Sqrt6PlusSqrt2 },
            { 90, 1 },
            { 105, Sqrt6PlusSqrt2 },
            { 120, HalfSqrt3 },
            { 135, ReciprocalSqrt2 },
            { 150, 0.5 },
            { 165, Sqrt6MinusSqrt2 },
            { 180, 0 },
            { 195, NegativeSqrt6PlusSqrt2 },
            { 210, -0.5 },
            { 225, -ReciprocalSqrt2 },
            { 240, -HalfSqrt3 },
            { 255, NegativeSqrt6MinusSqrt2 },
            { 270, -1 },
            { 285, NegativeSqrt6MinusSqrt2 },
            { 300, -HalfSqrt3 },
            { 315, -ReciprocalSqrt2 },
            { 330, -0.5 },
            { 345, NegativeSqrt6PlusSqrt2 },
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
            { -165, NegativeSqrt6MinusSqrt2 },
            { -150, -HalfSqrt3 },
            { -135, -ReciprocalSqrt2 },
            { -120, -0.5 },
            { -105, NegativeSqrt6PlusSqrt2 },
            { -90, 0 },
            { -75, Sqrt6MinusSqrt2 },
            { -60, 0.5 },
            { -45, ReciprocalSqrt2 },
            { -30, HalfSqrt3 },
            { -15, Sqrt6PlusSqrt2 },
            { 0, 1 },
            { 15, Sqrt6PlusSqrt2 },
            { 30, HalfSqrt3 },
            { 45, ReciprocalSqrt2 },
            { 60, 0.5 },
            { 75, Sqrt6MinusSqrt2 },
            { 90, 0 },
            { 105, NegativeSqrt6PlusSqrt2 },
            { 120, -0.5 },
            { 135, -ReciprocalSqrt2 },
            { 150, -HalfSqrt3 },
            { 165, NegativeSqrt6MinusSqrt2 },
            { 180, -1 },
            { 195, NegativeSqrt6MinusSqrt2 },
            { 210, -HalfSqrt3 },
            { 225, -ReciprocalSqrt2 },
            { 240, -0.5 },
            { 255, NegativeSqrt6PlusSqrt2 },
            { 270, 0 },
            { 285, Sqrt6MinusSqrt2 },
            { 300, 0.5 },
            { 315, ReciprocalSqrt2 },
            { 330, HalfSqrt3 },
            { 345, Sqrt6PlusSqrt2 },
            { 360, 1 }
        };
    }
}
