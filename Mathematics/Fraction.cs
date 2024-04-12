/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/Fraction.cs
 * PURPOSE:     Helper class that helps with some basic Fraction Operations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://dotnet-snippets.de/snippet/klasse-bruchrechnung-class-fraction/12049
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable NonReadonlyMemberInGetHashCode

using System;

namespace Mathematics
{
    /// <summary>
    ///     Basic fraction calculations
    /// </summary>
    public static partial class ExtendedMath
    {
        /// <inheritdoc />
        /// <summary>
        ///     Basic Fraction Object
        /// </summary>
        public sealed class Fraction : IEquatable<Fraction>
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="Fraction" /> class.
            /// </summary>
            /// <param name="numerator">The numerator.</param>
            /// <param name="denominator">The denominator.</param>
            /// <exception cref="DivideByZeroException">Math Exception division by zero</exception>
            public Fraction(int numerator, int denominator)
            {
                if (denominator == 0)
                {
                    throw new DivideByZeroException();
                }

                Numerator = numerator;
                Denominator = denominator;
                Exponent = 0;
                Reduce();
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Fraction" /> class.
            /// </summary>
            /// <param name="numerator">The numerator.</param>
            /// <param name="denominator">The denominator.</param>
            /// <param name="exponent">The exponent.</param>
            /// <exception cref="DivideByZeroException">Math Exception division by zero</exception>
            public Fraction(int numerator, int denominator, int exponent)
            {
                if (denominator == 0)
                {
                    throw new DivideByZeroException();
                }

                Denominator = denominator;
                Numerator = numerator;
                Exponent = exponent;
                Reduce();
            }

            /// <summary>
            ///     Gets the numerator.
            /// </summary>
            /// <value>
            ///     The numerator.
            /// </value>
            public int Numerator { get; private set; }

            /// <summary>
            ///     Gets the denominator.
            /// </summary>
            /// <value>
            ///     The denominator.
            /// </value>
            public int Denominator { get; private set; }

            /// <summary>
            ///     Gets or sets the exponent.
            /// </summary>
            /// <value>
            ///     The exponent.
            /// </value>
            public int Exponent { get; private set; }

            /// <summary>
            ///     Gets or sets the exponent.
            ///     numerator / Denominator
            /// </summary>
            /// <value>
            ///     The exponent.
            /// </value>
            public int ExponentNumerator
            {
                get
                {
                    if (Exponent == 0)
                    {
                        return Numerator;
                    }

                    if (Math.Abs(Denominator) == 1)
                    {
                        return Exponent * Numerator;
                    }

                    if (Exponent == 0)
                    {
                        return Numerator;
                    }

                    //catch negative exponent
                    var exponentNumerator = Math.Abs(Exponent * Denominator) + Numerator;
                    if (Exponent < 0)
                    {
                        return exponentNumerator * -1;
                    }

                    return exponentNumerator;
                }
            }

            /// <summary>
            ///     Gets the decimal.
            /// </summary>
            /// <value>
            ///     The decimal.
            /// </value>
            public decimal Decimal => ExponentNumerator / (decimal)Denominator;

            /// <inheritdoc />
            /// <summary>
            ///     Compares If a Fraction is equal to another Fraction
            /// </summary>
            /// <param name="other">other Coordinate</param>
            /// <returns>True if equal, false if not</returns>
            public bool Equals(Fraction other)
            {
                return Numerator == other?.Numerator && Denominator == other.Denominator && Exponent == other.Exponent;
            }

            /// <summary>
            ///     Determines whether the specified <see cref="object" />, is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
            /// <returns>
            ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                return obj is Fraction other && Equals(other);
            }

            /// <summary>
            ///     Implements the operator *.
            /// </summary>
            /// <param name="first">The first fraction.</param>
            /// <param name="second">The second fraction.</param>
            /// <returns>
            ///     The result of the operator.
            /// </returns>
            public static Fraction operator *(Fraction first, Fraction second)
            {
                return new Fraction(first.ExponentNumerator * second.ExponentNumerator,
                    first.Denominator * second.Denominator);
            }

            /// <summary>
            ///     Implements the operator /.
            /// </summary>
            /// <param name="first">The first fraction.</param>
            /// <param name="second">The second fraction.</param>
            /// <returns>
            ///     The result of the operator.
            /// </returns>
            public static Fraction operator /(Fraction first, Fraction second)
            {
                return new Fraction(first.ExponentNumerator * second.Denominator,
                    first.Denominator * second.ExponentNumerator);
            }

            /// <summary>
            ///     Implements the operator +.
            /// </summary>
            /// <param name="first">The first fraction.</param>
            /// <param name="second">The second fraction.</param>
            /// <returns>
            ///     The result of the operator.
            /// </returns>
            public static Fraction operator +(Fraction first, Fraction second)
            {
                return new Fraction((first.ExponentNumerator * second.Denominator) +
                                    (first.Denominator * second.ExponentNumerator),
                    first.Denominator * second.Denominator);
            }

            /// <summary>
            ///     Implements the operator -.
            /// </summary>
            /// <param name="first">The first fraction.</param>
            /// <param name="second">The second fraction.</param>
            /// <returns>
            ///     The result of the operator.
            /// </returns>
            public static Fraction operator -(Fraction first, Fraction second)
            {
                return new Fraction((first.ExponentNumerator * second.Denominator) -
                                    (first.Denominator * second.ExponentNumerator),
                    first.Denominator * second.Denominator);
            }

            /// <summary>
            ///     Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
            /// </returns>
            public override int GetHashCode()
            {
                return HashCode.Combine(Numerator, Denominator, Exponent);
            }

            /// <summary>
            ///     Reduces the Fraction.
            /// </summary>
            private void Reduce()
            {
                if (Numerator == 0 && Denominator != 0)
                {
                    Exponent = 0;
                    return;
                }

                if (Denominator == 0)
                {
                    Numerator = 0;
                    Denominator = 1;
                }

                if (Denominator < 0)
                {
                    Denominator *= -1;
                    Numerator *= -1;
                }

                var gcd = GetGcf(Numerator, Denominator);

                if (gcd != 0)
                {
                    Numerator /= gcd;
                    Denominator /= gcd;
                }

                if (Denominator == 1)
                {
                    Exponent = Numerator * Exponent;
                    Denominator = 1;
                    return;
                }

                if (Numerator <= Denominator)
                {
                    return;
                }

                var modulo = Numerator % Denominator;
                Exponent = (Numerator - modulo) / Denominator;
                Numerator = modulo;

                if (Numerator != 0)
                {
                    return;
                }

                Denominator = 1;
                Numerator = 1;
            }

            /// <summary>
            ///     Gets the greatest common Factor.
            ///     https://en.wikipedia.org/wiki/Greatest_common_divisor
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns>Greatest common Factor.</returns>
            private static int GetGcf(int a, int b)
            {
                while (true)
                {
                    if (a == b || b == 0)
                    {
                        return a;
                    }

                    var a1 = a;
                    a = b;
                    b = a1 % b;
                }
            }
        }
    }
}
