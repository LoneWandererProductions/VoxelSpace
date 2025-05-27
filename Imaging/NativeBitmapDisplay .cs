/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/NativeBitmapDisplay.cs
 * PURPOSE:     Fast Bitmap Display in Wpf do not expect wonders though.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MissingSpace
// ReSharper disable UnusedMember.Global

#nullable enable
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Imaging
{
    /// <inheritdoc />
    /// <summary>
    ///     Fast Bitmap Viewer for Wpf applications.
    /// </summary>
    /// <seealso cref="T:System.Windows.Forms.Integration.WindowsFormsHost" />
    public sealed class NativeBitmapDisplay : WindowsFormsHost
    {
        /// <summary>
        ///     The bitmap property
        /// </summary>
        public static readonly DependencyProperty BitmapProperty =
            DependencyProperty.Register(
                nameof(Bitmap),
                typeof(Bitmap),
                typeof(NativeBitmapDisplay),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnBitmapChanged));

        private readonly PictureBox _pictureBox;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Imaging.NativeBitmapDisplay" /> class.
        /// </summary>
        public NativeBitmapDisplay()
        {
            _pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent
            };
            EnableDoubleBuffering(_pictureBox);
            Child = _pictureBox;
        }

        /// <summary>
        ///     Gets or sets the bitmap.
        /// </summary>
        /// <value>
        ///     The bitmap.
        /// </value>
        public Bitmap? Bitmap
        {
            get => (Bitmap?)GetValue(BitmapProperty);
            set => SetValue(BitmapProperty, value);
        }

        /// <summary>
        ///     Enables the double buffering.
        /// </summary>
        /// <param name="pictureBox">The picture box.</param>
        private static void EnableDoubleBuffering(PictureBox pictureBox)
        {
            var doubleBufferProperty =
                typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            doubleBufferProperty?.SetValue(pictureBox, true, null);
        }

        /// <summary>
        ///     Refreshes the bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public void RefreshBitmap(Bitmap bitmap)
        {
            _pictureBox.Image?.Dispose();
            _pictureBox.Image = bitmap;
        }

        /// <summary>
        ///     Called when [bitmap changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnBitmapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not NativeBitmapDisplay control || e.NewValue is not Bitmap newBitmap) return;

            // Ensure we are on the UI thread before updating the image
            if (control.Dispatcher.CheckAccess())
                UpdateImage(control, newBitmap);
            else
                control.Dispatcher.Invoke(() => UpdateImage(control, newBitmap));
        }

        /// <summary>
        ///     Updates the image.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="newBitmap">The new bitmap.</param>
        private static void UpdateImage(NativeBitmapDisplay control, Bitmap newBitmap)
        {
            if (ReferenceEquals(control._pictureBox.Image, newBitmap)) return;

            if (control._pictureBox.Image is Bitmap oldBitmap) oldBitmap.Dispose();

            control._pictureBox.Image = newBitmap.Clone() as Bitmap; // Clone to ensure safe assignment
        }

        /// <inheritdoc />
        /// <summary>
        ///     Immediately frees any system resources that the object might hold.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pictureBox.Image?.Dispose();
                _pictureBox.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}