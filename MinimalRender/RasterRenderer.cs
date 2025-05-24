using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace MinimalRender
{
    public class RasterRenderer : IDisposable
    {
        private int _textureId;
        private int _vao, _vbo;
        private int _shaderProgram;
        private int _screenWidth, _screenHeight;

        public void Initialize(int screenWidth, int screenHeight)
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;

            GL.Viewport(0, 0, screenWidth, screenHeight);
            GL.ClearColor(Color4.Black);

            _shaderProgram = CreateShaderProgram();

            _textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, screenWidth, screenHeight, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            float[] vertices = {
                // positions   // texcoords
                -1f, -1f,  0f, 0f,
                 1f, -1f,  1f, 0f,
                 1f,  1f,  1f, 1f,
                -1f, -1f,  0f, 0f,
                 1f,  1f,  1f, 1f,
                -1f,  1f,  0f, 1f
            };

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.UseProgram(_shaderProgram);
            int texLocation = GL.GetUniformLocation(_shaderProgram, "tex");
            GL.Uniform1(texLocation, 0);
        }

        public void UpdateTexture(byte[] data, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, data);
        }

        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vao);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _textureId);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        private int CreateShaderProgram()
        {
            string vertexShaderSource = @"
                #version 330 core
                layout(location = 0) in vec2 aPos;
                layout(location = 1) in vec2 aTex;
                out vec2 TexCoord;
                void main()
                {
                    TexCoord = aTex;
                    gl_Position = vec4(aPos, 0.0, 1.0);
                }";

            string fragmentShaderSource = @"
                #version 330 core
                in vec2 TexCoord;
                out vec4 FragColor;
                uniform sampler2D tex;
                void main()
                {
                    FragColor = texture(tex, TexCoord);
                }";

            int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
            int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var status);

            if (status == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                throw new Exception($"Shader link failed: {info}");
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return program;
        }

        private int CompileShader(ShaderType type, string source)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var status);

            if (status == 0)
            {
                string info = GL.GetShaderInfoLog(shader);
                throw new Exception($"{type} compilation failed: {info}");
            }

            return shader;
        }

        public void Dispose()
        {
            GL.DeleteProgram(_shaderProgram);
            GL.DeleteTexture(_textureId);
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
        }
    }

}
