/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     OpenGler
 * FILE:        Program.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using MinimalRender;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Rays;
using Viewer;

namespace OpenGler;

internal static class Program
{
    private static RasterRaycast _raycaster;
    private static RenderHost _renderHost;
    private static readonly RenderMode _renderMode = RenderMode.RenderHost;


    [STAThread]
    private static void Main()
    {
        switch (_renderMode)
        {
            case RenderMode.Manual:
                RunManualMode();
                break;
            case RenderMode.Raycaster:
                RunRaycasterMode();
                break;
            case RenderMode.RenderHost:
                RunRenderHostMode();
                break;
        }
    }

    private static void RunManualMode()
    {
        var gameSettings = new GameWindowSettings { UpdateFrequency = 60.0 };
        var nativeSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Manual Texture Update Example"
        };

        using var window = new GameWindow(gameSettings, nativeSettings);
        var renderer = new RasterRenderer();

        var width = window.Size.X;
        var height = window.Size.Y;
        var pixels = new byte[width * height * 4];
        var frameCounter = 0;

        window.Load += () => renderer.Initialize(width, height);

        window.Resize += e =>
        {
            width = e.Width;
            height = e.Height;
            pixels = new byte[width * height * 4];
        };

        window.UpdateFrame += args =>
        {
            UpdateImageFromGameLogic(height, width, pixels, ref frameCounter);
            renderer.UpdateTexture(pixels, width, height);
        };

        window.RenderFrame += args =>
        {
            renderer.Render();
            window.SwapBuffers();
        };

        window.Unload += () => renderer.Dispose();
        window.Run();
    }

    private static void RunRaycasterMode()
    {
        var gameSettings = new GameWindowSettings { UpdateFrequency = 60.0 };
        var nativeSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Raycaster Example"
        };

        using var window = new GameWindow(gameSettings, nativeSettings);
        var renderer = new RasterRenderer();

        var width = window.Size.X;
        var height = window.Size.Y;
        var pixels = new byte[width * height * 4];

        window.Load += () =>
        {
            renderer.Initialize(width, height);
            InitiateVRaycaster(width, height);
        };

        window.Resize += e =>
        {
            width = e.Width;
            height = e.Height;
            pixels = new byte[width * height * 4];
            InitiateVRaycaster(width, height);
        };

        window.UpdateFrame += args =>
        {
            Array.Copy(_raycaster.Render().Bytes, pixels, pixels.Length);
            renderer.UpdateTexture(pixels, width, height);
        };

        window.RenderFrame += args =>
        {
            renderer.Render();
            window.SwapBuffers();
        };

        window.Unload += () => renderer.Dispose();
        window.Run();
    }

    private static void RunRenderHostMode()
    {
        int width = 800, height = 600;
        var frameCounter = 0;
        var pixels = new byte[width * height * 4];

        _renderHost = new RenderHost(width, height);

        _renderHost.UpdateLoop = () =>
        {
            UpdateImageFromGameLogic(height, width, pixels, ref frameCounter);
            _renderHost.SetFrame(pixels);
        };

        try
        {
            _renderHost.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception during Run: " + ex);
        }
    }

    private static void InitiateVRaycaster(int width, int height)
    {
        var map = new[,]
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 0, 1 },
            { 1, 0, 0, 0, 1, 0, 1, 0, 1, 1 },
            { 1, 0, 1, 0, 0, 0, 1, 1, 1, 1 },
            { 1, 0, 1, 1, 1, 1, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 1, 0, 1, 0, 1, 1 },
            { 1, 1, 1, 1, 1, 0, 1, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
        };

        var camera = new RvCamera(96, 96, 0);
        var context = new CameraContext(64, height, width);
        _raycaster = new RasterRaycast(map, camera, context);
    }

    private static void UpdateImageFromGameLogic(int height, int width, byte[] pixels, ref int frameCounter)
    {
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
        {
            var i = (y * width + x) * 4;
            pixels[i + 0] = (byte)((x + frameCounter) % 256); // Blue
            pixels[i + 1] = (byte)((y + frameCounter) % 256); // Green
            pixels[i + 2] = 0; // Red
            pixels[i + 3] = 255; // Alpha
        }

        frameCounter++;
    }

    private enum RenderMode
    {
        Manual,
        Raycaster,
        RenderHost
    }
}