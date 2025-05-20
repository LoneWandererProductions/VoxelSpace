/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileDetails.cs
 * PURPOSE:     Contains more Information about certain Files
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal, no it will be used externally
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace FileHandler
{
    /// <summary>
    ///     The file details class.
    /// </summary>
    public sealed class FileDetails
    {
        /// <summary>
        ///     Gets or sets the file name.
        /// </summary>
        public string FileName { get; internal init; }

        /// <summary>
        ///     Gets or sets the original filename.
        /// </summary>
        public string OriginalFilename { get; init; }

        /// <summary>
        ///     Gets or sets the path.
        /// </summary>
        public string Path { get; internal init; }

        /// <summary>
        ///     Gets or sets the extension.
        /// </summary>
        public string Extension { get; internal init; }

        /// <summary>
        ///     Gets or sets the size.
        /// </summary>
        public long Size { get; internal init; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public string Description { get; internal init; }

        /// <summary>
        ///     Gets or sets the company name.
        /// </summary>
        public string CompanyName { get; internal init; }

        /// <summary>
        ///     Gets or sets the product name.
        /// </summary>
        public string ProductName { get; internal init; }

        /// <summary>
        ///     Gets or sets the file version.
        /// </summary>
        public string FileVersion { get; internal init; }

        /// <summary>
        ///     Gets or sets the product version.
        /// </summary>
        public string ProductVersion { get; internal init; }
    }
}
