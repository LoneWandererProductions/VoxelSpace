/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ExtendedSystemObjects
 * FILE:        ExtendedSystemObjects/ValueNotFoundException.cs
 * PURPOSE:     New Exceptions for ExtendedSystemObjects
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Runtime.Serialization;

namespace ExtendedSystemObjects
{
    /// <inheritdoc />
    /// <summary>
    ///     The value not found exception class.
    /// </summary>
    /// <seealso cref="T:System.Exception" />
    [Serializable]
    public sealed class ValueNotFoundException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ExtendedSystemObjects.ValueNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ValueNotFoundException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ExtendedSystemObjects.ValueNotFoundException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        private ValueNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ExtendedSystemObjects.ValueNotFoundException" /> class.
        /// </summary>
        public ValueNotFoundException()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ExtendedSystemObjects.ValueNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message we declare</param>
        /// <param name="innerException">
        ///     The Exception that caused the Exception or a null reference <see langword="Nothing" /> in
        ///     Visual Basic), if there is no inner Exception.
        /// </param>
        public ValueNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
