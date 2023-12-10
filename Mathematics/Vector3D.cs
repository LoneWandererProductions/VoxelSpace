/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/Vector3D.cs
 * PURPOSE:     Basic 3D Vector implementation
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;

namespace Mathematics
{
    /// <summary>
    ///     Basic Vector Implementation
    /// </summary>
    public sealed class Vector3D
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector3D" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector3D" /> class.
        /// </summary>
        public Vector3D()
        {
        }

        /// <summary>
        ///     Gets or sets the x.
        /// </summary>
        /// <value>
        ///     The x.
        /// </value>
        public double X { get; set; }

        /// <summary>
        ///     Gets or sets the y.
        /// </summary>
        /// <value>
        ///     The y.
        /// </value>
        public double Y { get; set; }

        /// <summary>
        ///     Gets or sets the z.
        /// </summary>
        /// <value>
        ///     The z.
        /// </value>
        public double Z { get; set; }

        /// <summary>
        ///     Gets the rounded x.
        /// </summary>
        /// <value>
        ///     The rounded x.
        /// </value>
        public int RoundedX => (int) Math.Round(X, 0);

        /// <summary>
        ///     Gets the rounded y.
        /// </summary>
        /// <value>
        ///     The rounded y.
        /// </value>
        public int RoundedY => (int) Math.Round(Y, 0);

        /// <summary>
        ///     Gets the rounded z.
        /// </summary>
        /// <value>
        ///     The rounded z.
        /// </value>
        public int RoundedZ => (int) Math.Round(Z, 0);

        /// <summary>
        ///     Equals the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>Equal or not</returns>
        public bool Equals(Vector3D other)
        {
            return X.Equals(other?.X) && Y.Equals(other?.Y) && Z.Equals(other?.Z);
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
            return obj is Vector3D other && Equals(other);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        /// <summary>
        ///     Converts to text.
        /// </summary>
        /// <returns>string representation of the Vector</returns>
        public string ToText()
        {
            return string.Concat("X: ", X, " Y: ", Y, " Z: ", Z);
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector3D operator +(Vector3D first, Vector3D second)
        {
            return new(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector3D operator -(Vector3D first, Vector3D second)
        {
            return new(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static double operator *(Vector3D first, Vector3D second)
        {
            return (first.X * second.X) + (first.Y * second.Y) + (first.Z * second.Z);
        }

        /// <summary>
        ///     Implements the operator *. Cross Product
        /// </summary>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The Cross Product of the Vectors.
        /// </returns>
        public Vector3D CrossProduct(Vector3D second)
        {
            return new()
            {
                X = (Y * second.Z) - (Z * second.Y),
                Y = (Z * second.X) - (X * second.Z),
                Z = (X * second.Y) - (Y * second.X)
            };
        }


        /// <summary>
        ///     Multiplies the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public Vector3D Multiply(double value)
        {
            return new() {X = X * value, Y = Y * value, Z = Z * value};
        }

        /// <summary>
        ///     Vectors the length.
        /// </summary>
        /// <returns>Length of the Vector</returns>
        public double VectorLength()
        {
            return Math.Sqrt(this * this);
        }

        /// <summary>
        ///     Normalizes this instance.
        /// </summary>
        /// <returns>Normalized Vector</returns>
        public Vector3D Normalize()
        {
            var l = VectorLength();
            return new Vector3D {X = X / l, Y = Y / l, Z = Z / l};
        }
    }
}