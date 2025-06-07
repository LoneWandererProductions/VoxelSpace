// ReSharper disable UnusedType.Global

namespace RenderEngine
{
    /// <summary>
    ///     A static class containing different shader programs (vertex and fragment shaders) for various rendering effects.
    /// </summary>
    internal static class ShaderResource
    {
        /// <summary>
        ///     Skybox vertices.
        ///     A set of vertices representing a cube used to render a skybox, typically used in environment mapping.
        /// </summary>
        public static readonly float[] SkyboxVertices =
        {
            // Positions
            -1.0f, 1.0f, -1.0f, -1.0f, -1.0f, -1.0f, 1.0f, -1.0f, -1.0f, 1.0f, -1.0f, -1.0f, 1.0f, 1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f, -1.0f, 1.0f, -1.0f, -1.0f, -1.0f, -1.0f, 1.0f, -1.0f, -1.0f, 1.0f, -1.0f, -1.0f, 1.0f,
            1.0f, -1.0f, -1.0f, 1.0f, 1.0f, -1.0f, -1.0f, 1.0f, -1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f,
            1.0f, -1.0f, 1.0f, -1.0f, -1.0f, -1.0f, -1.0f, 1.0f, 1.0f, -1.0f, 1.0f, 1.0f, -1.0f, -1.0f, 1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, 1.0f, -1.0f, 1.0f, 1.0f, -1.0f, 1.0f, -1.0f, 1.0f, 1.0f, -1.0f,
            1.0f, 1.0f, -1.0f, 1.0f, 1.0f, 1.0f, -1.0f, 1.0f, 1.0f
        };

        /// <summary>
        ///     Gets the vertex shader source.
        ///     A basic shader that passes position and color from the vertex to the fragment shader. This is a simple shader
        ///     typically used for basic object rendering with color.
        /// </summary>
        /// <value>
        ///     The vertex shader source.
        /// </value>
        public static string VertexShaderSource => @"
            #version 450 core

            layout(location = 0) in vec3 aPosition;  // Changed from vec2 to vec3
            layout(location = 1) in vec3 aColor;

            out vec3 vColor;

            void main()
            {
                gl_Position = vec4(aPosition, 1.0);  // Use all three position values
                vColor = aColor;
            }
        ";


        /// <summary>
        ///     Gets the fragment shader source.
        ///     This is a basic fragment shader that outputs the color passed from the vertex shader. It's simple and can work with
        ///     any shader that just passes the color from the vertex stage.
        /// </summary>
        /// <value>
        ///     The fragment shader source.
        /// </value>
        public static string FragmentShaderSource => @"
                #version 450 core
                in vec3 vColor;
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(vColor, 1.0);
                }
            ";

        /// <summary>
        ///     Solid Color Shader
        ///     Renders objects with a solid color (e.g., red).
        ///     A vertex shader that just transforms the vertex positions for a solid color shader. This is simple and just passes
        ///     the position without color.
        /// </summary>
        /// <value>
        ///     The solid color vertex shader.
        /// </value>
        public static string SolidColorVertexShader => @"
            #version 450 core
            layout(location = 0) in vec3 aPosition; // Vertex position input

            void main()
            {
                gl_Position = vec4(aPosition, 1.0); // Convert position to clip space
            }
        ";

        /// <summary>
        ///     Gets the solid color fragment shader.
        /// </summary>
        /// <value>
        ///     The solid color fragment shader.
        /// </value>
        public static string SolidColorFragmentShader => @"
            #version 450 core
            out vec4 FragColor; // Output color

            void main()
            {
                FragColor = vec4(1.0, 0.0, 0.0, 1.0); // Output a red color
            }
        ";

        /// <summary>
        ///     Vertex Color Shader
        ///     Renders objects using colors assigned to each vertex.
        ///     A vertex shader that passes both position and color information from the vertex to the fragment shader. This is
        ///     used for vertex-colored objects.
        /// </summary>
        /// <value>
        ///     The vertex color vertex shader.
        /// </value>
        public static string VertexColorVertexShader => @"
            #version 450 core
            layout(location = 0) in vec3 aPosition; // Vertex position
            layout(location = 1) in vec3 aColor;    // Vertex color

            out vec3 vColor; // Pass color to fragment shader

