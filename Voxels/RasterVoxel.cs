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
    public class RasterVoxel
    {
        private const float MovementSpeed = 10f; // Movement speed (units per second)
        private const float RotationSpeed = 10f; // Rotation speed (degrees per second)

        /// <summary>
        /// The cache preload thread
        /// </summary>
        private readonly Thread _cachePreloadThread;
        private int _colorHeight;

        private Color[,] _colorMap;
        private int _colorWidth;
        private int[,] _heightMap;
        private readonly ConcurrentDictionary<Key, Bitmap> _lazyCache;
        private readonly object _lock = new();

        private int _topographyHeight;
        private int _topographyWidth;

        /// <summary>
        /// The cancellation token source
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource;

        private float _elapsedTime; // Time elapsed since the last frame


        /// <summary>
        /// The is cache preloading
        /// </summary>
        private bool _isCachePreloading;

        /// <summary>
        /// The last update time
        /// </summary>
        private DateTime _lastUpdateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="RasterVoxel"/> class.
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
        public RasterVoxel(int x, int y, int degree, int height, int horizon, int scale, int distance, Bitmap colorMap,
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
        /// Gets the camera.
        /// </summary>
        /// <value>
        /// The camera.
        /// </value>
        public Camera Camera { get; set;  }

        /// <summary>
        /// Gets the bitmap for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>new Bitmap</returns>
        public Bitmap GetBitmapForKey(Key key)
        {
            //TODO not working yet
            if (_lazyCache.TryGetValue(key, out var cachedBitmap)) return cachedBitmap;

            Camera = SimulateCameraMovement(key, Camera);

            var raster = new Raster();
            var bitmap = raster.CreateBitmapWithDepthBuffer(_colorMap, _heightMap, Camera,
                _topographyHeight, _topographyWidth, _colorHeight, _colorWidth);

            _lazyCache[key] = bitmap;

            return bitmap;
        }

        /// <summary>
        /// Starts the engine.
        /// </summary>
        /// <returns>Starter Image</returns>
        public Bitmap StartEngine()
        {
            if (!_isCachePreloading) _cachePreloadThread.Start();

            if (Camera == null) return null;

            UpdateDeltaTime();

            // Generate the start bitmap
            var raster = new Raster();
            return raster.CreateBitmapWithDepthBuffer(_colorMap, _heightMap, Camera,
                _topographyHeight, _topographyWidth, _colorHeight, _colorWidth);
        }

        /// <summary>
        /// Generates the and cache bitmap for key.
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
                _lazyCache[key] = raster.CreateBitmapWithDepthBuffer(_colorMap, _heightMap, simulatedCamera,
                    _topographyHeight, _topographyWidth, _colorHeight, _colorWidth);
            }
        }

        /// <summary>
        /// Simulates the camera movement.
        /// </summary>
        /// <param name="key">The key.</param>
        private Camera SimulateCameraMovement(Key key, Camera camera)
        {
            UpdateDeltaTime(); // Update deltaTime based on frame time

            //the key
            Trace.WriteLine($"Key: {key}");

            // Log the old camera state
            Trace.WriteLine($"Before: {Camera.ToString()}");

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
            }

            // Log the new camera state
            Trace.WriteLine($"After: {Camera.ToString()}");

            return camera;
        }


        /// <summary>
        /// Update method to calculate deltaTime
        /// </summary>
        private void UpdateDeltaTime()
        {
            var currentTime = DateTime.Now;
            _elapsedTime = (float)(currentTime - _lastUpdateTime).TotalSeconds;

            // If no time has elapsed, use a default small value to avoid zero movement on startup
            if (_elapsedTime == 0)
            {
                _elapsedTime = 0.016f; // Assuming ~60 FPS, 1 frame = ~0.016 seconds
            }

            // Optional: Cap delta time to avoid large jumps
            _elapsedTime = Math.Min(_elapsedTime, 0.1f); // 0.1s cap to prevent large frame gaps
            _lastUpdateTime = currentTime;
        }

        private void PreloadCache(CancellationToken cancellationToken)
        {
            // Start cache preloading in a background thread
            _isCachePreloading = true;

            // Simulate preloading for all possible directions
            foreach (var directionKey in new[] { Key.W, Key.S, Key.A, Key.D, Key.O, Key.P }.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
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
    }
}