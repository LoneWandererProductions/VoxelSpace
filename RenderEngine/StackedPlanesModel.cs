/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        StackedPlanesModel.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace RenderEngine
{
    public class StackedPlanesModel
    {
        private readonly float _cellSize;
        private readonly int _gridSizeX;
        private readonly int _gridSizeY;
        private readonly int _heightLevels;
        private int _texture;
        private int _vao, _vbo, _ebo;

        private readonly Vector3 _worldPosition;

        public StackedPlanesModel(int gridSizeX, int gridSizeY, int heightLevels, float cellSize, string texturePath, Vector3 worldPosition)
        {
            _gridSizeX = gridSizeX;
            _gridSizeY = gridSizeY;
            _heightLevels = heightLevels;
            _cellSize = cellSize;
            _worldPosition = worldPosition;
            LoadTexture(texturePath);
            GenerateModel();
        }

        // Fix: Add a constructor that takes 5 arguments to resolve CS1729
        public StackedPlanesModel(int gridSizeX, int gridSizeY, int heightLevels, float cellSize, string texturePath)
            : this(gridSizeX, gridSizeY, heightLevels, cellSize, texturePath, Vector3.Zero)
        {
        }

        public StackedPlanesModel(int gridSizeX, int gridSizeY, int heightLevels, float cellSize, string texturePath, RvCamera camera)
            : this(gridSizeX, gridSizeY, heightLevels, cellSize, texturePath)
        {
            _worldPosition = new Vector3(camera.X * cellSize, camera.Y * cellSize, camera.Z * cellSize);
        }

        private void GenerateModel()
        {
            var numPlanes = _gridSizeX * _gridSizeY * _heightLevels;
            var vertices = new float[numPlanes * 4 * 5]; // 4 vertices per plane, 5 attributes (x,y,z,u,v)
            var indices = new uint[numPlanes * 6]; // 6 indices per plane (2 triangles)

            int vIndex = 0, iIndex = 0, vertexOffset = 0;

            for (var x = 0; x < _gridSizeX; x++)
            {
                for (var y = 0; y < _gridSizeY; y++)
                {
                    for (var z = 0; z < _heightLevels; z++)
                    {
                        var xPos = _worldPosition.X + (x * _cellSize);
                        var yPos = _worldPosition.Y + (y * _cellSize);
                        var zPos = _worldPosition.Z + (z * _cellSize);

                        // Define 4 vertices for a single 2D plane
                        Vector3[] planeVertices =
                        {
                                new(xPos, yPos, zPos), // Bottom-left
                                new(xPos + _cellSize, yPos, zPos), // Bottom-right
                                new(xPos + _cellSize, yPos + _cellSize, zPos), // Top-right
                                new(xPos, yPos + _cellSize, zPos) // Top-left
                            };

                        // UV Coordinates (entire texture mapped)
                        float[] uv = { 0, 0, 1, 0, 1, 1, 0, 1 };

                        for (var v = 0; v < 4; v++)
                        {
                            vertices[vIndex++] = planeVertices[v].X;
                            vertices[vIndex++] = planeVertices[v].Y;
                            vertices[vIndex++] = planeVertices[v].Z;
                            vertices[vIndex++] = uv[v * 2];
                            vertices[vIndex++] = uv[(v * 2) + 1];
                        }

                        // Indices for two triangles per plane
                        indices[iIndex++] = (uint)vertexOffset;
                        indices[iIndex++] = (uint)(vertexOffset + 1);
                        indices[iIndex++] = (uint)(vertexOffset + 2);
                        indices[iIndex++] = (uint)(vertexOffset + 2);
                        indices[iIndex++] = (uint)(vertexOffset + 3);
                        indices[iIndex++] = (uint)vertexOffset;

                        vertexOffset += 4;
                    }
                }
            }

            // Generate OpenGL buffers
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices,
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices,
                BufferUsageHint.StaticDraw);

            // Position Attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Texture Coordinate Attribute
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        private void LoadTexture(string path)
        {
            _texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _texture);

            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            // Load image
            using (var image = new Bitmap(path))
            {
                var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                image.UnlockBits(data);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Render()
        {
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _gridSizeX * _gridSizeY * _heightLevels * 6,
                DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Renders the specified view.
        /// Compute camera matrices
        /// var view = cameraMatrices.GetViewMatrix();
        /// var proj = cameraMatrices.GetProjectionMatrix(aspectRatio);
        /// Render
        /// skybox.Render(view, proj, shaderProgram, viewLocation, projLocation);
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="projection">The projection.</param>
        /// <param name="shaderProgram">The shader program.</param>
        /// <param name="viewLoc">The view loc.</param>
        /// <param name="projLoc">The proj loc.</param>
        public void Render(Matrix4 view, Matrix4 projection, int shaderProgram, int viewLoc, int projLoc)
        {
            GL.UseProgram(shaderProgram);

            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _texture);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _gridSizeX * _gridSizeY * _heightLevels * 6,
                DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}