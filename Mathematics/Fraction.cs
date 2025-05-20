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
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Fraction a, Fraction b)
        {
            return a.Equals(b);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Fraction a, Fraction b)
        {
            return !(a == b);
        }

        /// <summary>
        ///     Implements the operator &lt;.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator <(Fraction a, Fraction b)
        {
            return a.Decimal < b.Decimal;
        }

        /// <summary>
        ///     Implements the operator &gt;.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator >(Fraction a, Fraction b)
        {
            return a.Decimal > b.Decimal;
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
            // If the numerator is zero and the denominator is non-zero, the fraction is 0, so set Exponent to 0
            if (Numerator == 0)
            {
                Exponent = 0;
                return;
            }

            // If the denominator is zero (undefined fraction), reset to 0/1
            if (Denominator == 0)
            {
                Numerator = 0;
                Denominator = 1;
                return;
            }

            // Ensure the denominator is positive by flipping the signs of both numerator and denominator if necessary
            if (Denominator < 0)
            {
                Denominator *= -1;
                Numerator *= -1;
            }

            // Simplify the fraction by dividing both the numerator and denominator by their greatest common factor
            var gcd = GetGcf(Numerator, Denominator);
            if (gcd != 0)
            {
                Numerator /= gcd;
                Denominator /= gcd;
            }

            // If the denominator becomes 1, the fraction is an integer, and we adjust the Exponent accordingly
            if (Denominator == 1)
            {
                Exponent = Numerator * Exponent;
                return;
            }

            // If the numerator is less than or equal to the denominator, there's no need to reduce further
            if (Numerator <= Denominator)
            {
                return;
            }

            // If the numerator is greater than the denominator, we perform the division to extract the whole part (Exponent)
            var modulo = Numerator % Denominator;
            Exponent = (Numerator - modulo) / Denominator; // Integer part of the division
            Numerator = modulo; // The remainder becomes the new numerator

            // If the remainder is zero, we have an exact division and the fraction reduces to a whole number
            if (Numerator == 0)
            {
                Denominator = 1; // Set denominator to 1 for whole number
            }
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
            while (b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }

            return Math.Abs(a);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Fraction" /> to <see cref="System.Decimal" />.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator decimal(Fraction f)
        {
            return (decimal)f.Numerator / f.Denominator;
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Fraction" /> to <see cref="System.Double" />.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator double(Fraction f)
        {
            return (double)f.Numerator / f.Denominator;
        }
    }
}
