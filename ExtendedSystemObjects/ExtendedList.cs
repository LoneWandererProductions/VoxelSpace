/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/ExtendedList.cs
 * PURPOSE:     Generic System Functions for Lists
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     The extended list class.
    /// </summary>
    public static class ExtendedList
    {
        /// <summary>
        ///     Check if a List is Null or just Empty
        /// </summary>
        /// <typeparam name="TValue">Generic Object Type</typeparam>
        /// <param name="lst">List we want to check</param>
        /// <returns>Empty or not</returns>
        public static bool IsNullOrEmpty<TValue>(this List<TValue> lst)
        {
            if (lst == null) return true;

            return lst.Count == 0;
        }

        /// <summary>
        ///     Add Element at the first Entry
        /// </summary>
        /// <typeparam name="TValue">Generic Object Type</typeparam>
        /// <param name="lst">List we want to add to</param>
        /// <param name="item">item we will replace or add</param>
        public static void AddFirst<TValue>(this List<TValue> lst, TValue item)
        {
            if (lst == null) throw new ArgumentNullException(nameof(lst));

            lst.Insert(0, item);
        }

        /// <summary>
        ///     Only works with equal and Implemented IEquality Interface
        /// </summary>
        /// <typeparam name="TValue">Generic Object Type</typeparam>
        /// <param name="lst">List we want to add to</param>
        /// <param name="item">item we will replace or add</param>
        /// <returns>if [true] item was added, else [false]</returns>
        public static bool AddDistinct<TValue>(this List<TValue> lst, TValue item)
        {
            var hashSet = new HashSet<TValue>(lst);

            // Check if the item already exists in the HashSet
            if (hashSet.Contains(item)) return false; // Item already exists, no need to add

            // Add the item to the list since it doesn't already exist
            lst.Add(item);

            return true;
        }

        /// <summary>
        ///     Converts a List to a dictionary.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="lst">The List.</param>
        /// <returns>A Dictionary from a list with int as the key</returns>
        public static Dictionary<int, TValue> ToDictionary<TValue>(this IEnumerable<TValue> lst)
        {
            var index = 0;
            return lst?.ToDictionary(_ => index++);
        }

        /// <summary>
        ///     Remove Contents of a List from ancurrentSequence
        /// </summary>
        /// <typeparam name="TValue">Generic Object Type</typeparam>
        /// <param name="lst">Base list we remove from</param>
        /// <param name="range">List with elements we want to remove</param>
        public static void RemoveListRange<TValue>(this List<TValue> lst, IEnumerable<TValue> range)
        {
            foreach (var element in range.Where(lst.Contains)) _ = lst.Remove(element);
        }

        /// <summary>
        ///     Try to Clone a List
        ///     Here we abuse the IEnumerable ToList Method
        /// </summary>
        /// <typeparam name="TValue">Generic Object Type</typeparam>
        /// <param name="lst">IEnumerable</param>
        /// <returns>Clone of the Input IEnumerable</returns>
        public static List<TValue> Clone<TValue>(this IEnumerable<TValue> lst)
        {
            return lst?.ToList();
        }

        /// <summary>
        ///     Equals the specified compare.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="lst">The first list.</param>
        /// <param name="compare">The compare.</param>
        /// <returns>If lists are equal, ignores order and count</returns>
        public static bool Equal<TValue>(this IEnumerable<TValue> lst, IEnumerable<TValue> compare)
        {
            var set1 = new HashSet<TValue>(lst);
            var set2 = new HashSet<TValue>(compare);
            return set1.SetEquals(set2);
        }

        /// <summary>
        ///     Equals the specified compare.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="lst">The first list.</param>
        /// <param name="compare">The compare.</param>
        /// <param name="comparer">The compare Operator.</param>
        /// <returns>If lists are equal,based on the condition</returns>
        /// <exception cref="ArgumentOutOfRangeException">comparer - null</exception>
        public static bool Equal<TValue>(this List<TValue> lst, List<TValue> compare, EnumerableCompare comparer)
        {
            switch (comparer)
            {
                case EnumerableCompare.IgnoreOrderCount:
                    return lst.Equal(compare);

                case EnumerableCompare.IgnoreOrder:
                    return lst.Count == compare.Count && lst.Equal(compare);
                case EnumerableCompare.AllEqual:
                    if (lst.Count != compare.Count) return false;

                    return !lst.Where((t, i) => !t.Equals(compare[i])).Any();
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparer), comparer, null);
            }
        }

        /// <summary>
        ///     Chunks a list by a certain amount.
        /// </summary>
        /// <typeparam name="TValue">Generic Object Type</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="chunkSize">Size of the chunk.</param>
        /// <returns>List split into chunks</returns>
        public static List<List<TValue>> ChunkBy<TValue>(this IEnumerable<TValue> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        /// <summary>
        ///     Shuffles the specified list.
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="lst">Shuffle the list.</param>
        public static void Shuffle<T>(this IList<T> lst)
        {
            for (var i = 0; i < lst.Count; i++)
            {
                var index = RandomNumberGenerator.GetInt32(i, lst.Count);
                (lst[index], lst[i]) = (lst[i], lst[index]);
            }
        }

        /// <summary>
        ///     Converts a list to dictionary.
        ///     but only if it implements the IIdHandling Interface
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <param name="lst">List of generic Objects.</param>
        /// <returns>
        ///     Dicitionary with an conversion from the attribute Id as Key
        /// </returns>
        public static Dictionary<TId, TValue> ToDictionaryId<TValue, TId>(this IList<TValue> lst)
            where TValue : IIdHandling<TId>
        {
            var dct = new Dictionary<TId, TValue>();

            foreach (var item in lst) dct.Add(item.Id, item);

            return dct;
        }
    }
}