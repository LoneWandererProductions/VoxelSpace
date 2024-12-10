/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/IIdHandling.cs
 * PURPOSE:     Interface to smooth out my way of switching between Lists and Dictionaries
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     Interface Id handling, for better handling of Dictionaries and list elements
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public interface IIdHandling<TId>
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        TId Id { get; set; }
    }
}