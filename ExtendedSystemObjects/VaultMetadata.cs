/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/VaultMetadata.cs
 * PURPOSE:     Metadata for all saved Infos about the stored items.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;

namespace ExtendedSystemObjects
{
    public sealed class VaultMetadata
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VaultMetadata" /> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="creationDate">The creation date.</param>
        internal VaultMetadata(long identifier, DateTime creationDate)
        {
            Identifier = identifier;
            CreationDate = creationDate;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VaultMetadata" /> class.
        /// </summary>
        public VaultMetadata()
        {
        }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public long Identifier { get; internal init; }

        /// <summary>
        ///     Gets or sets the creation date.
        /// </summary>
        /// <value>
        ///     The creation date.
        /// </value>
        public DateTime CreationDate { get; internal set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        public string Description { get; init; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance has expire time.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has expire time; otherwise, <c>false</c>.
        /// </value>
        public bool HasExpireTime { get; set; }

        /// <summary>
        ///     Gets or sets the additional metadata.
        ///     Future Proving for custom user data.
        /// </summary>
        /// <value>
        ///     The additional metadata.
        /// </value>
        public Dictionary<string, object> AdditionalMetadata { get; init; } = new();
    }
}