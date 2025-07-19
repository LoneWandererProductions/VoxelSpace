/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        IImageToolkit.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace RenderEngine
{
    /// <summary>
    ///     Interface for our ImageToolkit
    /// </summary>
    public interface IImageToolkit
    {
        /// <summary>
        ///     From int to array.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="bits">The bits.</param>
        /// <returns></returns>
        UnmanagedImageBuffer FromIntArray(int width, int height, int[] bits);

        /// <summary>
        ///     Converts to int array.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
        int[] ToIntArray(UnmanagedImageBuffer buffer);

        /// <summary>
        ///     Creates the layered container.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        LayeredImageContainer CreateLayeredContainer(int width, int height);

        /// <summary>
        ///     Creates from layers.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <returns></returns>
        LayeredImageContainer CreateFromLayers(params UnmanagedImageBuffer[] layers);

        /// <summary>
        ///     Composites the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        UnmanagedImageBuffer Composite(LayeredImageContainer container);

        /// <summary>
        ///     Merges the layers.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="layerIndices">The layer indices.</param>
        /// <param name="insertAt">The insert at.</param>
        void MergeLayers(LayeredImageContainer container, int[] layerIndices, int insertAt);
    }
}
