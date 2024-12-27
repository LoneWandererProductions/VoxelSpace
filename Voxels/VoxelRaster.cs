using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Imaging;

namespace Voxels
{
    public sealed class VoxelRaster : IDisposable
    {
        private bool _disposed;

        /// <summary>
        ///     The cache preload thread
        /// </summary>
        private readonly Thread _cachePreloadThread;

        /// <summary>
        ///     The cancellation token source
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly ConcurrentDictionary<Key, Bitmap> _lazyCache;

        private readonly object _lock = new();

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

        private readonly Key[] _directionKey;
        private Bitmap _currentImage;

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
        public VoxelRaster(int x, int y, int degree, int height, int horizon, int scale, int distance, Bitmap colorMap,
            Bitmap heightMap)
        {
            _directionKey = new[] {Key.W, Key.S, Key.A, Key.D, Key.O, Key.P, Key.X, Key.Y};

            Camera = new Camera
            {
                X = x,
                Y = y,
                Angle = degree,
                Height = height,
                Horizon = horizon,
                Scale = scale,
                ZFar = distance
            };

            ProcessColorMap(colorMap);

            ProcessHeightMap(heightMap);

            _lazyCache = new ConcurrentDictionary<Key, Bitmap>();

            // Initialize cancellation token source for managing thread cancellation
            _cancellationTokenSource = new CancellationTokenSource();

            // Initialize the cache preload thread
            _cachePreloadThread = new Thread(() => PreloadCache(_cancellationTokenSource.Token));

            // Initialize last update time
            Helper.LastUpdateTime = DateTime.Now;
        }

        /// <summary>
        ///     Gets the camera.
        /// </summary>
        /// <value>
        ///     The camera.
        /// </value>
        public Camera Camera { get; set; }

        /// <summary>
        ///     Gets the bitmap for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>new Bitmap</returns>
        public Bitmap GetBitmapForKey(Key key)
        {
            if (!_directionKey.Contains(key)) return _currentImage;

            // Always update the camera position first.
            Camera = Helper.SimulateCameraMovementVoxel(key, Camera);

            // Check if the bitmap is already cached.
            if (_lazyCache.TryGetValue(key, out var cachedBitmap))
            {
                // After returning the cached bitmap, rebuild the cache.
                RebuildCache();
                _currentImage = cachedBitmap;
                return cachedBitmap; // Return the cached bitmap if it exists.
            }

            // After each movement, rebuild the cache
            RebuildCache();

            // If not cached, generate the bitmap.
            var raster = new Raster();

            _currentImage = raster.RenderWithDepthBuffer(_colorMap, _heightMap, Camera, _topographyHeight,
                _topographyWidth, _colorHeight, _colorWidth);

            return _currentImage;
        }

        /// <summary>
        ///     Starts the engine.
        /// </summary>
        /// <returns>Starter Image</returns>
        public Bitmap StartEngine()
        {
            if (!_isCachePreloading) _cachePreloadThread.Start();

            if (Camera == null) return null;

            Helper.UpdateDeltaTime();

            // Generate the start bitmap
            var raster = new Raster();

            _currentImage = raster.RenderWithDepthBuffer(_colorMap, _heightMap, Camera, _topographyHeight,
                _topographyWidth, _colorHeight, _colorWidth);

            return _currentImage;
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
                if (_lazyCache.ContainsKey(key)) return;

                var simulatedCamera = Camera.Clone();
                // Simulate camera movement for the requested direction
                simulatedCamera = Helper.SimulateCameraMovementVoxel(key, simulatedCamera);

                // Generate the bitmap
                var raster = new Raster();

                // Cache the bitmap
                _lazyCache[key] = raster.RenderWithDepthBuffer(_colorMap, _heightMap, simulatedCamera,
                    _topographyHeight, _topographyWidth, _colorHeight, _colorWidth);
            }
        }

        // Cache preloading thread method to calculate images for all possible directions
        private void PreloadCache(CancellationToken cancellationToken)
        {
            _isCachePreloading = true;

            // Preload all possible directions
            foreach (var directionKey in _directionKey.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
            {
                GenerateAndCacheBitmapForKey(directionKey);
            }

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Dispose managed resources
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();

                if (_cachePreloadThread != null && _cachePreloadThread.IsAlive)
                {
                    try
                    {
                        _cachePreloadThread.Join(100); // Wait briefly for the thread to finish
                    }
                    catch (ThreadStateException)
                    {
                        // Handle if thread is already in a stopping/invalid state
                    }
                }

                // Dispose of any cached Bitmaps
                foreach (var bitmap in _lazyCache.Values)
                {
                    bitmap?.Dispose();
                }

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
}