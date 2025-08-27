/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngineTests
 * FILE:        LayeredImagePerformanceTests.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenderEngine;

namespace RenderEngineTests;

[TestClass]
public class LayeredImagePerformanceTests
{
    [TestMethod]
    public void Composite_PerformanceTest()
    {
        const int width = 1024;
        const int height = 1024;
        const int layers = 10;
        const int iterations = 50;

        var bufferSize = width * height * 4;
        var tempBuffer = new byte[bufferSize];

        // Warm-up
        FillBuffer(tempBuffer, 0);
        RunOnce(width, height, layers, tempBuffer);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var memBefore = GC.GetTotalMemory(true);
        var sw = Stopwatch.StartNew();

        for (var i = 0; i < iterations; i++)
        {
            FillBuffer(tempBuffer, i); // vary the fill per iteration
            RunOnce(width, height, layers, tempBuffer);
        }

        sw.Stop();
        var memAfter = GC.GetTotalMemory(true);

        Trace.WriteLine($"Composite Time: {sw.ElapsedMilliseconds} ms");
        Trace.WriteLine($"Average Time: {sw.Elapsed.TotalMilliseconds / iterations:F4} ms");
        Trace.WriteLine($"Memory Allocated: {(memAfter - memBefore) / 1024.0:F2} KB");
    }

    private static void RunOnce(int width, int height, int layers, byte[] tempBuffer)
    {
        using var container = new LayeredImageContainer(width, height);

        for (var i = 0; i < layers; i++)
        {
            var layer = new UnmanagedImageBuffer(width, height);
            layer.ReplaceBuffer(tempBuffer); // reuse the filled buffer
            container.AddLayer(layer);
        }

        using var result = container.Composite();
    }

    private static void FillBuffer(byte[] buffer, int seed)
    {
        // Basic pseudorandom fill based on iteration/seed
        for (var i = 0; i < buffer.Length; i += 4)
        {
            buffer[i + 0] = (byte)((seed * 13 + i) % 256); // B
            buffer[i + 1] = (byte)(255 - (seed * 7 + i) % 256); // G
            buffer[i + 2] = (byte)((seed * 3 + i) % 256); // R
            buffer[i + 3] = (byte)((seed * 5 + 128 + i) % 256); // A
        }
    }
}