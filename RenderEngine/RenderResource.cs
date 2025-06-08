namespace RenderEngine
{
    /// <summary>
    /// 
    /// </summary>
    internal static class RenderResource
    {
        internal static readonly string ErrorInputBuffer = "Input buffer size does not match.";

        internal static readonly string ErrorLayerSize = "Layer size does not match container size.";

        internal static readonly string ErrorLayerSizeMismatch = "Layer size mismatch.";

        internal static readonly string ErrorInvalidLayerIndex = "Invalid layer index {0}";

        internal static readonly string ErrorNoLayers = "No layers to composite.";


        internal static readonly string Resource6 = "OpenGL 4.5 or higher is required but not available.";

        internal static readonly string ShaderSkyboxVertex = "skybox_vertex.glsl";

        internal static readonly string ShaderSkyboxFragment = "skybox_fragment.glsl";

        internal static readonly string Resource10 = "right.jpg";
        internal static readonly string Resource11 = "left.jpg";
        internal static readonly string Resource12 = "top.jpg";
        internal static readonly string Resource13 = "bottom.jpg";
        internal static readonly string Resource14 = "front.jpg";
        internal static readonly string Resource15 = "back.jpg";
        internal static readonly string Resource16 = "Cube map must have exactly 6 textures.";
        internal static readonly string Resource17 = "OpenGL Renderer: {0}";
        internal static readonly string Resource18 = "OpenGL Vendor: {0}";
        internal static readonly string Resource19 = "OpenGL Version: {0}";
        internal static readonly string Resource20 = "OpenGL initialization failed: {0}";
        internal static readonly string Resource21 = "Error compiling shader of type {0}: {1}";
        internal static readonly string Resource22 = "Error linking shader program: {0}";
        internal static readonly string Resource23 = "Cube map texture not found: {0}";
        internal static readonly string Resource24 = "File not found: {0}";

        internal const string ErrorOpenGl = "OpenGL 4.5 or higher required.";
    }
}
