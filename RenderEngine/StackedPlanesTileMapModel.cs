/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        StackedPlanesTileMapModel.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using Contracts;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Media3D;
using Viewer;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace RenderEngine
{
    public class StackedPlanesTileMapModel
    {
        private readonly float _cellSize;
        private readonly int _gridSizeX;
        private readonly int _gridSizeY;
        private readonly int _heightLevels;

        private int _textureArrayId = -1;
        private int _vao, _vbo, _ebo;

        // Store tile indices for each plane, to select which texture layer to use
        private readonly int[] _tileIndices;

        public StackedPlanesTileMapModel(int gridSizeX, int gridSizeY, int heightLevels, float cellSize, List<string> tileTexturePaths, int[] tileIndices)
        {
            _gridSizeX = gridSizeX;
            _gridSizeY = gridSizeY;
            _heightLevels = heightLevels;
            _cellSize = cellSize;

            _tileIndices = tileIndices;

            LoadTextureArray(tileTexturePaths);
            GenerateModel();
        }

        private void LoadTextureArray(List<string> texturePaths)
        {
            if (texturePaths.Count == 0)
                throw new System.Exception("No textures provided for texture array.");

            _textureArrayId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, _textureArrayId);

            // Load the first image to get width/height
            using var firstImage = new Bitmap(texturePaths[0]);
            int width = firstImage.Width;
            int height = firstImage.Height;

            // Allocate storage for the texture array
            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, SizedInternalFormat.Rgba8, width, height, texturePaths.Count);

            // Upload each texture as a layer
            for (int i = 0; i < texturePaths.Count; i++)
            {
                using var image = new Bitmap(texturePaths[i]);
                if (image.Width != width || image.Height != height)
                    throw new System.Exception("All tile textures must have the same dimensions.");

                var data = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, width, height, 1,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                image.UnlockBits(data);
            }

            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        private void GenerateModel()
        {
            // Each plane has 4 vertices, each vertex has position(3), uv(2), tileIndex(1) = 6 floats per vertex
            int planesCount = _gridSizeX * _gridSizeY * _heightLevels;
            float[] vertices = new float[planesCount * 4 * 6];
            uint[] indices = new uint[planesCount * 6];

            int vIndex = 0;
            int iIndex = 0;
            int vertexOffset = 0;

            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    for (int z = 0; z < _heightLevels; z++)
                    {
                        float xPos = x * _cellSize;
                        float yPos = y * _cellSize;
                        float zPos = z * _cellSize;

                        int planeIndex = x + y * _gridSizeX + z * _gridSizeX * _gridSizeY;
                        int tileIndex = _tileIndices.Length > planeIndex ? _tileIndices[planeIndex] : 0;

                        // 4 vertices per plane
                        Vector3[] planeVertices =
                        {
                            new Vector3(xPos, yPos, zPos),
                            new Vector3(xPos + _cellSize, yPos, zPos),
                            new Vector3(xPos + _cellSize, yPos + _cellSize, zPos),
                            new Vector3(xPos, yPos + _cellSize, zPos)
                        };

                        float[] uv = { 0, 0, 1, 0, 1, 1, 0, 1 };

                        for (int v = 0; v < 4; v++)
                        {
                            vertices[vIndex++] = planeVertices[v].X;
                            vertices[vIndex++] = planeVertices[v].Y;
                            vertices[vIndex++] = planeVertices[v].Z;
                            vertices[vIndex++] = uv[v * 2];
                            vertices[vIndex++] = uv[v * 2 + 1];
                            vertices[vIndex++] = tileIndex; // Texture array layer
                        }

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

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            int stride = 6 * sizeof(float);
            // Position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            // UV
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // TileIndex (as float)
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, stride, 5 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public static int RenderSkyboxModelSingleTile(float aspectRatio, int width = 1024, int height = 1024)
        {
            using var framebuffer = new FramebufferRenderer(width, height);

            framebuffer.RenderToFramebuffer(() =>
            {
                int shaderProgram = OpenTkHelper.LoadShader(
                    ShaderResource.TextureMappingVertexShader,
                    ShaderResource.TextureMappingFragmentShader
                );

                int viewLoc = GL.GetUniformLocation(shaderProgram, "view");
                int projLoc = GL.GetUniformLocation(shaderProgram, "projection");
                int texLoc = GL.GetUniformLocation(shaderProgram, "uTexture");

                GL.UseProgram(shaderProgram);
                GL.Uniform1(texLoc, 0); // Bind texture to unit 0

                var camera = new RvCamera(0, 0, 0);
                var cameraMatrix = new PerspectiveMatrizes(camera);

                Matrix4 view = cameraMatrix.GetViewMatrix();
                Matrix4 proj = cameraMatrix.GetProjectionMatrix(aspectRatio);

                var skybox = new StackedPlanesModel(5, 5, 3, 64, "Textures/grass.png", camera);
                skybox.Render(view, proj, shaderProgram, viewLoc, projLoc);
            });

            return framebuffer.Texture;
        }

        public static int RenderSkyboxModelMultiTile(float aspectRatio, int width = 1024, int height = 1024)
        {
            using var framebuffer = new FramebufferRenderer(width, height);

            framebuffer.RenderToFramebuffer(() =>
            {
                var tileTextures = new List<string>
                {
                    "Textures/floor1.png",
                    "Textures/floor2.png",
                    "Textures/wall1.png",
                    "Textures/ceiling1.png"
                };

                int shaderProgram = OpenTkHelper.LoadShader(
                    ShaderResource.TextureArrayTilemapVertexShader,
                    ShaderResource.TextureArrayTilemapFragmentShader
                );

                int viewLoc = GL.GetUniformLocation(shaderProgram, "view");
                int projLoc = GL.GetUniformLocation(shaderProgram, "projection");
                int texLoc = GL.GetUniformLocation(shaderProgram, "uTexture");

                GL.UseProgram(shaderProgram);
                GL.Uniform1(texLoc, 0); // texture array at unit 0

                var camera = new RvCamera(0, 0, 0);
                var cameraMatrix = new PerspectiveMatrizes(camera);
                Matrix4 view = cameraMatrix.GetViewMatrix();
                Matrix4 proj = cameraMatrix.GetProjectionMatrix(aspectRatio);

                const int gridX = 5, gridY = 5, heightLevels = 3;
                int[] tileIndices = new int[gridX * gridY * heightLevels];

                for (int z = 0; z < heightLevels; z++)
                {
                    for (int y = 0; y < gridY; y++)
                    {
                        for (int x = 0; x < gridX; x++)
                        {
                            int idx = x + y * gridX + z * gridX * gridY;
                            tileIndices[idx] = z switch
                            {
                                0 => (x + y) % 2 == 0 ? 0 : 1, // checker floor
                                1 => 2,                         // wall
                                _ => 3                          // ceiling
                            };
                        }
                    }
                }

                var tileMapModel = new StackedPlanesTileMapModel(gridX, gridY, heightLevels, 64f, tileTextures, tileIndices);
                tileMapModel.Render(view, proj, shaderProgram, viewLoc, projLoc);
            });

            return framebuffer.Texture;
        }

        public void Render(Matrix4 view, Matrix4 projection, int shaderProgram, int viewLoc, int projLoc)
        {
            GL.UseProgram(shaderProgram);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, _textureArrayId);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _gridSizeX * _gridSizeY * _heightLevels * 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}
