/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/BaseMatrix.cs
 * PURPOSE:     Matrix Object with some basic Operators
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://bratched.com/en/?s=matrix
 */

// ReSharper disable MemberCanBeInternal

using System;
using System.Runtime.InteropServices;

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
            Matrix = new double[dimX, dimY];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMatrix" /> class.
        /// </summary>
        /// <param name="matrix">Basic Matrix</param>
        public BaseMatrix(double[,] matrix)
        {
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
        ///     Gets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height => Matrix.GetLength(0);

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
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
            set => Matrix[x, y] = value;
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
            var result = MatrixUtility.MatrixInverse(Matrix);
            return new BaseMatrix(result);
        }

        /// <summary>
        ///     Determinants this instance.
        /// </summary>
        /// <returns>Calculate the Determinant</returns>
        public double Determinant()
        {
            return MatrixUtility.MatrixDeterminant(Matrix);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static BaseMatrix operator *(BaseMatrix first, BaseMatrix second)
        {
            //Todo rework check
            if (first.Width != second.Width && first.Width != second.Height) throw new ArithmeticException();

            return MatrixUtility.UnsafeMultiplication(first, second);
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
    }
}