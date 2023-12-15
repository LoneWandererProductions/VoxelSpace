/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Main
 * FILE:        Main/MainWindow.cs
 * PURPOSE:     Just the Wpf Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Windows;
using System.Windows.Input;
using Imaging;
using Voxels;

namespace Main
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow
    {
        private VoxelRaster _voxel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Initiate();
        }

        /// <summary>
        /// Initiates this instance.
        /// </summary>
        private void Initiate()
        {
            _voxel = new VoxelRaster(100, 100, 0, 100, 120, 120, 300);

            var bmp = _voxel.Render();
            ImageView.Source = bmp.ToBitmapImage();

            TxtBox.Text = string.Concat(TxtBox.Text, " x: ", _voxel.Camera.X, " y: ", _voxel.Camera.Y, Environment.NewLine);
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _voxel.KeyInput(e.Key);
            var bmp = _voxel.Render();
            ImageView.Source = bmp.ToBitmapImage();

            TxtBox.Text = string.Concat(TxtBox.Text, " x: ", _voxel.Camera.X, " y: ", _voxel.Camera.Y, Environment.NewLine);
        }
    }
}