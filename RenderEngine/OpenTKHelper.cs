/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/TKHelper.cs
 * PURPOSE:     Basic Helper stuff for our engine
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Diagnostics;
using System.IO;
using Imaging; // For Marshal.Copy and memory management
using OpenTK.Graphics.OpenGL4; // For GL methods, ShaderType, TextureTarget, etc.
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace RenderEngine
{
    internal static class OpenTKHelper
    {
        internal static int CompileShader(ShaderType type, string source)
        {
            var shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var status);
            if (status == (int)All.True)
            {
                return shader;
            }

            var infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error compiling shader of type {type}: {infoLog}");
        }

        internal static float[] GenerateVertexData<T>(T[] data, int screenWidth, int screenHeight, Func<T, float[]> getVertexAttributes)
        {
            var vertexData = new float[data.Length * 6 * 5]; // 6 vertices * 5 attributes (x, y, r, g, b)

            for (var i = 0; i < data.Length; i++)
            {
                var attributes = getVertexAttributes(data[i]);
                var xLeft = (i / (float)screenWidth * 2.0f) - 1.0f;
                var xRight = ((i + 1) / (float)screenWidth * 2.0f) - 1.0f;

                var columnHeight = attributes[0]; // In case of columns, this would be height, for example
                var yTop = (columnHeight / screenHeight) * 2.0f - 1.0f;
                var yBottom = -1.0f; // Bottom of the screen is -1.0f

                var offset = i * 30; // 6 vertices * 5 attributes (x, y, r, g, b)

                // Set up vertices for the column or pixel
                vertexData[offset + 0] = xLeft;
                vertexData[offset + 1] = yTop;
                vertexData[offset + 2] = attributes[1]; // r
                vertexData[offset + 3] = attributes[2]; // g
                vertexData[offset + 4] = attributes[3]; // b

                vertexData[offset + 5] = xRight;
                vertexData[offset + 6] = yTop;
                vertexData[offset + 7] = attributes[1];
                vertexData[offset + 8] = attributes[2];
                vertexData[offset + 9] = attributes[3];

                vertexData[offset + 10] = xRight;
                vertexData[offset + 11] = yBottom;
                vertexData[offset + 12] = attributes[1];
                vertexData[offset + 13] = attributes[2];
                vertexData[offset + 14] = attributes[3];

                vertexData[offset + 15] = xLeft;
                vertexData[offset + 16] = yTop;
                vertexData[offset + 17] = attributes[1];
                vertexData[offset + 18] = attributes[2];
                vertexData[offset + 19] = attributes[3];

                vertexData[offset + 20] = xRight;
                vertexData[offset + 21] = yBottom;
                vertexData[offset + 22] = attributes[1];
                vertexData[offset + 23] = attributes[2];
                vertexData[offset + 24] = attributes[3];

                vertexData[offset + 25] = xLeft;
                vertexData[offset + 26] = yBottom;
                vertexData[offset + 27] = attributes[1];
                vertexData[offset + 28] = attributes[2];
                vertexData[offset + 29] = attributes[3];
            }

            return vertexData;
        }

        public static byte[] LoadTexture(string filePath, out int width, out int height)
        {
            using var directBitmap = new DirectBitmap(filePath);

            width = directBitmap.Width;
            height = directBitmap.Height;

            return directBitmap.Bytes(); // Assuming `Bits` is byte[] in the updated DirectBitmap class
        }

        public static int LoadTexture(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                Trace.WriteLine($"File not found: {filePath}");
                return -1;
            }

            var textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            using (var directBitmap = new DirectBitmap(filePath))
            {
                var pixels = directBitmap.Bytes(); // Assuming `Bits` is a byte array in the updated DirectBitmap class
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, directBitmap.Width, directBitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return textureId;
        }
    }
}
