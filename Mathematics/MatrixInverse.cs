/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/MatrixInverse.cs
 * PURPOSE:     Helper class that does some basic Matrix operations, Determinant, Solve, Inverse
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://bratched.com/en/?s=matrix
 *              https://jamesmccaffrey.wordpress.com/2015/03/06/inverting-a-matrix-using-c/
 */

using System;
using System.Collections.Generic;
using ExtendedSystemObjects;

namespace Mathematics
{
    /// <summary>
    ///     Calculate Inverse and all the other stuff around
    /// </summary>
    internal static class MatrixInverse
    {
        /// <summary>
        ///     Calculate the determinant of the matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>Determinant of the matrix</returns>
        /// <exception cref="ArithmeticException">Unable to compute MatrixDeterminant</exception>
        internal static double MatrixDeterminant(double[,] matrix)
        {
            var lum = MatrixDecompose(matrix, out _, out var toggle) ??
                      throw new ArithmeticException(MathResources.MatrixErrorDeterminant);

            double result = toggle;

            for (var i = 0; i < lum.GetLength(1); ++i)
            {
                result *= lum[i, i];
            }

            return result;
        }

        /// <summary>
        ///     Decompose the Matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="perm">The perm.</param>
        /// <param name="toggle">The toggle.</param>
        /// <returns>Decomposed Matrix</returns>
        /// <exception cref="ArithmeticException">
        ///     Attempt to decompose a non-square m
        ///     or
        ///     Cannot use Doolittle's method
        /// </exception>
        internal static double[,] MatrixDecompose(double[,] matrix, out int[] perm,
            out int toggle)
        {
            // Doolittle LUP decomposition with partial pivoting.
            // rerturns: result is L (with 1s on diagonal) and U;
            // perm holds row permutations; toggle is +1 or -1 (even or odd)
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1); // assume square

            if (rows != cols)
            {
                throw new ArithmeticException(MathResources.MatrixErrorInverseNotCubic);
            }

            var result = matrix.Duplicate();

            perm = new int[rows]; // set up row permutation result
            for (var i = 0; i < rows; ++i)
            {
                perm[i] = i;
            }

            toggle = 1; // toggle tracks row swaps.
            // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

            for (var j = 0; j < rows - 1; ++j) // each column
            {
                var colMax = Math.Abs(result[j, j]); // find largest val in col
                var pRow = j;

                // reader Matt V needed this:
                for (var i = j + 1; i < rows; ++i)
                {
                    if (Math.Abs(result[i, j]) > colMax)
                    {
                        colMax = Math.Abs(result[i, j]);
                        pRow = i;
                    }
                }
                // Not sure if this approach is needed always, or not.

                if (pRow != j) // if largest value not on pivot, swap rows
                {
                    result.SwapColumn(pRow, j);

                    (perm[pRow], perm[j]) = (perm[j], perm[pRow]);

                    toggle = -toggle; // adjust the row-swap toggle
                }

                // --------------------------------------------------
                // This part added later (not in original)
                // and replaces the 'return null' below.
                // if there is a 0 on the diagonal, find a good row
                // from i = j+1 down that doesn't have
                // a 0 in column j, and swap that good row with row j
                // --------------------------------------------------

                if (result[j, j] == 0.0)
                {
                    // find a good row to swap
                    var goodRow = -1;
                    for (var row = j + 1; row < rows; ++row)
                    {
                        if (result[row, j] != 0.0)
                        {
                            goodRow = row;
                        }
                    }

                    if (goodRow == -1)
                    {
                        throw new Exception(MathResources.MatrixErrorDoolittle);
                    }

                    result.SwapColumn(goodRow, j);

                    (perm[goodRow], perm[j]) = (perm[j], perm[goodRow]);

                    toggle = -toggle; // adjust the row-swap toggle
                }

                for (var i = j + 1; i < rows; ++i)
                {
                    result[i, j] /= result[j, j];
                    for (var k = j + 1; k < rows; ++k)
                    {
                        result[i, k] -= result[i, j] * result[j, k];
                    }
                }
            } // main j column loop

            return result;
        }

        /// <summary>
        ///     Inverses the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns></returns>
        /// <exception cref="ArithmeticException">Unable to compute inverse</exception>
        internal static double[,] Inverse(double[,] matrix)
        {
            var n = matrix.GetLength(0);
            var result = matrix.Duplicate();
            var lum = MatrixDecompose(matrix, out var perm,
                out _);

            if (lum == null)
            {
                throw new ArithmeticException(MathResources.MatrixErrorInverse);
            }

            var b = new double[n];
            for (var i = 0; i < n; ++i)
            {
                for (var j = 0; j < n; ++j)
                {
                    if (i == perm[j])
                    {
                        b[j] = 1.0;
                    }
                    else
                    {
                        b[j] = 0.0;
                    }
                }

                var x = HelperSolve(lum, b); // 

                for (var j = 0; j < n; ++j)
                {
                    result[j, i] = x[j];
                }
            }

            return result;
        }

        /// <summary>
        ///     Helpers the solve.
        /// </summary>
        /// <param name="luMatrix">The lu matrix.</param>
        /// <param name="b">The b.</param>
        /// <returns>Solved equation and results</returns>
        internal static double[] HelperSolve(double[,] luMatrix, double[] b)
        {
            // before calling this helper, permute b using the perm array
            // from MatrixDecompose that generated luMatrix
            var n = luMatrix.GetLength(0);
            var x = new double[n];
            b.CopyTo(x, 0);

            for (var i = 1; i < n; ++i)
            {
                var sum = x[i];
                for (var j = 0; j < i; ++j)
                {
                    sum -= luMatrix[i, j] * x[j];
                }

                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1, n - 1];
            for (var i = n - 2; i >= 0; --i)
            {
                var sum = x[i];
                for (var j = i + 1; j < n; ++j)
                {
                    sum -= luMatrix[i, j] * x[j];
                }

                x[i] = sum / luMatrix[i, i];
            }

            return x;
        }

        /// <summary>
        ///     LU decomposition.
        ///     https://en.wikipedia.org/wiki/LU_decomposition
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>LU decomposition</returns>
        internal static KeyValuePair<double[,], double[,]> LuDecomposition(double[,] matrix)
        {
            var width = matrix.GetLength(0);
            var height = matrix.GetLength(1);

            var lower = new double[width, height];
            var upper = new double[width, height];

            // Decomposing matrix into Upper and Lower
            // triangular matrix
            for (var i = 0; i < width; i++)
            {
                // Upper Triangular
                for (var k = i; k < width; k++)
                {
                    // Summation of L(i, j) * U(j, k)
                    double sum = 0;
                    for (var j = 0; j < i; j++)
                    {
                        sum += lower[i, j] * upper[j, k];
                    }

                    // Evaluating U(i, k)
                    upper[i, k] = matrix[i, k] - sum;
                }

                // Lower Triangular
                for (var k = i; k < width; k++)
                {
                    if (i == k)
                    {
                        lower[i, i] = 1; // Diagonal as 1
                    }
                    else
                    {
                        // Summation of L(k, j) * U(j, i)
                        double sum = 0;
                        for (var j = 0; j < i; j++)
                        {
                            sum += lower[k, j] * upper[j, i];
                        }

                        // Evaluating L(k, i)
                        lower[k, i]
                            = (matrix[k, i] - sum) / upper[i, i];
                    }
                }
            }

            return new KeyValuePair<double[,], double[,]>(lower, upper);
        }
    }
}
