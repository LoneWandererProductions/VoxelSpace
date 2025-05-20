/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/VaultItem.cs
 * PURPOSE:     Holds an Item that can expire
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     Vault item with expiration and data tracking
    /// </summary>
    /// <typeparam name="TU">Generic Type</typeparam>
    internal sealed class VaultItem<TU>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VaultItem{TU}" /> class.
        ///     Needed for Json serialization.
        /// </summary>
        public VaultItem() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VaultItem{U}" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="expiryTime">The expiry time.</param>
        /// <param name="description">A short description of the item, optional.</param>
        public VaultItem(TU data, TimeSpan? expiryTime, string description = "")
        {
            Data = data;
            ExpiryTime = expiryTime;
            if (expiryTime != null)
            {
                ExpiryDate = DateTime.UtcNow.Add((TimeSpan)expiryTime);
            }
            else
            {
                HasExpireTime = false;
            }

            Description = description;
            CreationDate = DateTime.Now;
            DataSize = MeasureMemoryUsage(data);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance has expire time.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has expire time; otherwise, <c>false</c>.
        /// </value>
        public bool HasExpireTime { get; set; }

        /// <summary>
        ///     Gets or sets the size of the data.
        /// </summary>
        /// <value>
        ///     The size of the data.
        /// </value>
        public long DataSize { get; }

        /// <summary>
        ///     Gets or sets the data.
        /// </summary>
        /// <value>
        ///     The data.
        /// </value>
        public TU Data { get; }

        /// <summary>
        ///     Gets the expiry date.
        /// </summary>
        /// <value>
        ///     The expiry date.
        /// </value>
        public DateTime ExpiryDate { get; }

        /// <summary>
        ///     Gets or sets the expiry time.
        /// </summary>
        /// <value>
        ///     The expiry time.
        /// </value>
        public TimeSpan? ExpiryTime { get; }

        /// <summary>
        ///     Gets or sets the creation date.
        /// </summary>
        /// <value>
        ///     The creation date.
        /// </value>
        public DateTime CreationDate { get; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        ///     HasExpired checks if the item has passed its expiration timed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has expired; otherwise, <c>false</c>.
        /// </value>
        public bool HasExpired => DateTime.UtcNow > ExpiryDate;

        /// <summary>
        ///     Gets or sets the additional metadata.
        ///     Future Proving for custom user data.
        /// </summary>
        /// <value>
        ///     The additional metadata.
        /// </value>
        public Dictionary<string, object> AdditionalMetadata { get; init; } = new();

        /// <summary>
        ///     Calculates the size of an object using memory allocation.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="data">The object to calculate the size of.</param>
        /// <returns>The size in bytes.</returns>
        private static long MeasureMemoryUsage<T>(T data)
        {
            if (data == null)
            {
                return 0;
            }

            var before = GC.GetTotalMemory(true);
            _ = data; // Ensures the data is referenced
            var after = GC.GetTotalMemory(true);
            return after - before;
        }
    }
}
