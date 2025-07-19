/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderResource.cs
 * PURPOSE:     String Resources for rendering operations.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace RenderEngine
{
    /// <summary>
    ///     Mostly static string constants.
    /// </summary>
    internal static class RenderResource
    {
        internal const string ErrorOpenGl = "OpenGL 4.5 or higher required.";

        internal const string ErrorInputBuffer = "Input buffer size does not match.";

        internal const string ErrorLayerSize = "Layer size does not match container size.";

        internal const string ErrorInvalidLayerIndex = "Invalid layer index {0}";

        internal const string ErrorNoLayers = "No layers to composite.";

        internal const string ShaderSkyboxVertex = "skybox_vertex.glsl";

        internal const string ShaderSkyboxFragment = "skybox_fragment.glsl";

        internal const string ErrorLayerSizeMismatch = "Layer size mismatch.";
    }
}
