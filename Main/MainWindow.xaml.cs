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
        private string _active;
        private RasterRaycast _raycaster;
        private VoxelRaster _voxel;

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
            RvCamera camera;

            switch (_active)
            {
                case "Raycast":
                    // Add logic for Raycaster
                    _active = "Raycast";
                    if (_raycaster == null) return;

                    bmp = _raycaster.Render(e.Key);
                    ImageView.Bitmap = bmp;
                    var camera6 = _raycaster.Camera;

                    TxtBox.Text = string.Concat(TxtBox.Text, " Time Diff:", timer.Elapsed, Environment.NewLine);
                    TxtBox.Text = string.Concat(TxtBox.Text, camera6.ToString(),
                        Environment.NewLine);
                    TxtBox.ScrollToEnd();
                    break;
                case "Voxel":
                    if (_voxel == null) return;

                    bmp = _voxel.GetBitmapForKey(e.Key);
                    ImageView.Bitmap = bmp;
                    camera = _voxel.Camera;


                    TxtBox.Text = string.Concat(TxtBox.Text, " Time Diff:", timer.Elapsed, Environment.NewLine);
                    TxtBox.Text = string.Concat(TxtBox.Text, camera.ToString(),
                        Environment.NewLine);
                    TxtBox.ScrollToEnd();

                    break;
                case "Hybrid":
                    break;
                default:
                    // Handle unexpected cases if needed
                    return;
            }

            timer.Stop();
        }

        /// <summary>
        ///     Handles the SelectionChanged event of the comboBoxRender control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void comboBoxRender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxRender.SelectedItem is ComboBoxItem selectedItem)
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
                    case "Hybrid":
                        // Add logic for Voxel
                        _active = "Hybrid";
                        InitiateHybrid();
                        break;
                }
        }

        private void InitiateVRaycaster()
        {
            // Simple map where 1 is a wall and 0 is empty space
            var map = new int[10, 10]
            {
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0, 0, 1 },
                { 1, 0, 0, 0, 1, 0, 1, 0, 1, 1 },
                { 1, 0, 1, 0, 0, 0, 1, 1, 1, 1 },
                { 1, 0, 1, 1, 1, 1, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 1, 0, 1, 0, 1, 1 },
                { 1, 1, 1, 1, 1, 0, 1, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            // Set up a camera
            var camera = new RvCamera(96, 96, 0); // Position and angle of the camera
            //setup the context
            CameraContext context = new(64, 600, 800);

            // Create Raycaster and render
            _raycaster = new RasterRaycast(map, camera, context);
            var bmp = _raycaster.Render();
            ImageView.Bitmap = bmp;
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

            _voxel = new VoxelRaster(100, 100, 0, 100, 120, 120, 300, colorMap, heightMap, 200, 300);

            var bmp = _voxel.StartEngine();
            ImageView.Bitmap = bmp;

            TxtBox.Text = string.Concat(TxtBox.Text, " x: ", _voxel.Camera.X, " y: ", _voxel.Camera.Y,
                Environment.NewLine);
            TxtBox.ScrollToEnd();
        }

        private void InitiateHybrid()
        {
            // Set up the camera context and Raycaster
            var context = new CameraContext
            {
                ScreenWidth = 800,
                ScreenHeight = 600,
                Fov = 60,
                Distance = 15,
                CellSize = 1,
                Scale = 100
                //Horizon = 200
            };

            // Generate the 2D map (height map) for the raycaster
            var mapWidth = 40;
            var mapHeight = 40;
            var map = new int[mapHeight, mapWidth];

            // Randomly assign some walls (1) and open spaces (0)
            var rand = new Random();
            for (var y = 0; y < mapHeight; y++)
            for (var x = 0; x < mapWidth; x++)
                map[y, x] = rand.Next(0, 3); // 0 = empty, 1 = wall, 2 = different type of wall

            // Create the voxel height map and color map
            var heightMap = new int[mapHeight, mapWidth];
            var colorMap = new Color[mapHeight, mapWidth];

            // Generate height and color for each voxel in the map
            for (var y = 0; y < mapHeight; y++)
            for (var x = 0; x < mapWidth; x++)
            {
                heightMap[y, x] = rand.Next(1, 10); // Random height between 1 and 10
                colorMap[y, x] = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255)); // Random color
            }

            // Set up the camera
            var camera = new RvCamera
            {
                X = 20,
                Y = 20,
                Z = 5,
                Angle = 0,
                Pitch = 0
            };

            // Initialize the VoxelRaster3D and Raycaster
            var voxelRaster3D =
                new VoxelRaster3D(context, colorMap, heightMap, mapWidth, mapHeight, mapWidth, mapHeight);
            var raycaster = new Raycaster(map, context);

            // Render the voxel map with the color and height map using the VoxelRaster3D
            var voxelRenderedBitmap = voxelRaster3D.RenderWithContainer(camera);

            // Render the map with raycasting
            var raycastedBitmap = raycaster.Render(camera);

            // Show the images (you can save or display them depending on your setup)
            voxelRenderedBitmap.Save("VoxelRendered.png");
            raycastedBitmap.Save("Raycasted.png");
        }
    }
}