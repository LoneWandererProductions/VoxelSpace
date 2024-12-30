/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Main
 * FILE:        Main/MainWindow.cs
 * PURPOSE:     Just the Wpf Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Imaging;
using Voxels;
using Image = System.Drawing.Image;

namespace Main
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow
    {
        private VoxelRaster _voxel;
        private string _active;
        private RasterRaycast _raycaster;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Handles the PreviewKeyDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var timer = new Stopwatch();

            timer.Start();

            Bitmap bmp;
            RVCamera camera;

            switch (_active)
            {
                case "Raycast":
                    // Add logic for Raycaster
                    _active = "Raycast";
                    if (_raycaster == null) return;

                    bmp = _raycaster.Render(e.Key);
                    ImageView.Source = bmp.ToBitmapImage();
                    var camera6 = _raycaster.Camera;

                    TxtBox.Text = string.Concat(TxtBox.Text, " Time Diff:", timer.Elapsed, Environment.NewLine);
                    TxtBox.Text = string.Concat(TxtBox.Text, camera6.ToString(),
                        Environment.NewLine);
                    break;
                case "Voxel":
                    if (_voxel == null) return;

                    bmp = _voxel.GetBitmapForKey(e.Key);
                    ImageView.Source = bmp.ToBitmapImage();
                    camera = _voxel.Camera;


                    TxtBox.Text = string.Concat(TxtBox.Text, " Time Diff:", timer.Elapsed, Environment.NewLine);
                    TxtBox.Text = string.Concat(TxtBox.Text, camera.ToString(),
                        Environment.NewLine);
                    break;
                default:
                    // Handle unexpected cases if needed
                    return;
            }

            timer.Stop();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the comboBoxRender control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void comboBoxRender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxRender.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "Raycast":
                        // Add logic for Raycaster
                        _active = "Raycast";
                        InitiateVRaycaster();
                        break;
                    case "Voxel":
                        // Add logic for Voxel
                        _active = "Voxel";
                        InitiateVoxel();
                        break;
                }
            }
        }

        private void InitiateVRaycaster()
        {
            // Simple map where 1 is a wall and 0 is empty space
            int[,] map = new int[10, 10]
            {
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                {1, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                {1, 0, 1, 0, 1, 0, 1, 0, 0, 1},
                {1, 0, 1, 0, 1, 0, 1, 0, 0, 1},
                {1, 0, 0, 0, 1, 0, 1, 0, 1, 1},
                {1, 0, 1, 0, 0, 0, 1, 1, 1, 1},
                {1, 0, 1, 1, 1, 1, 0, 0, 0, 1},
                {1, 0, 0, 0, 1, 0, 1, 0, 1, 1},
                {1, 1, 1, 1, 1, 0, 1, 0, 0, 1},
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
            };

            // Set up a camera
            var camera = new RVCamera(96, 96, 60, 0);  // Position and angle of the camera
            //setup the context
            CameraContext context =new (64, 800, 600);

            // Create Raycaster and render
            _raycaster = new RasterRaycast(map, camera, context);
            var bmp = _raycaster.Render();
            ImageView.Source = bmp.ToBitmapImage();
        }

        /// <summary>
        ///     Initiates this instance.
        /// </summary>
        private void InitiateVoxel()
        {
            var colorMap =
                new Bitmap(Image.FromFile(string.Concat(Directory.GetCurrentDirectory(), "\\Terrain\\C1W.png")));
            var heightMap =
                new Bitmap(Image.FromFile(string.Concat(Directory.GetCurrentDirectory(), "\\Terrain\\D1.png")));

            _voxel = new VoxelRaster(100, 100, 0, 100, 120, 120, 300, colorMap, heightMap, 300, 200);

            var bmp = _voxel.StartEngine();
            ImageView.Source = bmp.ToBitmapImage();

            TxtBox.Text = string.Concat(TxtBox.Text, " x: ", _voxel.Camera.X, " y: ", _voxel.Camera.Y,
                Environment.NewLine);
        }
    }
}