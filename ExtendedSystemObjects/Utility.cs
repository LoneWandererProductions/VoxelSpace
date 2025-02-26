/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/Utility.cs
 * PURPOSE:     Some Methods I seem to use very often. Might add a better way to search the keys!
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeInternal

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

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
        ///     Thread Safe
        /// </summary>
        /// <param name="lst">The List.</param>
        /// <returns>The first available Index<see cref="int" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetFirstAvailableIndex(List<int> lst)
        {
            if (lst == null)
            {
                throw new ArgumentNullException(nameof(lst));
            }

            lock (lst) // Ensure exclusive access to the list
            {
                // Materialize the IEnumerable to avoid multiple enumerations
                var snapshot = new HashSet<int>(lst.ToList());

                for (var i = 0; i < int.MaxValue; i++)
                {
                    if (!snapshot.Contains(i))
                    {
                        return i;
                    }
                }
            }

            throw new InvalidOperationException("No available index found.");
        }

        /// <summary>
        ///     Get the first available index.
        ///     Only usable for positive long Values
        ///     Thread Safe
        /// </summary>
        /// <param name="lst">The List.</param>
        /// <returns>The first available Index<see cref="long" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetFirstAvailableIndex(List<long> lst)
        {
            if (lst == null)
            {
                throw new ArgumentNullException(nameof(lst));
            }

            lock (lst) // Ensure exclusive access to the list
            {
                // Materialize the IEnumerable to avoid multiple enumerations
                var snapshot = new HashSet<long>(lst.ToList());

                for (long i = 0; i < long.MaxValue; i++)
                {
                    if (!snapshot.Contains(i))
                    {
                        return i;
                    }
                }
            }

            throw new InvalidOperationException("No available index found.");
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

        /// <summary>
        ///     Finds the sequences.
        ///     Looks for consecutive numbers in a sequence.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>Return the start, end, and the repeated value of that streak.</returns>
        public static List<(int start, int end, int value)> FindSequences(List<int> numbers)
        {
            var result = new List<(int start, int end, int value)>();

            if (numbers == null || numbers.Count == 0)
            {
                return result;
            }

            var start = 0;
            for (var i = 1; i <= numbers.Count; i++)
            {
                // Check if we're at the end of a sequence or at the end of the list
                if (i != numbers.Count && numbers[i] == numbers[start])
                {
                    continue;
                }

                result.Add((start, i - 1, numbers[start]));
                start = i;
            }

            return result;
        }
    }
}
