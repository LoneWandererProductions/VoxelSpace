using System.Drawing;

namespace Voxels
{
    internal sealed class Slice
    {
        internal Color Shade { get; init; }

        internal int X1 { get; init; }

        internal int Y1 { get; init; }

        internal float Y2 { get; init; }
    }
}