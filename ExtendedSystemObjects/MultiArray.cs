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
        public static string ToText<TValue>(this TValue[,] array)
        {
            var str = new StringBuilder();

            for (var i = 0; i < array.GetLength(0); i++)
            for (var j = 0; j < array.GetLength(1); j++)
            {
                var tmp = array[i, j];
                _ = str.Append(tmp);
                _ = str.Append(j != array.GetLength(1) - 1
                    ? ExtendedSystemObjectsResources.Separator
                    : Environment.NewLine);
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
        public static TValue[,] Duplicate<TValue>(this TValue[,] array)
        {
            // allocates/creates a duplicate of a matrix.
            var result = new TValue[array.GetLength(0), array.GetLength(1)];

            for (var i = 0; i < array.GetLength(0); ++i) // copy the values
            for (var j = 0; j < array.GetLength(1); ++j)
                result[i, j] = array[i, j];

            return result;
        }

        /// <summary>
        ///     Equals the specified arrays.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="compare">The compare target.</param>
        /// <returns>Equal or not</returns>
        public static bool Equal<TValue>(this TValue[,] array, TValue[,] compare)
        {
            if (array.GetLength(0) != compare.GetLength(0)) return false;

            if (array.GetLength(1) != compare.GetLength(1)) return false;

            for (var i = 0; i < array.GetLength(0); ++i)
            for (var j = 0; j < array.GetLength(1); ++j)
                if (!array[i, j].Equals(compare[i, j]))
                    return false;

            return true;
        }
    }
}