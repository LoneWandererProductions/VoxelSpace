/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ICustomImageFormat.cs
 * PURPOSE:     Implementation and main entry of our custom Image Format
 *              Main use is a small save footprint and Image Color manipulation on a large scale
 *              Further uses are Image analysis, mostly color and color range.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using DataFormatter;

//TODO Checksum in line 1. Max Numbers, custom exception class
// check sum for repairs perhaps?
namespace Imaging
{
    /// <inheritdoc />
    /// <summary>
    ///     The Custom Image Format
    /// </summary>
    /// <seealso cref="T:Imaging.ICustomImageFormat" />
    public sealed class CustomImageFormat : ICustomImageFormat
    {
        /// <inheritdoc />
        /// <summary>
        ///     Load a cif file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Image in pure Cif format</returns>
        [return: MaybeNull]
        public Cif GetCif(string path)
        {
            return CifProcessing.CifFromFile(path);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Generates Image from Cif.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Image</returns>
        [return: MaybeNull]
        public Bitmap GetImageFromCif(string path)
        {
            return CifProcessing.CifFileToImage(path);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Generate Cif from Bitmap
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Cif Object</returns>
        public Cif GenerateCifFromBitmap(Bitmap image)
        {
            var format = CifProcessing.ConvertToCifFromBitmap(image);

            return new Cif
            {
                Compressed = false,
                Height = image.Height,
                Width = image.Width,
                CifImage = format,
                NumberOfColors = format.Count
            };
        }

        /// <inheritdoc />
        /// <summary>
        ///     Saves Bitmap to cif file.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="path">The path.</param>
        public void GenerateBitmapToCifFile(Bitmap image, string path)
        {
            var imageFormat = CifProcessing.ConvertToCifFromBitmap(image);
            var lst = CifProcessing.GenerateCsv(image.Height, image.Width, imageFormat);
            CsvHandler.WriteCsv(path, lst);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Compresses the cif file.
        ///     Uses sequences to minimize the size of the file.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="path">The path.</param>
        public void GenerateCifCompressedFromBitmap(Bitmap image, string path)
        {
            var imageFormat = CifProcessing.ConvertToCifFromBitmap(image);
            var lst = CifProcessing.GenerateCsvCompressed(image.Height, image.Width, imageFormat);
            CsvHandler.WriteCsv(path, lst);
        }
    }
}