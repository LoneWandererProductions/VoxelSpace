/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     ExtendedSystemObjects
* FILE:        ExtendedSystemObjects/CategorizedDictionary.cs
* PURPOSE:     Extended Dictionary with an Category.
* PROGRAMER:   Peter Geinitz (Wayfarer)
*/

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ExtendedSystemObjects
{
    /// <inheritdoc cref="IEnumerable" />
    /// <summary>
    ///     Dictionary with a category.
    /// </summary>
    /// <typeparam name="TK">Key Type</typeparam>
    /// <typeparam name="TV">Value Type</typeparam>
    public sealed class CategorizedDictionary<TK, TV> : IEnumerable, IEquatable<CategorizedDictionary<TK, TV>>
    {
        /// <summary>
        ///     The internal data of our custom Dictionary
        /// </summary>
        private readonly Dictionary<TK, (string Category, TV Value)> _data;

        /// <summary>
        ///     The lock for thread safety
        /// </summary>
        private readonly ReaderWriterLockSlim _lock;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CategorizedDictionary{TK, TV}" /> class.
        /// </summary>
        public CategorizedDictionary()
        {
            _data = new Dictionary<TK, (string Category, TV Value)>();
            _lock = new ReaderWriterLockSlim();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CategorizedDictionary{TK, TV}" /> class.
        /// </summary>
        /// <param name="count">The count.</param>
        public CategorizedDictionary(int count)
        {
            _data = new Dictionary<TK, (string Category, TV Value)>(count);
            _lock = new ReaderWriterLockSlim();
        }

        /// <summary>
        ///     Gets the number of elements contained in the CategorizedDictionary.
        /// </summary>
        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _data.Count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns an enumerator for iterating over the dictionary's key-value pairs.
        /// </summary>
        /// <returns>An enumerator for the dictionary.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Checks for equality between two CategorizedDictionary instances.
        /// </summary>
        /// <param name="other">The other CategorizedDictionary to compare.</param>
        /// <returns>True if equal, otherwise false.</returns>
        public bool Equals(CategorizedDictionary<TK, TV> other)
        {
            if (other == null || Count != other.Count) return false;

            foreach (var (key, category, value) in this)
            {
                if (!other.TryGetValue(key, out var otherValue)) return false;

                var otherCategory = other.GetCategoryAndValue(key)?.Category ?? string.Empty;

                if (!string.Equals(category, otherCategory, StringComparison.OrdinalIgnoreCase) ||
                    !EqualityComparer<TV>.Default.Equals(value, otherValue))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Gets the keys.
        /// </summary>
        /// <returns>List of Keys</returns>
        public IEnumerable<TK> GetKeys()
        {
            _lock.EnterReadLock();
            try
            {
                return _data.Keys; // Create a copy for thread safety
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Adds a value to the dictionary under the specified category.
        /// </summary>
        /// <param name="category">The category under which to add the key-value pair. Can be null.</param>
        /// <param name="key">The key of the value to add.</param>
        /// <param name="value">The value to add.</param>
        public void Add(string category, TK key, TV value)
        {
            _lock.EnterWriteLock();
            try
            {
                if (_data.ContainsKey(key))
                    throw new ArgumentException($"{ExtendedSystemObjectsResources.ErrorKeyExists}{key}");

                _data[key] = (category, value);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TK key, TV value)
        {
            _lock.EnterWriteLock();
            try
            {
                Add(string.Empty, key, value);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Gets a value from the dictionary based on the key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value if found, otherwise the default value for the type.</returns>
        public TV Get(TK key)
        {
            _lock.EnterReadLock();
            try
            {
                return _data.TryGetValue(key, out var entry) ? entry.Value : default;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Gets the category and value from the dictionary based on the key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>A tuple containing the category and value if found, otherwise null.</returns>
        public (string Category, TV Value)? GetCategoryAndValue(TK key)
        {
            _lock.EnterReadLock();
            try
            {
                return _data.TryGetValue(key, out var entry) ? entry : null;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Gets all key-value pairs under the specified category.
        /// </summary>
        /// <param name="category">The category to retrieve values from. Can be null.</param>
        /// <returns>A dictionary of key-value pairs in the specified category.</returns>
        public Dictionary<TK, TV> GetCategory(string category)
        {
            _lock.EnterReadLock();
            try
            {
                return _data
                    .Where(entry => entry.Value.Category == category)
                    .ToDictionary(entry => entry.Key, entry => entry.Value.Value);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Gets all categories.
        /// </summary>
        /// <returns>An enumerable of all categories.</returns>
        public IEnumerable<string> GetCategories()
        {
            _lock.EnterReadLock();
            try
            {
                return _data.Values
                    .Select(entry => entry.Category)
                    .Distinct();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Updates the category of an existing entry.
        /// </summary>
        /// <param name="key">The key of the entry to update.</param>
        /// <param name="newCategory">The new category to assign to the entry.</param>
        /// <returns>True if the entry was updated, false if the key does not exist.</returns>
        public bool SetCategory(TK key, string newCategory)
        {
            _lock.EnterWriteLock();
            try
            {
                if (!_data.TryGetValue(key, out var entry)) return false;

                _data[key] = (newCategory, entry.Value);
                return true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Tries to get the category for a given key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="category">The category associated with the key.</param>
        /// <returns>True if the key exists, otherwise false.</returns>
        public bool TryGetCategory(TK key, out string category)
        {
            _lock.EnterReadLock();
            try
            {
                if (_data.TryGetValue(key, out var entry))
                {
                    category = entry.Category;
                    return true;
                }

                category = null;
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Tries to get the value for a given key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <returns>True if the key exists, otherwise false.</returns>
        public bool TryGetValue(TK key, out TV value)
        {
            _lock.EnterReadLock();
            try
            {
                if (_data.TryGetValue(key, out var entry))
                {
                    value = entry.Value;
                    return true;
                }

                value = default;
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Converts to key value list.
        /// </summary>
        /// <returns>A list of Keys and Values</returns>
        public List<KeyValuePair<TK, TV>> ToKeyValueList()
        {
            _lock.EnterReadLock();
            try
            {
                return _data.Select(entry => new KeyValuePair<TK, TV>(entry.Key, entry.Value.Value)).ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Checks if two CategorizedDictionary instances are equal and provides a message.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expected">The expected dictionary.</param>
        /// <param name="actual">The actual dictionary.</param>
        /// <param name="message">The message.</param>
        /// <returns>True if dictionaries are equal, otherwise false.</returns>
        public static bool AreEqual<TKey, TValue>(CategorizedDictionary<TKey, TValue> expected,
            CategorizedDictionary<TKey, TValue> actual, out string message)
        {
            if (expected == null || actual == null)
            {
                message = ExtendedSystemObjectsResources.NullDictionaries;
                return false;
            }

            if (expected.Equals(actual))
            {
                message = ExtendedSystemObjectsResources.DictionariesEqual;
                return true;
            }

            message = ExtendedSystemObjectsResources.DictionaryComparisonFailed;
            return false;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns a string representation of the dictionary's contents.
        /// </summary>
        /// <returns>A string representing the dictionary's contents.</returns>
        public override string ToString()
        {
            _lock.EnterReadLock();
            try
            {
                var entries = _data.Select(entry =>
                    string.Format(ExtendedSystemObjectsResources.KeyCategoryValueFormat, entry.Key,
                        entry.Value.Category,
                        entry.Value.Value));

                return string.Join(Environment.NewLine, entries);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Returns an enumerator for iterating over the dictionary's key-value pairs.
        /// </summary>
        /// <returns>An enumerator for the dictionary.</returns>
        public IEnumerator<(TK Key, string Category, TV Value)> GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                return _data.Select(entry => (entry.Key, entry.Value.Category, entry.Value.Value)).GetEnumerator();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as CategorizedDictionary<TK, TV>);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 17;

                foreach (var (key, category, value) in this)
                {
                    hashCode = hashCode * 23 + EqualityComparer<TK>.Default.GetHashCode(key);
                    hashCode = hashCode * 23 + (category?.GetHashCode() ?? 0);
                    hashCode = hashCode * 23 + EqualityComparer<TV>.Default.GetHashCode(value);
                }

                return hashCode;
            }
        }
    }
}