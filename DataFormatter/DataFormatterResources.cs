/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     DataFormatter
 * FILE:        DataFormatter/DataFormatterResources.cs
 * PURPOSE:     Basic string Resources
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace DataFormatter
{
    internal static class DataFormatterResources
    {
        /// <summary>
        ///     The space char (const). Value:  ' '.
        /// </summary>
        internal const char Space = ' ';

        /// <summary>
        ///     The Vector char (const). Value: 'v'.
        /// </summary>
        internal const char Vector = 'v';

        /// <summary>
        ///     The Face char (const). Value: 'f'.
        /// </summary>
        internal const char Face = 'f';

        /// <summary>
        ///     The Comment char (const). Value:   '#'.
        /// </summary>
        internal const char Comment = '#';

        /// <summary>
        ///     The Splitter string (const). Value:  ",".
        /// </summary>
        internal const string Splitter = ",";

        /// <summary>
        ///     The string X (const). Value: "X: ".
        /// </summary>
        internal const string StrX = "X: ";

        /// <summary>
        ///     The string Y (const). Value: " Y: ".
        /// </summary>
        internal const string StrY = " Y: ";

        /// <summary>
        ///     The string Z (const). Value: " Z: ".
        /// </summary>
        internal const string StrZ = " Z: ";

        /// <summary>
        ///     The Error file was empty (const). Value: "Error reading CSV file: ".
        /// </summary>
        internal const string ErrorFileEmpty = "Error reading CSV file: ";

        /// <summary>
        ///     The Error data was empty (const). Value: "CSV data cannot be null.".
        /// </summary>
        internal const string ErrorDataEmpty = "CSV data cannot be null.";

        internal const string ErrorWritingToFile = "Error writing to file: ";

        /// <summary>
        ///     Throw Message, file was empty (const). Value: "File path cannot be null or empty.".
        /// </summary>
        internal const string ThrowFileEmpty = "File path cannot be null or empty.";

        /// <summary>
        ///     Encoding Parameter (const). Value: "UTF-32BE".
        /// </summary>
        internal const string EncodingUtf32 = "UTF-32BE";
    }
}
