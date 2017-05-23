using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using System.Runtime.Caching;

namespace Galary.Cache
{


    public static class CacheStore
    {
        private static MemoryCache _store = new MemoryCache("APICache");

        /// <summary>
        /// Completely dump and reset the API cache
        /// </summary>
        public static void ClearCache()
        {
            _store = new MemoryCache("APICache");
        }

        /// <summary>
        /// Retrieve a list of the current contents of the API cache
        /// </summary>
        /// <returns>An enumerable list of the cache keys</returns>
        public static IEnumerable<string> ViewCache()
        {
            return _store.Select(val => val.Key);
        }

        public static TValue Get<TValue>(int id) where TValue : class
        {
            return _store.GetSafe<int, TValue>(id);
        }

        /// <summary>
        /// Get an existing value based on the return type, inserting it if it does not exist.  
        /// Use this method for a singleton cache instance where there will only exist a single cached item at a time of this type
        /// </summary>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="valueFactory">The zero-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <returns>The existing or newly created value that exists for this type</returns>
        public static TValue GetOrAdd<TValue>(Func<TValue> valueFactory) where TValue : class
        {
            return GetOrAdd(valueFactory, null);
        }
        /// <summary>
        /// Get an existing value based on the return type, inserting it if it does not exist.  
        /// Use this method for a singleton cache instance where there will only exist a single cached item at a time of this type
        /// </summary>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="valueFactory">The zero-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <param name="evictionLengthHours">The length of time (in hours) until this item will be considered stale.  If null, will never expire</param>
        /// <returns>The existing or newly created value that exists for this type</returns>
        public static TValue GetOrAdd<TValue>(Func<TValue> valueFactory, double? evictionLengthHours) where TValue : class
        {
            var policy = BuildCacheItemPolicy(evictionLengthHours);
            object value = _store.GetOrAddSafe(valueFactory, policy);
            return (TValue)value;
        }

        /// <summary>
        /// Get an existing value based on the given key and the return type, inserting it if it does not exist.  
        /// Use this method for a cache instance of which several of the same type may exist, but that does not require a key value in the retrieval function (only for uniqueness)
        /// </summary>
        /// <typeparam name="TKey">The type of the key object</typeparam>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="key">The key that will be unique amongst this type in the Cache</param>
        /// <param name="valueFactory">The zero-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <returns>The existing or newly created value that exists for this type and key</returns>
        public static TValue GetOrAdd<TKey, TValue>(TKey key, Func<TValue> valueFactory) where TValue : class
        {
            return GetOrAdd(key, valueFactory, null);
        }
        /// <summary>
        /// Get an existing value based on the given key and the return type, inserting it if it does not exist.  
        /// Use this method for a cache instance of which several of the same type may exist, but that does not require a key value in the retrieval function (only for uniqueness)
        /// </summary>
        /// <typeparam name="TKey">The type of the key object</typeparam>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="key">The key that will be unique amongst this type in the Cache</param>
        /// <param name="valueFactory">The zero-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <param name="evictionLengthHours">The length of time (in hours) until this item will be considered stale.  If null, will never expire</param>
        /// <returns>The existing or newly created value that exists for this type and key</returns>
        public static TValue GetOrAdd<TKey, TValue>(TKey key, Func<TValue> valueFactory, double? evictionLengthHours) where TValue : class
        {
            var policy = BuildCacheItemPolicy(evictionLengthHours);
            object value = _store.GetOrAddSafe(key, valueFactory, policy);
            return (TValue)value;
        }

