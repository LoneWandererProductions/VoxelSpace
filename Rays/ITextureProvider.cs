using Imaging;

namespace Rays
{
    public interface ITextureProvider
    {
        DirectBitmap GetWallTexture(int tileId);
        DirectBitmap? GetFloorTexture(int tileId);
        DirectBitmap? GetCeilingTexture(int tileId);
    }

}