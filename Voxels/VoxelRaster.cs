/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Voxels
 * FILE:        VoxelRaster.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Imaging;
using Viewer;

namespace Voxels;

public sealed class VoxelRaster : IDisposable
{
    private static readonly List<Task> PendingTasks = new(); // To track pending tasks

    /// <summary>
    ///     The cache preload thread
    /// </summary>
    private readonly Thread _cachePreloadThread;

    /// <summary>
    ///     The cancellation token source
    /// </summary>
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly Key[] _directionKey;

    private readonly ConcurrentDictionary<Key, bool> _isImageBeingGenerated = new();

    private readonly ConcurrentDictionary<Key, Bitmap> _lazyCache;

    private readonly object _lock = new();

    private readonly VoxelRaster3D _raster;
    private Dictionary<int, Color> _colorDictionary;

    /// <summary>
    ///     The color height
    /// </summary>
    private int _colorHeight;

    /// <summary>
    ///     The color map
    ///     Buffer/array to hold color values (1024*1024)
    /// </summary>
    private Color[,] _colorMap;

    /// <summary>
    ///     The color width
    /// </summary>
    private int _colorWidth;

    private Bitmap _currentImage;
    private bool _disposed;

    /// <summary>
    ///     The height map
    ///     Buffer/array to hold height values (1024*1024)
    /// </summary>
    private int[,] _heightMap;

    /// <summary>
    ///     The is cache preloading
    /// </summary>
    private bool _isCachePreloading;

    private int _topographyHeight;
    private int _topographyWidth;

    /// <summary>
    ///     Initializes a new instance of the <see cref="VoxelRaster" /> class.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="degree">The degree.</param>
    /// <param name="height">The height.</param>
    /// <param name="horizon">The horizon.</param>
    /// <param name="scale">The scale.</param>
    /// <param name="distance">The distance.</param>
    /// <param name="colorMap">The bitmap with the colors</param>
    /// <param name="heightMap">The bitmap with the height map.</param>
    /// <param name="screenHeight">Height of the screen.</param>
    /// <param name="screenWidth">Width of the screen.</param>
    public VoxelRaster(int x, int y, int degree, int height, int horizon, int scale, int distance, Bitmap colorMap,
        Bitmap heightMap, int screenHeight, int screenWidth)
    {
        _directionKey = new[] { Key.W, Key.S, Key.A, Key.D, Key.O, Key.P, Key.X, Key.Y };

        Camera = new RvCamera
        {
            X = x,
            Y = y,
            Angle = degree,
            Horizon = horizon,
            ZFar = distance
        };

        var context = new CameraContext
        {
            Height = height, Distance = distance, ScreenWidth = screenWidth, ScreenHeight = screenHeight,
            Scale = scale
        };

        ProcessColorMap(colorMap);

        ProcessHeightMap(heightMap);

        BuildColorDictionary();

        _raster = new VoxelRaster3D(context, _colorMap, _heightMap, _topographyWidth, _topographyHeight,
            _colorWidth, _colorHeight);

        _lazyCache = new ConcurrentDictionary<Key, Bitmap>();

        // Initialize cancellation token source for managing thread cancellation
        _cancellationTokenSource = new CancellationTokenSource();

        // Initialize the cache preload thread
        _cachePreloadThread = new Thread(() => PreloadCache(_cancellationTokenSource.Token));

        // Initialize last update time
        InputHelper.LastUpdateTime = DateTime.Now;
    }

    public long CameraTime { get; set; }

    /// <summary>
    ///     Gets the camera.
    /// </summary>
    /// <value>
    ///     The camera.
    /// </value>
    public RvCamera Camera { get; set; }

    /// <summary>
    ///     Gets the image render.
    /// </summary>
    /// <value>
    ///     The image render.
    /// </value>
    public long ImageRender { get; private set; }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Gets the bitmap for key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>new Bitmap</returns>
    public Bitmap GetBitmapForKey(Key key)
    {
        // Check if the direction key is valid; otherwise, return the current image.
        if (!_directionKey.Contains(key))
            return _currentImage;

        // Update the camera position.
        var stopwatch = Stopwatch.StartNew();
        Camera = InputHelper.SimulateCameraMovementVoxel(key, Camera);
        stopwatch.Stop();
        CameraTime = stopwatch.ElapsedMilliseconds;

        // Check if the bitmap is already cached.
        //if (_lazyCache.TryGetValue(key, out var cachedBitmap))
        //{
        //    // Trigger cache rebuilding asynchronously, if needed.
        //    Task.Run(RebuildCache);
        //    _currentImage = cachedBitmap;
        //    return cachedBitmap;
        //}

        // Render synchronously for the requested key (to avoid freezes).
        stopwatch.Restart();
        var renderedImage = _raster.RenderWithContainer(Camera);
        stopwatch.Stop();
        ImageRender = stopwatch.ElapsedMilliseconds;

        return renderedImage;

        //TODO make threadsafe

        // Update the current image and cache.
        lock (_lock)
        {
            _currentImage = renderedImage;
            _lazyCache[key] = renderedImage;
        }

        // Add cache rebuilding to pending tasks
        lock (PendingTasks)
        {
            var cacheRebuildTask = Task.Run(RebuildCache);
            PendingTasks.Add(cacheRebuildTask);

            // If too many tasks are pending, wait for one to complete before adding another
            if (PendingTasks.Count >= 4)
            {
                var completedTask = Task.WhenAny(PendingTasks).Result; // Wait for any task to complete
                PendingTasks.Remove(completedTask); // Remove the completed task from the queue
            }
        }

        return renderedImage;
    }


