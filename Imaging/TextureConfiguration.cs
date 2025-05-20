/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/TextureConfiguration.cs
 * PURPOSE:     Basic stuff for generating textures, this class is used for finetuning
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * Sources:     https://lodev.org/cgtutor/randomnoise.html
 */

using System.Drawing;

namespace Imaging
{
    /// <summary>
    ///     Attributes for our texture generators
    /// </summary>
    public sealed class TextureConfiguration
    {
        /// <summary>
        ///     Gets or sets the minimum value.
        /// </summary>
        /// <value>
        ///     The minimum value.
        /// </value>
        public int MinValue { get; init; }

        /// <summary>
        ///     Gets or sets the maximum value.
        /// </summary>
        /// <value>
        ///     The maximum value.
        /// </value>
        public int MaxValue { get; init; } = 255;

        /// <summary>
        ///     Gets or sets the alpha.
        /// </summary>
        /// <value>
        ///     The alpha.
        /// </value>
        public int Alpha { get; init; } = 255;

        /// <summary>
        ///     Gets or sets the x period.
        /// </summary>
        /// <value>
        ///     The x period.
        /// </value>
        public double XPeriod { get; init; } = 5.0;

        /// <summary>
        ///     Gets or sets the y period.
        /// </summary>
        /// <value>
        ///     The y period.
        /// </value>
        public double YPeriod { get; init; } = 10.0;

        /// <summary>
        ///     Gets or sets the turbulence power.
        /// </summary>
        /// <value>
        ///     The turbulence power.
        /// </value>
        public double TurbulencePower { get; init; } = 5.0;

        /// <summary>
        ///     Gets or sets the size of the turbulence.
        /// </summary>
        /// <value>
        ///     The size of the turbulence.
        /// </value>
        public double TurbulenceSize { get; init; } = 64.0;

        /// <summary>
        ///     Gets or sets the color of the base.
        /// </summary>
        /// <value>
        ///     The color of the base.
        /// </value>
        public Color BaseColor { get; init; } = Color.White;

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is monochrome.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is monochrome; otherwise, <c>false</c>.
        /// </value>
        public bool IsMonochrome { get; init; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is tiled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tiled; otherwise, <c>false</c>.
        /// </value>
        public bool IsTiled { get; init; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether to use smooth noise.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [use smooth noise]; otherwise, <c>false</c>.
        /// </value>
        public bool UseSmoothNoise { get; init; }

        /// <summary>
        ///     Gets or sets a value indicating whether to use turbulence.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [use turbulence]; otherwise, <c>false</c>.
        /// </value>
        public bool UseTurbulence { get; init; }

        /// <summary>
        ///     Gets or sets the xy period, used for wave and wood textures.
        /// </summary>
        /// <value>
        ///     The xy period.
        /// </value>
        public double XyPeriod { get; init; } = 12.0;

        // New parameters for crosshatch texture

        /// <summary>
        ///     Gets or sets the spacing between lines for crosshatch texture.
        /// </summary>
        /// <value>
        ///     The spacing between lines.
        /// </value>
        public int LineSpacing { get; init; } = 10;

        /// <summary>
        ///     Gets or sets the color of the lines for crosshatch texture.
        /// </summary>
        /// <value>
        ///     The color of the lines.
        /// </value>
        public Color LineColor { get; init; } = Color.Black;

        /// <summary>
        ///     Gets or sets the thickness of the lines for crosshatch texture.
        /// </summary>
        /// <value>
        ///     The thickness of the lines.
        /// </value>
        public int LineThickness { get; init; } = 1;

        /// <summary>
        ///     Gets or sets the angle of the first set of lines for crosshatch texture, in degrees.
        /// </summary>
        /// <value>
        ///     The angle of the first set of lines.
        /// </value>
        public double AnglePrimary { get; init; } = 45.0;

        /// <summary>
        ///     Gets or sets the angle of the second set of lines for crosshatch texture, in degrees.
        /// </summary>
        /// <value>
        ///     The angle of the second set of lines.
        /// </value>
        public double AngleSecondary { get; init; } = 135.0;

        /// <summary>
        ///     Gets or sets the wave frequency.
        /// </summary>
        /// <value>
        ///     The wave frequency.
        /// </value>
        public double WaveFrequency { get; init; } = 0.02;

        /// <summary>
        ///     Gets the wave amplitude.
        /// </summary>
        /// <value>
        ///     The wave amplitude.
        /// </value>
        public double WaveAmplitude { get; init; } = 3;

        /// <summary>
        ///     Gets the randomization factor.
        /// </summary>
        /// <value>
        ///     The randomization factor.
        /// </value>
        public double RandomizationFactor { get; init; } = 1.5;

        /// <summary>
        ///     Gets or sets the edge jaggedness limit.
        /// </summary>
        /// <value>
        ///     The edge jaggedness limit.
        /// </value>
        public int EdgeJaggednessLimit { get; init; } = 20;

        /// <summary>
        ///     Gets or sets the jaggedness threshold.
        /// </summary>
        /// <value>
        ///     The jaggedness threshold.
        /// </value>
        public int JaggednessThreshold { get; init; } = 10;
    }
}
