using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenderEngine;

namespace RenderEngineTests
{
    [TestClass]
    public class LayeredImageContainerTests
    {
        [TestMethod]
        public void Composite_MergesLayersWithAlpha()
        {
            var width = 1;
            var height = 1;

            var container = new LayeredImageContainer(width, height);

            var bottom = new UnmanagedImageBuffer(width, height);
            bottom.SetPixel(0, 0, 255, 0, 0, 255); // fully opaque blue
            container.AddLayer(bottom);

            var top = new UnmanagedImageBuffer(width, height);
            top.SetPixel(0, 0, 128, 255, 0, 0); // 50% transparent red
            container.AddLayer(top);

            var result = container.Composite();
            var pixel = result.BufferSpan.Slice(0, 4).ToArray();

            // Simple blend between blue and red (50% red alpha)
            // R: 50% of 255 = 127.5 => 127 or 128
            // G: 0
            // B: 50% of 255 = 127.5 => 127 or 128
            Assert.IsTrue(pixel[0] >= 127 && pixel[0] <= 128); // B
            Assert.AreEqual(0, pixel[1]);                     // G
            Assert.IsTrue(pixel[2] >= 127 && pixel[2] <= 128); // R
            Assert.AreEqual(255, pixel[3]);                   // A (fully opaque after blend)
        }

        [TestMethod]
        public void AddLayer_ThrowsIfSizeMismatch()
        {
            using var container = new LayeredImageContainer(2, 2);
            using var badLayer = new UnmanagedImageBuffer(1, 1);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                container.AddLayer(badLayer);
            });
        }

        [TestMethod]
        public void AddEmptyLayer_AddsTransparentLayer()
        {
            using var container = new LayeredImageContainer(2, 2);

            var layer = container.AddEmptyLayer();
            var span = layer.BufferSpan;

            for (var i = 0; i < span.Length; i++)
            {
                Assert.AreEqual(0, span[i]);
            }
        }

        [TestMethod]
        public void Composite_ThrowsIfNoLayers()
        {
            using var container = new LayeredImageContainer(2, 2);
            Assert.ThrowsException<InvalidOperationException>(() => container.Composite());
        }
    }
}