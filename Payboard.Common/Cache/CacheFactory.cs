using System;
using Microsoft.ApplicationServer.Caching;
using Microsoft.WindowsAzure;
using Ninject.Activation;
using NLog;

namespace Payboard.Common.Cache
{
    /// <summary>
    /// Provides a configurable way to either retrieve simple (local) cache services (for development), 
    /// or to use the Azure Cache Service (for production).
    /// </summary>
    public static class CacheFactory
    {
        private static CacheType cacheType;
        private static readonly DataCacheFactory dataCacheFactory;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string redisCs;

        static CacheFactory()
        {
            if (dataCacheFactory != null) return;

            redisCs = CloudConfigurationManager.GetSetting("redisCacheConnectionString");
            var cacheSystem = (CloudConfigurationManager.GetSetting("cacheSystem") ?? "").ToLowerInvariant();
            var cacheName = CloudConfigurationManager.GetSetting("azureCacheName");
            if (cacheSystem == "azure")
            {
                cacheType = CacheType.Azure;
                var config = new DataCacheFactoryConfiguration(cacheName);

                // Default is 15 seconds - see http://msdn.microsoft.com/en-us/library/ee790816(v=azure.10).aspx
                config.RequestTimeout = TimeSpan.FromSeconds(10);

                dataCacheFactory = new DataCacheFactory(config);
            }
            else if (cacheSystem == "redis")
            {
                cacheType = CacheType.Redis;
            }
            else if (cacheSystem == "passthrough")
            {
                cacheType = CacheType.PassThrough;
            }
            else
            {
                cacheType = CacheType.Simple;
            }
        }

        public static ISimpleCache GetCache(string keyPrefix = null, TimeSpan? duration = null)
        {
            duration = duration ?? TimeSpan.FromHours(1);
            switch (cacheType)
            {
                case CacheType.Azure:
                    try
                    {
                        var cache = dataCacheFactory.GetDefaultCache();
                        return new SimpleCacheAzure(cache, keyPrefix);
                    }
                    catch (Exception ex)
                    {
                        // If we've got one error, let's bail on trying to use Azure, and just default to the simple cache.
                        cacheType = CacheType.Simple;

                        logger.Error("Unable to instantiate Azure cache; returning simple cache. Error = " + ex);
                        return new SimpleCache(duration.Value);
                    }
                case CacheType.Redis:
                    try
                    {
                        return new SimpleCacheRedis(redisCs, keyPrefix, null, duration);
                    }
                    catch (Exception ex)
                    {
                        // If we've got one error, let's bail on trying to use Azure, and just default to the simple cache.
                        cacheType = CacheType.Simple;

                        logger.Error("Unable to instantiate Redis cache; returning simple cache. Error = " + ex);
                        return new SimpleCache(duration.Value);
                    }
                case CacheType.PassThrough:
                    return new SimpleCachePassThroughProd();
            }

            // Return a short timespan because this will almost always just be used in testing
            return new SimpleCache(TimeSpan.FromMinutes(5));
        }
    }

    public enum CacheType
    {
        Azure,
        Redis,
        PassThrough,
        Simple
    }

    /// <summary>
    /// A nice little factory class for Ninject to bind to
    /// </summary>
    public class CacheProvider : Provider<ISimpleCache>
    {
        private readonly string _keyPrefix;
        private readonly TimeSpan? _duration;

        public CacheProvider(string keyPrefix = null, TimeSpan? duration = null)
        {
            _keyPrefix = keyPrefix;
            _duration = duration;
        }

        protected override ISimpleCache CreateInstance(IContext context)
        {
            return CacheFactory.GetCache(_keyPrefix, _duration);
        }
    }
}