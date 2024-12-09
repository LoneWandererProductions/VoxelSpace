/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/TextureAreas.cs
 * PURPOSE:     Provide textures for certain areas
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * Sources:     https://lodev.org/cgtutor/randomnoise.html
 */

using System;
using System.Drawing;

namespace Imaging
{
    /// <summary>
    ///     Apply textures to certain areas
    /// </summary>
    internal static class TextureAreas
    {
        /// <summary>
        ///     Generates the texture.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="texture">The filter.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="imageSettings"></param>
        /// <param name="shapeParams">The shape parameters.</param>
        /// <param name="startPoint">The optional starting point (top-left corner) of the rectangle. Defaults to (0, 0).</param>
        /// <returns>
        ///     Generates a filter for a certain area
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     filter - null
        ///     or
        ///     shape - null
        /// </exception>
        internal static Bitmap GenerateTexture(int width,
            int height,
            TextureType texture,
            MaskShape shape,
            ImageRegister imageSettings,
            object shapeParams = null,
            Point? startPoint = null)
        {
            // If no start point is provided, default to (0, 0)
            var actualStartPoint = startPoint ?? new Point(0, 0);

            // Retrieve the settings for the specified filter
            var settings = imageSettings.GetSettings(texture);

            // Create a bitmap to apply the texture
            Bitmap textureBitmap;

            // Generate texture based on the selected filter
            switch (texture)
            {
                case TextureType.Noise:
                    textureBitmap = Texture.GenerateNoiseBitmap(
                        width,
                        height,
                        settings.MinValue,
                        settings.MaxValue,
                        settings.Alpha,
                        settings.IsMonochrome,
                        settings.IsTiled,
                        settings.TurbulenceSize);
                    break;

                case TextureType.Clouds:
                    textureBitmap = Texture.GenerateCloudsBitmap(
                        width,
                        height,
                        settings.MinValue,
                        settings.MaxValue,
                        settings.Alpha,
                        settings.TurbulenceSize);
                    break;

                case TextureType.Marble:
                    textureBitmap = Texture.GenerateMarbleBitmap(
                        width,
                        height,
                        settings.Alpha,
                        baseColor: settings.BaseColor);
                    break;

                case TextureType.Wood:
                    textureBitmap = Texture.GenerateWoodBitmap(
                        width,
                        height,
                        settings.Alpha,
                        baseColor: settings.BaseColor);
                    break;

                case TextureType.Wave:
                    textureBitmap = Texture.GenerateWaveBitmap(width, height, settings.Alpha);
                    break;
                case TextureType.Crosshatch:
                    textureBitmap = Texture.GenerateCrosshatchBitmap(width, height, settings.LineSpacing,
                        settings.LineColor, settings.LineThickness, settings.Angle1, settings.Angle2, settings.Alpha);
                    break;
                case TextureType.Concrete:
                    textureBitmap = Texture.GenerateConcreteBitmap(width, height, settings.MinValue,
                        settings.MaxValue, settings.Alpha, settings.TurbulenceSize);
                    break;
                case TextureType.Canvas:
                    textureBitmap = Texture.GenerateCanvasBitmap(width, height, settings.LineSpacing,
                        settings.LineColor, settings.LineThickness, settings.Alpha);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(texture), texture, null);
            }

            // Apply the texture to the specified area shape
            switch (shape)
            {
                case MaskShape.Rectangle:
                    return ImageMask.ApplyRectangleMask(textureBitmap, width, height, actualStartPoint);

                case MaskShape.Circle:
                    return ImageMask.ApplyCircleMask(textureBitmap, width, height, actualStartPoint);

                case MaskShape.Polygon:
                    return ImageMask.ApplyPolygonMask(textureBitmap, (Point[])shapeParams);

                default:
                    throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
            }
        }
    }
}