    /// <summary>
    ///     Starts the engine.
    /// </summary>
    /// <returns>Starter Image</returns>
    public Bitmap StartEngine()
    {
        if (!_isCachePreloading) _cachePreloadThread.Start();

        if (Camera == null) return null;

        Camera.Pitch = 50;

        InputHelper.UpdateDeltaTime();

        // initiate new instance
        //var raster = new VoxelRaster3D(_context);

        _currentImage = _raster.RenderWithContainer(Camera);

        return _currentImage;
    }

    private void BuildColorDictionary()
    {
        _colorDictionary = new Dictionary<int, Color>();

        // Iterate over all pixels in the colorMap and add them to the dictionary
        for (var x = 0; x < _colorWidth; x++)
        for (var y = 0; y < _colorHeight; y++)
        {
            var color = _colorMap[x, y];
            var colorId = color.ToArgb();
            if (!_colorDictionary.ContainsKey(colorId)) _colorDictionary.Add(colorId, color);
        }
    }


    /// <summary>
    ///     Rebuilds the cache.
    /// </summary>
    private void RebuildCache()
    {
        _lazyCache.Clear(); // Clear the old cache

        // Preload new cache based on the new position
        PreloadCache(_cancellationTokenSource.Token);
    }

    /// <summary>
    ///     Generates the and cache bitmap for key.
    /// </summary>
    /// <param name="key">The key.</param>
    private void GenerateAndCacheBitmapForKey(Key key)
    {
        lock (_lock)
        {
            // Check if the bitmap is already being generated in another thread
            if (_lazyCache.ContainsKey(key) ||
                (_isImageBeingGenerated.ContainsKey(key) && _isImageBeingGenerated[key]))
                return;

            // Mark that we are generating the image for this key
            _isImageBeingGenerated[key] = true;

            // Simulate camera movement for the requested direction
            var simulatedCamera = Camera.Clone();
            simulatedCamera = InputHelper.SimulateCameraMovementVoxel(key, simulatedCamera);

            // Generate the bitmap
            // Cache the bitmap
            _lazyCache[key] = _raster.RenderWithContainer(simulatedCamera);

            // Mark the image generation as complete
            _isImageBeingGenerated[key] = false;
        }
    }

    // Cache preloading thread method to calculate images for all possible directions
    private void PreloadCache(CancellationToken cancellationToken)
    {
        _isCachePreloading = true;

        // Preload all possible directions
        foreach (var directionKey in _directionKey.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
            GenerateAndCacheBitmapForKey(directionKey);

        _isCachePreloading = false;
    }


    /// <summary>
    ///     Processes the height map.
    /// </summary>
    /// <param name="bmp">The BMP.</param>
    private void ProcessHeightMap(Bitmap bmp)
    {
        if (bmp == null) return;

        _topographyHeight = bmp.Height;
        _topographyWidth = bmp.Width;

        var dbm = DirectBitmap.GetInstance(bmp);

        _heightMap = new int[bmp.Width, bmp.Height];
        for (var i = 0; i < bmp.Width; i++)
        for (var j = 0; j < bmp.Height; j++)
            _heightMap[i, j] = dbm.GetPixel(i, j).R;
    }

    /// <summary>
    ///     Processes the color map.
    /// </summary>
    /// <param name="bmp">The BMP.</param>
    private void ProcessColorMap(Bitmap bmp)
    {
        if (bmp == null) return;

        _colorHeight = bmp.Height;
        _colorWidth = bmp.Width;

        var dbm = DirectBitmap.GetInstance(bmp);

        _colorMap = new Color[bmp.Width, bmp.Height];
        for (var i = 0; i < bmp.Width; i++)
        for (var j = 0; j < bmp.Height; j++)
            _colorMap[i, j] = dbm.GetPixel(i, j);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            if (_cachePreloadThread is { IsAlive: true })
                try
                {
                    _cachePreloadThread.Join(100); // Wait briefly for the thread to finish
                }
                catch (ThreadStateException)
                {
                    // Handle if thread is already in a stopping/invalid state
                }

            // Dispose of any cached Bitmaps
            foreach (var bitmap in _lazyCache.Values) bitmap?.Dispose();

            _lazyCache.Clear();
        }

        // Free unmanaged resources here, if any

        _disposed = true;
    }

    ~VoxelRaster()
    {
        Dispose(false);
    }
}