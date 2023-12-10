using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Imaging;

namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly VoxelEngine.VoxelEngine _voxel = new();

        public MainWindow()
        {
            InitializeComponent();
            Initiate();
        }

        private void Initiate()
        {
            var bmp = _voxel.Render(new PointF(100, 100), 0, 100, 120, 120, 300);
            ImageView.Source = bmp.ToBitmapImage();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //TODO
            //https://stackoverflow.com/questions/31220870/how-do-i-create-keyboard-input-in-a-wpf-application

            if (e.Key == Key.Enter)
                //Process user input
                e.Handled = true;
        }
    }

}