            void main()
            {
                gl_Position = vec4(aPosition, 1.0);
                vColor = aColor; // Pass the vertex color to the fragment shader
            }
        ";

        /// <summary>
        ///     Texture Mapping Shader
        ///     Maps a 2D texture onto a 3D object.
        ///     This vertex shader passes texture coordinates, which is useful when working with texture mapping.
        /// </summary>
        /// <value>
        ///     The texture mapping vertex shader.
        /// </value>
        public static string TextureMappingVertexShader => @"
            #version 450 core
            layout(location = 0) in vec3 aPosition;  // Vertex position
            layout(location = 1) in vec2 aTexCoord; // Texture coordinates

            out vec2 vTexCoord; // Pass texture coordinates to fragment shader

            void main()
            {
                gl_Position = vec4(aPosition, 1.0);
                vTexCoord = aTexCoord; // Pass texture coordinates to fragment shader
            }
        ";

        /// <summary>
        ///     Gets the texture mapping fragment shader.
        /// </summary>
        /// <value>
        ///     The texture mapping fragment shader.
        /// </value>
        public static string TextureMappingFragmentShader => @"
            #version 450 core
            in vec2 vTexCoord;        // Interpolated texture coordinates
            out vec4 FragColor;       // Output color

            uniform sampler2D uTexture; // The texture

            void main()
            {
                FragColor = texture(uTexture, vTexCoord); // Sample texture at coordinates
            }
        ";

        /// <summary>
        ///     Basic Lighting Shader (Phong Lighting)
        ///     Adds simple directional lighting to a 3D object.
        /// </summary>
        /// <value>
        ///     The basic lighting vertex shader.
        /// </value>
        public static string BasicLightingVertexShader => @"
            #version 450 core
            layout(location = 0) in vec3 aPosition; // Vertex position
            layout(location = 1) in vec3 aNormal;   // Vertex normal

            out vec3 vNormal; // Pass normal to fragment shader

            void main()
            {
                gl_Position = vec4(aPosition, 1.0);
                vNormal = aNormal; // Pass the normal to the fragment shader
            }
        ";

        /// <summary>
        ///     Gets the basic lighting fragment shader.
        /// </summary>
        /// <value>
        ///     The basic lighting fragment shader.
        /// </value>
        public static string BasicLightingFragmentShader => @"
            #version 450 core
            in vec3 vNormal;         // Interpolated normal
            out vec4 FragColor;      // Output color

            uniform vec3 uLightDir;  // Direction of the light

            void main()
            {
                // Normalize the normal and light direction
                vec3 normal = normalize(vNormal);
                vec3 lightDir = normalize(uLightDir);

                // Compute the lighting intensity
                float intensity = max(dot(normal, lightDir), 0.0);

                // Output the shaded color
                FragColor = vec4(vec3(intensity), 1.0); // Grayscale shading
            }
        ";

        /// <summary>
        ///     Grayscale Shader
        ///     Converts a texture to grayscale using luminance formula.
        /// </summary>
        public static string GrayscaleFragmentShader => @"
            #version 450 core
            in vec2 vTexCoord;        // Interpolated texture coordinates
            out vec4 FragColor;       // Output color

            uniform sampler2D uTexture; // The texture

            void main()
            {
                vec4 color = texture(uTexture, vTexCoord); // Sample texture
                float gray = dot(color.rgb, vec3(0.299, 0.587, 0.114)); // Luminance formula
                FragColor = vec4(vec3(gray), 1.0); // Output grayscale color
            }
        ";

        /// <summary>
        ///     Wireframe Shader
        ///     Renders a mesh in wireframe mode (green color).
        /// </summary>
        /// <value>
        ///     The wireframe vertex shader.
        /// </value>
        public static string WireframeVertexShader => @"
            #version 450 core
            layout(location = 0) in vec3 aPosition;

            void main()
            {
                gl_Position = vec4(aPosition, 1.0);
            }
        ";

        /// <summary>
        ///     Gets the wireframe fragment shader.
        /// </summary>
        /// <value>
        ///     The wireframe fragment shader.
        /// </value>
        public static string WireframeFragmentShader => @"
            #version 450 core
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(0.0, 1.0, 0.0, 1.0); // Green wireframe
            }
        ";
    }
}
