/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/DictionaryExtensions.cs
 * PURPOSE:     Helper class that extends the already versatile Dictionary, most operations are not thread safe, so beware.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMethodReturnValue.Global

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     The dictionary extensions class.
    /// </summary>
    public static class DictionaryExtensions

    {
        /// <summary>
        ///     Adds the specified key to the Value, that is a list.
        ///     I know it is not recommended to use List and Dictionary together but in case you do,
        ///     this extension should avoid ugly null reference Exceptions and make the code more readable.
        ///     List handling.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dic">The dictionary we work on.</param>
        /// <param name="key">The key we like to add.</param>
        /// <param name="value">The value of the List we like to add.</param>
        public static void Add<TKey, TValue>(this IDictionary<TKey, List<TValue>> dic,
            TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key].Add(value);
            }
            else
            {
                dic.Add(key, new List<TValue>());
                dic[key].Add(value);
            }
        }

        /// <summary>
        ///     Adds the specified key and adds a new initialized SortedList.
        ///     List handling.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dic">The dic.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Add<TKey, TValue>(this IDictionary<TKey, SortedSet<TValue>> dic,
            TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key].Add(value);
            }
            else
            {
                dic.Add(key, new SortedSet<TValue>());
                dic[key].Add(value);
            }
        }

        /// <summary>
        ///     Add or Replace Key Value Pair
        ///     List handling.
        /// </summary>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <param name="key">Unique Key</param>
        /// <param name="value">Value to add</param>
        public static bool AddDistinct<TKey, TValue>(this IDictionary<TKey, List<TValue>> dic, TKey key, TValue value)
        {
            if (!dic.ContainsKey(key))
            {
                var lst = new List<TValue> { value };
                dic.Add(key, lst);
                return true;
            }

            var cache = dic[key];

            if (cache.Contains(value)) return false;

            cache.Add(value);
            dic[key] = cache;

            return true;
        }

        /// <summary>
        ///     Add or Replace Key Value Pair
        /// </summary>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <param name="key">Unique Key</param>
        /// <param name="value">Value to add</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddDistinct<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            dic[key] = value;
        }

        /// <summary>
        ///     Adds a key-value pair to the dictionary if the key is not already present,
        ///     throwing exceptions if either the key or value already exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dic">The dictionary to add the key-value pair to.</param>
        /// <param name="key">The key of the key-value pair to add.</param>
        /// <param name="value">The value of the key-value pair to add.</param>
        /// <exception cref="ArgumentException">Thrown if the key or value already exist in the dictionary.</exception>
        public static void AddDistinctKeyValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
                throw new ArgumentException(string.Concat(ExtendedSystemObjectsResources.ErrorKeyExists,
                    nameof(value)));

            if (dic.ContainsValue(value))
                throw new ArgumentException(string.Concat(ExtendedSystemObjectsResources.ErrorValueExists,
                    nameof(value)));

            dic.Add(key, value);
        }

        /// <summary>
        ///     Sort a Generic Dictionary by Key
        /// </summary>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <returns>Sorted Dictionary</returns>
        public static Dictionary<TKey, TValue> Sort<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            var sortedPairs = dic.OrderBy(pair => pair.Key).ToList();

            var sortedDictionary = new Dictionary<TKey, TValue>();

            foreach (var pair in sortedPairs) sortedDictionary.Add(pair.Key, pair.Value);

            return sortedDictionary;
        }

        /// <summary>
        ///     Check if a Dictionary is Null or just Empty
        /// </summary>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <returns>If Dictionary is Null or has zero Elements</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            if (dic == null) return true;

            return dic.Count == 0;
        }

        /// <summary>
        ///     Check if all Keys are contained
        /// </summary>
        /// <param name="dic">The Dictionary.</param>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>False if one is missing. <see cref="bool" />.</returns>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        public static bool ContainsKeys<TKey, TValue>(this Dictionary<TKey, TValue> dic, IEnumerable<TKey> enumerable)
        {
            return enumerable.All(dic.ContainsKey);
        }

        /// <summary>
        ///     Check if a Dictionary has distinct values
        /// </summary>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <returns>If Dictionary has distinct Values</returns>
        public static bool IsValueDistinct<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            var uniqueValues = new HashSet<TValue>();

            foreach (var value in dic.Values)
                if (!uniqueValues.Add(value))
                    return false; // Non-unique value found

            return true; // All values are distinct
        }

        /// <summary>
        ///     Get First Key by Value
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <param name="value">Value we look up</param>
        /// <returns>
        ///     First appearance of Value
        /// </returns>
        /// <exception cref="ValueNotFoundException"><paramref name="value" /> not found.</exception>
        public static TKey GetFirstKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TValue value)
        {
            foreach (var pair in dic.Where(pair => value.Equals(pair.Value))) return pair.Key;

            throw new ValueNotFoundException(ExtendedSystemObjectsResources.ErrorValueNotFound);
        }

        /// <summary>
        ///     Get All Keys by Value
        /// </summary>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <param name="value">Value we look up</param>
        /// <returns>List of Keys with described Value</returns>
        /// <exception cref="ValueNotFoundException"><paramref name="dic" /> value not found.</exception>
        public static List<TKey> GetKeysByValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TValue value)
        {
            var collection = (from pair in dic where value.Equals(pair.Value) select pair.Key).ToList();

            if (collection.Count == 0)
                throw new ValueNotFoundException(ExtendedSystemObjectsResources.ErrorValueNotFound);

            return collection;
        }

        /// <summary>
        ///     Get All Keys by Value
        /// </summary>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <param name="value">Value we look up</param>
        /// <returns>
        ///     List of Keys with described Value
        /// </returns>
        /// <exception cref="ValueNotFoundException"><paramref name="dic" /> value not found.</exception>
        public static Dictionary<TKey, TValue> GetDictionaryByValues<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            IEnumerable<TKey> value)
        {
            var collection = value.Where(dic.ContainsKey).ToDictionary(key => key, key => dic[key]);

            if (collection.Count == 0)
                throw new ValueNotFoundException(ExtendedSystemObjectsResources.ErrorNoValueFound);

            return collection;
        }

        /// <summary>
        ///     Try to Clone a Dictionary
        ///     Here we abuse the ToDictionary Method
        /// </summary>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <returns>Clone of the Input Dictionary</returns>
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        {
            return dic?.ToDictionary(dctClone => dctClone.Key, dctClone => dctClone.Value);
        }

        /// <summary>
        ///     Swaps two Elements in the Dictionary by Key.
        /// </summary>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <param name="i">The i.</param>
        /// <param name="j">The j.</param>
        /// <returns>The Dictionary in question with swapped Elements<see cref="T:IDictionary{TKey, TValue}" />.</returns>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        public static void Swap<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey i, TKey j)
        {
            if (!dic.ContainsKey(j))
            {
                dic[j] = dic[i];
                _ = dic.Remove(i);
            }
            else
            {
                (dic[j], dic[i]) = (dic[i], dic[j]);
            }
        }

        /// <summary>
        ///     Reduces the specified Dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dic">The dic.</param>
        /// <returns>
        ///     [true] if success, else [false], Reduces Dictionary, first Element will be removed until it empty
        /// </returns>
        public static bool Reduce<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            return !dic.IsNullOrEmpty() && dic.Remove(dic.Keys.First());
        }

        /// <summary>
        ///     Converts to list.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <param name="dic">The dic.</param>
        /// <returns>
        ///     A list with the Key as id
        /// </returns>
        public static List<TValue> ToListId<TId, TValue>(this Dictionary<TId, TValue> dic)
            where TValue : IIdHandling<TId>
        {
            var lst = new List<TValue>();

            foreach (var kvp in dic)
            {
                kvp.Value.Id = kvp.Key;
                lst.Add(kvp.Value);
            }

            return lst;
        }
    }
}