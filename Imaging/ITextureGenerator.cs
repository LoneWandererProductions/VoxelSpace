/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ITextureGenerator.cs
 * PURPOSE:    Interface of TextureGenerator
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * Sources:     https://lodev.org/cgtutor/randomnoise.html
 */

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeInternal

using System.Drawing;

namespace Imaging
{
    /// <summary>
    ///     Skeleton for an Texture Interface
    /// </summary>
    public interface ITextureGenerator
    {
        /// <summary>
        ///     Generates the noise bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        Bitmap GenerateNoiseBitmap(
            int width,
            int height);

        /// <summary>
        ///     Generates the clouds bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        Bitmap GenerateCloudsBitmap(
            int width,
            int height);

        /// <summary>
        ///     Generates the marble bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        Bitmap GenerateMarbleBitmap(
            int width,
            int height);

        /// <summary>
        ///     Generates the wave bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        Bitmap GenerateWaveBitmap(
            int width,
            int height);

        /// <summary>
        ///     Generates the wood bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        Bitmap GenerateWoodBitmap(
            int width,
            int height);

        /// <summary>
        ///     Generates the crosshatch bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        Bitmap GenerateCrosshatchBitmap(
            int width,
            int height);

        /// <summary>
        ///     Generates the concrete bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        Bitmap GenerateConcreteBitmap(
            int width,
            int height);

        /// <summary>
        ///     Generates the canvas bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        Bitmap GenerateCanvasBitmap(
            int width,
            int height);

        /// <summary>
        ///     Generates the texture.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="startPoint">The Start point.</param>
        /// <param name="shapeParams">The shape parameters.</param>
        /// <returns>
        ///     Texture Bitmap
        /// </returns>
        Bitmap GenerateTexture(
            int width,
            int height,
            TextureType filter,
            MaskShape shape,
            Point? startPoint = null,
            object shapeParams = null);
    }
}