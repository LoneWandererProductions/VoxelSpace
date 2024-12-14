using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Imaging;
using Mathematics;

namespace Voxels
{
    public sealed class VoxelRaster : IDisposable
    {
        private const float MovementSpeed = 10f; // Movement speed (units per second)
        private const float RotationSpeed = 10f; // Rotation speed (degrees per second)

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
        ///     Time elapsed since the last frame
        /// </summary>
        private float _elapsedTime;

        /// <summary>
        ///     The height map
        ///     Buffer/array to hold height values (1024*1024)
        /// </summary>
        private int[,] _heightMap;

        /// <summary>
        ///     The is cache preloading
        /// </summary>
        private bool _isCachePreloading;

        /// <summary>
        ///     The last update time
        /// </summary>
        private DateTime _lastUpdateTime;

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
        public VoxelRaster(int x, int y, int degree, int height, int horizon, int scale, int distance, Bitmap colorMap,
            Bitmap heightMap)
        {
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
            _lastUpdateTime = DateTime.Now;
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
            // Always update the camera position first.
            Camera = SimulateCameraMovement(key, Camera);

            // Check if the bitmap is already cached.
            if (_lazyCache.TryGetValue(key, out var cachedBitmap))
            {
                // After returning the cached bitmap, rebuild the cache.
                RebuildCache();
                return cachedBitmap; // Return the cached bitmap if it exists.
            }

            // After each movement, rebuild the cache
            RebuildCache();

            // If not cached, generate the bitmap.
            var raster = new Raster();
            return raster.RenderImmediate(_colorMap, _heightMap, Camera, _topographyHeight,
                _topographyWidth, _colorHeight, _colorWidth);
        }

        /// <summary>
        ///     Starts the engine.
        /// </summary>
        /// <returns>Starter Image</returns>
        public Bitmap StartEngine()
        {
            if (!_isCachePreloading) _cachePreloadThread.Start();

            if (Camera == null) return null;

            UpdateDeltaTime();

            // Generate the start bitmap
            var raster = new Raster();
            return raster.RenderWithDepthBuffer(_colorMap, _heightMap, Camera, _topographyHeight,
                _topographyWidth, _colorHeight, _colorWidth);
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
                simulatedCamera = SimulateCameraMovement(key, simulatedCamera);

                // Generate the bitmap
                var raster = new Raster();

                // Cache the bitmap
                _lazyCache[key] = raster.RenderWithDepthBuffer(_colorMap, _heightMap, simulatedCamera,
                    _topographyHeight, _topographyWidth, _colorHeight, _colorWidth);
            }
        }

        /// <summary>
        /// Simulates the camera movement.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="camera">The camera.</param>
        /// <returns>New Camera Values</returns>
        private Camera SimulateCameraMovement(Key key, Camera camera)
        {
            UpdateDeltaTime(); // Update deltaTime based on frame time

            //the key
            Trace.WriteLine($"Key: {key}");

            // Log the old camera state
            Trace.WriteLine($"Before: {Camera}");

            // Update the actual camera object directly
            switch (key)
            {
                case Key.W:
                    camera.X -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSin(Camera.Angle));
                    camera.Y -= (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCos(Camera.Angle));
                    break;
                case Key.S:
                    camera.X += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcSin(Camera.Angle));
                    camera.Y += (int)Math.Round(MovementSpeed * _elapsedTime * ExtendedMath.CalcCos(Camera.Angle));
                    break;
                case Key.A:
                    camera.Angle += (int)(RotationSpeed * _elapsedTime); // Turn left
                    break;
                case Key.D:
                    camera.Angle -= (int)(RotationSpeed * _elapsedTime); // Turn right
                    break;
                case Key.O:
                    camera.Horizon += (int)(RotationSpeed * _elapsedTime); // Move up
                    break;
                case Key.P:
                    camera.Horizon -= (int)(RotationSpeed * _elapsedTime); // Move down
                    break;
                case Key.X:
                    camera.Pitch = Math.Max(camera.Pitch - (int)(RotationSpeed * _elapsedTime), -90); // Look down
                    break;
                case Key.Y:
                    camera.Pitch = Math.Min(camera.Pitch + (int)(RotationSpeed * _elapsedTime), 90); // Look up
                    break;
            }

            // Log the new camera state
            Trace.WriteLine($"After: {Camera}");

            return camera;
        }

        /// <summary>
        ///     Update method to calculate deltaTime
        /// </summary>
        private void UpdateDeltaTime()
        {
            var currentTime = DateTime.Now;
            _elapsedTime = (float)(currentTime - _lastUpdateTime).TotalSeconds;

            // If no time has elapsed, use a default small value to avoid zero movement on startup
            if (_elapsedTime == 0) _elapsedTime = 0.016f; // Assuming ~60 FPS, 1 frame = ~0.016 seconds

            // Optional: Cap delta time to avoid large jumps
            _elapsedTime = Math.Min(_elapsedTime, 0.1f); // 0.1s cap to prevent large frame gaps
            _lastUpdateTime = currentTime;
        }

        // Cache preloading thread method to calculate images for all possible directions
        private void PreloadCache(CancellationToken cancellationToken)
        {
            _isCachePreloading = true;

            // Preload all possible directions
            foreach (var directionKey in new[] { Key.W, Key.S, Key.A, Key.D, Key.O, Key.P, Key.X, Key.Y }.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
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