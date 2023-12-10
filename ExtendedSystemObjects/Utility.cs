/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/Utility.cs
 * PURPOSE:     Some Methods I seem to use very often. Might add a better way to search the keys!
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     The utility class.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        ///     Get the first available index.
        ///     Only usable for positive int Values
        /// </summary>
        /// <param name="lst">The List.</param>
        /// <returns>The first available Index<see cref="int" />.</returns>
        public static int GetFirstAvailableIndex(IEnumerable<int> lst)
        {
            return Enumerable.Range(0, int.MaxValue)
                .Except(lst)
                .FirstOrDefault();
        }

        /// <summary>
        ///     Gets the next element.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="lst">The LST.</param>
        /// <returns>Next Element</returns>
        public static int GetNextElement(int position, List<int> lst)
        {
            if (position == lst.Max()) return lst.Min();

            var index = lst.IndexOf(position);

            return index == -1 ? lst.Min() : lst[index + 1];
        }

        /// <summary>
        ///     Gets the previous element.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="lst">The LST.</param>
        /// <returns>Previous Element</returns>
        public static int GetPreviousElement(int position, List<int> lst)
        {
            if (position == lst.Min()) return lst.Max();

            var index = lst.IndexOf(position);

            return index == -1 ? lst.Max() : lst[index - 1];
        }

        /// <summary>
        ///     Gets the index of the available indexes.
        ///     Only usable for positive int Values
        /// </summary>
        /// <param name="lst">The List.</param>
        /// <param name="count">The count of keys we need.</param>
        /// <returns>A list of keys we can use</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<int> GetAvailableIndexes(List<int> lst, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count),
                    ExtendedSystemObjectsResources.ErrorValueNotAllowed);

            var keys = new List<int>();
            for (var i = 0; i < count; i++)
            {
                var key = GetFirstAvailableIndex(lst);
                lst.Add(key);
                keys.Add(key);
            }

            return keys;
        }

        /// <summary>
        ///     Sequencers the specified List.
        /// </summary>
        /// <param name="lst">The input list.</param>
        /// <param name="sequence">The min count of the sequence.</param>
        /// <returns>List of Sequences, with start and end index, null if none were found.</returns>
        [return: MaybeNull]
        public static List<KeyValuePair<int, int>> Sequencer(List<int> lst, int sequence)
        {
            var box = new List<List<int>>();
            var other = new List<int>();

            for (var i = 1; i < lst.Count; i++)
            {
                var cache = Math.Abs(lst[i]);

                if (Math.Abs(lst[i - 1] + 1) == cache)
                {
                    //should be only the first case
                    if (!other.Contains(i - 1)) other.Add(i - 1);

                    other.Add(i);
                }
                else
                {
                    if (other.Count == 0) continue;

                    box.Add(other);
                    other = new List<int>();
                }
            }

            return box.Count == 0
                ? null
                : (from stack in box
                    where stack.Count >= sequence
                    let start = stack[0]
                    let end = stack[^1]
                    select new KeyValuePair<int, int>(start, end)).ToList();
        }

        /// <summary>
        ///     Sequences the specified list.
        /// </summary>
        /// <param name="lst">The list.</param>
        /// <param name="width">The width.</param>
        /// <param name="sequence">The sequence.</param>
        /// <returns>List of Sequences, with start and end index, null if none were found.</returns>
        public static List<KeyValuePair<int, int>> Sequencer(List<int> lst, int width, int sequence)
        {
            lst.Sort();
            var max = lst.Max();

            var box = new List<List<int>>();
            var other = new List<int>();
            var observer = new List<int>();

            foreach (var element in lst)
            {
                var cache = Math.Abs(element);
                var count = cache;

                do
                {
                    if (other.Contains(cache)) break;

                    count += width;

                    if (observer.Contains(count)) continue;

                    if (!lst.Contains(count)) break;

                    other.Add(count);
                    observer.Add(count);
                } while (count < max);

                if (other.Count == 0) continue;

                other.AddFirst(cache);
                box.Add(other);
                other = new List<int>();
            }

            return box.Count == 0
                ? null
                : (from stack in box
                    where stack.Count >= sequence
                    let start = stack[0]
                    let end = stack[^1]
                    select new KeyValuePair<int, int>(start, end)).ToList();
        }
    }
}