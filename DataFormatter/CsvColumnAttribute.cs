/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/CsvColumnAttribute.cs
 * PURPOSE:     Helper for csv, Helps converter
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;

namespace DataFormatter
{
    /// <inheritdoc />
    /// <summary>
    ///     Needed for our custom csv conversion
    /// </summary>
    /// <seealso cref="T:System.Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CsvColumnAttribute : Attribute
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DataFormatter.CsvColumnAttribute" /> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="converterType">Type of the converter.</param>
        public CsvColumnAttribute(int index, Type converterType)
        {
            Index = index;
            ConverterType = converterType;
        }

        /// <summary>
        ///     Gets the index of the row.
        /// </summary>
        /// <value>
        ///     The index.
        /// </value>
        internal int Index { get; }

        /// <summary>
        ///     Gets the type of the converter.
        /// </summary>
        /// <value>
        ///     The type of the converter.
        /// </value>
        internal Type ConverterType { get; }
    }
}