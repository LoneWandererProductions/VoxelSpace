/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/TkRender.cs
 * PURPOSE:     Main Render Control
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace RenderEngine
{
    public partial class TkRender
    {
        private GLControl _glControl;
        private int _shaderProgram;
        private int _vao;
        private int _vbo;

        // New texture variable for the background image
        private int _backgroundTexture;

        public TkRender()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var windowsFormsHost = new WindowsFormsHost();
            _glControl = new GLControl { Dock = DockStyle.Fill };
            windowsFormsHost.Child = _glControl;
            MainGrid.Children.Add(windowsFormsHost);

            _glControl.Load += GlControl_Load;
            _glControl.Paint += GlControl_Paint;
            _glControl.Resize += GlControl_Resize;

            // Load background texture
            _backgroundTexture = TkHelper.LoadTexture("path_to_your_background_image.jpg");

            _glControl.Invalidate();
        }

        public void Initialize()
        {
            const string vertexShaderSource = @"
                #version 450 core
                layout(location = 0) in vec2 aPosition;
                layout(location = 1) in vec3 aColor;
                out vec3 vColor;
                void main()
                {
                    gl_Position = vec4(aPosition, 0.0, 1.0);
                    vColor = aColor;
                }
            ";

            const string fragmentShaderSource = @"
                #version 450 core
                in vec3 vColor;
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(vColor, 1.0);
                }
            ";

            var vertexShader = TkHelper.CompileShader(ShaderType.VertexShader, vertexShaderSource);
            var fragmentShader = TkHelper.CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

            _shaderProgram = GL.CreateProgram();
            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, fragmentShader);
            GL.LinkProgram(_shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        private void RenderBackground(int textureId)
        {
            GL.UseProgram(_shaderProgram);

            // Bind the background texture
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            // Draw the background as a full-screen quad
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6); // Full screen quad (6 vertices)
        }

        public void RenderColumns(ColumnData[] columns, int screenWidth, int screenHeight)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Render the background first
            RenderBackground(_backgroundTexture);

            // Generate vertex data for columns
            var vertexData = TkHelper.GenerateVertexData(columns, screenWidth, screenHeight, column => new[] { column.Height / screenHeight, column.Color.X, column.Color.Y, column.Color.Z });

            // Upload column vertex data to GPU
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.DynamicDraw);

            // Render columns on top of background
            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, columns.Length * 6);
        }

        public void RenderPixels(PixelData[] pixels, int screenWidth, int screenHeight)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Render the background first
            RenderBackground(_backgroundTexture);

            // Generate vertex data for pixels
            var vertexData = TkHelper.GenerateVertexData(pixels, screenWidth, screenHeight, pixel =>
            {
                var pixelWidth = 2.0f / screenWidth;
                var pixelHeight = 2.0f / screenHeight;

                // Assuming you want to use both pixelWidth and pixelHeight for vertex position,
                // you can define the coordinates for each vertex of the pixel quad
                return new[]
                {
                    -1 + pixel.X * pixelWidth, -1 + pixel.Y * pixelHeight, 0.0f, // Bottom-left vertex
                    -1 + (pixel.X + 1) * pixelWidth, -1 + pixel.Y * pixelHeight, 0.0f, // Bottom-right vertex
                    -1 + pixel.X * pixelWidth, -1 + (pixel.Y + 1) * pixelHeight, 0.0f, // Top-left vertex

                    -1 + (pixel.X + 1) * pixelWidth, -1 + pixel.Y * pixelHeight, 0.0f, // Bottom-right vertex
                    -1 + (pixel.X + 1) * pixelWidth, -1 + (pixel.Y + 1) * pixelHeight, 0.0f, // Top-right vertex
                    -1 + pixel.X * pixelWidth, -1 + (pixel.Y + 1) * pixelHeight, 0.0f, // Top-left vertex

                    pixel.Color.X, pixel.Color.Y, pixel.Color.Z // Color for each pixel
                };
            });

            // Upload pixel vertex data to GPU
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.DynamicDraw);

            // Render pixels on top of the background
            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, pixels.Length * 6);
        }

        // Screenshot method
        public void CaptureScreenshot(string filePath)
        {
            // Step 1: Determine the dimensions of the OpenGL viewport or control
            int width = _glControl.Width;  // Replace _glControl with your actual OpenGL control or window
            int height = _glControl.Height;

            // Step 2: Allocate a byte array to store the pixel data (RGBA format)
            byte[] pixels = new byte[width * height * 4]; // 4 bytes per pixel (RGBA)

            // Step 3: Read the pixels from the OpenGL framebuffer
            GL.ReadPixels(0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            // Step 4: Create a Bitmap and copy the pixel data into it
            using (var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var rect = new Rectangle(0, 0, width, height);
                var bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                // Copy pixel data from the byte array to the Bitmap
                Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);

                bitmap.UnlockBits(bitmapData);

                // Step 5: Save the Bitmap as a file
                bitmap.Save(filePath, ImageFormat.Png);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_shaderProgram != 0) GL.DeleteProgram(_shaderProgram);
            if (_vao != 0) GL.DeleteVertexArray(_vao);
            if (_vbo != 0) GL.DeleteBuffer(_vbo);
            GL.DeleteTexture(_backgroundTexture); // Delete the background texture
            _glControl?.Dispose();
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _glControl.SwapBuffers();
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, _glControl.Width, _glControl.Height);
            // Initialize shaders and other OpenGL resources here
            Initialize();
        }
    }
}
