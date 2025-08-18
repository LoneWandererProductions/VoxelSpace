/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Main
 * FILE:        Main/MainWindow.cs
 * PURPOSE:     Just the Wpf Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Rays;
using RenderEngine;
using Viewer;
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
        private readonly DispatcherTimer _keyRepeatTimer;

        private readonly HashSet<Key> _pressedKeys = new();
        private readonly Stopwatch _timer = new();
        private string _active;
        private RasterRaycast _raycaster;
        private RasterRaycastV2 _raycasterV2;
        private VoxelRaster _voxel;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Main.MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _keyRepeatTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _keyRepeatTimer.Tick += KeyRepeatTimer_Tick;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_pressedKeys.Contains(e.Key)) return; // Ignore if already pressed

            _pressedKeys.Add(e.Key);

            HandleKeyAction(e.Key); // Handle the action when key is first pressed

            if (_pressedKeys.Count == 1) // Start the timer once any key is pressed
            {
                _timer.Start();
                _keyRepeatTimer.Start();
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            _pressedKeys.Remove(e.Key);
            if (_pressedKeys.Count == 0) // Stop the timer when no keys are pressed
            {
                _keyRepeatTimer.Stop();
                _timer.Stop();
            }
        }

        private void KeyRepeatTimer_Tick(object sender, EventArgs e)
        {
            // Handle repeated actions when a key is held down
            foreach (var key in _pressedKeys) HandleKeyAction(key);
        }

        private void HandleKeyAction(Key key)
        {
            Bitmap bmp = null;
            RvCamera camera = null;

            // Measure full execution time
            var stopwatchFull = Stopwatch.StartNew();

            switch (_active)
            {
                case "Raycast":
                    if (_raycaster == null) return;

                    // Measure image rendering time
                    bmp = _raycaster.Render(key);

                    camera = _raycaster.Camera;
                    break;

                case "RaycastV2":
                    if (_raycasterV2 == null) return;

                    // Measure image rendering time
                    bmp = _raycasterV2.Render(key);

                    camera = _raycasterV2.Camera;
                    break;

                case "Voxel":
                    if (_voxel == null) return;

                    bmp = _voxel.GetBitmapForKey(key);

                    camera = _voxel.Camera;
                    break;

                case "Hybrid":
                    // Handle hybrid logic
                    break;

                case "TileGL":
                    _active = "TileGL";
                    InitiateTileGL();
                    break;

                default:
                    return;
            }

            stopwatchFull.Stop();

            // Update UI with timing details
            UpdateTextBox(camera?.ToString(), stopwatchFull.ElapsedMilliseconds);
            ImageView.Bitmap = bmp ?? new Bitmap(1, 1); // Use placeholder if bmp is null
        }

        /// <summary>
        ///     Updates the TextBox with the specified content and elapsed times.
        /// </summary>
        /// <param name="message">The message to append to the TextBox.</param>
        /// <param name="stopwatchFullElapsedMilliseconds">The total elapsed time in milliseconds.</param>
        private void UpdateTextBox(string message, long stopwatchFullElapsedMilliseconds)
        {
            if (_voxel == null) return;

            var formattedMessage = $"{message}{Environment.NewLine}" +
                                   $"Total Time: {stopwatchFullElapsedMilliseconds} ms{Environment.NewLine}" +
                                   $"Render Time: {_voxel.ImageRender} ms{Environment.NewLine}" +
                                   $"Camera Time: {_voxel.CameraTime} ms{Environment.NewLine}";
            TxtBox.Text = string.Concat(TxtBox.Text, formattedMessage);
            TxtBox.ScrollToEnd();
        }

        private void comboBoxRender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxRender.SelectedItem is ComboBoxItem selectedItem)
                switch (selectedItem.Content.ToString())
                {
                    case "Raycast":
                        _active = "Raycast";
                        InitiateVRaycaster();
                        break;

                    case "RaycastV2":
                        _active = "RaycastV2";
                        InitiateVRaycasterV2();
                        break;
                    case "Voxel":
                        _active = "Voxel";
                        InitiateVoxel();
                        break;

                    case "Hybrid":
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


            var maps = new MapCell[10, 10];
            for (var y = 0; y < 10; y++)
            for (var x = 0; x < 10; x++)
                maps[y, x] = new MapCell
                {
                    WallId = map[y, x],
                    FloorId = 1, // Default floor tile ID
                    CeilingId = 1 // Default ceiling tile ID
                };

            // Set up a camera
            var camera = new RvCamera(96, 96, 0); // Position and angle of the camera
            //setup the context
            CameraContext context = new(64, 600, 800);

            // Create Raycaster and render
            _raycaster = new RasterRaycast(map, camera, context);
            var result = _raycaster.Render();
            ImageView.Bitmap = result.Bitmap;
        }

        private void InitiateVRaycasterV2()
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


            var maps = new MapCell[10, 10];
            for (var y = 0; y < 10; y++)
            for (var x = 0; x < 10; x++)
                maps[y, x] = new MapCell
                {
                    WallId = map[y, x],
                    FloorId = 1, // Default floor tile ID
                    CeilingId = 1 // Default ceiling tile ID
                };

            // Set up a camera
            var camera = new RvCamera(96, 96, 0); // Position and angle of the camera
            //setup the context
            CameraContext context = new(64, 600, 800);

            // Create Raycaster and render
            _raycasterV2 = new RasterRaycastV2(maps, camera, context, null);
            var result = _raycasterV2.Render();
            ImageView.Bitmap = result.Bitmap;
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

            ImageView.Bitmap = _voxel.StartEngine();

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
            raycastedBitmap.Bitmap.Save("Raycasted.png");
        }

        private void InitiateTileGL()
        {
            var camera = new RvCamera(0, 0, 0);
            var context = new CameraContext(64, 600, 800); // Match your resolution and tile size

            // Call your OpenGL rendering method and get the result as UnmanagedImageBuffer
            var buffer = RenderSkybox.RenderSkyboxModelSingleTile(context.AspectRatio, camera);

            // Convert to Bitmap and display
            ImageView.Bitmap = buffer.ToBitmap();

            TxtBox.Text = $"TileGL Rendered - Camera: {camera}\n";
            TxtBox.ScrollToEnd();
        }
    }
}