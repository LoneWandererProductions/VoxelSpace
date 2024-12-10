/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/MultiArray.cs
 * PURPOSE:     Some Extensions for Arrays, all generic
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Text;

namespace ExtendedSystemObjects
{
    public static class MultiArray
    {
        /// <summary>
        ///     Swaps the row.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="xOne">The first row.</param>
        /// <param name="xTwo">The second row.</param>
        public static void SwapColumn<TValue>(this TValue[,] array, int xOne, int xTwo)
        {
            for (var i = 0; i < array.GetLength(1); i++)
                (array[xOne, i], array[xTwo, i]) = (array[xTwo, i], array[xOne, i]);
        }

        /// <summary>
        ///     Swaps the row.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="xOne">The x one.</param>
        /// <param name="xTwo">The x two.</param>
        public static void SwapRow<TValue>(this TValue[,] array, int xOne, int xTwo)
        {
            for (var i = 0; i < array.GetLength(0); i++)
                (array[i, xOne], array[i, xTwo]) = (array[i, xTwo], array[i, xOne]);
        }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="array">The array.</param>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public static unsafe string ToText<TValue>(this TValue[,] array) where TValue : unmanaged
        {
            var str = new StringBuilder();

            var length = array.GetLength(0) * array.GetLength(1);
            var row = array.GetLength(1);

            fixed (TValue* one = array)
            {
                for (var i = 0; i < length; i++)
                {
                    var tmp = one[i];
                    _ = str.Append(tmp);

                    if ((i + 1) % row == 0 && i + 1 >= row)
                        _ = str.Append(Environment.NewLine);
                    else
                        _ = str.Append(ExtendedSystemObjectsResources.Separator);
                }
            }

            return str.ToString();
        }

        /// <summary>
        ///     Duplicates the specified array.
        ///     Deep Copy of the array
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="array">The array.</param>
        /// <returns>Copy of the called array</returns>
        public static unsafe TValue[,] Duplicate<TValue>(this TValue[,] array) where TValue : unmanaged
        {
            // allocates/creates a duplicate of a matrix.
            var result = new TValue[array.GetLength(0), array.GetLength(1)];

            fixed (TValue* one = result, two = array)
            {
                for (var i = 0; i < array.GetLength(0); i++)
                for (var j = 0; j < array.GetLength(1); j++)
                {
                    var cursor = i + j * array.GetLength(1);

                    one[cursor] = two[cursor];
                }
            }

            return result;
        }

        /// <summary>
        ///     Equals the specified arrays.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="compare">The compare target.</param>
        /// <returns>Equal or not</returns>
        public static unsafe bool Equal<TValue>(this TValue[,] array, TValue[,] compare) where TValue : unmanaged
        {
            if (array.GetLength(0) != compare.GetLength(0)) return false;

            if (array.GetLength(1) != compare.GetLength(1)) return false;

            var length = array.GetLength(0) * array.GetLength(1);

            fixed (TValue* one = array, two = compare)
            {
                for (var i = 0; i < length; ++i)
                    if (!one[i].Equals(two[i]))
                        return false;
            }

            return true;
        }

        /// <summary>
        ///     Converts an array to span.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="array">The array.</param>
        /// <returns>A Multi array as span Type.</returns>
        public static unsafe Span<TValue> ToSpan<TValue>(this TValue[,] array) where TValue : unmanaged
        {
            var length = array.GetLength(0) * array.GetLength(1);
            Span<TValue> result;

            fixed (TValue* a = array)
            {
                result = new Span<TValue>(a, length);
            }

            return result;
        }
    }
}