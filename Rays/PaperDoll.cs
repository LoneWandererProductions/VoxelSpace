using RenderEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rays
{
    /// <summary>
    /// Represents a paper doll character.
    /// </summary>
    public class PaperDoll
    {
        public Vector3 Position { get; set; }          // World position
        public UnmanagedImageBuffer? Sprite { get; set; } // 2D sprite texture

        /// <summary>
        /// Approximate world radius for frustum culling.
        /// Adjust if sprites are very wide/tall.
        /// </summary>
        public float Radius { get; set; } = 0.5f;
    }
}
