/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        FramebufferRenderer.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using OpenTK.Graphics.OpenGL4;

namespace RenderEngine
{
    public class FramebufferRenderer : IDisposable
    {
        public FramebufferRenderer(int width, int height)
        {
            Width = width;
            Height = height;
            InitializeFramebuffer();
        }

        public int Framebuffer { get; private set; }
        public int Texture { get; private set; }
        public int DepthBuffer { get; private set; }

        public int Width { get; }

        public int Height { get; }

        public void Dispose()
        {
            if (Texture != 0) GL.DeleteTexture(Texture);
            if (DepthBuffer != 0) GL.DeleteRenderbuffer(DepthBuffer);
            if (Framebuffer != 0) GL.DeleteFramebuffer(Framebuffer);
        }

        private void InitializeFramebuffer()
        {
            Framebuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

            Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, Texture, 0);

            DepthBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24,
                Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, DepthBuffer);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Framebuffer is not complete: " + status);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void RenderToFramebuffer(Action renderAction)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            renderAction.Invoke();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);
            GL.Viewport(0, 0, Width, Height);
        }

        public byte[] ReadPixels()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

            var pixelData = new byte[Width * Height * 4]; // RGBA 4 bytes
            GL.ReadPixels(0, 0, Width, Height, PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);

            return pixelData;
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}