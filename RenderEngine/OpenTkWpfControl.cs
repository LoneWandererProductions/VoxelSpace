using System;
using System.Windows;
using System.Windows.Controls;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;

namespace RenderEngine
{
    public class OpenTkWpfControl : UserControl, IDisposable
    {
        private readonly GLWpfControl _glControl;
        private int _backgroundTexture = -1;
        private int _shaderProgram;
        private int _vao, _vbo;

        public OpenTkWpfControl()
        {
            var settings = new GLWpfControlSettings { MajorVersion = 4, MinorVersion = 5 };
            _glControl = new GLWpfControl();
            _glControl.Start(settings);

            Content = _glControl;

            _glControl.Render += OnRender;
            _glControl.SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public void Dispose()
        {
            if (_vao != 0) GL.DeleteVertexArray(_vao);
            if (_vbo != 0) GL.DeleteBuffer(_vbo);
            if (_shaderProgram != 0) GL.DeleteProgram(_shaderProgram);
            if (_backgroundTexture != 0) GL.DeleteTexture(_backgroundTexture);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!OpenTkHelper.IsOpenGlCompatible())
                throw new NotSupportedException(RenderResource.ErrorOpenGl);

            InitializeShaders();
            InitializeBuffers();

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);

            _backgroundTexture = OpenTkHelper.LoadTexture("path_to_your_background_image.jpg");

            // Set uniform for texture unit
            GL.UseProgram(_shaderProgram);
            var uniformLocation = GL.GetUniformLocation(_shaderProgram, "uTexture");
            GL.Uniform1(uniformLocation, 0); // Texture unit 0
        }

        private void InitializeShaders()
        {
            _shaderProgram = GL.CreateProgram();

            var vertexShader = OpenTkHelper.CompileShader(ShaderType.VertexShader, ShaderResource.VertexShaderSource);
            var fragmentShader =
                OpenTkHelper.CompileShader(ShaderType.FragmentShader, ShaderResource.FragmentShaderSource);

            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, fragmentShader);

            // Optional: bind attributes before linking
            GL.BindAttribLocation(_shaderProgram, 0, "aPosition");
            GL.BindAttribLocation(_shaderProgram, 1, "aTexCoord");

            GL.LinkProgram(_shaderProgram);

            GL.GetProgram(_shaderProgram, GetProgramParameterName.LinkStatus, out var status);
            if (status == 0)
            {
                var infoLog = GL.GetProgramInfoLog(_shaderProgram);
                throw new Exception("Shader program linking failed: " + infoLog);
            }

            GL.DetachShader(_shaderProgram, vertexShader);
            GL.DetachShader(_shaderProgram, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        private void InitializeBuffers()
        {
            float[] vertices =
            {
                // Position     // TexCoords
                -1.0f, -1.0f, 0.0f, 0.0f,
                1.0f, -1.0f, 1.0f, 0.0f,
                1.0f, 1.0f, 1.0f, 1.0f,

                1.0f, 1.0f, 1.0f, 1.0f,
                -1.0f, 1.0f, 0.0f, 1.0f,
                -1.0f, -1.0f, 0.0f, 0.0f
            };

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices,
                BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0); // Unbind for safety
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GL.Viewport(0, 0, (int)_glControl.ActualWidth, (int)_glControl.ActualHeight);
        }

        private void OnRender(TimeSpan delta)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_backgroundTexture != -1) RenderBackground(_backgroundTexture);

            // Add additional rendering here
        }

        private void RenderBackground(int textureId)
        {
            GL.UseProgram(_shaderProgram);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
    }
}