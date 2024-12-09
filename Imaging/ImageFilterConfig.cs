/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageFilterConfig.cs
 * PURPOSE:     Object that will be used for finetuning the filters
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Imaging
{
    /// <summary>
    ///     Settings for the Filter
    /// </summary>
    public sealed class ImageFilterConfig
    {
        /// <summary>
        ///     Gets or sets the factor.
        /// </summary>
        /// <value>
        ///     The factor.
        /// </value>
        public double Factor { get; init; } = 1.0;

        /// <summary>
        ///     Gets or sets the bias.
        /// </summary>
        /// <value>
        ///     The bias.
        /// </value>
        public double Bias { get; init; }

        /// <summary>
        ///     Gets or sets the sigma.
        /// </summary>
        /// <value>
        ///     The sigma.
        /// </value>
        public double Sigma { get; init; } = 1.0;

        /// <summary>
        ///     Gets or sets the size of the base window.
        /// </summary>
        /// <value>
        ///     The size of the base window.
        /// </value>
        public int BaseWindowSize { get; init; } = 5;

        /// <summary>
        ///     Gets or sets the scale.
        /// </summary>
        /// <value>
        ///     The scale.
        /// </value>
        public int Scale { get; init; } = 1;
    }
}