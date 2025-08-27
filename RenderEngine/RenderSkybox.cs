/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderSkybox.cs
 * PURPOSE:     Entry for the Skybox rendering functionality.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using Contracts;
using OpenTK.Graphics.OpenGL4;

namespace RenderEngine;

public static class RenderSkybox
{
    private const int FramebufferSize = 512;

    /// <summary>
    ///     Renders the skybox model single tile.
    /// </summary>
    /// <param name="aspectRatio">The aspect ratio.</param>
    /// <param name="camera">The camera.</param>
    /// <returns>Custom image Container with rendered Image</returns>
    public static UnmanagedImageBuffer RenderSkyboxModelSingleTile(float aspectRatio, ICamera? camera = null)
    {
        using var fbo = new FramebufferRenderer(FramebufferSize, FramebufferSize);
        fbo.Bind();

        var shaderProgram = OpenTkHelper.LoadShader(
            ShaderResource.TextureMappingVertexShader,
            ShaderResource.TextureMappingFragmentShader
        );

        var viewLoc = GL.GetUniformLocation(shaderProgram, "view");
        var projLoc = GL.GetUniformLocation(shaderProgram, "projection");
        var texLoc = GL.GetUniformLocation(shaderProgram, "uTexture");

        GL.UseProgram(shaderProgram);
        GL.Uniform1(texLoc, 0);

        var cameraMatrix = new PerspectiveMatrizes(camera);

        var view = cameraMatrix.GetViewMatrix();
        var proj = cameraMatrix.GetProjectionMatrix(aspectRatio);

        var skybox = new StackedPlanesModel(5, 5, 3, 64, "Textures/grass.png", camera);
        skybox.Render(view, proj, shaderProgram, viewLoc, projLoc);

        fbo.Unbind();

        var rgbaPixels = fbo.ReadPixels();
        return FramebufferHelper.ConvertToUnmanagedBuffer(rgbaPixels, FramebufferSize, FramebufferSize);
    }

    /// <summary>
    ///     Renders the skybox model multi tile.
    /// </summary>
    /// <param name="aspectRatio">The aspect ratio.</param>
    /// <param name="camera">The camera.</param>
    /// <returns>Custom image Container with rendered Image</returns>
    public static UnmanagedImageBuffer RenderSkyboxModelMultiTile(float aspectRatio, ICamera? camera = null)
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

        var shaderProgram = OpenTkHelper.LoadShader(
            ShaderResource.TextureArrayTilemapVertexShader,
            ShaderResource.TextureArrayTilemapFragmentShader
        );

        var viewLoc = GL.GetUniformLocation(shaderProgram, "view");
        var projLoc = GL.GetUniformLocation(shaderProgram, "projection");
        var texLoc = GL.GetUniformLocation(shaderProgram, "uTexture");

        GL.UseProgram(shaderProgram);
        GL.Uniform1(texLoc, 0);

        var cameraMatrix = new PerspectiveMatrizes(camera);
        var view = cameraMatrix.GetViewMatrix();
        var proj = cameraMatrix.GetProjectionMatrix(aspectRatio);

        const int gridX = 5, gridY = 5, heightLevels = 3;
        var tileIndices = new int[gridX * gridY * heightLevels];

        for (var z = 0; z < heightLevels; z++)
        for (var y = 0; y < gridY; y++)
        for (var x = 0; x < gridX; x++)
        {
            var idx = x + y * gridX + z * gridX * gridY;
            tileIndices[idx] = z switch
            {
                0 => (x + y) % 2, // checker floor
                1 => 2, // walls
                _ => 3 // ceiling
            };
        }

        var tileMapModel =
            new StackedPlanesTileMapModel(gridX, gridY, heightLevels, 64f, tileTextures, tileIndices);
        tileMapModel.Render(view, proj, shaderProgram, viewLoc, projLoc);

        fbo.Unbind();

        var rgbaPixels = fbo.ReadPixels();
        return FramebufferHelper.ConvertToUnmanagedBuffer(rgbaPixels, FramebufferSize, FramebufferSize);
    }
}