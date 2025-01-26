/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Mathematics
 * FILE:        Mathematics/MatrixUtility.cs
 * PURPOSE:     Helper class that does some basic Matrix operations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://bratched.com/en/?s=matrix
 *              https://jamesmccaffrey.wordpress.com/2015/03/06/inverting-a-matrix-using-c/
 */

// ReSharper disable MemberCanBeInternal

using System;

namespace Mathematics
{
    /// <summary>
    ///     Helper Methods, all unsafe for Matrix operations
    /// </summary>
    public static class MatrixUtility
    {
        /// <summary>
        ///     Create an Identity Matrix.
        /// </summary>
        /// <param name="n">The size of the identity matrix.</param>
        /// <returns>An Identity Matrix</returns>
        public static BaseMatrix MatrixIdentity(int n)
        {
            if (n <= 0)
            {
                throw new ArgumentException(MathResources.MatrixErrorNegativeValue, nameof(n));
            }

            var result = new double[n, n];

            for (var i = 0; i < n; ++i)
            {
                result[i, i] = 1d;
            }

            return new BaseMatrix(result);
        }

        /// <summary>
        ///     Unsafe Matrix multiplication.
        ///     Source:
        ///     https://bratched.com/en/?s=matrix
        /// </summary>
        /// <param name="mOne">The first Matrix.</param>
        /// <param name="mTwo">The second Matrix.</param>
        /// <returns>Multiplied Matrix</returns>
        internal static unsafe BaseMatrix UnsafeMultiplication(BaseMatrix mOne, BaseMatrix mTwo)
        {
            var h = mOne.Height;
            var w = mTwo.Width;
            var l = mOne.Width;

            var result = new BaseMatrix(h, w);

            fixed (double* pm = result.Matrix, pmOne = mOne.Matrix, pmTwo = mTwo.Matrix)
            {
                for (var i = 0; i < h; i++)
                {
                    var iOne = i * l;

                    for (var j = 0; j < w; j++)
                    {
                        var iTwo = j;

                        var res = 0d;

                        for (var k = 0; k < l; k++, iTwo += w)
                        {
                            res += pmOne[iOne + k] * pmTwo[iTwo];
                        }

                        pm[(i * w) + j] = res;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Unsafe Matrix addition.
        /// </summary>
        /// <param name="mOne">The m one.</param>
        /// <param name="mTwo">The m two.</param>
        /// <returns>Matrix Addition</returns>
        internal static unsafe BaseMatrix UnsafeAddition(BaseMatrix mOne, BaseMatrix mTwo)
        {
            var h = mOne.Height;
            var w = mOne.Width;

            var result = new BaseMatrix(h, w);

            fixed (double* pm = result.Matrix, pmOne = mOne.Matrix, pmTwo = mTwo.Matrix)
            {
                for (var i = 0; i < h; i++)
                for (var j = 0; j < w; j++)
                {
                    var cursor = i + (j * mOne.Width);

                    pm[cursor] = pmOne[cursor] + pmTwo[cursor];
                }
            }

            return result;
        }

        /// <summary>
        ///     Unsafe Matrix compare.
        /// </summary>
        /// <param name="mOne">The m one.</param>
        /// <param name="mTwo">The m two.</param>
        /// <returns>If Matrices are equal with our preconfigured tolerance.</returns>
        internal static unsafe bool UnsafeCompare(BaseMatrix mOne, BaseMatrix mTwo)
        {
            if (mOne.Height != mTwo.Height)
            {
                return false;
            }

            if (mOne.Width != mTwo.Width)
            {
                return false;
            }

            var h = mOne.Height;
            var w = mOne.Width;

            fixed (double* pmOne = mOne.Matrix, pmTwo = mTwo.Matrix)
            {
                for (var i = 0; i < h; i++)
                for (var j = 0; j < w; j++)
                {
                    var cursor = i + (j * mOne.Width);
                    if (Math.Abs(pmOne[cursor] - pmTwo[cursor]) > MathResources.Tolerance)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Unsafe Matrix subtraction.
        /// </summary>
        /// <param name="mOne">The m one.</param>
        /// <param name="mTwo">The m two.</param>
        /// <returns>Matrix Subtraction</returns>
        internal static unsafe BaseMatrix UnsafeSubtraction(BaseMatrix mOne, BaseMatrix mTwo)
        {
            var h = mOne.Height;
            var w = mOne.Width;
            var result = new BaseMatrix(h, w);

            fixed (double* pm = result.Matrix, pmOne = mOne.Matrix, pmTwo = mTwo.Matrix)
            {
                for (var i = 0; i < h; i++)
                for (var j = 0; j < w; j++)
                {
                    var cursor = i + (j * mOne.Width);

                    pm[cursor] = pmOne[cursor] - pmTwo[cursor];
                }
            }

            return result;
        }
    }
}
