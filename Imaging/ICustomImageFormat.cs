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
    internal interface ICustomImageFormat
    {
        Bitmap GetCifFile(string path);

        Cif GetCifFile(Bitmap image);

        void SaveToCifFile(Bitmap image, string path);

        void CompressedToCifFile(Bitmap image, string path);
    }
}
