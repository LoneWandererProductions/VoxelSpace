using System.Drawing;
using Imaging;
using Viewer;

namespace Rays
{
    public sealed class FlatFloorCeilingRenderer : IFloorCeilingRenderer
    {
        public void Render(DirectBitmap dbm, RvCamera camera, CameraContext context)
        {
            var screenHeight = context.ScreenHeight;
            var screenWidth = context.ScreenWidth;

            var floorColor = Color.DarkSlateGray;
            var ceilingColor = Color.DarkBlue;

            for (var y = 0; y < screenHeight / 2; y++)
            {
                var factor = (double)y / (screenHeight / 2);
                var r = (int)(ceilingColor.R * (1 - factor));
                var g = (int)(ceilingColor.G * (1 - factor));
                var b = (int)(ceilingColor.B * (1 - factor));
                var color = Color.FromArgb(r, g, b);

                for (var x = 0; x < screenWidth; x++)
                    dbm.SetPixel(x, y, color);
            }

            for (var y = screenHeight / 2; y < screenHeight; y++)
            {
                var factor = (double)(y - screenHeight / 2) / (screenHeight / 2);
                var r = (int)(floorColor.R * factor);
                var g = (int)(floorColor.G * factor);
                var b = (int)(floorColor.B * factor);
                var color = Color.FromArgb(r, g, b);

                for (var x = 0; x < screenWidth; x++)
                    dbm.SetPixel(x, y, color);
            }
        }
    }

}