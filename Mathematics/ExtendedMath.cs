/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/ExtendedMath.cs
 * PURPOSE:     Helper class that extends some Math functions, mostly comparing double values
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://stackoverflow.com/questions/961038/how-do-i-properly-write-math-extension-methods-for-int-double-float-etc
 */

// ReSharper disable UnusedMember.Global

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
        ///     Calculate cos.
        ///     https://de.wikipedia.org/wiki/Radiant_(Einheit)
        /// </summary>
        /// <param name="degree">Degree we want to Rotate</param>
        /// <returns>The <see cref="double" /> The radial Value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CalcCos(int degree)
        {
            // Use the absolute value of the degree for cosine lookup.
            var lookupDegree = Math.Abs(degree);

            // Look up the cosine value in the dictionary for the positive angle.
            if (Constants.CoSinus.ContainsKey(lookupDegree))
            {
                return Constants.CoSinus[lookupDegree];
            }

            // If the value is not found, calculate it normally.
            const double rad = Math.PI / 180.0;
            return Math.Cos(degree * rad);
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
            double sin;

            if (Constants.Sinus.ContainsKey(degree))
            {
                sin = Constants.Sinus[Math.Abs(degree)];

                //catch negative degrees
                if (degree < 0)
                {
                    sin *= -1;
                }
            }
            else
            {
                const double rad = Math.PI / 180.0;
                sin = Math.Sin(degree * rad);
            }

            return sin;
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
            // Normalize degree to [0, 360)
            var normalizedDegree = ((degree % 360) + 360) % 360;

            // Check if the angle is predefined in the lookup table
            if (Constants.Tangents.ContainsKey(normalizedDegree))
            {
                var tangent = Constants.Tangents[normalizedDegree];
                if (double.IsNaN(tangent))
                {
                    throw new DivideByZeroException($"Tangent is undefined for {degree} degrees.");
                }

                return tangent;
            }

            // Calculate tangent for other angles
            const double rad = Math.PI / 180.0;
            return Math.Tan(normalizedDegree * rad);
        }

        /// <summary>
        /// Calculates the cos as float. Variation of CalcCos.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns>The <see cref="float" /> The radial Value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcCosF(int degree) => (float)CalcCos(degree);

        /// <summary>
        /// Calculates the sin float. Variation of CalcSin.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns>The <see cref="float" /> The radial Value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcSinF(int degree) => (float)CalcSin(degree);

        /// <summary>
        /// Calculates the tan float. Variation of CalcTan.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns>The <see cref="float" /> The radial Value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcTanF(int degree) => (float)CalcTan(degree);
    }
}
