using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using RenderEngine;
using Voxels; // your Raycaster namespace

public class MainGame : GameWindow
{
    private RasterRenderer _renderer;
    private Raycaster _raycaster;

    public MainGame(GameWindowSettings gws, NativeWindowSettings nws)
        : base(gws, nws) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(Color.Black);

        int screenWidth = 320;
        int screenHeight = 200;

        _renderer = new RasterRenderer();
        _renderer.Initialize(screenWidth, screenHeight);

        // Basic test map
        int[,] map = new int[8, 8]
        {
            {1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,1},
            {1,0,1,0,1,0,0,1},
            {1,0,1,0,1,0,0,1},
            {1,0,0,0,0,0,0,1},
            {1,0,0,1,0,1,0,1},
            {1,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1}
        };

        var context = new CameraContext
        {
            ScreenWidth = screenWidth,
            ScreenHeight = screenHeight,
            Fov = 60,
            Distance = 10,
            CellSize = 1
        };

        var camera = new RvCamera
        {
            X = 4,
            Y = 4,
            Z = 5,
            Angle = 0
        };

        _raycaster = new Raycaster(map, context);
        _camera = camera;
    }

    private RvCamera _camera;

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        // Turn slightly each frame
        _camera.Angle += 5;

        using Raycaster.RenderResult result = _raycaster.Render(_camera);
        byte[] pixelData = GetBitmapPixels(result.Bitmap);

        _renderer.UpdateTexture(pixelData, result.Bitmap.Width, result.Bitmap.Height);
        _renderer.Render();

        SwapBuffers();
    }

    private byte[] GetBitmapPixels(Bitmap bitmap)
    {
        BitmapData data = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        int byteCount = Math.Abs(data.Stride) * data.Height;
        byte[] pixels = new byte[byteCount];
        System.Runtime.InteropServices.Marshal.Copy(data.Scan0, pixels, 0, byteCount);

        bitmap.UnlockBits(data);

        // Convert ARGB to RGBA for OpenGL
        for (int i = 0; i < pixels.Length; i += 4)
        {
            byte a = pixels[i + 3];
            byte r = pixels[i + 2];
            byte g = pixels[i + 1];
            byte b = pixels[i + 0];

            pixels[i + 0] = r;
            pixels[i + 1] = g;
            pixels[i + 2] = b;
            pixels[i + 3] = a;
        }

        return pixels;
    }
}
