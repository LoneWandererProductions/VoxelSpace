namespace Voxels
{
    using System;
    using System.Collections.Generic;
    using System.Drawing; // Assuming this is used for Color.

    public class PixelContainer
    {
        private readonly Dictionary<ValueTuple<int, int>, Color> _pixels;

        public PixelContainer(int capacity)
        {
            // Preallocate capacity
            _pixels = new Dictionary<ValueTuple<int, int>, Color>(capacity);
        }

        public void AddPixel(int x, int y, Color color)
        {
            _pixels[(x, y)] = color; // Directly use ValueTuple<int, int> as key.
        }

        public Color? GetPixel(int x, int y)
        {
            return _pixels.TryGetValue((x, y), out var color) ? color : (Color?)null;
        }

        public Dictionary<ValueTuple<int, int>, Color> GetAllPixels()
        {
            return _pixels;
        }
    }

}