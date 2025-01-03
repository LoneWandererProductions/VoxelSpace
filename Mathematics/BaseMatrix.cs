/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/BaseMatrix.cs
 * PURPOSE:     Matrix Object with some basic Operators
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://bratched.com/en/?s=matrix
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExtendedSystemObjects;

namespace Mathematics
{
    /// <inheritdoc />
    /// <summary>
    ///     Idea and Inspiration:
    ///     https://bratched.com/en/?s=matrix
    /// </summary>
    public sealed class BaseMatrix : IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMatrix" /> class.
        /// </summary>
        /// <param name="dimX">The Width.</param>
        /// <param name="dimY">The Height.</param>
        public BaseMatrix(int dimX, int dimY)
        {
            if (dimX <= 0) throw new ArgumentException(MathResources.MatrixErrorNegativeValue, nameof(dimX));

            if (dimY <= 0) throw new ArgumentException(MathResources.MatrixErrorNegativeValue, nameof(dimY));

            Matrix = new double[dimX, dimY];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMatrix" /> class.
        /// </summary>
        /// <param name="matrix">Basic Matrix</param>
        public BaseMatrix(double[,] matrix)
        {
            if (matrix.GetLength(0) <= 0 || matrix.GetLength(1) <= 0)
                throw new ArgumentException(MathResources.MatrixErrorNegativeValue, nameof(matrix));

            Matrix = matrix;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMatrix" /> class.
        /// </summary>
        public BaseMatrix()
        {
        }

        /// <summary>
        ///     Gets the bits handle.
        /// </summary>
        /// <value>
        ///     The bits handle.
        /// </value>
        private GCHandle BitsHandle { get; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="BaseMatrix" /> is disposed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        private bool Disposed { get; set; }

        /// <summary>
        ///     Gets or sets the matrix.
        /// </summary>
        /// <value>
        ///     The matrix.
        /// </value>
        public double[,] Matrix { get; set; }

        /// <summary>
        ///     Gets the height. Y
        /// </summary>
        /// <value>
        ///     The height. Y
        /// </value>
        public int Height => Matrix.GetLength(0);

        /// <summary>
        ///     Gets the width. X
        /// </summary>
        /// <value>
        ///     The width. X
        /// </value>
        public int Width => Matrix.GetLength(1);

        /// <summary>
        ///     Gets or sets the <see cref="BaseMatrix" /> the time with the specified x and y.
        /// </summary>
        /// <value>
        ///     The <see cref="double" />.
        /// </value>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>the result</returns>
        public double this[int x, int y]
        {
            get => Matrix[x, y];
            init => Matrix[x, y] = value;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Free up all the Memory.
        ///     See:
        ///     https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1063?view=vs-2019
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing)
            {
                // free managed resources
                Matrix = null;
                BitsHandle.Free();
            }

            Disposed = true;
        }

        /// <summary>
        ///     NOTE: Leave out the finalizer altogether if this class doesn't
        ///     own unmanaged resources, but leave the other methods
        ///     exactly as they are.
        ///     Finalizes an instance of the <see cref="BaseMatrix" /> class.
        /// </summary>
        ~BaseMatrix()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        /// <summary>
        ///     Inverses the specified matrix.
        /// </summary>
        /// <returns>The Inverse Matrix</returns>
        public BaseMatrix Inverse()
        {
            if (Height != Width) throw new NotImplementedException(MathResources.MatrixErrorInverseNotCubic);

            var result = MatrixInverse.Inverse(Matrix);
            return new BaseMatrix(result);
        }

        /// <summary>
        ///     Lus the decomposition.
        /// </summary>
        /// <returns>Key Value Pair of L and U Matrix</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public KeyValuePair<BaseMatrix, BaseMatrix> LuDecomposition()
        {
            if (Height != Width) throw new NotImplementedException(MathResources.MatrixErrorInverseNotCubic);

            var (l, u) = MatrixInverse.LuDecomposition(Matrix);

            var lMatrix = new BaseMatrix(l);
            var uMatrix = new BaseMatrix(u);

            return new KeyValuePair<BaseMatrix, BaseMatrix>(lMatrix, uMatrix);
        }

        /// <summary>
        ///     Determinants this instance.
        /// </summary>
        /// <returns>Calculate the Determinant</returns>
        public double Determinant()
        {
            return MatrixInverse.MatrixDeterminant(Matrix);
        }

        /// <summary>
        ///     Implements the operator *.
        ///     https://en.wikipedia.org/wiki/Matrix_multiplication
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static BaseMatrix operator *(BaseMatrix first, BaseMatrix second)
        {
            if (first.Width != second.Height) throw new ArithmeticException(MathResources.MatrixErrorColumns);

            return MatrixUtility.UnsafeMultiplication(first, second);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="first">The first Matrix.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector3D operator *(Vector3D v, BaseMatrix first)
        {
            var mat = (BaseMatrix)v;
            return (Vector3D)(mat * first);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="first">The first Matrix.</param>
        /// <param name="v">The v.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector3D operator *(BaseMatrix first, Vector3D v)
        {
            var mat = (BaseMatrix)v;
            return (Vector3D)(first * mat);
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static BaseMatrix operator +(BaseMatrix first, BaseMatrix second)
        {
            if (first.Width != second.Width) throw new ArithmeticException();

            if (first.Height != second.Height) throw new ArithmeticException();

            return MatrixUtility.UnsafeAddition(first, second);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static BaseMatrix operator -(BaseMatrix first, BaseMatrix second)
        {
            if (first.Width != second.Width) throw new ArithmeticException();

            if (first.Height != second.Height) throw new ArithmeticException();

            return MatrixUtility.UnsafeSubtraction(first, second);
        }

        /// <summary>
        ///     Equals the specified other.
        ///     Does not use the jagged array Equal, because sadly, that doesn't play well enough with double values.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>Equal or not</returns>
        public bool Equals(BaseMatrix other)
        {
            return MatrixUtility.UnsafeCompare(this, other);
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
            return obj is BaseMatrix other && Equals(other);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Matrix);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(BaseMatrix first, BaseMatrix second)
        {
            return first?.Equals(second) == true;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(BaseMatrix first, BaseMatrix second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="BaseMatrix" /> to <see cref="Vector3D" />.
        ///     Here is the only case where w will be set!
        ///     Only usable for 3D stuff.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Vector3D(BaseMatrix first)
        {
            if (first.Height != 4 && first.Width != 4) return null;

            var v = new Vector3D(first[0, 0], first[0, 1], first[0, 2]);
            v.SetW(first[0, 3]);
            return v;
        }

        /// <summary>
        ///     Performs an implicit conversion from jagged array double to <see cref="BaseMatrix" />.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator BaseMatrix(double[,] m)
        {
            return new BaseMatrix { Matrix = m };
        }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Matrix.ToText();
        }
    }
}