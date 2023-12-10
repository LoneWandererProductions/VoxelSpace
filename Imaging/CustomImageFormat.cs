/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ICustomImageFormat.cs
 * PURPOSE:     Implementation and main entry of our custom Image Format
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
    public sealed class CustomImageFormat : ICustomImageFormat
    {
        /// <summary>
        ///     Gets the cif file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Image</returns>
        [return: MaybeNull]
        public Bitmap GetCifFile(string path)
        {
            var image = CsvHandler.ReadCsv(path, ImagingResources.Separator);
            if (image == null) return null;

            //compressed or not
            return image[0][2] == ImagingResources.CifCompressed
                ? CifProcessing.CifToImageCompressed(image)
                : CifProcessing.CifToImage(image);
        }

        /// <summary>
        ///     Gets the cif file.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Cif Object</returns>
        public Cif GetCifFile(Bitmap image)
        {
            var format = CifProcessing.ConvertToCif(image);

            return new Cif
            {
                Compressed = false,
                Height = image.Height,
                Width = image.Width,
                CifImage = format,
                NumberOfColors = format.Count
            };
        }

        /// <summary>
        ///     Saves to cif file.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="path">The path.</param>
        public void SaveToCifFile(Bitmap image, string path)
        {
            var imageFormat = CifProcessing.ConvertToCif(image);
            var lst = CifProcessing.GenerateCsv(image.Height, image.Width, imageFormat);
            CsvHandler.WriteCsv(path, lst);
        }

        /// <summary>
        ///     Compresses the cif file.
        ///     Uses sequences to minimize the size of the file.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="path">The path.</param>
        public void CompressedToCifFile(Bitmap image, string path)
        {
            var imageFormat = CifProcessing.ConvertToCif(image);
            var lst = CifProcessing.GenerateCsvCompressed(image.Height, image.Width, imageFormat);
            CsvHandler.WriteCsv(path, lst);
        }
    }
}