using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DapperRepo.Core.Cache
{
    public class RedisCacheManager : IStaticCacheManager
    {
        #region Fields

        private readonly IRedisConnectionWrapper _connectionWrapper;

        private readonly IDatabase _db;

        #endregion

        #region Ctor

        public RedisCacheManager(IRedisConnectionWrapper connectionWrapper)
        {
            // ConnectionMultiplexer.Connect should only be called once and shared between callers
            _connectionWrapper = connectionWrapper;

            _db = _connectionWrapper.GetDatabase();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Key of cached item</param>
        /// <returns>The cached value associated with the specified key</returns>
        protected async Task<T> GetAsync<T>(string key)
        {
            //get serialized item from cache
            var serializedItem = await _db.StringGetAsync(key);
            if (!serializedItem.HasValue)
                return default(T);

            //deserialize item
            var item = JsonConvert.DeserializeObject<T>(serializedItem);
            if (item == null)
                return default(T);

            return item;
        }

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        /// <param name="cacheTime">Cache time in minutes</param>
        protected async Task SetAsync(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            //set cache time
            var expiresIn = TimeSpan.FromMinutes(cacheTime);

            //serialize item
            var serializedItem = JsonConvert.SerializeObject(data);

            //and set it to cache
            await _db.StringSetAsync(key, serializedItem, expiresIn);
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        protected async Task<bool> IsSetAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        protected async Task RemoveAsync(string key)
        {
            //remove item from caches
            await _db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// Removes items by key pattern
        /// </summary>
        /// <param name="pattern">String key pattern</param>
        protected async Task RemoveByPatternAsync(string pattern)
        {
            foreach (var endPoint in _connectionWrapper.GetEndPoints())
            {
                var server = _connectionWrapper.GetServer(endPoint);
                var keys = server.Keys(database: _db.Database, pattern: $"*{pattern}*");

                await _db.KeyDeleteAsync(keys.ToArray());
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        protected async Task ClearAsync()
        {
            foreach (var endPoint in _connectionWrapper.GetEndPoints())
            {
                var server = _connectionWrapper.GetServer(endPoint);

                //we can use the code below (commented), but it requires administration permission - ",allowAdmin=true"
                //server.FlushDatabase();

                //that's why we manually delete all elements
                var keys = server.Keys(database: _db.Database);

                await _db.KeyDeleteAsync(keys.ToArray());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <param name="cacheTime">Cache time in minutes</param>
        /// <returns>The cached value associated with the specified key</returns>
        public T Get<T>(string key, Func<T> acquire, int cacheTime = 60)
        {
            //item already is in cache, so return it
            if (IsSetAsync(key).Result)
                return GetAsync<T>(key).Result;

            //or create it using passed function
            var result = acquire();

            //and set in cache (if cache time is defined)
            if (cacheTime > 0)
                SetAsync(key, result, cacheTime).Wait();

            return result;
        }

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        /// <param name="cacheTime">Cache time in minutes</param>
        public async void Set(string key, object data, int cacheTime)
        {
            await SetAsync(key, data, cacheTime);
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        public bool IsSet(string key)
        {
            return IsSetAsync(key).Result;
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        public async void Remove(string key)
        {
            await RemoveAsync(key);
        }

        /// <summary>
        /// Removes items by key pattern
        /// </summary>
        /// <param name="pattern">String key pattern</param>
        public async void RemoveByPattern(string pattern)
        {
            await RemoveByPatternAsync(pattern);
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public async void Clear()
        {
            await ClearAsync();
        }

        /// <summary>
        /// Dispose cache manager
        /// </summary>
        public void Dispose()
        {
            //if (_connectionWrapper != null)
            //    _connectionWrapper.Dispose();
        }

        #endregion
    }
}
