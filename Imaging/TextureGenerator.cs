/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/TextureGenerator.cs
 * PURPOSE:     Generate some textures
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * Sources:     https://lodev.org/cgtutor/randomnoise.html
 */

// ReSharper disable UnusedType.Global

using System.Drawing;

namespace Imaging
{
    /// <inheritdoc />
    /// <summary>
    ///     Main Entry Class that will handle all things related to textures
    /// </summary>
    /// <seealso cref="T:Imaging.ITextureGenerator" />
    public sealed class TextureGenerator : ITextureGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TextureGenerator" /> class.
        /// </summary>
        public TextureGenerator()
        {
            ImageSettings = ImageRegister.Instance; // Ensure singleton instance is available
        }

        /// <summary>
        ///     The image Settings
        /// </summary>
        /// <value>
        ///     The image settings.
        /// </value>
        private ImageRegister ImageSettings { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Generates the noise bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        public Bitmap GenerateNoiseBitmap(int width, int height)
        {
            var config = ImageSettings.GetSettings(TextureType.Noise);
            return TextureStream.GenerateNoiseBitmap(
                width,
                height,
                config.MinValue,
                config.MaxValue,
                config.Alpha,
                config.UseSmoothNoise,
                config.UseTurbulence,
                config.TurbulenceSize
            );
        }

        /// <inheritdoc />
        /// <summary>
        ///     Generates the clouds bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        public Bitmap GenerateCloudsBitmap(int width, int height)
        {
            var config = ImageSettings.GetSettings(TextureType.Clouds);
            return TextureStream.GenerateCloudsBitmap(
                width,
                height,
                config.MinValue,
                config.MaxValue,
                config.Alpha,
                config.TurbulenceSize
            );
        }

        /// <inheritdoc />
        /// <summary>
        ///     Generates the marble bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        public Bitmap GenerateMarbleBitmap(int width, int height)
        {
            var config = ImageSettings.GetSettings(TextureType.Marble);
            return TextureStream.GenerateMarbleBitmap(
                width,
                height,
                config.XPeriod,
                config.YPeriod,
                config.Alpha,
                config.TurbulencePower,
                config.TurbulenceSize,
                config.BaseColor
            );
        }

        /// <inheritdoc />
        /// <summary>
        ///     Generates the wave bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        public Bitmap GenerateWaveBitmap(int width, int height)
        {
            var config = ImageSettings.GetSettings(TextureType.Wave);
            return TextureStream.GenerateWaveBitmap(
                width,
                height,
                config.Alpha,
                config.XyPeriod,
                config.TurbulencePower,
                config.TurbulenceSize
            );
        }

        /// <inheritdoc />
        /// <summary>
        ///     Generates the wood bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        public Bitmap GenerateWoodBitmap(int width, int height)
        {
            var config = ImageSettings.GetSettings(TextureType.Wood);
            return TextureStream.GenerateWoodBitmap(
                width,
                height,
                config.Alpha,
                config.XyPeriod,
                config.TurbulencePower,
                config.TurbulenceSize,
                config.BaseColor
            );
        }

        /// <inheritdoc />
        /// <summary>
        ///     Generates the texture.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="filter">The texture type filter.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="startPoint">The Start point.</param>
        /// <param name="shapeParams">The shape parameters.</param>
        /// <returns>Texture Bitmap</returns>
        public Bitmap GenerateTexture(int width, int height, TextureType filter, MaskShape shape,
            Point? startPoint = null,
            object shapeParams = null)
        {
            return TextureAreas.GenerateTexture(
                null,
                width,
                height,
                filter,
                shape,
                ImageSettings, shapeParams, startPoint);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Generates the texture overlay.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="height">The height.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="shapeParams">The shape parameters.</param>
        /// <param name="width">The width.</param>
        /// <returns>
        ///     Texture Bitmap
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     filter - null
        ///     or
        ///     shape - null
        /// </exception>
        public Bitmap GenerateTextureOverlay(Bitmap image, int width, int height, TextureType filter, MaskShape shape,
            Point? startPoint = null,
            object shapeParams = null)
        {
            return TextureAreas.GenerateTexture(
                image,
                width,
                height,
                filter,
                shape,
                ImageSettings, shapeParams, startPoint);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Generates the crosshatch bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        public Bitmap GenerateCrosshatchBitmap(int width, int height)
        {
            var config = ImageSettings.GetSettings(TextureType.Crosshatch);

            return TextureStream.GenerateCrosshatchBitmap(
                width,
                height,
                config.LineSpacing,
                config.LineColor,
                config.LineThickness,
                config.AnglePrimary,
                config.AngleSecondary
            );
        }

        /// <inheritdoc />
        /// <summary>
        ///     Generates the concrete bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        public Bitmap GenerateConcreteBitmap(int width, int height)
        {
            var config = ImageSettings.GetSettings(TextureType.Crosshatch);

            return TextureStream.GenerateConcreteBitmap(
                width,
                height,
                config.MinValue,
                config.MaxValue,
                config.Alpha,
                config.XPeriod,
                config.YPeriod,
                config.TurbulencePower,
                config.TurbulenceSize
            );
        }

        /// <inheritdoc />
        /// <summary>
        ///     Generates the canvas bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Texture Bitmap</returns>
        public Bitmap GenerateCanvasBitmap(int width, int height)
        {
            var config = ImageSettings.GetSettings(TextureType.Crosshatch);

            return TextureStream.GenerateCanvasBitmap(
                width,
                height,
                config.LineSpacing,
                config.LineColor,
                config.LineThickness,
                config.Alpha,
                config.WaveFrequency,
                config.WaveAmplitude,
                config.RandomizationFactor,
                config.EdgeJaggednessLimit,
                config.JaggednessThreshold
            );
        }
    }
}