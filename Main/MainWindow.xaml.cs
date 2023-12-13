using System.Windows.Input;
using Imaging;
using Voxels;

namespace Main
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private VoxelRaster _voxel;

        public MainWindow()
        {
            InitializeComponent();
            Initiate();
        }

        private void Initiate()
        {
            _voxel = new VoxelRaster(100, 100, 0, 100, 120, 120, 300);

            var bmp = _voxel.Render();
            ImageView.Source = bmp.ToBitmapImage();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _voxel.KeyInput(e.Key);
            var bmp = _voxel.Render();
            ImageView.Source = bmp.ToBitmapImage();
        }
    }
}