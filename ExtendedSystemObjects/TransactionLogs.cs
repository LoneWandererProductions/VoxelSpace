/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/TransactionLogs.cs
 * PURPOSE:     Basic Transaction Log, log Changes
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Generic;
using System.Linq;

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     Basic Transaction Log with unique entries and generic entries
    /// </summary>
    public sealed class TransactionLogs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TransactionLogs" /> class.
        /// </summary>
        public TransactionLogs()
        {
            Changelog = new Dictionary<int, LogEntry>();
        }

        /// <summary>
        ///     The changelog
        /// </summary>
        public Dictionary<int, LogEntry> Changelog { get; init; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="TransactionLogs" /> is changed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if changed; currentSequencewise, <c>false</c>.
        /// </value>
        public bool Changed { get; private set; }

        /// <summary>
        ///     Adds the specified unique identifier.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="item">The item.</param>
        /// <param name="startData">if set to <c>true</c> [start data].</param>
        public void Add(int uniqueIdentifier, object item, bool startData)
        {
            var log = new LogEntry
            {
                State = LogState.Add, Data = item, UniqueIdentifier = uniqueIdentifier, StartData = startData
            };
            Changelog.Add(Changelog.Count, log);

            Changed = true;
        }

        /// <summary>
        ///     Adds the specified unique identifier.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        public void Remove(int uniqueIdentifier)
        {
            var id = GetItem(uniqueIdentifier, LogState.Add);
            var item = Changelog[id].Data;

            var log = new LogEntry { State = LogState.Remove, Data = item, UniqueIdentifier = uniqueIdentifier };
            Changelog.Add(Changelog.Count, log);

            Changed = true;
        }

        /// <summary>
        ///     Adds the specified unique identifier.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="item">The item.</param>
        public void Change(int uniqueIdentifier, object item)
        {
            var entry = GetItem(uniqueIdentifier, LogState.Change);

            if (entry == -1)
            {
                var log = new LogEntry { State = LogState.Change, Data = item, UniqueIdentifier = uniqueIdentifier };
                Changelog.Add(Changelog.Count, log);

                Changed = true;
            }
            else
            {
                var existingItem = Changelog[entry];

                if (existingItem.Equals(item)) return;

                Changelog[entry] = new LogEntry
                {
                    State = LogState.Change, Data = item, UniqueIdentifier = uniqueIdentifier
                };

                Changed = true;
            }
        }

        /// <summary>
        ///     Gets the predecessor.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The predecessor</returns>
        public int GetPredecessor(int id)
        {
            var unique = Changelog[id].UniqueIdentifier;

            foreach (var item in Changelog.Reverse().Where(item =>
                         item.Key < id && item.Value.UniqueIdentifier == unique && item.Value.State == LogState.Add))
                return item.Key;

            return -1;
        }

        /// <summary>
        ///     Gets the new items.
        /// </summary>
        /// <returns>Newly added items</returns>
        public Dictionary<int, LogEntry> GetNewItems()
        {
            return Changelog.Count == 0
                ? null
                : Changelog.Where(item => !item.Value.StartData).ToDictionary(item => item.Key, item => item.Value);
        }

        /// <summary>
        ///     Gets the item.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="state">State of Item</param>
        /// <returns>ChangedItem</returns>
        private int GetItem(int uniqueIdentifier, LogState state)
        {
            if (Changelog == null || Changelog.Count == 0) return -1;

            foreach (var (key, value) in Changelog.Reverse())
                if (value.UniqueIdentifier == uniqueIdentifier && value.State == state)
                    return key;

            return -1;
        }

        /// <summary>
        ///     Gets the new key.
        /// </summary>
        /// <returns>First available Key</returns>
        public int GetNewKey()
        {
            if (Changelog == null) return -1;

            if (Changelog.Count == 0) return 0;

            var lst = Changelog.Keys.ToList();

            return Utility.GetFirstAvailableIndex(lst);
        }
    }
}