using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Caching;

namespace Payboard.Common.Cache
{
    /// <summary>
    ///     Provides an ISimpleCache-compatible wrapper around the Azure Cache Service. Mostly this is helpful when
    ///     we want to switch between caching systems, depending on whether we're running locally or up on Azure.
    /// </summary>
    public class SimpleCacheAzure : SimpleCacheBase
    {
        private readonly DataCache _cache;

        public SimpleCacheAzure(DataCache cache, string keyPrefix = null)
        {
            _cache = cache;
            _keyPrefix = keyPrefix;
            _logger.Debug("Created Azure cache with key prefix " + _keyPrefix);
        }

        public override TValue Get<TValue>(string key, Func<TValue> missingFunc)
        {
            // We need to ensure that two processes don't try to calculate the same value at the same time. That just wastes resources.
            // So we pull out a value from the _cacheLocks dictionary, and lock on that before trying to retrieve the object.
            // This does add a bit more locking, and hence the chance for one process to lock up everything else.
            // We may need to add some timeouts here at some point in time. It also doesn't prevent two processes on different
            // machines from trying the same bit o' nonsense. Oh well. It's probably still a worthwhile optimization.
            key = _keyPrefix + "." + key;
            var value = default(TValue);
            var present = true;

            // Try to get the value from the cache.
            try
            {
                value = _cache.Get(key) as TValue;
            }
            catch (SerializationException ex)
            {
                // This can happen when the app restarts, and we discover that the dynamic entity names have changed, and the desired type 
                // is no longer around, e.g., "Organization_6BA9E1E1184D9B7BDCC50D94471D7A730423456A15BBAFB6A2C6AC0FF94C0D41"
                // If that's the error, we should probably warn about it, but no point in logging it as an error, since it's more-or-less expected.
                _logger.Warn("Error retrieving item '" + key +
                             "' from Azure cache; falling back to missingFunc(). Error = " + ex);
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving item '" + key +
                              "' from Azure cache; falling back to missingFunc(). Error = " + ex);
            }

            // If we didn't get anything interesting, then call the function that should be able to retrieve it for us.
            if (value == default(TValue))
            {
                // If that function throws an exception, don't swallow it.
                value = missingFunc();
                present = false;

                // If we try to put it into the cache, and *that* throws an exception, 
                // log it, and then swallow it.
                try
                {
                    _cache.Put(key, value);
                }
                catch (Exception ex)
                {
                    _logger.Error("Error putting item '" + key + "' into Azure cache. Error = " + ex);
                }
            }
            RecordLookup(present);
            return value;
        }

        public override async Task<TValue> GetAsync<TValue>(string key, Func<Task<TValue>> missingFunc)
        {
            key = _keyPrefix + "." + key;
            var value = default(TValue);
            var present = true;
            // Try to get the value from the cache.
            try
            {
                value = _cache.Get(key) as TValue;
            }
            catch (SerializationException ex)
            {
                // This can happen when the app restarts, and we discover that the dynamic entity names have changed, and the desired type 
                // is no longer around, e.g., "Organization_6BA9E1E1184D9B7BDCC50D94471D7A730423456A15BBAFB6A2C6AC0FF94C0D41"
                // If that's the error, we should probably warn about it, but no point in logging it as an error, since it's more-or-less expected.
                _logger.Warn("Error retrieving item '" + key +
                             "' from Azure cache; falling back to missingFunc(). Error = " + ex);
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving item '" + key +
                              "' from Azure cache; falling back to missingFunc(). Error = " + ex);
            }

            // If we didn't get anything interesting, then call the function that should be able to retrieve it for us.
            if (value == default(TValue))
            {
                // If that function throws an exception, don't swallow it.
                value = await missingFunc();
                present = false;

                // If we try to put it into the cache, and *that* throws an exception, 
                // log it, and then swallow it.
                try
                {
                    if (value != default(TValue))
                    {
                        _cache.Put(key, value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error putting item '" + key + "' into Azure cache. Error = " + ex);
                }
            }
            RecordLookup(present);
            return value;
        }

        public override void Add<TValue>(string key, TValue value)
        {
            try
            {
                key = _keyPrefix + "." + key;
                _cache.Put(key, value);
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to place object in cache: " + ex);
            }
        }

        public override bool Remove<TValue>(string key)
        {
            try
            {
                key = GetKey<TValue>(key);
                return _cache.Remove(key);
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to remove item from cache: " + ex);
            }
            return false;
        }

        public override bool TryGetValue<TValue>(string key, out TValue value)
        {
            var present = false;
            value = null;
            try
            {
                key = GetKey<TValue>(key);
                value = _cache.Get(key) as TValue;
                if (value != null)
                {
                    present = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Error attempting to retrieve item from cache: " + ex);
            }
            RecordLookup(present);
            return present;
        }
    }
}