        /// <summary>
        /// Get an existing value based on the given key and the return type, inserting it if it does not exist.  
        /// Use this method for a cache instance of which several of the same type may exist, and that requires the key value in the retrieval function
        /// </summary>
        /// <typeparam name="TKey">The type of the key object</typeparam>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="key">The key that will be unique amongst this type in the Cache, and that is necessary for creation of the return value</param>
        /// <param name="valueFactory">The one-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <returns>The existing or newly created value that exists for this type and key</returns>
        public static TValue GetOrAdd<TKey, TValue>(TKey key, Func<TKey, TValue> valueFactory) where TValue : class
        {
            return GetOrAdd(key, valueFactory, null);
        }
        /// <summary>
        /// Get an existing value based on the given key and the return type, inserting it if it does not exist.  
        /// Use this method for a cache instance of which several of the same type may exist, and that requires the key value in the retrieval function
        /// </summary>
        /// <typeparam name="TKey">The type of the key object</typeparam>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="key">The key that will be unique amongst this type in the Cache, and that is necessary for creation of the return value</param>
        /// <param name="valueFactory">The one-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <param name="evictionLengthHours">The length of time (in hours) until this item will be considered stale.  If null, will never expire</param>
        /// <returns>The existing or newly created value that exists for this type and key</returns>
        public static TValue GetOrAdd<TKey, TValue>(TKey key, Func<TKey, TValue> valueFactory, double? evictionLengthHours) where TValue : class
        {
            var policy = BuildCacheItemPolicy(evictionLengthHours);
            object value = _store.GetOrAddSafe(key, valueFactory, policy);
            return (TValue)value;
        }


        /// <summary>
        /// Get an existing value based on the return type, inserting it if it does not exist, and updating the cache if it does.  
        /// Use this method for a singleton cache instance where there will only exist a single cached item at a time of this type
        /// </summary>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="valueFactory">The zero-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <returns>The existing or newly created value that exists for this type</returns>
        public static TValue AddOrUpdate<TValue>(Func<TValue> valueFactory) where TValue : class
        {
            return AddOrUpdate(valueFactory, null);
        }
        /// <summary>
        /// Get an existing value based on the return type, inserting it if it does not exist, and updating the cache if it does.  
        /// Use this method for a singleton cache instance where there will only exist a single cached item at a time of this type
        /// </summary>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="valueFactory">The zero-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <param name="evictionLengthHours">The length of time (in hours) until this item will be considered stale.  If null, will never expire</param>
        /// <returns>The existing or newly created value that exists for this type</returns>
        public static TValue AddOrUpdate<TValue>(Func<TValue> valueFactory, double? evictionLengthHours) where TValue : class
        {
            var policy = BuildCacheItemPolicy(evictionLengthHours);
            object value = _store.AddOrUpdateSafe(valueFactory, oldValue => valueFactory(), policy);
            return (TValue)value;
        }
        /// <summary>
        /// Get an existing value based on the given key and the return type, inserting it if it does not exist.  
        /// Use this method for a cache instance of which several of the same type may exist, but that does not require a key value in the retrieval function (only for uniqueness)
        /// </summary>
        /// <typeparam name="TKey">The type of the key object</typeparam>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="key">The key that will be unique amongst this type in the Cache</param>
        /// <param name="valueFactory">The zero-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <returns>The existing or newly created value that exists for this type and key</returns>
        public static TValue AddOrUpdate<TKey, TValue>(TKey key, Func<TValue> valueFactory) where TValue : class
        {
            return AddOrUpdate(key, valueFactory, null);
        }
        /// <summary>
        /// Get an existing value based on the given key and the return type, inserting it if it does not exist.  
        /// Use this method for a cache instance of which several of the same type may exist, but that does not require a key value in the retrieval function (only for uniqueness)
        /// </summary>
        /// <typeparam name="TKey">The type of the key object</typeparam>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="key">The key that will be unique amongst this type in the Cache</param>
        /// <param name="valueFactory">The zero-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <param name="evictionLengthHours">The length of time (in hours) until this item will be considered stale.  If null, will never expire</param>
        /// <returns>The existing or newly created value that exists for this type and key</returns>
        public static TValue AddOrUpdate<TKey, TValue>(TKey key, Func<TValue> valueFactory, double? evictionLengthHours) where TValue : class
        {
            var policy = BuildCacheItemPolicy(evictionLengthHours);
            object value = _store.AddOrUpdateSafe(key, valueFactory, oldValue => valueFactory(), policy);
            return (TValue)value;
        }
        /// <summary>
        /// Get an existing value based on the given key and the return type, inserting it if it does not exist.  
        /// Use this method for a cache instance of which several of the same type may exist, and that requires the key value in the retrieval function
        /// </summary>
        /// <typeparam name="TKey">The type of the key object</typeparam>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="key">The key that will be unique amongst this type in the Cache, and that is necessary for creation of the return value</param>
        /// <param name="valueFactory">The one-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <returns>The existing or newly created value that exists for this type and key</returns>
        public static TValue AddOrUpdate<TKey, TValue>(TKey key, Func<TKey, TValue> valueFactory) where TValue : class
        {
            return AddOrUpdate(key, valueFactory, null);
        }
        /// <summary>
        /// Get an existing value based on the given key and the return type, inserting it if it does not exist.  
        /// Use this method for a cache instance of which several of the same type may exist, and that requires the key value in the retrieval function
        /// </summary>
        /// <typeparam name="TKey">The type of the key object</typeparam>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="key">The key that will be unique amongst this type in the Cache, and that is necessary for creation of the return value</param>
        /// <param name="valueFactory">The one-parameter function to retrieve the value fresh if it does not currently exist</param>
        /// <param name="evictionLengthHours">The length of time (in hours) until this item will be considered stale.  If null, will never expire</param>
        /// <returns>The existing or newly created value that exists for this type and key</returns>
        public static TValue AddOrUpdate<TKey, TValue>(TKey key, Func<TKey, TValue> valueFactory, double? evictionLengthHours) where TValue : class
        {
            var policy = BuildCacheItemPolicy(evictionLengthHours);
            object value = _store.AddOrUpdateSafe(key, valueFactory, (k, oldValue) => valueFactory(k), policy);
            return (TValue)value;
        }

