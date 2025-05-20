/*
 * COPYRIGHT:   See COPYING in the top-level directory
 * PROJECT:     CommonLibraryTests
 * FILE:        CommonLibraryTests/DynamicEnum.cs
 * PURPOSE:     Framework for our Dynamic Enum
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBeInternal

using System;
using System.Collections.Generic;

namespace ExtendedSystemObjects
{
    /// <summary>
    ///     A dynamic enum framework that allows for the creation of custom enums with dynamic
    ///     addition, removal, and retrieval of entries at runtime.
    /// </summary>
    /// <typeparam name="T">
    ///     The type that inherits from <see cref="T:ExtendedSystemObjects.DynamicEnum`1" />. This is typically
    ///     a class that defines specific instances of the enum.
    /// </typeparam>
    public class DynamicEnum<T> : IEquatable<DynamicEnum<T>>, IComparable<DynamicEnum<T>> where T : DynamicEnum<T>
    {
        /// <summary>
        ///     A dictionary that holds all the enum values, indexed by their names.
        /// </summary>
        private static readonly Dictionary<string, T> Values = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicEnum{T}" /> class.
        ///     This constructor is called when a new enum entry is created.
        /// </summary>
        /// <param name="name">The name of the enum entry.</param>
        /// <param name="value">The integer value of the enum entry.</param>
        protected DynamicEnum(string name, int value)
        {
            Name = name;
            Value = value;
            Values[name] = (T)this;
        }

        /// <summary>
        ///     Gets the name of the enum entry.
        /// </summary>
        /// <value>
        ///     A string representing the name of the enum entry.
        /// </value>
        public string Name { get; }

        /// <summary>
        ///     Gets the integer value associated with the enum entry.
        /// </summary>
        /// <value>
        ///     An integer value representing the enum entry.
        /// </value>
        public int Value { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
        ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings:
        ///     <list type="table">
        ///         <listheader>
        ///             <term> Value</term><description> Meaning</description>
        ///         </listheader>
        ///         <item>
        ///             <term> Less than zero</term>
        ///             <description> This instance precedes <paramref name="other" /> in the sort order.</description>
        ///         </item>
        ///         <item>
        ///             <term> Zero</term>
        ///             <description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description>
        ///         </item>
        ///         <item>
        ///             <term> Greater than zero</term>
        ///             <description> This instance follows <paramref name="other" /> in the sort order.</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public int CompareTo(DynamicEnum<T> other)
        {
            return other == null ? 1 : Value.CompareTo(other.Value);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public bool Equals(DynamicEnum<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Value == other.Value && Name == other.Name;
        }

        /// <summary>
        ///     Adds a new entry to the enum or returns the existing entry if it already exists.
        /// </summary>
        /// <param name="name">The name of the enum entry to add.</param>
        /// <param name="value">The integer value of the enum entry.</param>
        /// <returns>
        ///     The <typeparamref name="T" /> instance representing the newly added or existing enum entry.
        /// </returns>
        /// <remarks>
        ///     If the entry already exists, this method will return the existing entry without creating a new instance.
        /// </remarks>
        public static T Add(string name, int value)
        {
            if (!Values.ContainsKey(name))
            {
                return (T)Activator.CreateInstance(typeof(T), name, value)!;
            }

            return Values[name];
        }

        /// <summary>
        ///     Removes the specified enum entry by name.
        /// </summary>
        /// <param name="name">The name of the enum entry to remove.</param>
        /// <returns>
        ///     A boolean value indicating whether the removal was successful.
        /// </returns>
        /// <remarks>
        ///     If the entry does not exist, this method returns false and does not affect the dictionary.
        /// </remarks>
        public static bool Remove(string name)
        {
            return Values.Remove(name);
        }

        /// <summary>
        ///     Attempts to retrieve an enum entry by name.
        /// </summary>
        /// <param name="name">The name of the enum entry to retrieve.</param>
        /// <param name="result">The resulting enum entry, if found.</param>
        /// <returns>
        ///     A boolean value indicating whether the enum entry was found.
        /// </returns>
        /// <remarks>
        ///     If the entry does not exist, <paramref name="result" /> will be set to null.
        /// </remarks>
        public static bool TryGet(string name, out T result)
        {
            return Values.TryGetValue(name, out result);
        }

        /// <summary>
        ///     Retrieves all enum entries.
        /// </summary>
        /// <returns>
        ///     A read-only collection of all <typeparamref name="T" /> enum entries.
        /// </returns>
        /// <remarks>
        ///     The collection is read-only, meaning entries cannot be modified directly.
        /// </remarks>
        public static IReadOnlyCollection<T> GetAll()
        {
            return Values.Values;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Converts the enum entry to its string representation.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> representing the name of the enum entry.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Determines whether two instances are equal by comparing their name and value.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if both instances are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is DynamicEnum<T> other)
            {
                return Equals(other);
            }

            return false;
        }

        /// <summary>
        ///     Determines whether two instances are equal using the equality operator.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>True if the instances are equal; otherwise, false.</returns>
        public static bool operator ==(DynamicEnum<T> left, DynamicEnum<T> right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether two instances are not equal using the inequality operator.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>True if the instances are not equal; otherwise, false.</returns>
        public static bool operator !=(DynamicEnum<T> left, DynamicEnum<T> right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the hash code based on name and value.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="DynamicEnum{T}" /> to <see cref="System.Int32" />.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator int(DynamicEnum<T> e)
        {
            return e.Value;
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="DynamicEnum{T}" /> to <see cref="System.String" />.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator string(DynamicEnum<T> e)
        {
            return e.Name;
        }
    }
}
