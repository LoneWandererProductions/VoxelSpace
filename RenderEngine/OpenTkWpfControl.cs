using System;
using System.Windows;
using System.Windows.Controls;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;

namespace RenderEngine
{
    public class OpenTkWpfControl : UserControl
    {
        private int _backgroundTexture = -1;
        private readonly GLWpfControl _glControl;
        private int _shaderProgram;
        private int _vao, _vbo;

        public OpenTkWpfControl()
        {
            // Kein InitializeComponent, wenn kein XAML
            // InitializeComponent();

            var settings = new GLWpfControlSettings { MajorVersion = 4, MinorVersion = 5 };

            _glControl = new GLWpfControl();
            _glControl.Start(settings);

            Content = _glControl;

            _glControl.Render += OnRender;

            // Kein Resize-Event - stattdessen WPF SizeChanged abonnieren
            _glControl.SizeChanged += OnSizeChanged;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!OpenTkHelper.IsOpenGlCompatible())
            {
                throw new NotSupportedException("OpenGL 4.5 or higher required.");
            }

            InitializeShaders();
            InitializeBuffers();

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);

            _backgroundTexture = OpenTkHelper.LoadTexture("path_to_your_background_image.jpg");
        }

        private void InitializeShaders()
        {
            _shaderProgram = GL.CreateProgram();

            var vertexShader =
                OpenTkHelper.CompileShader(ShaderType.VertexShader, ShaderResource.VertexShaderSource);
            var fragmentShader =
                OpenTkHelper.CompileShader(ShaderType.FragmentShader, ShaderResource.FragmentShaderSource);

            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, fragmentShader);
            GL.LinkProgram(_shaderProgram);

            GL.DetachShader(_shaderProgram, vertexShader);
            GL.DetachShader(_shaderProgram, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        private void InitializeBuffers()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GL.Viewport(0, 0, (int)_glControl.ActualWidth, (int)_glControl.ActualHeight);
        }

        private void OnRender(TimeSpan delta)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_backgroundTexture != -1)
            {
                RenderBackground(_backgroundTexture);
            }

            // Hier weitere Renderlogik einfügen
        }

        private void RenderBackground(int textureId)
        {
            GL.UseProgram(_shaderProgram);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
    }
}
