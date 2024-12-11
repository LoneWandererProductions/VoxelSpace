using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Input;
using Mathematics;

namespace Voxels
{
    public class RasterVoxel
    {
        private const float MovementSpeed = 10f; // Movement speed (units per second)
        private const float RotationSpeed = 10f; // Rotation speed (degrees per second)

        private readonly Thread _cachePreloadThread;
        private readonly int _colorHeight;

        private readonly Color[,] _colorMap;
        private readonly int _colorWidth;
        private readonly int[,] _heightMap;
        private readonly Dictionary<Key, Bitmap> _lazyCache;
        private readonly object _lock = new();

        private readonly int _topographyHeight;
        private readonly int _topographyWidth;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private float _elapsedTime; // Time elapsed since the last frame


        private bool _isCachePreloading;
        private DateTime _lastUpdateTime;

        public RasterVoxel(Color[,] colorMap, int[,] heightMap, int topographyHeight, int topographyWidth,
            int colorHeight, int colorWidth, Camera camera)
        {
            _colorMap = colorMap;
            _heightMap = heightMap;
            _topographyHeight = topographyHeight;
            _topographyWidth = topographyWidth;
            _colorHeight = colorHeight;
            _colorWidth = colorWidth;
            Camera = camera;

            _lazyCache = new Dictionary<Key, Bitmap>();

            // Initialize cancellation token source for managing thread cancellation
            _cancellationTokenSource = new CancellationTokenSource();

            // Initialize the cache preload thread
            _cachePreloadThread = new Thread(() => PreloadCache(_cancellationTokenSource.Token));

            // Initialize last update time
            _lastUpdateTime = DateTime.Now;
        }

        public Camera Camera { get; }

        public Bitmap GetBitmapForKey(Key key)
        {
            lock (_lock)
            {
                if (_lazyCache.TryGetValue(key, out var cachedBitmap)) return cachedBitmap;
            }

            var simulatedCamera = Camera.Clone();
            SimulateCameraMovement(simulatedCamera, key);

            var raster = new Raster();
            var bitmap = raster.CreateBitmapWithDepthBuffer(_colorMap, _heightMap, simulatedCamera,
                _topographyHeight, _topographyWidth, _colorHeight, _colorWidth);

            lock (_lock)
            {
                _lazyCache[key] = bitmap;
            }

            return bitmap;
        }

        public void StartCachePreload()
        {
            if (!_isCachePreloading) _cachePreloadThread.Start();
        }

        private void GenerateAndCacheBitmapForKey(Key key)
        {
            lock (_lock)
            {
                // Check if the bitmap is already being generated in another thread
                if (!_lazyCache.ContainsKey(key))
                {
                    var simulatedCamera = Camera.Clone();
                    // Simulate camera movement for the requested direction
                    SimulateCameraMovement(simulatedCamera, key);

                    // Generate the bitmap
                    var raster = new Raster();
                    var bitmap = raster.CreateBitmapWithDepthBuffer(_colorMap, _heightMap, simulatedCamera,
                        _topographyHeight, _topographyWidth, _colorHeight, _colorWidth);

                    // Cache the bitmap
                    _lazyCache[key] = bitmap;
                }
            }
        }

        private void SimulateCameraMovement(Camera camera, Key key)
        {
            UpdateDeltaTime(); // Update deltaTime based on frame time

            switch (key)
            {
                case Key.W:
                    camera.X -= (int)(MovementSpeed * _elapsedTime * ExtendedMath.CalcSin(camera.Angle));
                    camera.Y -= (int)(MovementSpeed * _elapsedTime * ExtendedMath.CalcCos(camera.Angle));
                    break;
                case Key.S:
                    camera.X += (int)(MovementSpeed * _elapsedTime * ExtendedMath.CalcSin(camera.Angle));
                    camera.Y += (int)(MovementSpeed * _elapsedTime * ExtendedMath.CalcCos(camera.Angle));
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
        }

        // Update method to calculate deltaTime
        private void UpdateDeltaTime()
        {
            var currentTime = DateTime.Now;
            _elapsedTime = (float)(currentTime - _lastUpdateTime).TotalSeconds;
            _lastUpdateTime = currentTime;
        }

        private void PreloadCache(CancellationToken cancellationToken)
        {
            // Start cache preloading in a background thread
            _isCachePreloading = true;

            // Simulate preloading for all possible directions
            foreach (var directionKey in new[] { Key.W, Key.S, Key.A, Key.D, Key.O, Key.P })
            {
                if (cancellationToken.IsCancellationRequested)
                    // If cancellation is requested, stop preloading
                    break;

                GenerateAndCacheBitmapForKey(directionKey);
            }

            _isCachePreloading = false;
        }
    }
}