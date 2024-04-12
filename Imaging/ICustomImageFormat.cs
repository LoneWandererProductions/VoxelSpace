/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ICustomImageFormat.cs
 * PURPOSE:     Interface of our custom Image Format
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

using System.Drawing;

namespace Imaging
{
    /// <summary>
    /// Interface for Custom Image Format
    /// </summary>
    internal interface ICustomImageFormat
    {
        /// <summary>
        /// Gets the cif file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A bitmap Image</returns>
        Bitmap GetCifFile(string path);

        /// <summary>
        /// Gets the cif file.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Custom Image Format of the Image</returns>
        Cif GetCifFile(Bitmap image);

        /// <summary>
        /// Saves to cif file.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="path">The path.</param>
        void SaveToCifFile(Bitmap image, string path);

        /// <summary>
        /// Compressed cif file.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="path">The path.</param>
        void CompressedToCifFile(Bitmap image, string path);
    }
}
