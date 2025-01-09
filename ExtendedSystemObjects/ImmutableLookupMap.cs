/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/ImmutableLookupMap.cs
 * PURPOSE:     A high-performance, immutable lookup map that uses an array-based internal structure for fast key-value lookups.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;

namespace ExtendedSystemObjects
{
    /// <summary>
    /// A high-performance, immutable lookup map using an array-based internal structure for key-value lookups.
    /// </summary>
    public sealed class ImmutableLookupMap<TKey, TValue> where TKey : struct, IEquatable<TKey>
    {
        private readonly TValue[] _lookupTable;
        private readonly bool[] _keyPresence;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableLookupMap{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="data">A dictionary containing the key-value pairs to initialize the map.</param>
        public ImmutableLookupMap(IDictionary<TKey, TValue> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            int capacity = data.Count * 2; // Adjust for hash collisions
            _lookupTable = new TValue[capacity];
            _keyPresence = new bool[capacity];

            foreach (var kvp in data)
            {
                int hash = Math.Abs(kvp.Key.GetHashCode() % capacity);
                if (_keyPresence[hash])
                {
                    throw new InvalidOperationException($"Hash collision detected for key: {kvp.Key}");
                }

                _lookupTable[hash] = kvp.Value;
                _keyPresence[hash] = true;
            }
        }

        /// <summary>
        /// Retrieves the value associated with the specified key.
        /// </summary>
        public TValue Get(TKey key)
        {
            int hash = Math.Abs(key.GetHashCode() % _lookupTable.Length);
            if (!_keyPresence[hash])
            {
                throw new KeyNotFoundException($"The key {key} was not found in the lookup map.");
            }

            return _lookupTable[hash];
        }

        /// <summary>
        /// Attempts to retrieve the value associated with the specified key.
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int hash = Math.Abs(key.GetHashCode() % _lookupTable.Length);
            if (_keyPresence[hash])
            {
                value = _lookupTable[hash];
                return true;
            }

            value = default;
            return false;
        }
    }
}

