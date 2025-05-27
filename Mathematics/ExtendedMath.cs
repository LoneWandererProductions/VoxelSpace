/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/ExtendedMath.cs
 * PURPOSE:     Helper class that extends some Math functions, mostly comparing double values
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://stackoverflow.com/questions/961038/how-do-i-properly-write-math-extension-methods-for-int-double-float-etc
 */

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeInternal

using System;
using System.Runtime.CompilerServices;

namespace Mathematics
{
    /// <summary>
    ///     Adds some further improvements to certain Math functions
    /// </summary>
    public static class ExtendedMath
    {
        /// <summary>
        ///     The sin f lookup
        /// </summary>
        private static readonly float[] SinFLookup = new float[360];

        /// <summary>
        ///     The sin d lookup
        /// </summary>
        private static readonly double[] SinDLookup = new double[360];

        /// <summary>
        ///     The cos f lookup
        /// </summary>
        private static readonly float[] CosFLookup = new float[360];

        /// <summary>
        ///     The cos d lookup
        /// </summary>
        private static readonly double[] CosDLookup = new double[360];

        /// <summary>
        ///     The tan f lookup
        /// </summary>
        private static readonly float[] TanFLookup = new float[360];

        /// <summary>
        ///     The tan d lookup
        /// </summary>
        private static readonly double[] TanDLookup = new double[360];

        /// <summary>
        ///     Initializes the <see cref="ExtendedMath" /> class.
        ///     Precalculate all necessary values.
        /// </summary>
        static ExtendedMath()
        {
            for (var degree = 0; degree < 360; degree++)
            {
                // SIN Lookup
                if (Constants.Sinus.TryGetValue(degree, out var sinValue))
                {
                    SinDLookup[degree] = sinValue;
                    SinFLookup[degree] = (float)sinValue;
                }
                else
                {
                    SinDLookup[degree] = Math.Sin(degree * Math.PI / 180.0);
                    SinFLookup[degree] = (float)SinDLookup[degree];
                }

                // COS Lookup
                if (Constants.CoSinus.TryGetValue(degree, out var cosValue))
                {
                    CosDLookup[degree] = cosValue;
                    CosFLookup[degree] = (float)cosValue;
                }
                else
                {
                    CosDLookup[degree] = Math.Cos(degree * Math.PI / 180.0);
                    CosFLookup[degree] = (float)CosDLookup[degree];
                }

                // TAN Lookup
                if (Constants.Tangents.TryGetValue(degree, out var tanValue))
                {
                    TanDLookup[degree] = tanValue;
                    TanFLookup[degree] = (float)tanValue;
                }
                else
                {
                    // Handle TAN for undefined values (90° and 270°)
                    if (degree % 180 == 90)
                    {
                        TanDLookup[degree] = double.PositiveInfinity;
                        TanFLookup[degree] = float.PositiveInfinity;
                    }
                    else
                    {
                        TanDLookup[degree] = Math.Tan(degree * Math.PI / 180.0);
                        TanFLookup[degree] = (float)TanDLookup[degree];
                    }
                }
            }
        }

        /// <summary>
        ///     Calculate cos.
        ///     https://de.wikipedia.org/wiki/Radiant_(Einheit)
        /// </summary>
        /// <param name="degree">Degree we want to Rotate</param>
        /// <returns>The <see cref="double" /> The radial Value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CalcCos(int degree)
        {
            return CosDLookup[NormalizeAngle(degree)];
        }

        /// <summary>
        ///     Calculate sin.
        ///     https://de.wikipedia.org/wiki/Radiant_(Einheit)
        /// </summary>
        /// <param name="degree">Degree we want to Rotate</param>
        /// <returns>The <see cref="double" /> The radial Value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CalcSin(int degree)
        {
            return SinDLookup[NormalizeAngle(degree)];
        }

        /// <summary>
        ///     Calculate tangent.
        ///     https://de.wikipedia.org/wiki/Radiant_(Einheit)
        /// </summary>
        /// <param name="degree">Degree we want to compute the tangent for</param>
        /// <returns>The <see cref="double" />.</returns>
        /// <exception cref="DivideByZeroException">Thrown when attempting to calculate tangent for angles where cosine is zero.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CalcTan(int degree)
        {
            return TanDLookup[NormalizeAngle(degree)];
        }

        /// <summary>
        ///     Calculates the cos as float. Variation of CalcCos.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns>The <see cref="float" /> The radial Value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcCosF(int degree)
        {
            return CosFLookup[NormalizeAngle(degree)];
        }

        /// <summary>
        ///     Calculates the sin float. Variation of CalcSin.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns>The <see cref="float" /> The radial Value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcSinF(int degree)
        {
            return SinFLookup[NormalizeAngle(degree)];
        }

        /// <summary>
        ///     Calculates the tan float. Variation of CalcTan.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns>The <see cref="float" /> The radial Value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcTanF(int degree)
        {
            return TanFLookup[NormalizeAngle(degree)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int NormalizeAngle(int degrees)
        {
            degrees %= 360;
            return degrees < 0 ? degrees + 360 : degrees;
        }
    }
}