/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/TransactionLogs.cs
 * PURPOSE:     Basic Transaction Log, log Changes
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     Basic Transaction Log with unique entries and generic entries
    /// </summary>
    public sealed class TransactionLogs
    {
        /// <summary>
        ///     The lock for non-thread-safe operations.
        /// </summary>
        private readonly object _lock = new();

        private int _changedFlag; // 0 = false, 1 = true

        /// <summary>
        ///     Initializes a new instance of the <see cref="TransactionLogs" /> class.
        /// </summary>
        public TransactionLogs()
        {
            Changelog = new ConcurrentDictionary<int, LogEntry>();
        }

        /// <summary>
        ///     The changelog
        /// </summary>
        public ConcurrentDictionary<int, LogEntry> Changelog { get; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="TransactionLogs" /> is changed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if changed; currentSequence-wise, <c>false</c>.
        /// </value>
        public bool Changed
        {
            get => Interlocked.CompareExchange(ref _changedFlag, 0, 0) == 1;
            private set => Interlocked.Exchange(ref _changedFlag, value ? 1 : 0);
        }

        /// <summary>
        ///     Adds the specified unique identifier.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="item">The item.</param>
        /// <param name="startData">if set to <c>true</c> [start data].</param>
        public void Add(int uniqueIdentifier, object item, bool startData)
        {
            lock (_lock)
            {
                var log = new LogEntry
                {
                    State = LogState.Add, Data = item, UniqueIdentifier = uniqueIdentifier, StartData = startData
                };

                // Add the new log entry to the Changelog
                Changelog[Changelog.Count] = log; // Using index as the key
                Changed = true;
            }
        }

        /// <summary>
        ///     Removes the specified unique identifier.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        public void Remove(int uniqueIdentifier)
        {
            lock (_lock)
            {
                var id = GetItem(uniqueIdentifier, LogState.Add);
                if (id != -1)
                {
                    var item = Changelog[id].Data;
                    var log = new LogEntry
                    {
                        State = LogState.Remove, Data = item, UniqueIdentifier = uniqueIdentifier
                    };
                    Changelog[Changelog.Count] = log; // Add remove log entry
                    Changed = true;
                }
            }
        }

        /// <summary>
        ///     Changes the specified unique identifier.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="item">The item.</param>
        public void Change(int uniqueIdentifier, object item)
        {
            lock (_lock)
            {
                if (GetItem(uniqueIdentifier, LogState.Change) is var entry && entry != -1)
                {
                    var existingItem = Changelog[entry];
                    if (!existingItem.Data.Equals(item))
                    {
                        Changelog[entry] = new LogEntry
                        {
                            State = LogState.Change, Data = item, UniqueIdentifier = uniqueIdentifier
                        };
                        Changed = true;
                    }
                }
                else
                {
                    var log = new LogEntry
                    {
                        State = LogState.Change, Data = item, UniqueIdentifier = uniqueIdentifier
                    };
                    Changelog[Changelog.Count] = log; // Add change log entry
                    Changed = true;
                }
            }
        }

        /// <summary>
        ///     Gets the predecessor of a given entry.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The predecessor's key or -1 if not found.</returns>
        public int GetPredecessor(int id)
        {
            lock (_lock)
            {
                var unique = Changelog[id].UniqueIdentifier;

                foreach (var item in Changelog.Reverse().Where(item =>
                             item.Key < id && item.Value.UniqueIdentifier == unique &&
                             item.Value.State == LogState.Add))
                    return item.Key;

                return -1;
            }
        }

        /// <summary>
        ///     Gets the new items.
        /// </summary>
        /// <returns>Newly added items</returns>
        public Dictionary<int, LogEntry> GetNewItems()
        {
            lock (_lock)
            {
                return Changelog.Count == 0
                    ? null
                    : Changelog.Where(item => !item.Value.StartData).ToDictionary(item => item.Key, item => item.Value);
            }
        }

        /// <summary>
        ///     Gets the item for a specific state.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="state">State of Item</param>
        /// <returns>ChangedItem</returns>
        internal int GetItem(int uniqueIdentifier, LogState state)
        {
            lock (_lock)
            {
                if (Changelog == null || Changelog.Count == 0) return -1;

                foreach (var (key, value) in Changelog.Reverse())
                    if (value.UniqueIdentifier == uniqueIdentifier && value.State == state)
                        return key;

                return -1;
            }
        }

        /// <summary>
        ///     Gets the next available key.
        /// </summary>
        /// <returns>First available Key</returns>
        public int GetNewKey()
        {
            lock (_lock)
            {
                if (Changelog == null || Changelog.Count == 0) return 0;

                var lst = Changelog.Keys.ToList();
                return Utility.GetFirstAvailableIndex(lst);
            }
        }
    }
}