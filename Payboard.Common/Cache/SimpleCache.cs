using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using NLog;
using Timer = System.Timers.Timer;

namespace Payboard.Common.Cache
{
    public interface ISimpleCache
    {
        TValue Get<TValue>(string key, Func<TValue> missingFunc) where TValue : class;
        Task<TValue> GetAsync<TValue>(string key, Func<Task<TValue>> missingFunc) where TValue : class;
        void Add<TValue>(string key, TValue value) where TValue : class;
        bool Remove<TValue>(string key) where TValue : class;
        bool TryGetValue<TValue>(string key, out TValue value) where TValue : class;
    }

    public abstract class SimpleCacheBase : ISimpleCache
    {
        protected string _keyPrefix;
        protected int failedLookups;
        protected int successfulLookups;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected SimpleCacheBase(string keyPrefix = null)
        {
            _keyPrefix = keyPrefix;
        }

        public virtual string GetKey<TValue>(string key)
        {
            var typeName = typeof(TValue).GetFriendlyTypeName();
            if (!string.IsNullOrWhiteSpace(_keyPrefix))
            {
                return _keyPrefix + "." + typeName + "." + key;
            }
            return typeName + "." + key;
        }

        protected void RecordLookup(bool present)
        {
            if (present)
            {
                Interlocked.Increment(ref successfulLookups);
            }
            else
            {
                Interlocked.Increment(ref failedLookups);
            }
            var totalLookups = successfulLookups + failedLookups;
            totalLookups.AtDebugIntervals(x => _logger.Debug("{0} stats: {1} lookups; {2} successful; {3} failed; {4:P} success rate",
                GetType().Name,
                totalLookups, successfulLookups, failedLookups,
                successfulLookups / (double)totalLookups));
        }

        public abstract TValue Get<TValue>(string key, Func<TValue> missingFunc) where TValue : class;
        public abstract Task<TValue> GetAsync<TValue>(string key, Func<Task<TValue>> missingFunc) where TValue : class;
        public abstract void Add<TValue>(string key, TValue value) where TValue : class;
        public abstract bool Remove<TValue>(string key) where TValue : class;
        public abstract bool TryGetValue<TValue>(string key, out TValue value) where TValue : class;
    }

    public class SimpleCache : SimpleCacheBase, IDisposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<string, CacheValue> _internalCache =
            new ConcurrentDictionary<string, CacheValue>();

        private Timer _cleanupTimer;

        public SimpleCache(TimeSpan expiration, string keyPrefix = null)
            : base(keyPrefix)
        {
            Expiration = expiration;
#if DEBUG
            _cleanupTimer = new Timer(5000);
#else
            _cleanupTimer = new Timer(60000);
#endif
            _cleanupTimer.Elapsed += CleanupTimer_Elapsed;
            _cleanupTimer.Start();
        }

        public TimeSpan Expiration { get; set; }

        public override void Add<TValue>(string key, TValue value)
        {
            key = GetKey<TValue>(key);
            _internalCache[key] = new CacheValue(value, SystemTime.UtcNow() + Expiration);
        }

        public override bool Remove<TValue>(string key)
        {
            key = GetKey<TValue>(key);
            CacheValue value;
            return _internalCache.TryRemove(key, out value);
        }

        public override bool TryGetValue<TValue>(string key, out TValue value)
        {
            key = GetKey<TValue>(key);
            return TryGetValueInternal(key, out value);
        }

        private bool TryGetValueInternal<TValue>(string key, out TValue value) where TValue : class
        {
            CacheValue cacheValue;
            var present = _internalCache.TryGetValue(key, out cacheValue);
            if (present && cacheValue.ExpiresOn < SystemTime.UtcNow())
            {
                _internalCache.TryRemove(key, out cacheValue);
                present = false;
            }
            value = present ? (TValue)cacheValue.Value : null;
            if (present)
            {
                Interlocked.Increment(ref successfulLookups);
            }
            else
            {
                Interlocked.Increment(ref failedLookups);
            }
            var totalLookups = successfulLookups + failedLookups;
            totalLookups.AtDebugIntervals(x => logger.Debug("Simple cache stats for {0}: {1} lookups; {2} successful; {3} failed; {4:P} success rate",
                typeof(TValue).Name, totalLookups, successfulLookups, failedLookups,
                successfulLookups / (double)totalLookups));
            return present;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_cleanupTimer != null)
                {
                    _cleanupTimer.Elapsed -= CleanupTimer_Elapsed;
                    _cleanupTimer.Stop();
                    _cleanupTimer = null;
                }
            }
        }

        private void CleanupTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var itemsToRemove = _internalCache.Where(kvp => kvp.Value.ExpiresOn < SystemTime.UtcNow()).ToList();
                if (itemsToRemove.Count == 0)
                {
                    return;
                }
                foreach (var keyValuePair in itemsToRemove)
                {
                    CacheValue value;
                    _internalCache.TryRemove(keyValuePair.Key, out value);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error cleaning up cache: " + ex);
            }
        }

        public override TValue Get<TValue>(string key, Func<TValue> missingFunc)
        {
            key = GetKey<TValue>(key);
            TValue value;
            if (!TryGetValueInternal(key, out value))
            {
                value = missingFunc();
                var cacheValue = new CacheValue(value, SystemTime.UtcNow() + Expiration);
                _internalCache[key] = cacheValue;
            }
            return value;
        }

        public async override Task<TValue> GetAsync<TValue>(string key, Func<Task<TValue>> missingFunc)
        {
            key = GetKey<TValue>(key);
            TValue value;
            if (!TryGetValueInternal(key, out value))
            {
                value = await missingFunc();
                if (value != default(TValue))
                {
                    var cacheValue = new CacheValue(value, SystemTime.UtcNow() + Expiration);
                    _internalCache[key] = cacheValue;
                }
            }
            return value;
        }
    }

    public class CacheValue : IEquatable<CacheValue>
    {
#if DEBUG
        private static readonly TimeSpan defaultExpiration = TimeSpan.FromSeconds(5);
#else
        private static readonly TimeSpan defaultExpiration = TimeSpan.FromSeconds(60);
#endif

        public CacheValue(object value)
            : this(value, SystemTime.UtcNow() + defaultExpiration)
        {
        }

        public CacheValue(object value, DateTime expiration)
        {
            Value = value;
            ExpiresOn = expiration;
        }

        public DateTime ExpiresOn { get; set; }
        public object Value { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(CacheValue))
            {
                return false;
            }
            return Equals((CacheValue)obj);
        }

        public bool Equals(CacheValue other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.Value, Value);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public static bool operator ==(CacheValue left, CacheValue right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CacheValue left, CacheValue right)
        {
            return !Equals(left, right);
        }
    }
}