/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/DictionaryExtensions.cs
 * PURPOSE:     Helper class that extends the already versatile Dictionary
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMethodReturnValue.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     The dictionary extensions class.
    /// </summary>
    public static class DictionaryExtensions

    {
        /// <summary>
        ///     Add or Replace Key Value Pair
        /// </summary>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <param name="key">Unique Key</param>
        /// <param name="value">Value to add</param>
        public static void AddDistinct<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            dic[key] = value;
        }

        /// <summary>
        ///     Add or Replace Key Value Pair
        /// </summary>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <param name="key">Unique Key</param>
        /// <param name="value">Value to add</param>
        /// <exception cref="ArgumentNullException"><paramref name="dic" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Element was already Contained</exception>
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
        /// <exception cref="ArgumentNullException"><paramref name="dic" /> is <c>null</c>.</exception>
        public static Dictionary<TKey, TValue> Sort<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            var sorted = new SortedDictionary<TKey, TValue>(dic);
            return new Dictionary<TKey, TValue>(sorted);
        }

        /// <summary>
        ///     Check if a Dictionary is Null or just Empty
        /// </summary>
        /// <typeparam name="TKey">Internal Key</typeparam>
        /// <typeparam name="TValue">Internal Value</typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <returns>If Dictionary is Null or has zero Elements</returns>
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
        /// <exception cref="ArgumentNullException"><paramref name="dic" /> is <c>null</c>.</exception>
        public static bool IsValueDistinct<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));

            var sort = new Dictionary<TKey, TValue>(dic);

            foreach (var element in dic)
            {
                _ = sort.Remove(element.Key);
                if (sort.ContainsValue(element.Value)) return false;
            }

            return true;
        }

        /// <summary>
        ///     Get First Key by Value
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic">Internal Target Dictionary</param>
        /// <param name="value">Value we look up</param>
        /// <returns>First appearance of Value</returns>
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
        /// <exception cref="ArgumentNullException"><paramref name="dic" /> is <c>null</c>.</exception>
        public static List<TKey> GetKeysByValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TValue value)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));

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
        /// <returns>List of Keys with described Value</returns>
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
    }
}