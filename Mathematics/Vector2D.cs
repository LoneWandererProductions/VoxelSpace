/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/Vector2D.cs
 * PURPOSE:     Basic 2D Vector implementation
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable PropertyCanBeMadeInitOnly.Global

using System;

namespace Mathematics
{
    /// <inheritdoc />
    /// <summary>
    ///     Basic Vector Implementation
    /// </summary>
    public sealed class Vector2D : ICloneable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2D" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2D" /> class.
        /// </summary>
        public Vector2D()
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
        ///     Gets the zero Vector.
        /// </summary>
        /// <value>
        ///     The zero Vector.
        /// </value>
        public static Vector2D ZeroVector
        {
            get;
        } = new(0d, 0d);

        /// <summary>
        ///     Gets the Unit vector.
        /// </summary>
        /// <value>
        ///     The Unit vector.
        /// </value>
        public static Vector2D UnitVector
        {
            get;
        } = new(1d, 1d);

        /// <summary>
        ///     Gets the rounded x.
        /// </summary>
        /// <value>
        ///     The rounded x.
        /// </value>
        public int RoundedX => (int)Math.Round(X, 0);

        /// <summary>
        ///     Gets the rounded y.
        /// </summary>
        /// <value>
        ///     The rounded y.
        /// </value>
        public int RoundedY => (int)Math.Round(Y, 0);

        /// <inheritdoc />
        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new Vector2D(X, Y);
        }

        /// <summary>
        ///     Equals the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>Equal or not</returns>
        public bool Equals(Vector2D other)
        {
            return X.Equals(other?.X) && Y.Equals(other?.Y);
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
            return obj is Vector2D other && Equals(other);
        }

        /// <summary>
        ///     Converts to text.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Concat(MathResources.StrX, X, MathResources.StrY, Y);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Vector2D first, Vector2D second)
        {
            return second is not null && first is not null && Math.Abs(first.X - second.X) < MathResources.Tolerance &&
                   Math.Abs(first.Y - second.Y) < MathResources.Tolerance;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Vector2D first, Vector2D second)
        {
            return second is not null && first is not null && (Math.Abs(first.X - second.X) > MathResources.Tolerance ||
                                                               Math.Abs(first.Y - second.Y) > MathResources.Tolerance);
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector2D operator +(Vector2D first, Vector2D second)
        {
            return new Vector2D(first.X + second.X, first.Y + second.Y);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector2D operator -(Vector2D first, Vector2D second)
        {
            return new Vector2D(first.X - second.X, first.Y - second.Y);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector2D operator -(Vector2D first)
        {
            return new Vector2D(-first.X, -first.Y);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static double operator *(Vector2D first, Vector2D second)
        {
            return (first.X * second.X) + (first.Y * second.Y);
        }

        /// <summary>
        ///     Implements the operator *.
        ///     Multiplication Factors can be switched.
        /// </summary>
        /// <param name="first">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector2D operator *(Vector2D first, double scalar)
        {
            return new Vector2D(first.X * scalar, first.Y * scalar);
        }

        /// <summary>
        ///     Implements the operator *.
        ///     Multiplication Factors can be switched.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="first">The vector.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector2D operator *(double scalar, Vector2D first)
        {
            return first * scalar;
        }

        /// <summary>
        ///     Implements the operator /.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector2D operator /(Vector2D v, double scalar)
        {
            return new Vector2D(v.X / scalar, v.Y / scalar);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Vector2D" /> to <see cref="Coordinate2D" />.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Coordinate2D(Vector2D first)
        {
            return new Coordinate2D(first.RoundedX, first.RoundedY);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Vector3D" /> to <see cref="Vector2D" />.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Vector2D(Vector3D first)
        {
            return new Vector2D(first.X, first.Y);
        }

        /// <summary>
        ///     Implements the operator *. Cross Product
        /// </summary>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The Cross Product of the Vectors.
        /// </returns>
        public double CrossProduct(Vector2D second)
        {
            return (X * second.Y) - (Y * second.X);
        }

        /// <summary>
        ///     Vectors the length.
        ///     Magnitude
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
        public Vector2D Normalize()
        {
            var l = VectorLength();
            return new Vector2D { X = X / l, Y = Y / l };
        }

        /// <summary>
        ///     Returns the Vector Angle
        /// </summary>
        /// <returns>The Vectors angle based on the X- Axis in rad.</returns>
        public double Angle()
        {
            return Math.Atan(Y / X);
        }

        /// <summary>
        ///     Angle between this and the other Vector
        /// </summary>
        /// <returns>Angle between both Vectors in rad.</returns>
        public double Angle(Vector2D second)
        {
            return Math.Acos(this * second / (VectorLength() * second.VectorLength()));
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Vector2D" /> to <see cref="Vector3D" />.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Vector3D(Vector2D first)
        {
            return new Vector3D(first.RoundedX, first.RoundedY);
        }

        /// <summary>
        ///     Converts to matrix.
        /// </summary>
        /// <returns>Vector transformed to Matrix</returns>
        public BaseMatrix ToMatrix()
        {
            return new BaseMatrix { [0, 0] = X, [0, 1] = Y };
        }
    }
}
