/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/ExtendedList.cs
 * PURPOSE:     Generic System Functions for Lists
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global

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
        ///     Only works with custom equal
        ///     Right now it is only usable with Coordinates
        /// </summary>
        /// <typeparam name="TValue">Generic Object Type</typeparam>
        /// <param name="lst">List we want to add to</param>
        /// <param name="item">item we will replace or add</param>
        public static void AddDistinct<TValue>(this List<TValue> lst, TValue item)
        {
            if (!lst.Contains(item))
            {
                lst.Add(item);
            }
            else
            {
                _ = lst.Remove(item);
                lst.Add(item);
            }
        }

        /// <summary>
        ///     Adds distinct Item to list.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="lst">The LST.</param>
        /// <param name="item">The item.</param>
        /// <returns>if [true] item was added, else [false]</returns>
        public static bool AddIsDistinct<TValue>(this List<TValue> lst, TValue item)
        {
            if (lst.Contains(item)) return false;

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
        ///     Remove Contents of a List from another
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
        ///     Chunks the by.
        /// </summary>
        /// <typeparam name="TValue">Generic Object Type</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="chunkSize">Size of the chunk.</param>
        /// <returns>List split into chunks</returns>
        public static List<List<TValue>> ChunkBy<TValue>(this IEnumerable<TValue> source, int chunkSize)
        {
            return source
                .Select((x, i) => new {Index = i, Value = x})
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
    }
}