/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/FastMath.cs
 * PURPOSE:     Simple optimized Math Functions
 * PROGRAMMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Numerics;

namespace Mathematics
{
    /// <summary>
    ///     Some faster if less accurate calculations
    /// </summary>
    public static class FastMath
    {
        /// <summary>
        ///     The pi constant
        /// </summary>
        private const float Pi = (float)Math.PI;

        /// <summary>
        ///     The two pi constant
        /// </summary>
        private const float TwoPi = 2 * Pi;

        /// <summary>
        ///     Fast approximation of sin(x) using a cubic polynomial.
        /// </summary>
        public static float FastSin(float x)
        {
            x -= (int)(x / TwoPi) * TwoPi; // Faster modulo operation
            return x * (1.27323954f - 0.405284735f * x * x);
        }

        /// <summary>
        ///     Fast log base 2.
        /// </summary>
        public static int FastLog2(int x)
        {
            return x > 0 ? 31 - BitOperations.LeadingZeroCount((uint)x) : -1;
        }
    }
}