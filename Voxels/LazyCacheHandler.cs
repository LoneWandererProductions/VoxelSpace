using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Voxels
{
    public class LazyCacheHandler
    {
        private readonly object _lock = new();
        private readonly Dictionary<string, Bitmap> _lazyCache = new();
        private CancellationTokenSource _idleTokenSource;
        private bool _isCaching;

        public event Action CacheUpdated; // Optional: Notify when cache is updated

        /// <summary>
        /// Gets a cached image or generates it if not already cached.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="generateImage">The function to generate the image if not cached.</param>
        /// <returns>The cached or newly generated image.</returns>
        public Bitmap GetOrGenerate(string key, Func<Bitmap> generateImage)
        {
            lock (_lock)
            {
                if (_lazyCache.ContainsKey(key))
                    return _lazyCache[key];

                // Generate and cache the image
                var image = generateImage();
                _lazyCache[key] = image;
                return image;
            }
        }

        /// <summary>
        /// Triggers an idle-based cache generation task.
        /// </summary>
        /// <param name="generateCache">The function to generate cache entries during idle.</param>
        public void StartIdleCacheGeneration(Action generateCache)
        {
            ResetIdleDetection();

            _idleTokenSource = new CancellationTokenSource();
            var token = _idleTokenSource.Token;

            _ = Task.Delay(1000, token).ContinueWith(t =>
            {
                if (!t.IsCanceled && !_isCaching)
                {
                    _isCaching = true;
                    generateCache();
                    _isCaching = false;
                    CacheUpdated?.Invoke();
                }
            });
        }

        /// <summary>
        /// Resets the idle detection timer.
        /// </summary>
        public void ResetIdleDetection()
        {
            _idleTokenSource?.Cancel();
            _idleTokenSource = null;
        }
    }
}
