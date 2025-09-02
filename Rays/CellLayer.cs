using RenderEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rays
{
    /// <summary>
    /// Represents a decorative or functional layer within a map cell,
    /// such as water, grass, lava, fog, etc.
    /// </summary>
    public class CellLayer
    {
        public string Name { get; set; } = "Layer";   // e.g., "Water", "Grass"

        /// <summary>
        /// Height above the floor at which this layer sits.
        /// Example: Water surface = 0.2f if floor is 0.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Simple solid color fallback if no texture is given.
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Optional mask texture for transparency or detail.
        /// </summary>
        public UnmanagedImageBuffer? Mask { get; set; }

        /// <summary>
        /// Whether this layer should alpha-blend with others.
        /// </summary>
        public bool AlphaBlend { get; set; } = true;

        /// <summary>
        /// Optional custom draw logic for advanced rendering.
        /// </summary>
        public Action<IRenderer, Point, UnmanagedImageBuffer>? CustomDraw { get; set; }
    }
}
