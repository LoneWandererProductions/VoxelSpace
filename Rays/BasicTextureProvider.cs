using System.Collections.Generic;
using System.Drawing;
using Imaging;

namespace Rays
{
    internal class BasicTextureProvider : ITextureProvider
    {
        private readonly Dictionary<int, DirectBitmap> _wallTextures;
        private readonly DirectBitmap _defaultWallTexture;

        public BasicTextureProvider(Dictionary<int, DirectBitmap> wallTextures, DirectBitmap? defaultTexture = null)
        {
            _wallTextures = wallTextures ?? new();
            _defaultWallTexture = defaultTexture ?? CreateFallbackTexture();
        }

        public DirectBitmap GetWallTexture(int tileId)
        {
            if (_wallTextures.TryGetValue(tileId, out var bmp))
                return bmp;
            return _defaultWallTexture;
        }

        public DirectBitmap? GetFloorTexture(int tileId)
        {
            // Not implemented yet, stub for future use
            return null;
        }

        public DirectBitmap? GetCeilingTexture(int tileId)
        {
            // Not implemented yet, stub for future use
            return null;
        }

        private static DirectBitmap CreateFallbackTexture()
        {
            var bmp = new DirectBitmap(64, 64, Color.Magenta);
            for (var y = 0; y < 64; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    var isDark = (x / 8 % 2 == y / 8 % 2);
                    bmp.SetPixel(x, y, isDark ? Color.Magenta : Color.Black);
                }
            }
            return bmp;
        }
    }
}