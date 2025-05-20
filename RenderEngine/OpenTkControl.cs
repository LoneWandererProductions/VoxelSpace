/*
 * COPYRIGHT:   See COPYING in the top-level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/OpenTkControl.cs
 * PURPOSE:     OpenGL Viewer for WPF applications.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

#nullable enable
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace RenderEngine
{
#nullable enable
    namespace RenderEngine
    {
        /// <inheritdoc />
        /// <summary>
        ///     Display Control for OpenGL
        /// </summary>
        /// <seealso cref="T:System.Windows.Forms.Integration.WindowsFormsHost" />
        public sealed class OpenTkControl : WindowsFormsHost
        {
            private int _backgroundTexture;
            private bool _enableSkybox;
            private GLControl? _glControl;
            private int _shaderProgram;
            private int _skyboxShader;
            private int _skyboxTexture;
            private int _skyboxVAO, _skyboxVBO;
            private int _vao, _vbo;

            public OpenTkControl()
            {
                if (!OpenTkHelper.IsOpenGlCompatible())
                {
                    throw new NotSupportedException("OpenGL 4.5 or higher is required but not available.");
                }

                InitializeGlControl();
                InitializeShaders();
                InitializeBuffers();
                Child = _glControl; // Set GLControl as the hosted child
            }

            public bool EnableSkybox
            {
                get => _enableSkybox;
                set
                {
                    _enableSkybox = value;
                    _glControl?.Invalidate(); // Redraw when toggled
                }
            }

            private void InitializeGlControl()
            {
                _glControl = new GLControl();

                _glControl.HandleCreated += (s, e) =>
                {
                    _glControl.MakeCurrent(); // OpenGL context setup
                    GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f); // Set background color
                };

                GL.Enable(EnableCap.DepthTest);

                _glControl.Paint += GlControl_Paint;
                _glControl.Resize += GlControl_Resize;
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

                // Load background texture
                _backgroundTexture = OpenTkHelper.LoadTexture("path_to_your_background_image.jpg");
            }

            private void InitializeBuffers()
            {
                _vao = GL.GenVertexArray();
                _vbo = GL.GenBuffer();

                GL.BindVertexArray(_vao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                    2 * sizeof(float));
                GL.EnableVertexAttribArray(1);
            }

            private void InitializeSkybox()
            {
                _skyboxShader = OpenTkHelper.LoadShader("skybox_vertex.glsl", "skybox_fragment.glsl");

                _skyboxTexture = OpenTkHelper.LoadCubeMap(new[]
                {
                    "right.jpg", "left.jpg", "top.jpg", "bottom.jpg", "front.jpg", "back.jpg"
                });

                _skyboxVAO = GL.GenVertexArray();
                _skyboxVBO = GL.GenBuffer();

                GL.BindVertexArray(_skyboxVAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _skyboxVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, ShaderResource.SkyboxVertices.Length * sizeof(float),
                    ShaderResource.SkyboxVertices, BufferUsageHint.StaticDraw);
            }

            private void RenderSkybox()
            {
                GL.DepthFunc(DepthFunction.Lequal);

                GL.UseProgram(_skyboxShader);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.TextureCubeMap, _skyboxTexture);

                GL.BindVertexArray(_skyboxVAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

                GL.DepthFunc(DepthFunction.Less);
            }

            private void GlControl_Paint(object? sender, PaintEventArgs e)
            {
                if (_glControl == null)
                {
                    return;
                }

                if (_glControl.Context is { IsCurrent: false })
                {
                    _glControl.MakeCurrent();
                }

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                if (_backgroundTexture != -1)
                {
                    RenderBackground(_backgroundTexture);
                }

                if (_skyboxTexture != -1)
                {
                    RenderSkybox();
                }

                _glControl.SwapBuffers();
            }

            private void GlControl_Resize(object? sender, EventArgs e)
            {
                if (_glControl == null)
                {
                    return;
                }

                if (!_glControl.Context.IsCurrent)
                {
                    _glControl.MakeCurrent();
                }

                GL.Viewport(0, 0, _glControl.Width, _glControl.Height);
            }

            private void RenderBackground(int textureId)
            {
                GL.UseProgram(_shaderProgram);

                // Bind the background texture
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textureId);

                // Draw the background as a full-screen quad
                GL.BindVertexArray(_vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6); // Full-screen quad (6 vertices)
            }

            public void RenderColumns(ColumnData[] columns, int screenWidth, int screenHeight)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                RenderBackground(_backgroundTexture);

                var vertexData = OpenTkHelper.GenerateVertexData(columns, screenWidth, screenHeight, column =>
                    new[] { column.Height / screenHeight, column.Color.X, column.Color.Y, column.Color.Z });

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData,
                    BufferUsageHint.DynamicDraw);

                GL.UseProgram(_shaderProgram);
                GL.BindVertexArray(_vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, columns.Length * 6);
            }

            public void RenderPixels(PixelData[] pixels, int screenWidth, int screenHeight)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                RenderBackground(_backgroundTexture);

                var vertexData = OpenTkHelper.GenerateVertexData(pixels, screenWidth, screenHeight, pixel =>
                {
                    var pixelWidth = 2.0f / screenWidth;
                    var pixelHeight = 2.0f / screenHeight;

                    return new[]
                    {
                        -1 + (pixel.X * pixelWidth), -1 + (pixel.Y * pixelHeight), 0.0f,
                        -1 + ((pixel.X + 1) * pixelWidth), -1 + (pixel.Y * pixelHeight), 0.0f,
                        -1 + (pixel.X * pixelWidth), -1 + ((pixel.Y + 1) * pixelHeight), 0.0f, pixel.Color.X,
                        pixel.Color.Y, pixel.Color.Z
                    };
                });

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData,
                    BufferUsageHint.DynamicDraw);

                GL.UseProgram(_shaderProgram);
                GL.BindVertexArray(_vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, pixels.Length * 6);
            }

            public void CaptureScreenshot(string filePath)
            {
                if (_glControl == null)
                {
                    return;
                }

                var width = _glControl.Width;
                var height = _glControl.Height;

                var pixels = new byte[width * height * 4];
                GL.ReadPixels(0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

                using var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var rect = new Rectangle(0, 0, width, height);
                var bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);

                bitmap.UnlockBits(bitmapData);
                bitmap.Save(filePath, ImageFormat.Png);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    GL.DeleteProgram(_shaderProgram);
                    GL.DeleteVertexArray(_vao);
                    GL.DeleteBuffer(_vbo);
                    _glControl?.Dispose();
                    //TODO add more cleaning here
                }

                base.Dispose(disposing);
            }
        }
    }
}
