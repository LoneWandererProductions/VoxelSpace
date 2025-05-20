/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageRegister.cs
 * PURPOSE:     Register for Image Operations, and some helpful extensions
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://docs.rainmeter.net/tips/colormatrix-guide/
 *              https://archive.ph/hzR2W
 *              https://www.codeproject.com/Articles/3772/ColorMatrix-Basics-Simple-Image-Color-Adjustment
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.Json;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global

namespace Imaging
{
    /// <summary>
    ///     The image register class.
    /// </summary>
    public sealed class ImageRegister
    {
        // Private static instance of the singleton
        private static readonly Lazy<ImageRegister> Settings = new(() => new ImageRegister());

        /// <summary>
        ///     The filter property map
        ///     Mapping of filters to their used properties
        /// </summary>
        private readonly Dictionary<FiltersType, HashSet<string>> _filterPropertyMap = new()
        {
            {
                FiltersType.GaussianBlur,
                new HashSet<string> { nameof(FiltersConfig.Factor), nameof(FiltersConfig.Bias) }
            },
            { FiltersType.BoxBlur, new HashSet<string> { nameof(FiltersConfig.Factor), nameof(FiltersConfig.Bias) } },
            {
                FiltersType.MotionBlur, new HashSet<string> { nameof(FiltersConfig.Factor), nameof(FiltersConfig.Bias) }
            },
            { FiltersType.Sharpen, new HashSet<string> { nameof(FiltersConfig.Factor), nameof(FiltersConfig.Bias) } },
            { FiltersType.Emboss, new HashSet<string> { nameof(FiltersConfig.Factor), nameof(FiltersConfig.Bias) } },
            { FiltersType.Laplacian, new HashSet<string> { nameof(FiltersConfig.Factor), nameof(FiltersConfig.Bias) } },
            {
                FiltersType.EdgeEnhance,
                new HashSet<string> { nameof(FiltersConfig.Factor), nameof(FiltersConfig.Bias) }
            },
            {
                FiltersType.UnsharpMask,
                new HashSet<string> { nameof(FiltersConfig.Factor), nameof(FiltersConfig.Bias) }
            },
            { FiltersType.AnisotropicKuwahara, new HashSet<string> { nameof(FiltersConfig.BaseWindowSize) } },
            { FiltersType.SupersamplingAntialiasing, new HashSet<string> { nameof(FiltersConfig.Scale) } },
            { FiltersType.PostProcessingAntialiasing, new HashSet<string> { nameof(FiltersConfig.Sigma) } },
            { FiltersType.PencilSketchEffect, new HashSet<string> { nameof(FiltersConfig.Sigma) } }

            // Add other filters as necessary
        };

