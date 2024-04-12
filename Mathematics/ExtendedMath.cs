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
                if (degree < 0)
                {
                    cos *= -1;
                }
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
    }
}
