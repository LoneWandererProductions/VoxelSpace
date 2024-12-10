/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        RenderEngine/RenderView.xaml.cs
 * PURPOSE:     Frontend for our drawing Engine. I must admit I had no clue about Skia, but chatGPT helped a lot.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedType.Global

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Mathematics;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RenderEngine
{
    /// <summary>
    ///     Our Render Control
    /// </summary>
    /// <seealso cref="UserControl" />
    /// <seealso cref="IComponentConnector" />
    /// <inheritdoc cref="UserControl" />
    public sealed partial class RenderView
    {
        /// <summary>
        ///     The Canvas clicked Delegate
        /// </summary>
        /// <param name="coordinate">The coordinate.</param>
        public delegate void DelegateCoordinate(Coordinate2D coordinate);

        /// <summary>
        ///     The bitmap
        /// </summary>
        private SKBitmap _bitmap;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:RenderEngine.RenderView" /> class.
        ///     C an't be used until Window is completely loaded!
        /// </summary>
        public RenderView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="RenderView" /> is debug.
        /// </summary>
        /// <value>
        ///     <c>true</c> if debug; otherwise, <c>false</c>.
        /// </value>
        public bool Debug
        {
            get => RenderRegister.Debug;
            set => RenderRegister.Debug = value;
        }

        /// <summary>
        ///     Occurs when [canvas clicked].
        /// </summary>
        public event DelegateCoordinate CanvasClicked;

        /// <summary>
        ///     Called when [paint surface].
        ///     Won't be called until Form is completely loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SKPaintSurfaceEventArgs" /> instance containing the event data.</param>
        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            _bitmap ??= new SKBitmap(e.Info.Width, e.Info.Height);

            CheckGpuUsage(e.Surface);

            e.Surface.Canvas.DrawBitmap(_bitmap, 0, 0);

            SkiaElement.InvalidateVisual();
        }

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            if (_bitmap == null) return;

            using var canvas = new SKCanvas(_bitmap);
            canvas.Clear(SKColors.White);

            SkiaElement.InvalidateVisual();
        }

        /// <summary>
        ///     Draws the specified objects.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <param name="style">The style.</param>
        /// <param name="clear">if set to <c>true</c> [clear].</param>
        public void Draw(IEnumerable<object> objects, GraphicStyle style, bool clear)
        {
            Clear();

            foreach (var item in objects)
                switch (item)
                {
                    case Polygons polygons:
                        DrawShape(polygons, style, clear);
                        break;
                    case Rectangular rectangular:
                        DrawShape(rectangular, style, clear);
                        break;
                    case CubicBezierCurve curve:
                        DrawShape(curve, style, clear);
                        break;
                    case Circle circle:
                        DrawShape(circle, style, clear);
                        break;
                }
        }

        /// <summary>
        ///     Draws the shape.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="shape">The shape.</param>
        /// <param name="style">The style.</param>
        /// <param name="clear">if set to <c>true</c> [clear].</param>
        public void DrawShape<T>(T shape, GraphicStyle style, bool clear) where T : IDrawable
        {
            using var paint = new SKPaint { Color = shape.Color };
            using var canvas = new SKCanvas(_bitmap);

            if (clear) canvas.Clear(SKColors.White);

            paint.Color = shape.Color;

            switch (style)
            {
                case GraphicStyle.Mesh:
                    paint.Style = SKPaintStyle.Stroke;
                    break;
                case GraphicStyle.Fill:
                    paint.Style = SKPaintStyle.Fill;
                    break;
                case GraphicStyle.Plot:
                    RenderHelper.DrawPoint(canvas, shape.Start, paint);
                    shape.Draw(canvas, paint, style);
                    break;
            }

            if (style != GraphicStyle.Plot) shape.Draw(canvas, paint, style);

            SkiaElement.InvalidateVisual();
        }

        /// <summary>
        ///     Draws the dot.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="clear">if set to <c>true</c> [clear].</param>
        public void DrawDot(Coordinate2D point, bool clear)
        {
            using var paint = new SKPaint();
            using var canvas = new SKCanvas(_bitmap);

            if (clear) canvas.Clear(SKColors.White);

            RenderHelper.DrawPoint(canvas, point, paint);

            SkiaElement.InvalidateVisual();
        }

        /// <summary>
        ///     Draws the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="clear">if set to <c>true</c> [clear].</param>
        public void DrawLine(Line line, bool clear)
        {
            using var paint = new SKPaint { Color = line.Color };
            using var canvas = new SKCanvas(_bitmap);

            if (clear) canvas.Clear(SKColors.White);

            using var fillPaint = new SKPaint { Style = SKPaintStyle.Fill };

            var startPoint = new SKPoint(line.Start.X, line.Start.Y);
            var endPoint = new SKPoint(line.EndPoint.X, line.EndPoint.Y);

            canvas.DrawLine(startPoint, endPoint, paint);

            SkiaElement.InvalidateVisual();
        }

        /// <summary>
        ///     Gets the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Color of the Pixel</returns>
        public SKColor GetPixel(int x, int y)
        {
            return _bitmap != null && x >= 0 && x < _bitmap.Width && y >= 0 && y < _bitmap.Height
                ? _bitmap.GetPixel(x, y)
                : SKColors.Transparent;
        }

        /// <summary>
        ///     Sets the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        public void SetPixel(int x, int y, SKColor color)
        {
            if (_bitmap != null && x >= 0 && x < _bitmap.Width && y >= 0 && y < _bitmap.Height)
            {
                _bitmap.SetPixel(x, y, color);
                SkiaElement.InvalidateVisual();
            }
        }

        /// <summary>
        ///     Handles the MouseDown event of the SKcanvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void SKcanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(SkiaElement);

            var cursor = new Coordinate2D(position.X, position.Y);

            CanvasClicked?.Invoke(cursor);
        }

        // Optional: Check GPU usage and report it (based on SkiaSharp OpenGL/Vulkan capabilities)
        public void CheckGpuUsage(SKSurface surface)
        {
            var gpuUsed = surface.Handle != IntPtr.Zero; // Simple GPU check
            if (gpuUsed) System.Diagnostics.Debug.WriteLine("GPU is being used!");
        }
    }
}