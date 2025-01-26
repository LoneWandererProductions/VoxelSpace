using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace RenderEngine
{
    public class SkiaPixelControl : SKElement
    {
        private readonly List<SKRect> _dirtyRegions;
        private SKBitmap _bitmap;
        private Bitmap _gdiBitmap;

        static SkiaPixelControl()
        {
            // Default style for the control (if needed)
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SkiaPixelControl),
                new FrameworkPropertyMetadata(typeof(SkiaPixelControl)));
        }

        public SkiaPixelControl()
        {
            _dirtyRegions = new List<SKRect>();
        }

        public void Initialize(int width, int height)
        {
            _bitmap = new SKBitmap(width, height);
            _gdiBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb); // Set to BGRA format for compatibility
            using var canvas = new SKCanvas(_bitmap);
            canvas.Clear(SKColors.Black);
        }

        public void SetPixel(int x, int y, SKColor color)
        {
            if (_bitmap == null)
            {
                throw new InvalidOperationException("The control must be initialized before setting pixels.");
            }

            _bitmap.SetPixel(x, y, color);

            // Mark the updated region as dirty (so that only relevant areas are repainted)
            _dirtyRegions.Add(new SKRect(x, y, x + 1, y + 1));

            // Trigger a repaint
            InvalidateVisual();
        }

        public SKColor GetPixel(int x, int y)
        {
            if (_bitmap == null)
            {
                throw new InvalidOperationException("The control must be initialized before getting pixels.");
            }

            return _bitmap.GetPixel(x, y);
        }

        public void ChangeAreaOfPixels(int startX, int startY, int width, int height, byte[] pixelData)
        {
            if (_bitmap == null)
            {
                throw new InvalidOperationException("The control must be initialized before changing pixels.");
            }

            // Update the pixels in the area defined by startX, startY, width, height
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var srcIndex = ((y * width) + x) * 4; // BGRA32 format
                    var dstIndex = (((startY + y) * _bitmap.Width) + startX + x) * 4;

                    var color = new SKColor(
                        pixelData[srcIndex + 2], // Red
                        pixelData[srcIndex + 1], // Green
                        pixelData[srcIndex], // Blue
                        pixelData[srcIndex + 3] // Alpha
                    );

                    _bitmap.SetPixel(startX + x, startY + y, color);
                }
            }

            // Mark the updated area as dirty
            _dirtyRegions.Add(new SKRect(startX, startY, startX + width, startY + height));

            // Trigger a repaint
            InvalidateVisual();
        }

        public Bitmap GetGdiBitmap()
        {
            // Convert SkiaSharp Bitmap to System.Drawing Bitmap (BGRA to ARGB conversion)
            for (var y = 0; y < _bitmap.Height; y++)
            {
                for (var x = 0; x < _bitmap.Width; x++)
                {
                    var color = _bitmap.GetPixel(x, y);
                    _gdiBitmap.SetPixel(x, y, Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue));
                }
            }

            return _gdiBitmap;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            if (_bitmap == null)
            {
                return;
            }

            var canvas = e.Surface.Canvas;

            // Clear the canvas
            canvas.Clear(SKColors.Black);

            // Draw the bitmap with dirty region optimizations
            foreach (var region in _dirtyRegions)
            {
                canvas.Save();

                // Calculate the scaling factors to fit the control
                var scaleX = (float)e.Info.Width / _bitmap.Width;
                var scaleY = (float)e.Info.Height / _bitmap.Height;

                // Scale the canvas
                canvas.Scale(scaleX, scaleY);

                // Transform the dirty region to the bitmap's coordinate space
                var scaledRegion = new SKRect(
                    region.Left / scaleX,
                    region.Top / scaleY,
                    region.Right / scaleX,
                    region.Bottom / scaleY
                );

                // Clip to the scaled dirty region
                canvas.ClipRect(region);

                // Draw only the affected region of the bitmap
                canvas.DrawBitmap(_bitmap, scaledRegion, region);

                canvas.Restore();
            }


            // Clear the dirty regions after drawing
            _dirtyRegions.Clear();
        }
    }
}
