using RenderEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rays
{
    public class CellLayer
    {
        public string Name;                 // e.g., "Water", "Grass"
        public float Height;                // relative to floor
        public Color Color = Color.White;   // simple solid color fallback
        public UnmanagedImageBuffer? Mask;  // optional alpha pattern
        public bool AlphaBlend = true;      // whether to blend
        public Action<IRenderer, Point, UnmanagedImageBuffer>? CustomDraw; // optional custom logic
    }
}
