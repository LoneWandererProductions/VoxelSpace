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

namespace Mathematics
{
    /// <summary>
    ///     Adds some further improvements to certain Math functions
    /// </summary>
    public static partial class ExtendedMath
    {
        /// <summary>
        ///     Determines whether [is equal to] [the specified second].
        /// </summary>
        /// <param name="first">The first Parameter.</param>
        /// <param name="second">The second Parameter.</param>
        /// <param name="margin">The margin as int, 10^(-margin). Margin is always positive</param>
        /// <returns>
        ///     <c>true</c> if first is equal to the specified second; otherwise, <c>false</c>.
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
        ///     <c>true</c> if first is equal to the specified second; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEqualTo(this double first, double second)
        {
            return Math.Abs(first - second) < double.Epsilon;
        }

        /// <summary>
        ///     Calculate cos.
        ///     https://de.wikipedia.org/wiki/Radiant_(Einheit)
        /// </summary>
        /// <param name="degree">Degree we want to Rotate</param>
        /// <returns>The <see cref="double" />.</returns>
        public static double CalcCos(int degree)
        {
            double cos;

            if (Constants.CoSinus.ContainsKey(Math.Abs(degree)))
            {
                cos = Constants.CoSinus[Math.Abs(degree)];

                //catch negative degrees
                if (degree < 0) cos *= -1;
            }
            else
            {
                const double rad = Math.PI / 180.0;
                cos = Math.Cos(degree * rad);
            }

            return cos;
        }

        /// <summary>
        ///     Calculate sin.
        ///     https://de.wikipedia.org/wiki/Radiant_(Einheit)
        /// </summary>
        /// <param name="degree">Degree we want to Rotate</param>
        /// <returns>The <see cref="double" />.</returns>
        public static double CalcSin(int degree)
        {
            double sin;

            if (Constants.Sinus.ContainsKey(Math.Abs(degree)))
            {
                sin = Constants.Sinus[Math.Abs(degree)];

                //catch negative degrees
                if (degree < 0) sin *= -1;
            }
            else
            {
                const double rad = Math.PI / 180.0;
                sin = Math.Sin(degree * rad);
            }

            return sin;
        }
    }
}