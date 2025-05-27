/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/EnumerableCompare.cs
 * PURPOSE:     Compare operator, for now mostly Enumerable
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     Compare conditions for lists
    /// </summary>
    public enum EnumerableCompare
    {
        /// <summary>
        ///     Ignore order and count
        /// </summary>
        IgnoreOrderCount = 0,

        /// <summary>
        ///     Ignore order, but not count
        /// </summary>
        IgnoreOrder = 1,

        /// <summary>
        ///     List must be identical in order and count
        /// </summary>
        AllEqual = 2
    }
}