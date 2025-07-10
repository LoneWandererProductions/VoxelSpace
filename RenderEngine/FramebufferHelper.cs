/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        FramebufferHelper.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;

namespace RenderEngine
{
    public static class FramebufferHelper
    {
        /// <summary>
        ///     Converts raw OpenGL RGBA pixel data into an UnmanagedImageBuffer (BGRA).
        /// </summary>
        /// <param name="rgbaPixels">OpenGL pixel data in RGBA format.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <returns>UnmanagedImageBuffer with BGRA-formatted pixels.</returns>
        public static UnmanagedImageBuffer ConvertToUnmanagedBuffer(byte[] rgbaPixels, int width, int height)
        {
            if (rgbaPixels == null)
                throw new ArgumentNullException(nameof(rgbaPixels));

            if (rgbaPixels.Length != width * height * 4)
                throw new ArgumentException("Pixel buffer size does not match image dimensions.", nameof(rgbaPixels));

            var buffer = new UnmanagedImageBuffer(width, height);

            var dst = buffer.BufferSpan;

            for (var i = 0; i < rgbaPixels.Length; i += 4)
            {
                var r = rgbaPixels[i + 0];
                var g = rgbaPixels[i + 1];
                var b = rgbaPixels[i + 2];
                var a = rgbaPixels[i + 3];

                dst[i + 0] = b;
                dst[i + 1] = g;
                dst[i + 2] = r;
                dst[i + 3] = a;
            }

            return buffer;
        }
    }
}