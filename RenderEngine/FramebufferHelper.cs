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
        /// Converts raw OpenGL RGBA pixel data into an UnmanagedImageBuffer (BGRA).
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

            for (int i = 0; i < rgbaPixels.Length; i += 4)
            {
                byte r = rgbaPixels[i + 0];
                byte g = rgbaPixels[i + 1];
                byte b = rgbaPixels[i + 2];
                byte a = rgbaPixels[i + 3];

                dst[i + 0] = b;
                dst[i + 1] = g;
                dst[i + 2] = r;
                dst[i + 3] = a;
            }

            return buffer;
        }
    }
}
