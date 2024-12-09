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
        // General Messages

        /// <summary>
        /// The error message displayed when a file is not found. Value: "File not found."
        /// </summary>
        internal const string FileNotFoundMessage = "File not found.";

        /// <summary>
        /// The error message displayed when the file is not a valid GIF. Value: "Not a valid GIF file."
        /// </summary>
        internal const string InvalidGifMessage = "Not a valid GIF file.";

        /// <summary>
        /// The message displayed when skipping padding or a sub-block terminator. Value: "Skipping padding or sub-block terminator (0x00)"
        /// </summary>
        internal const string SkipPaddingMessage = "Skipping padding or sub-block terminator (0x00)";

        /// <summary>
        /// The message displayed when the GIF trailer is found, signaling the parsing is complete. Value: "GIF Trailer found, parsing complete."
        /// </summary>
        internal const string GifTrailerMessage = "GIF Trailer found, parsing complete.";

        /// <summary>
        /// The description for image frames. Value: "Image Frame"
        /// </summary>
        internal const string ImageFrameDescription = "Image Frame";

        // Formatting

        /// <summary>
        /// The message format for processing a block. Example: "Processing block: 0x{0:X2}"
        /// </summary>
        internal const string ProcessingBlockMessage = "Processing block: 0x{0:X2}";

        /// <summary>
        /// The message format for an unknown block being encountered. Example: "Unknown block encountered: 0x{0:X2}. Skipping."
        /// </summary>
        internal const string UnknownBlockMessage = "Unknown block encountered: 0x{0:X2}. Skipping.";

        /// <summary>
        /// The message format for skipping an unknown block. Example: "Skipping unknown block: 0x{0:X2}"
        /// </summary>
        internal const string SkipUnknownBlockMessage = "Skipping unknown block: 0x{0:X2}";

        /// <summary>
        /// The message format for skipping an extension block. Example: "Skipping extension block of size: {0}"
        /// </summary>
        internal const string SkipExtensionBlockMessage = "Skipping extension block of size: {0}";

        // GIF Format

        /// <summary>
        /// The length of the GIF header in bytes. Value: 6
        /// </summary>
        internal const int GifHeaderLength = 6;

        /// <summary>
        /// The expected start string for a valid GIF header. Value: "GIF"
        /// </summary>
        internal const string GifHeaderStart = "GIF";

        /// <summary>
        /// The meta data needed for a valid GIF header. Value: "gif"
        /// </summary>
        internal const string GifMetadata = "gif";

        /// <summary>
        /// The GIF metadata query delay. Value: "/grctlext/Delay"
        /// </summary>
        internal const string GifMetadataQueryDelay = "/grctlext/Delay";

        /// <summary>
        /// The flag indicating the presence of a global color table in the GIF file. Value: 0x80
        /// </summary>
        internal const int GlobalColorTableFlag = 0x80;

        /// <summary>
        /// The bitmask for extracting the color resolution field from the packed fields in the GIF file. Value: 0x70
        /// </summary>
        internal const int ColorResolutionMask = 0x70;

        /// <summary>
        /// The bitmask for extracting the size of the color table from the packed fields in the GIF file. Value: 0x07
        /// </summary>
        internal const int TableSizeMask = 0x07;

        // Blocks and IDs

        /// <summary>
        /// The ID for padding or sub-block terminator in GIF files. Value: 0x00
        /// </summary>
        internal const byte PaddingBlockId = 0x00;

        /// <summary>
        /// The ID indicating an extension introducer in the GIF file. Value: 0x21
        /// </summary>
        internal const byte ExtensionIntroducer = 0x21;

        /// <summary>
        /// The label for application extensions in the GIF file. Value: 0xFF
        /// </summary>
        internal const byte ApplicationExtensionLabel = 0xFF;

        /// <summary>
        /// The label for graphics control extensions in the GIF file. Value: 0xF9
        /// </summary>
        internal const byte GraphicsControlExtensionLabel = 0xF9;

        /// <summary>
        /// The ID for image descriptors in the GIF file. Value: 0x2C
        /// </summary>
        internal const byte ImageDescriptorId = 0x2C;

        /// <summary>
        /// The ID for the trailer block in the GIF file, signaling the end of the file. Value: 0x3B
        /// </summary>
        internal const byte TrailerBlockId = 0x3B;

        /// <summary>
        /// The length of the image descriptor block in the GIF file, in bytes. Value: 9
        /// </summary>
        internal const int ImageDescriptorLength = 9;

        /// <summary>
        /// The flag indicating the presence of a local color table in the GIF file. Value: 0x80
        /// </summary>
        internal const byte LocalColorTableFlag = 0x80;

        // Application Extension

        /// <summary>
        /// The identifier string for the NETSCAPE application extension. Value: "NETSCAPE"
        /// </summary>
        internal const string NetScapeIdentifier = "NETSCAPE";

        /// <summary>
        /// The length of the application identifier in the application extension block. Value: 8
        /// </summary>
        internal const int AppIdentifierLength = 8;

        /// <summary>
        /// The length of the application authentication code in the application extension block. Value: 3
        /// </summary>
        internal const int AppAuthCodeLength = 3;

        // Miscellaneous

        /// <summary>
        /// The divisor used for converting delay times in the GIF file from hundredths of a second to seconds. Value: 100.0
        /// </summary>
        internal const double DelayDivisor = 100.0;

        /// <summary>
        /// The ID for the block terminator in the GIF file. Value: 0x00
        /// </summary>
        internal const byte TerminatorBlockId = 0x00;

        /// <summary>
        ///     The error missing file (const). Value: "File not Found: ".
        /// </summary>
        internal const string ErrorFileNotFound = "File not Found: ";

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
        ///     The Interval Splitter (const). Value: "-".
        /// </summary>
        internal const string IntervalSplitter = "-";

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
        ///     The error, interface is null (const). Value: "Error: Interface is Null."
        /// </summary>
        internal const string ErrorInterface = "Error: Interface is Null.";

        /// <summary>
        ///     The error, image is null (const). Value: "Error: Image is Null."
        /// </summary>
        internal const string ErrorImage = "Error: Image is Null.";

        /// <summary>
        ///     The error, Radius is smaller null (const). Value: "Error: Radius cannot be negative."
        /// </summary>
        internal const string ErrorRadius = "Error: Radius cannot be negative.";

        /// <summary>
        ///     The error out of bounds (const). Value: "Error: Point is outside the bounds of the image."
        /// </summary>
        internal const string ErrorOutOfBounds = "Error: Point is outside the bounds of the image.";

        /// <summary>
        ///     The error Invalid Operation (const). Value: "Error: Bits array is not properly initialized."
        /// </summary>
        internal const string ErrorInvalidOperation = "Error: Bits array is not properly initialized.";

        /// <summary>
        ///     The error for Pixel Operation (const). Value: "Error setting pixels: "
        /// </summary>
        internal const string ErrorPixel = "Error setting pixels: ";

        /// <summary>
        ///     The error, Path is null (const). Value: "Error: Path is Null."
        /// </summary>
        internal const string ErrorPath = "Error: Path is Null.";

        /// <summary>
        ///     The error, could not load Setting (const). Value: "Error loading Configuration:"
        /// </summary>
        internal const string ErrorLoadSettings = "Error loading Configuration:";

        /// <summary>
        ///     The error, min/ max value exceeded 0 or 255 (const). Value: "Error: minValue and maxValue must be between 0 and
        ///     255, and minValue must not be greater than maxValue."
        /// </summary>
        internal const string ErrorColorRange =
            "Error: minValue and maxValue must be between 0 and 255, and minValue must not be greater than maxValue.";

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