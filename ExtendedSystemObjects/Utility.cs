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
            if (position == lst.Max())
            {
                return lst.Min();
            }

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
            if (position == lst.Min())
            {
                return lst.Max();
            }

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
            {
                throw new ArgumentOutOfRangeException(nameof(count),
                    ExtendedSystemObjectsResources.ErrorValueNotAllowed);
            }

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
        ///     Sequencers the specified set.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="sequence">The sequence.</param>
        /// <returns>List of Sequences, with start and end index, null if none were found.</returns>
        [return: MaybeNull]
        public static List<KeyValuePair<int, int>> Sequencer(SortedSet<int> set, int sequence)
        {
            var sequenceGroups = new List<List<int>>();
            var currentSequence = new List<int>();

            var sortedList = new List<int>(set);

            for (var i = 1; i < sortedList.Count; i++)
            {
                var cache = Math.Abs(sortedList[i]);

                if (Math.Abs(sortedList[i - 1] + 1) == cache)
                {
                    //should be only the first case
                    if (!currentSequence.Contains(i - 1))
                    {
                        currentSequence.Add(i - 1);
                    }

                    currentSequence.Add(i);
                }
                else
                {
                    if (currentSequence.Count == 0)
                    {
                        continue;
                    }

                    sequenceGroups.Add(currentSequence);
                    currentSequence = new List<int>();
                }
            }

            return sequenceGroups.Count == 0
                ? null
                : (from stack in sequenceGroups
                    where stack.Count >= sequence
                    let start = stack[0]
                    let end = stack[^1]
                    select new KeyValuePair<int, int>(start, end)).ToList();
        }

        /// <summary>
        ///     Sequencers the specified List.
        /// </summary>
        /// <param name="numbers">The input list.</param>
        /// <param name="sequenceLength">The min count of the sequence.</param>
        /// <returns>List of Sequences, with start and end index, null if none were found.</returns>
        [return: MaybeNull]
        public static List<KeyValuePair<int, int>> Sequencer(List<int> numbers, int sequenceLength)
        {
            var sequenceGroups = new List<List<int>>();
            var currentSequence = new List<int>();

            for (var i = 1; i < numbers.Count; i++)
            {
                var cache = Math.Abs(numbers[i]);

                if (Math.Abs(numbers[i - 1] + 1) == cache)
                {
                    //should be only the first case
                    if (!currentSequence.Contains(i - 1))
                    {
                        currentSequence.Add(i - 1);
                    }

                    currentSequence.Add(i);
                }
                else
                {
                    if (currentSequence.Count == 0)
                    {
                        continue;
                    }

                    sequenceGroups.Add(currentSequence);
                    currentSequence = new List<int>();
                }
            }

            return sequenceGroups.Count == 0
                ? null
                : (from stack in sequenceGroups
                    where stack.Count >= sequenceLength
                    let start = stack[0]
                    let end = stack[^1]
                    select new KeyValuePair<int, int>(start, end)).ToList();
        }

        /// <summary>
        ///     Sequences the specified list.
        /// </summary>
        /// <param name="numbers">The list.</param>
        /// <param name="stepWidth">The step width.</param>
        /// <param name="sequenceLength">The sequence.</param>
        /// <returns>List of Sequences, with start and end index, null if none were found.</returns>
        public static List<KeyValuePair<int, int>> Sequencer(List<int> numbers, int stepWidth, int sequenceLength)
        {
            numbers.Sort();
            var max = numbers.Max();

            var sequenceGroups = new List<List<int>>();
            var currentSequence = new List<int>();
            var observer = new List<int>();

            foreach (var element in numbers)
            {
                var cache = Math.Abs(element);
                var count = cache;

                do
                {
                    if (currentSequence.Contains(cache))
                    {
                        break;
                    }

                    count += stepWidth;

                    if (observer.Contains(count))
                    {
                        continue;
                    }

                    if (!numbers.Contains(count))
                    {
                        break;
                    }

                    currentSequence.Add(count);
                    observer.Add(count);
                } while (count < max);

                if (currentSequence.Count == 0)
                {
                    continue;
                }

                currentSequence.AddFirst(cache);
                sequenceGroups.Add(currentSequence);
                currentSequence = new List<int>();
            }

            return sequenceGroups.Count == 0
                ? null
                : (from stack in sequenceGroups
                    where stack.Count >= sequenceLength
                    let start = stack[0]
                    let end = stack[^1]
                    select new KeyValuePair<int, int>(start, end)).ToList();
        }
    }
}
