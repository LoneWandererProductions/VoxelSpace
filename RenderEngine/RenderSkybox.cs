/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderSkybox.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace RenderEngine
{
    public static class RenderSkybox
    {
        private const int FramebufferSize = 512;

        public static UnmanagedImageBuffer RenderSkyboxModelSingleTile(float aspectRatio)
        {
            using var fbo = new FramebufferRenderer(FramebufferSize, FramebufferSize);
            fbo.Bind();

            int shaderProgram = OpenTkHelper.LoadShader(
                ShaderResource.TextureMappingVertexShader,
                ShaderResource.TextureMappingFragmentShader
            );

            int viewLoc = GL.GetUniformLocation(shaderProgram, "view");
            int projLoc = GL.GetUniformLocation(shaderProgram, "projection");
            int texLoc = GL.GetUniformLocation(shaderProgram, "uTexture");

            GL.UseProgram(shaderProgram);
            GL.Uniform1(texLoc, 0);

            var camera = new RvCamera(0, 0, 0);
            var cameraMatrix = new PerspectiveMatrizes(camera);

            Matrix4 view = cameraMatrix.GetViewMatrix();
            Matrix4 proj = cameraMatrix.GetProjectionMatrix(aspectRatio);

            var skybox = new StackedPlanesModel(5, 5, 3, 64, "Textures/grass.png", camera);
            skybox.Render(view, proj, shaderProgram, viewLoc, projLoc);

            fbo.Unbind();

            byte[] rgbaPixels = fbo.ReadPixels();
            return FramebufferHelper.ConvertToUnmanagedBuffer(rgbaPixels, FramebufferSize, FramebufferSize);
        }

        public static UnmanagedImageBuffer RenderSkyboxModelMultiTile(float aspectRatio)
        {
            using var fbo = new FramebufferRenderer(FramebufferSize, FramebufferSize);
            fbo.Bind();

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
            GL.Uniform1(texLoc, 0);

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
                            0 => (x + y) % 2, // checker floor
                            1 => 2,          // walls
                            _ => 3           // ceiling
                        };
                    }
                }
            }

            var tileMapModel = new StackedPlanesTileMapModel(gridX, gridY, heightLevels, 64f, tileTextures, tileIndices);
            tileMapModel.Render(view, proj, shaderProgram, viewLoc, projLoc);

            fbo.Unbind();

            byte[] rgbaPixels = fbo.ReadPixels();
            return FramebufferHelper.ConvertToUnmanagedBuffer(rgbaPixels, FramebufferSize, FramebufferSize);
        }
    }
}
