/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImagingResources.cs
 * PURPOSE:     String Resources
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Generic;

namespace Imaging
{
    /// <summary>
    ///     The com Control resources class.
    /// </summary>
    public static class ImagingResources
    {
        /// <summary>
        ///     The error missing file (const). Value: "File not Found: ".
        /// </summary>
        internal const string ErrorMissingFile = "File not Found: ";

        /// <summary>
        ///     Error, wrong parameters (const). Value: "Wrong Arguments provided".
        /// </summary>
        internal const string ErrorWrongParameters = "Wrong Arguments provided: ";

        /// <summary>
        ///     Memory Error (const). Value: " used Memory: ".
        /// </summary>
        internal const string ErrorMemory = " used Memory: ";

        /// <summary>
        ///     The Spacing (const). Value:  " : ".
        /// </summary>
        internal const string Spacing = " : ";

        /// <summary>
        ///     The Separator (const). Value:  ','.
        /// </summary>
        internal const char Separator = ',';

        /// <summary>
        ///     Separator (const). Value: " , ".
        /// </summary>
        internal const string Indexer = " , ";

        /// <summary>
        ///     Color string (const). Value: "Color: ".
        /// </summary>
        internal const string Color = "Color: ";

        /// <summary>
        ///     The flag that indicates that image is not compressed (const). Value:  "0".
        /// </summary>
        internal const string CifUnCompressed = "0";

        /// <summary>
        ///     The flag that indicates if image is compressed (const). Value:  "1".
        /// </summary>
        internal const string CifCompressed = "1";

        /// <summary>
        ///     The cif Separator used for compression (const). Value:  "-".
        /// </summary>
        internal const string CifSeparator = "-";

        /// <summary>
        ///     The jpg Extension (const). Value: ".jpg"
        /// </summary>
        public const string JpgExt = ".jpg";

        /// <summary>
        ///     The jpeg Extension (const). Value: ".jpeg"
        /// </summary>
        public const string JpegExt = ".jpeg";

        /// <summary>
        ///     The png Extension (const). Value: ".png"
        /// </summary>
        public const string PngExt = ".png";

        /// <summary>
        ///     The Bmp Extension (const). Value: ".Bmp"
        /// </summary>
        public const string BmpExt = ".Bmp";

        /// <summary>
        ///     The Gif Extension (const). Value: ".gif"
        /// </summary>
        public const string GifExt = ".gif";

        /// <summary>
        ///     The Tif Extension (const). Value: ".tif"
        /// </summary>
        public const string TifExt = ".tif";

        /// <summary>
        ///     The File Appendix
        /// </summary>
        public static readonly List<string> Appendix = new()
        {
            JpgExt,
            PngExt,
            BmpExt,
            GifExt,
            TifExt
        };
    }
}
