using System;
using OpenTK.Graphics.OpenGL4; // Use OpenGL4 for desktop GL (not ES11)
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat; // Avoid conflict with System.Drawing

namespace RenderEngine
{
    public class RasterPipeline
    {
        public static void UpdateTexture(int textureId, byte[] data, int width, int height)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Texture data cannot be null or empty.", nameof(data));

            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.TexSubImage2D(
                TextureTarget.Texture2D,
                level: 0,
                xoffset: 0,
                yoffset: 0,
                width,
                height,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                data
            );
        }
    }
}