        /// <summary>
        /// Delete an existing value based on the given key if it exists.  
        /// Use this method for a singleton cache instance where there will only exist a single cached item at a time of this type
        /// </summary>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        public static void Delete<TValue>() where TValue : class
        {
            _store.DeleteSafe<TValue>();
        }
        /// <summary>
        /// Delete an existing value based on the given key if it exists.  
        /// Use this method for a cache instance of which several of the same type may exist, and that requires the key value in the retrieval function
        /// </summary>
        /// <typeparam name="TKey">The type of the key object</typeparam>
        /// <typeparam name="TValue">The return type of the stored value</typeparam>
        /// <param name="key">The key that will be unique amongst this type in the Cache, and that is necessary for creation of the return value</param>
        public static void Delete<TKey, TValue>(TKey key) where TValue : class
        {
            _store.DeleteSafe<TKey, TValue>(key);
        }


        private static CacheItemPolicy BuildCacheItemPolicy(double? evictionLengthHours)
        {
            var callback = new CacheEntryRemovedCallback(MyCachedItemRemovedCallback);

            var policy = new CacheItemPolicy
            {
                Priority = evictionLengthHours.HasValue ? CacheItemPriority.Default : CacheItemPriority.NotRemovable
            };

            if(evictionLengthHours.HasValue)
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddHours(evictionLengthHours.Value);
            policy.RemovedCallback = callback;

            return policy;
        }

        private static void MyCachedItemRemovedCallback(CacheEntryRemovedArguments arguments)
        {
            // Log these values from arguments list 
#if DEBUG
            String strLog = String.Concat("Reason: ", arguments.RemovedReason.ToString(), " | Key-Name: ", arguments.CacheItem.Key, " | Value-Object: ", arguments.CacheItem.Value.ToString());

            //Log.Writer.Write("Cache Removal:" + strLog, TraceEventType.Verbose);
#endif
        }
    }
}