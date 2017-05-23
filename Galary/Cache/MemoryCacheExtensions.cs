using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;


namespace Galary.Cache
{
    public static class MemoryCacheExtensions
    {
        public const string SingletonTypeCacheIdentifier = "STCI";


        public static TValue GetOrAddSafe<TValue>(
            this MemoryCache dictionary,
            Func<TValue> valueFactory,
            CacheItemPolicy policy) where TValue : class
        {
            return GetOrAddSafe(dictionary, SingletonTypeCacheIdentifier, valueFactory, policy);
        }

        public static TValue GetOrAddSafe<TKey, TValue>(
            this MemoryCache dictionary, TKey key,
            Func<TValue> valueFactory,
            CacheItemPolicy policy) where TValue : class
        {
            return GetOrAddSafe(dictionary, key, x => valueFactory(), policy);
        }

        public static TValue GetOrAddSafe<TKey, TValue>(
            this MemoryCache dictionary,
            TKey key,
            Func<TKey, TValue> valueFactory,
            CacheItemPolicy policy) where TValue : class
        {
            var trueKey = CalculateTrueKey<TValue>(key);

            var lazy = dictionary.AddOrGetExisting(trueKey, new Lazy<TValue>(() => valueFactory(key)), policy) ??
                       dictionary.Get(trueKey);

            try
            {
                var result = TypeSafeRetrieveValue<TValue>(trueKey, lazy);
                return result;
            }
            catch (Exception)
            {
                dictionary.Remove(trueKey);
                throw;
            }

        }

        public static TValue GetSafe<TKey, TValue>(
            this MemoryCache dictionary,
            TKey key) where TValue : class
        {
            var trueKey = CalculateTrueKey<TValue>(key);

            var lazy = dictionary.Get(trueKey);
            try
            {
                var result = TypeSafeRetrieveValue<TValue>(trueKey, lazy);
                return result;
            }
            catch (Exception)
            {
                dictionary.Remove(trueKey);
                throw;
            }

        }


        public static TValue AddOrUpdateSafe<TValue>(
            this MemoryCache dictionary,
            Func<TValue> valueFactory,
            Func<TValue, TValue> updateValueFactory,
            CacheItemPolicy policy)
            where TValue : class
        {
            return AddOrUpdateSafe(dictionary, SingletonTypeCacheIdentifier, valueFactory, updateValueFactory, policy);
        }

        public static TValue AddOrUpdateSafe<TKey, TValue>(
            this MemoryCache dictionary,
            TKey key,
            Func<TValue> valueFactory,
            Func<TValue, TValue> updateValueFactory,
            CacheItemPolicy policy)
            where TValue : class
        {
            return AddOrUpdateSafe(dictionary, key, x => valueFactory(), (x, y) => updateValueFactory(y), policy);
        }

        public static TValue AddOrUpdateSafe<TKey, TValue>(
            this MemoryCache dictionary,
            TKey key,
            Func<TKey, TValue> valueFactory,
            Func<TKey, TValue, TValue> updateValueFactory,
            CacheItemPolicy policy)
            where TValue : class
        {
            var trueKey = CalculateTrueKey<TValue>(key);

            dictionary.Set(trueKey, new Lazy<TValue>(() => valueFactory(key)), policy);
            var lazy = dictionary.Get(trueKey);

            try
            {
                var result = TypeSafeRetrieveValue<TValue>(trueKey, lazy);
                return result;
            }
            catch (Exception)
            {
                dictionary.Remove(trueKey);
                throw;
            }
        }

        public static void DeleteSafe<TValue>(
            this MemoryCache dictionary)
            where TValue : class
        {
            DeleteSafe<string, TValue>(dictionary, SingletonTypeCacheIdentifier);
        }

        public static void DeleteSafe<TKey, TValue>(
            this MemoryCache dictionary,
            TKey key)
            where TValue : class
        {
            var trueKey = CalculateTrueKey<TValue>(key);

            dictionary.Remove(trueKey);
        }


        private static string CalculateTrueKey<TValue>(object key)
        {
            return String.Format("{0}:{1}", key, typeof(TValue));
        }

        private static TValue TypeSafeRetrieveValue<TValue>(object key, object lazy)
        {
            if (lazy == null)
                return default(TValue);
            var lazyTypeMatch = lazy as Lazy<TValue>;
            if (lazyTypeMatch == null)
                throw new InvalidCastException(
                    String.Format(
                        "The Cache Key '{0}' had a cached Value that didn't match the requested type of '{1}', as it instead stored a value of type '{2}'.  " +
                        "This is likely the results of a conflicting Cache Key, make sure the Cache Key is unique", key,
                        typeof (TValue), lazy == null ? null : lazy.GetType()));
            return lazyTypeMatch.Value;
        }
    }
}