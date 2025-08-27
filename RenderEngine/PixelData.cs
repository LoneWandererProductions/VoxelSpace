/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/PixelData.cs
 * PURPOSE:     Custom Pixel Container for DirectBitmapImage
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace RenderEngine;

/// <summary>
///     Pixel Data Container
/// </summary>
public struct PixelData
{
    /// <summary>
    ///     Gets or sets the x.
    ///     X-coordinate
    /// </summary>
    /// <value>
    ///     The x.
    /// </value>
    public int X { get; set; }

    /// <summary>
    ///     Gets or sets the y.
    ///     Y-coordinate
    /// </summary>
    /// <value>
    ///     The y.
    /// </value>
    public int Y { get; set; }

    /// <summary>
    ///     Gets or sets the r.
    ///     Red component
    /// </summary>
    /// <value>
    ///     The r.
    /// </value>
    public byte R { get; set; }

    /// <summary>
    ///     Gets or sets the g.
    ///     Green component
    /// </summary>
    /// <value>
    ///     The g.
    /// </value>
    public byte G { get; set; }

    /// <summary>
    ///     Gets or sets the b.
    ///     Blue component
    /// </summary>
    /// <value>
    ///     The b.
    /// </value>
    public byte B { get; set; }

    /// <summary>
    ///     Gets or sets a.
    ///     Alpha component
    /// </summary>
    /// <value>
    ///     a.
    /// </value>
    public byte A { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PixelData" /> struct.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="r">The r.</param>
    /// <param name="g">The g.</param>
    /// <param name="b">The b.</param>
    /// <param name="a">a.</param>
    public PixelData(int x, int y, byte r, byte g, byte b, byte a = 255)
    {
        X = x;
        Y = y;
        R = r;
        G = g;
        B = b;
        A = a;
    }
}