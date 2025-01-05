using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace CommonViewer
{
    public sealed class NativeBitmapDisplay : WindowsFormsHost
    {
        private readonly PictureBox _pictureBox;

        public NativeBitmapDisplay()
        {
            _pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = System.Drawing.Color.Transparent
            };

            Child = _pictureBox;
        }

        public static readonly DependencyProperty BitmapProperty =
            DependencyProperty.Register(
                nameof(Bitmap),
                typeof(Bitmap),
                typeof(NativeBitmapDisplay),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnBitmapChanged));

        public Bitmap? Bitmap
        {
            get => (Bitmap?)GetValue(BitmapProperty);
            set => SetValue(BitmapProperty, value);
        }

        private static void OnBitmapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NativeBitmapDisplay control && e.NewValue is Bitmap bitmap)
            {
                // Dispose of previous image to prevent memory leaks
                control._pictureBox.Image?.Dispose();
                control._pictureBox.Image = bitmap;
            }
        }

        // Dispose method to clean up resources when the control is no longer needed
        public void Dispose()
        {
            _pictureBox.Image?.Dispose();
            _pictureBox.Dispose();
        }
    }
}