        /// <summary>
        ///     The texture property map
        ///     Mapping of textures to their used properties
        /// </summary>
        private readonly Dictionary<TextureType, HashSet<string>> _texturePropertyMap = new()
        {
            {
                TextureType.Noise,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.MinValue),
                    nameof(TextureConfiguration.MaxValue),
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.UseSmoothNoise),
                    nameof(TextureConfiguration.UseTurbulence),
                    nameof(TextureConfiguration.TurbulenceSize)
                }
            },
            {
                TextureType.Clouds,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.MinValue),
                    nameof(TextureConfiguration.MaxValue),
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.TurbulenceSize)
                }
            },
            {
                TextureType.Marble,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.XPeriod),
                    nameof(TextureConfiguration.YPeriod),
                    nameof(TextureConfiguration.TurbulencePower),
                    nameof(TextureConfiguration.TurbulenceSize),
                    nameof(TextureConfiguration.BaseColor)
                }
            },
            {
                TextureType.Wave,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.XyPeriod),
                    nameof(TextureConfiguration.TurbulencePower),
                    nameof(TextureConfiguration.TurbulenceSize)
                }
            },
            {
                TextureType.Wood,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.XyPeriod),
                    nameof(TextureConfiguration.TurbulencePower),
                    nameof(TextureConfiguration.TurbulenceSize),
                    nameof(TextureConfiguration.BaseColor)
                }
            },
            {
                TextureType.Crosshatch,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.LineSpacing),
                    nameof(TextureConfiguration.LineColor),
                    nameof(TextureConfiguration.LineThickness),
                    nameof(TextureConfiguration.AnglePrimary),
                    nameof(TextureConfiguration.AngleSecondary)
                }
            },
            {
                TextureType.Concrete,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.MinValue),
                    nameof(TextureConfiguration.MaxValue),
                    nameof(TextureConfiguration.YPeriod),
                    nameof(TextureConfiguration.XPeriod),
                    nameof(TextureConfiguration.TurbulencePower),
                    nameof(TextureConfiguration.TurbulenceSize)
                }
            },
            {
                TextureType.Canvas,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.LineSpacing),
                    nameof(TextureConfiguration.LineColor),
                    nameof(TextureConfiguration.LineThickness)
                }
            }

            // Add other textures as necessary
        };

        /// <summary>
        ///     the color matrix needed to Color Swap an image to BlackAndWhite
        ///     Source:
        ///     https://docs.rainmeter.net/tips/colormatrix-guide/
        /// </summary>
        internal readonly ColorMatrix BlackAndWhite = new(new[]
        {
            new[] { 1.5f, 1.5f, 1.5f, 0, 0 }, new[] { 1.5f, 1.5f, 1.5f, 0, 0 }, new[] { 1.5f, 1.5f, 1.5f, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { -1, -1, -1, 0, 1 }
        });

        /// <summary>
        ///     The box blur
        /// </summary>
        internal readonly double[,] BoxBlur = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };

        /// <summary>
        ///     The brightness Filter
        ///     Adjusts the brightness of the image by scaling the color values.
        /// </summary>
        internal readonly ColorMatrix Brightness = new(new[]
        {
            new[] { 1.2f, 0, 0, 0, 0 }, new[] { 0, 1.2f, 0, 0, 0 }, new[] { 0, 0, 1.2f, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     The color balance Filter
        ///     Adjusts the balance of colors to emphasize or de-emphasize specific color channels.
        /// </summary>
        internal readonly ColorMatrix ColorBalance = new(new[]
        {
            new[] { 1f, 0.2f, -0.2f, 0, 0 }, new[] { -0.2f, 1f, 0.2f, 0, 0 }, new[] { 0.2f, -0.2f, 1f, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     The contrast Filter
        ///     Adjusts the contrast of the image by scaling the differences between pixel values.
        /// </summary>
        internal readonly ColorMatrix Contrast = new(new[]
        {
            new[] { 1.5f, 0, 0, 0, -0.2f }, new[] { 0, 1.5f, 0, 0, -0.2f }, new[] { 0, 0, 1.5f, 0, -0.2f },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     The edge enhance
        /// </summary>
        internal readonly double[,] EdgeEnhance = { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };

        /// <summary>
        ///     The emboss filter
        /// </summary>
        internal readonly double[,] EmbossFilter = { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };

        /// <summary>
        ///     The gaussian blur
        /// </summary>
        internal readonly double[,] GaussianBlur = { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };

        /// <summary>
        ///     the color matrix needed to GrayScale an image
        ///     Source:
        ///     https://archive.ph/hzR2W
        ///     ColorMatrix:
        ///     | m11 m12 m13 m14 m15 |
        ///     | m21 m22 m23 m24 m25 |
        ///     | m31 m32 m33 m34 m35 |
        ///     | m41 m42 m43 m44 m45 |
        ///     | m51 m52 m53 m54 m55 |
        ///     translates to:
        ///     NewR = (m11 * R + m12 * G + m13 * B + m14 * A + m15)
        ///     NewG = (m21* R + m22* G + m23* B + m24* A + m25)
        ///     NewB = (m31* R + m32* G + m33* B + m34* A + m35)
        ///     NewA = (m41* R + m42* G + m43* B + m44* A + m45)
        /// </summary>
        internal readonly ColorMatrix GrayScale = new(new[]
        {
            new[] { .3f, .3f, .3f, 0, 0 }, new[] { .59f, .59f, .59f, 0, 0 }, new[] { .11f, .11f, .11f, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     The hue shift Filter
        ///     Shifts the hue of the image, effectively rotating the color wheel.
        /// </summary>
        internal readonly ColorMatrix HueShift = new(new[]
        {
            new[] { 0.213f, 0.715f, 0.072f, 0, 0 }, new[] { 0.213f, 0.715f, 0.072f, 0, 0 },
            new[] { 0.213f, 0.715f, 0.072f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     the color matrix needed to invert an image
        ///     Source:
        ///     https://archive.ph/hzR2W
        /// </summary>
        internal readonly ColorMatrix Invert = new(new[]
        {
            new float[] { -1, 0, 0, 0, 0 }, new float[] { 0, -1, 0, 0, 0 }, new float[] { 0, 0, -1, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { 1, 1, 1, 0, 1 }
        });

        /// <summary>
        ///     The kernel 135 degrees
        ///     Defines directional edge detection kernel for crosshatching
        /// </summary>
        internal readonly double[,] Kernel135Degrees = { { 2, -1, -1 }, { -1, 2, -1 }, { -1, -1, 2 } };

        /// <summary>
        ///     The kernel 45 degrees
        ///     Defines directional edge detection kernel for crosshatching
        /// </summary>
        internal readonly double[,] Kernel45Degrees = { { -1, -1, 2 }, { -1, 2, -1 }, { 2, -1, -1 } };

        /// <summary>
        ///     The laplacian filter
        /// </summary>
        internal readonly double[,] LaplacianFilter = { { 0, -1, 0 }, { -1, 4, -1 }, { 0, -1, 0 } };

        /// <summary>
        ///     The motion blur
        /// </summary>
        internal readonly double[,] MotionBlur =
        {
            { 1, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 0, 0, 1 }
        };

        /// <summary>
        ///     the color matrix needed to Color Swap an image to Polaroid
        ///     Source:
        ///     https://docs.rainmeter.net/tips/colormatrix-guide/
        /// </summary>
        internal readonly ColorMatrix Polaroid = new(new[]
        {
            new[] { 1.438f, -0.062f, -0.062f, 0, 0 }, new[] { -0.122f, 1.378f, -0.122f, 0, 0 },
            new[] { 0.016f, -0.016f, 1.483f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 },
            new[] { 0.03f, 0.05f, -0.02f, 0, 1 }
        });

        /// <summary>
        ///     the color matrix needed to Sepia an image
        ///     Source:
        ///     https://archive.ph/hzR2W
        /// </summary>
        internal readonly ColorMatrix Sepia = new(new[]
        {
            new[] { .393f, .349f, .272f, 0, 0 }, new[] { .769f, .686f, .534f, 0, 0 },
            new[] { 0.189f, 0.168f, 0.131f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     The sharpen filter
        /// </summary>
        internal readonly double[,] SharpenFilter = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };


        /// <summary>
        ///     The sobel x kernel
        /// </summary>
        internal readonly int[,] SobelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };

        /// <summary>
        ///     The sobel y kernel
        /// </summary>
        internal readonly int[,] SobelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

        /// <summary>
        ///     The unsharp mask
        /// </summary>
        internal readonly double[,] UnsharpMask = { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };

        /// <summary>
        ///     The vintage Filter
        ///     Applies a vintage effect by modifying the color matrix to mimic old photo tones.
        /// </summary>
        internal readonly ColorMatrix Vintage = new(new[]
        {
            new[] { 0.393f, 0.349f, 0.272f, 0, 0 }, new[] { 0.769f, 0.686f, 0.534f, 0, 0 },
            new[] { 0.189f, 0.168f, 0.131f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     Initializes the <see cref="ImageRegister" /> class.
        /// </summary>
        static ImageRegister()
        {
            // Initialize default Filter settings
            FilterSettings[FiltersType.GaussianBlur] = new FiltersConfig { Factor = 1.0 / 16.0, Bias = 0.0 };
            FilterSettings[FiltersType.BoxBlur] = new FiltersConfig { Factor = 1.0 / 9.0, Bias = 0.0 };
            FilterSettings[FiltersType.MotionBlur] = new FiltersConfig { Factor = 1.0 / 5.0, Bias = 0.0 };
            FilterSettings[FiltersType.Sharpen] =
                new FiltersConfig { Factor = 1.0, Bias = 0.0 }; // Assuming default values
            FilterSettings[FiltersType.Emboss] =
                new FiltersConfig { Factor = 1.0, Bias = 0.0 }; // Assuming default values
            FilterSettings[FiltersType.Laplacian] =
                new FiltersConfig { Factor = 1.0, Bias = 0.0 }; // Assuming default values
            FilterSettings[FiltersType.EdgeEnhance] =
                new FiltersConfig { Factor = 1.0, Bias = 0.0 }; // Assuming default values
            FilterSettings[FiltersType.UnsharpMask] =
                new FiltersConfig { Factor = 1.0, Bias = 0.0 }; // Assuming default values
            FilterSettings[FiltersType.AnisotropicKuwahara] = new FiltersConfig { BaseWindowSize = 5 };
            FilterSettings[FiltersType.SupersamplingAntialiasing] = new FiltersConfig { Scale = 1 };
            FilterSettings[FiltersType.PostProcessingAntialiasing] = new FiltersConfig { Sigma = 1.0 };
            FilterSettings[FiltersType.PencilSketchEffect] = new FiltersConfig { Sigma = 1.0 };
            // Add more default settings as needed

            // Initialize default Texture settings
            TextureSetting[TextureType.Noise] = new TextureConfiguration
            {
                MinValue = 0,
                MaxValue = 255,
                Alpha = 255,
                TurbulenceSize = 64,
                UseSmoothNoise = false,
                UseTurbulence = false
            };

            TextureSetting[TextureType.Clouds] = new TextureConfiguration
            {
                MinValue = 0, MaxValue = 255, Alpha = 255, TurbulenceSize = 64
            };

            TextureSetting[TextureType.Marble] = new TextureConfiguration
            {
                Alpha = 255,
                XPeriod = 5.0,
                YPeriod = 10.0,
                TurbulencePower = 5.0,
                TurbulenceSize = 32.0,
                BaseColor = Color.FromArgb(30, 10, 0)
            };

            TextureSetting[TextureType.Wave] = new TextureConfiguration
            {
                Alpha = 255, XyPeriod = 12.0, TurbulencePower = 0.1, TurbulenceSize = 32.0
            };

            TextureSetting[TextureType.Wood] = new TextureConfiguration
            {
                Alpha = 255,
                XyPeriod = 12.0,
                TurbulencePower = 0.1,
                TurbulenceSize = 32.0,
                BaseColor = Color.FromArgb(80, 30, 30)
            };

            TextureSetting[TextureType.Crosshatch] = new TextureConfiguration
            {
                LineSpacing = 2,
                LineColor = Color.Black,
                LineThickness = 1,
                AnglePrimary = 45.0f,
                AngleSecondary = 135.0f
            };

            TextureSetting[TextureType.Concrete] = new TextureConfiguration
            {
                MinValue = 50,
                MaxValue = 200,
                TurbulenceSize = 16,
                XPeriod = 5.0,
                YPeriod = 10.0,
                TurbulencePower = 5.0
            };

            TextureSetting[TextureType.Canvas] = new TextureConfiguration
            {
                LineSpacing = 8,
                LineColor = Color.FromArgb(210, 180, 140),
                LineThickness = 1,
                WaveFrequency = 0.021,
                WaveAmplitude = 3,
                RandomizationFactor = 1.5,
                EdgeJaggednessLimit = 20,
                JaggednessThreshold = 10
            };

            // Add more default settings as needed
        }

        // Private constructor to prevent instantiation from outside
        internal ImageRegister()
        {
            // Initialization logic
        }

        /// <summary>
        ///     Gets the error log.
        /// </summary>
        /// <value>
        ///     The error log.
        /// </value>
        public Dictionary<DateTime, string> ErrorLog { get; } = new();

        /// <summary>
        ///     Gets the last error.
        /// </summary>
        /// <value>
        ///     The last error.
        /// </value>
        public string LastError => ErrorLog.Values.Last();

        // Public static property to get the instance
        internal static ImageRegister Instance => Settings.Value;

        /// <summary>
        ///     The settings for our Filter
        /// </summary>
        public static ConcurrentDictionary<FiltersType, FiltersConfig> FilterSettings { get; set; } = new();

        /// <summary>
        ///     The texture setting
        /// </summary>
        public static ConcurrentDictionary<TextureType, TextureConfiguration> TextureSetting { get; set; } = new();

        /// <summary>
        ///     Gets or sets the count of retries.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        internal static int Count { get; set; }

        /// <summary>
        ///     Gets the settings.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Return the current config</returns>
        public FiltersConfig GetSettings(FiltersType filter)
        {
            return FilterSettings.TryGetValue(filter, out var config) ? config : new FiltersConfig();
        }

        /// <summary>
        ///     Sets the settings.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="config">The configuration.</param>
        public void SetSettings(FiltersType filter, FiltersConfig config)
        {
            FilterSettings[filter] = config;
        }

        /// <summary>
        ///     Gets the available filters.
        /// </summary>
        /// <returns>List of available Filters</returns>
        public IEnumerable<FiltersType> GetAvailableFilters()
        {
            return FilterSettings.Keys;
        }

        /// <summary>
        ///     Gets the used properties.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>List of properties needed for our Filters</returns>
        public HashSet<string> GetUsedProperties(FiltersType filter)
        {
            return _filterPropertyMap.TryGetValue(filter, out var properties) ? properties : new HashSet<string>();
        }

        /// <summary>
        ///     Gets the settings.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Get the Setting based on Filter</returns>
        public TextureConfiguration GetSettings(TextureType filter)
        {
            return TextureSetting.TryGetValue(filter, out var config) ? config : new TextureConfiguration();
        }

        /// <summary>
        ///     Sets the settings.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="config">The configuration.</param>
        public void SetSettings(TextureType filter, TextureConfiguration config)
        {
            TextureSetting[filter] = config;
        }

        /// <summary>
        ///     Gets the used properties.
        ///     Method to get the used properties for a specific texture type
        /// </summary>
        /// <param name="textureType">Type of the texture.</param>
        /// <returns>List of properties needed for our Textures</returns>
        public HashSet<string> GetUsedProperties(TextureType textureType)
        {
            return _texturePropertyMap.TryGetValue(textureType, out var properties)
                ? properties
                : new HashSet<string>();
        }

        /// <summary>
        ///     Sets the error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void SetError(Exception exception)
        {
            ErrorLog.Add(DateTime.MinValue, exception.Message);
        }

        /// <summary>
        ///     Loads settings from a JSON string.
        /// </summary>
        /// <param name="json">The JSON string containing settings.</param>
        public void LoadSettingsFromJson(string json)
        {
            try
            {
                var settings = JsonSerializer.Deserialize<Dictionary<FiltersType, HashSet<string>>>(json);

                if (settings != null)
                {
                    foreach (var (imageFilters, filter) in settings)
                    {
                        _filterPropertyMap[imageFilters] = filter;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException or JsonException or NotSupportedException)
            {
                ErrorLog.Add(DateTime.MinValue, $"{ImagingResources.ErrorLoadSettings} {ex.Message}");
            }
        }

        /// <summary>
        ///     Retrieves the current settings as a JSON string.
        /// </summary>
        /// <returns>JSON representation of current settings.</returns>
        public string GetSettingsAsJson()
        {
            return JsonSerializer.Serialize(_filterPropertyMap, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
