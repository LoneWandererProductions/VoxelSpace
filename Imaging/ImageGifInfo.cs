/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageGifInfo.cs
 * PURPOSE:     Class Container that holds all informations about the gif in question.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://stackoverflow.com/questions/18719302/net-creating-a-looping-gif-using-gifbitmapencoder
 *              https://debugandrelease.blogspot.com/2018/12/creating-gifs-in-c.html
 *              http://www.matthewflickinger.com/lab/whatsinagif/bits_and_bytes.asp
 */


using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Imaging
{
    /// <summary>
    ///     Gif Information
    /// </summary>
    public sealed class ImageGifInfo
    {
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has global color table.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has global color table; otherwise, <c>false</c>.
        /// </value>
        public bool HasGlobalColorTable { get; set; }

        /// <summary>
        /// Gets or sets the size of the global color table.
        /// </summary>
        /// <value>
        /// The size of the global color table.
        /// </value>
        public int GlobalColorTableSize { get; set; }

        /// <summary>
        /// Gets or sets the color resolution.
        /// </summary>
        /// <value>
        /// The color resolution.
        /// </value>
        public int ColorResolution { get; set; }

        /// <summary>
        /// Gets or sets the index of the background color.
        /// </summary>
        /// <value>
        /// The index of the background color.
        /// </value>
        public int BackgroundColorIndex { get; set; }

        /// <summary>
        /// Gets or sets the pixel aspect ratio.
        /// </summary>
        /// <value>
        /// The pixel aspect ratio.
        /// </value>
        public int PixelAspectRatio { get; set; }

        /// <summary>
        /// Gets or sets the loop count.
        /// </summary>
        /// <value>
        /// The loop count.
        /// </value>
        public int? LoopCount { get; set; } // Nullable, since not all GIFs have this

        /// <summary>
        /// Gets or sets the frames.
        /// </summary>
        /// <value>
        /// The frames.
        /// </value>
        public List<FrameInfo> Frames { get; set; } = new();

        /// <summary>
        ///     Gets the total duration.
        /// </summary>
        /// <value>
        ///     The total duration.
        /// </value>
        public double TotalDuration => Frames.Sum(f => f.DelayTime); // In seconds

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public long Size { get; set; }
    }

    /// <summary>
    /// Infos about the frame and timing
    /// </summary>
    public sealed class FrameInfo
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the delay time.
        /// </summary>
        /// <value>
        /// The delay time.
        /// </value>
        public double DelayTime { get; set; } // Delay time in seconds


        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public Bitmap Image { get; set; } // Image of the frame
    }
}