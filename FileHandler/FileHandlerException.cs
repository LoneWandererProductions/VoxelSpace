/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileHandlerException.cs
 * PURPOSE:     Exception Class
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://msdn.microsoft.com/en-us/library/system.exception.getobjectdata.aspx
 */

using System;
using System.Runtime.Serialization;

namespace FileHandler
{
    /// <inheritdoc />
    /// <summary>
    ///     The file handler exception class.
    /// </summary>
    /// <seealso cref="T:System.Exception" />
    [Serializable]
    public sealed class FileHandlerException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:FileHandler.FileHandlerException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        internal FileHandlerException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:FileHandler.FileHandlerException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        private FileHandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:FileHandler.FileHandlerException" /> class.
        /// </summary>
        public FileHandlerException()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:FileHandler.FileHandlerException" /> class.
        /// </summary>
        /// <param name="message">The message we declarte</param>
        /// <param name="innerException">
        ///     The Exception that caused the Exception or a null reference <see langword="Nothing" /> in
        ///     Visual Basic), if there is no inner Exception.
        /// </param>
        public FileHandlerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}