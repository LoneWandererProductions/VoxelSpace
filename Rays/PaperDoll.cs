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
    }
}
