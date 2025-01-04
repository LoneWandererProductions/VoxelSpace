/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/FiltersType.cs
 * PURPOSE:     Enum that holds all possible Filter
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://docs.rainmeter.net/tips/colormatrix-guide/
 *              https://archive.ph/hzR2W
 *              https://www.codeproject.com/Articles/3772/ColorMatrix-Basics-Simple-Image-Color-Adjustment
 */

namespace Imaging
{
    /// <summary>
    ///     Enum of possible Filter
    /// </summary>
    public enum FiltersType
    {
        /// <summary>
        ///     No Filter
        /// </summary>
        None = 0,

        /// <summary>
        ///     The gray scale filter
        /// </summary>
        GrayScale = 1,

        /// <summary>
        ///     The invert filter
        /// </summary>
        Invert = 2,

        /// <summary>
        ///     The sepia filter
        /// </summary>
        Sepia = 3,

        /// <summary>
        ///     The black and white filter
        /// </summary>
        BlackAndWhite = 4,

        /// <summary>
        ///     The polaroid filter
        /// </summary>
        Polaroid = 5,

        /// <summary>
        ///     The contour
        /// </summary>
        Contour = 6,

        // New color filters

        /// <summary>
        ///     The brightness filter
        /// </summary>
        Brightness = 7,

        /// <summary>
        ///     The contrast filter
        /// </summary>
        Contrast = 8,

        /// <summary>
        ///     The hue shift filter
        /// </summary>
        HueShift = 9,

        /// <summary>
        ///     The color balance filter
        /// </summary>
        ColorBalance = 10,

        /// <summary>
        ///     The vintage filter
        /// </summary>
        Vintage = 11,

        // New convolution-based filters

        /// <summary>
        ///     The sharpen filter
        /// </summary>
        Sharpen = 12,

        /// <summary>
        ///     The gaussian blur filter
        /// </summary>
        GaussianBlur = 13,

        /// <summary>
        ///     The emboss filter
        /// </summary>
        Emboss = 14,

        /// <summary>
        ///     The box blur filter
        /// </summary>
        BoxBlur = 15,

        /// <summary>
        ///     The laplacian filter
        /// </summary>
        Laplacian = 16,

        /// <summary>
        ///     The edge enhance filter
        /// </summary>
        EdgeEnhance = 17,

        /// <summary>
        ///     The motion blur filter
        /// </summary>
        MotionBlur = 18,

        /// <summary>
        ///     The unsharp mask filter
        /// </summary>
        UnsharpMask = 19,

        /// <summary>
        ///     The difference of gaussians
        /// </summary>
        DifferenceOfGaussians = 20,

        /// <summary>
        ///     The crosshatch
        /// </summary>
        Crosshatch = 21,

        /// <summary>
        ///     The floyd steinberg dithering
        /// </summary>
        FloydSteinbergDithering = 22,

        /// <summary>
        ///     The anisotropic kuwahara
        /// </summary>
        AnisotropicKuwahara = 23,


        /// <summary>
        ///     The supersampling antialiasing
        /// </summary>
        SupersamplingAntialiasing = 24,

        /// <summary>
        ///     The post processing antialiasing
        /// </summary>
        PostProcessingAntialiasing = 25,

        /// <summary>
        ///     The pencil sketch effect
        /// </summary>
        PencilSketchEffect = 26
    }
}
