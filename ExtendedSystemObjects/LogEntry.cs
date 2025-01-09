/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/LogEntry.cs
 * PURPOSE:     Basic Transaction Log Object that holds all Infos
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     Entry that describes Changes to Objects in the Dictionary
    /// </summary>
    public sealed class LogEntry
    {
        /// <summary>
        ///     Gets or sets the data.
        /// </summary>
        /// <value>
        ///     The data.
        /// </value>
        public object Data { get; init; }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        public LogState State { get; internal init; }

        /// <summary>
        ///     Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        ///     The unique identifier.
        /// </value>
        public int UniqueIdentifier { get; init; }

        /// <summary>
        ///     Gets a value indicating whether [start data].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [start data]; currentSequence-wise, <c>false</c>.
        /// </value>
        public bool StartData { get; init; }
    }

    /// <summary>
    ///     Log State
    /// </summary>
    public enum LogState
    {
        Add = 1,
        Remove = 2,
        Change = 3
    }
}
