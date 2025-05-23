using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RenderEngine
{
    public class RasterRenderer
    {
        private int _textureId;
        private int _vao;
        private int _vbo;
        private Shader _shader;

        public void Initialize(int width, int height)
        {
            // Step 3: Create texture
            _textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                          width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            // Step 4: Set texture parameters (min/mag filtering)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Step 5: Setup a fullscreen quad
            float[] quadVertices = {
                // positions   // tex coords
                -1f, -1f, 0f, 0f,
                 1f, -1f, 1f, 0f,
                 1f,  1f, 1f, 1f,
                -1f,  1f, 0f, 1f
            };

            uint[] indices = {
                0, 1, 2,
                2, 3, 0
            };

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            int ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Position attribute
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // TexCoord attribute
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // Shader
            _shader = new Shader(VertexShaderSource, FragmentShaderSource);
        }

        public void UpdateTexture(byte[] pixelData, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height,
                             PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);
        }

        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();
            GL.BindVertexArray(_vao);
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        // Minimal shaders
        private static readonly string VertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec2 aPos;
            layout(location = 1) in vec2 aTex;
            out vec2 vTex;
            void main()
            {
                gl_Position = vec4(aPos, 0.0, 1.0);
                vTex = aTex;
            }
        ";

        private static readonly string FragmentShaderSource = @"
            #version 330 core
            in vec2 vTex;
            out vec4 FragColor;
            uniform sampler2D uTexture;
            void main()
            {
                FragColor = texture(uTexture, vTex);
            }
        ";
    }
}
