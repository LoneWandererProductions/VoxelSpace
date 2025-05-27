/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/VaultMemoryThresholdExceededEventArgs.cs
 * PURPOSE:     Event when Memory exceeds a certain threshold.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;

namespace ExtendedSystemObjects
{
    /// <inheritdoc />
    /// <summary>
    ///     Event arguments for the memory threshold exceeded event.
    /// </summary>
    public sealed class VaultMemoryThresholdExceededEventArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ExtendedSystemObjects.VaultMemoryThresholdExceededEventArgs" />
        ///     class.
        /// </summary>
        /// <param name="currentMemoryUsage">The current memory usage.</param>
        public VaultMemoryThresholdExceededEventArgs(long currentMemoryUsage)
        {
            CurrentMemoryUsage = currentMemoryUsage;
        }

        /// <summary>
        ///     Gets the current memory usage.
        /// </summary>
        /// <value>
        ///     The current memory usage.
        /// </value>
        public long CurrentMemoryUsage { get; }
    }
}