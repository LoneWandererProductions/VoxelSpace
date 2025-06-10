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
        UnmanagedImageBuffer FromIntArray(int width, int height, int[] bits);
        int[] ToIntArray(UnmanagedImageBuffer buffer);

        LayeredImageContainer CreateLayeredContainer(int width, int height);
        LayeredImageContainer CreateFromLayers(params UnmanagedImageBuffer[] layers);

        UnmanagedImageBuffer Composite(LayeredImageContainer container);

        void MergeLayers(LayeredImageContainer container, int[] layerIndices, int insertAt);
    }
}