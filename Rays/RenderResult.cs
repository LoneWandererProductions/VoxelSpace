/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Rays
 * FILE:        RenderResult.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Drawing;

namespace Rays;

public sealed class RenderResult
{
    public Bitmap Bitmap { get; set; }
    public byte[] Bytes { get; set; }
}