/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/MemoryVault.cs
 * PURPOSE:     In Memory Storage
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeInternal


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace ExtendedSystemObjects
{
    /// <inheritdoc />
    /// <summary>
    ///     A thread-safe memory vault for managing data with expiration and metadata enrichment.
    /// </summary>
    /// <typeparam name="TU">Generic type of the data being stored.</typeparam>
    public sealed class MemoryVault<TU> : IDisposable
    {
        /// <summary>
        ///     The instance
        /// </summary>
        private static MemoryVault<TU> _instance;

        /// <summary>
        ///     The instance lock
        /// </summary>
        private static readonly object InstanceLock = new();

        /// <summary>
        ///     The cleanup timer
        /// </summary>
        private readonly Timer _cleanupTimer;

        /// <summary>
        ///     The lock
        /// </summary>
        private readonly ReaderWriterLockSlim _lock = new();

        /// <summary>
        ///     The vault
        /// </summary>
        private readonly Dictionary<long, VaultItem<TU>> _vault;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryVault{TU}" /> class.
        /// </summary>
        private MemoryVault()
        {
            _vault = new Dictionary<long, VaultItem<TU>>();
            // Initialize the cleanup timer
            _cleanupTimer =
                new Timer(CleanupExpiredItems, null, TimeSpan.Zero, TimeSpan.FromMinutes(5)); // Run every 5 minutes
        }

        /// <summary>
        ///     Public static property to access the Singleton instance
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static MemoryVault<TU> Instance
        {
            get
            {
                lock (InstanceLock)
                {
                    return _instance ??= new MemoryVault<TU>();
                }
            }
        }

        /// <summary>
        ///     The memory usage threshold in bytes.
        ///     If the memory usage exceeds this value, the event will be triggered.
        /// </summary>
        public long MemoryThreshold { get; set; } = 10 * 1024 * 1024; // Default 10 MB

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _lock?.Dispose();
            _cleanupTimer?.Dispose();
        }

        /// <summary>
        ///     Event triggered when memory usage exceeds the threshold.
        /// </summary>
        public event EventHandler<VaultMemoryThresholdExceededEventArgs> MemoryThresholdExceeded;

        /// <summary>
        ///     Adds data to the vault with an optional expiration time.
        /// </summary>
        public long Add(TU data, TimeSpan? expiryTime = null, string description = "", long identifier = -1)
        {
            // If identifier is not provided (default -1), generate a new identifier
            if (identifier == -1) identifier = Utility.GetFirstAvailableIndex(_vault.Keys.ToList());

            var expiry = expiryTime;

            var vaultItem = new VaultItem<TU>(data, expiry, description);

            _lock.EnterWriteLock();
            try
            {
                _vault[identifier] = vaultItem;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            // Now calculate memory usage outside the write lock
            var currentMemoryUsage = CalculateMemoryUsage();
            if (currentMemoryUsage > MemoryThreshold)
                MemoryThresholdExceeded?.Invoke(this, new VaultMemoryThresholdExceededEventArgs(currentMemoryUsage));

            return identifier;
        }


        /// <summary>
        ///     Gets the data by its identifier.
        /// </summary>
        public TU Get(long identifier)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                // Try to get the item from the vault
                if (_vault.TryGetValue(identifier, out var item))
                {
                    // Check if the item has expired and has an expiration time
                    if (item.HasExpireTime && item.HasExpired)
                    {
                        // Remove expired item within a write lock
                        _lock.EnterWriteLock();
                        try
                        {
                            _vault.Remove(identifier);
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }

                        // Return default value if the item is expired
                        return default; // For reference types, this will be 'null'
                    }

                    // Return the item's data if it hasn't expired
                    return item.Data;
                }

                // If the item is not found, return default value
                return
                    default; // Or you can throw an exception or return a specific value depending on your requirements
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        ///     Removes an item from the vault by its identifier.
        /// </summary>
        public bool Remove(long identifier)
        {
            _lock.EnterWriteLock();
            try
            {
                return _vault.Remove(identifier);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Gets all non-expired items in the vault.
        /// </summary>
        public List<TU> GetAll()
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                var nonExpiredItems = new List<TU>();
                var expiredKeys = new List<long>();

                // Iterate over the vault and classify items
                foreach (var (identifier, item) in _vault)
                    if (item.HasExpired && item.HasExpireTime)
                        // Add expired item keys for removal
                        expiredKeys.Add(identifier);
                    else
                        // Add non-expired item data
                        nonExpiredItems.Add(item.Data);

                // Remove expired items with a write lock
                if (expiredKeys.Count > 0)
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        foreach (var key in expiredKeys) _vault.Remove(key);
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }

                return nonExpiredItems;
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }


        /// <summary>
        ///     Cleanups the expired items.
        /// </summary>
        /// <param name="state">The state.</param>
        private void CleanupExpiredItems(object state)
        {
            _lock.EnterWriteLock();
            try
            {
                foreach (var key in _vault.Where(kvp => kvp.Value.HasExpired && kvp.Value.HasExpireTime)
                             .Select(kvp => kvp.Key).ToList())
                    _vault.Remove(key);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Calculates the current memory usage of the vault.
        /// </summary>
        private long CalculateMemoryUsage()
        {
            _lock.EnterReadLock();
            try
            {
                // Assuming 'Values' is a collection of items, each having a 'DataSize' property.
                return _vault.Values.Sum(item => item.DataSize);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }


        /// <summary>
        ///     Retrieves metadata for an item.
        /// </summary>
        public VaultMetadata GetMetadata(long identifier)
        {
            _lock.EnterReadLock();
            try
            {
                // Try to get the item from the vault
                if (!_vault.TryGetValue(identifier, out var item)) return default; // Item not found

                // Check if the item has expired and can expire
                if (item.HasExpireTime && item.HasExpired)
                {
                    // Item has expired, remove it
                    _lock.EnterWriteLock();
                    try
                    {
                        _vault.Remove(identifier);
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }

                    return default; // Expired item, return default or indicate expired state
                }

                // Item is valid (not expired or does not expire), return metadata
                return new VaultMetadata
                {
                    Description = item.Description,
                    CreationDate = item.CreationDate,
                    Identifier = identifier,
                    HasExpireTime = item.HasExpireTime
                };
            }
            finally
            {
                _lock.ExitReadLock(); // Ensure read lock is released
            }
        }

        /// <summary>
        ///     Adds metadata to an item.
        /// </summary>
        public void AddMetadata(long identifier, VaultMetadata metaData)
        {
            if (metaData == null) throw new ArgumentNullException(nameof(metaData));

            _lock.EnterWriteLock();
            try
            {
                if (!_vault.ContainsKey(identifier)) return;

                _vault[identifier].Description = metaData.Description;
                _vault[identifier].HasExpireTime = metaData.HasExpireTime;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Saves an item to disk by its identifier in a binary format.
        /// </summary>
        /// <param name="identifier">The identifier of the item.</param>
        /// <param name="filePath">The file path to save the item.</param>
        /// <returns>True if the item is saved successfully; otherwise, false.</returns>
        public bool SaveToDisk(long identifier, string filePath)
        {
            try
            {
                var item = _vault[identifier]; // Retrieve the item from the vault

                // Serialize the VaultItem to JSON
                var json = JsonSerializer.Serialize(item);

                // Write the JSON string to a file
                File.WriteAllText(filePath, json);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Loads an item from disk into the vault from a binary format.
        /// </summary>
        /// <param name="filePath">The file path of the saved item.</param>
        /// <returns>The identifier of the loaded item, or -1 if loading fails.</returns>
        public long LoadFromDisk(string filePath)
        {
            try
            {
                // Read the JSON string from the file
                var json = File.ReadAllText(filePath);

                // Deserialize the JSON string to a VaultItem<TU>
                var item = JsonSerializer.Deserialize<VaultItem<TU>>(json);

                if (item != null)
                {
                    var identifier = Add(item.Data);

                    //set metadata
                    var metadata = new VaultMetadata
                    {
                        Description = item.Description,
                        CreationDate = item.CreationDate,
                        AdditionalMetadata = item.AdditionalMetadata
                    };

                    AddMetadata(identifier, metadata);

                    return identifier;
                }
            }
            catch (Exception)
            {
                return -1; // Return -1 if loading fails
            }

            return -1; // Return -1 if loading fails
        }
    }
}