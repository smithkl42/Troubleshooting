using System;
using System.IO;
using System.Threading.Tasks;
using Payboard.Common.Serializers;
using StackExchange.Redis;

namespace Payboard.Common.Cache
{
    public class SimpleCacheRedis : SimpleCacheBase
    {
        private readonly TimeSpan _duration;
        private readonly ISerializer _jsonSerializer;
        private readonly ISimpleCache _localCache;
        private readonly string _redisCs;

        private bool _attemptingToConnect;
        private ConnectionMultiplexer _connection;

        public SimpleCacheRedis(string redisCs, string keyPrefix = null, ISimpleCache localCache = null,
            TimeSpan? duration = null)
            : base(keyPrefix)
        {
            _localCache = localCache ?? new SimpleCache(TimeSpan.FromMinutes(5));
            _duration = duration ?? TimeSpan.FromMinutes(60);
            _jsonSerializer = new JsonSerializer();
            _redisCs = redisCs;
            CreateConnection();
        }

        private void CreateConnection()
        {
            var sw = new StringWriter();
            try
            {
                _connection = ConnectionMultiplexer.Connect(_redisCs, sw);
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to connect to redis: " + ex);
                _logger.Debug("internal log: \r\n" + sw);
                throw;
            }
        }

        private async Task CreateConnectionAsync()
        {
            if (_attemptingToConnect) return;
            var sw = new StringWriter();
            try
            {
                _attemptingToConnect = true;
                _connection = await ConnectionMultiplexer.ConnectAsync(_redisCs, sw);
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to connect to redis async: " + ex);
                _logger.Debug("internal log: \r\n" + sw);
                throw;
            }
            finally
            {
                _attemptingToConnect = false;
            }
        }

        private void HandleRedisConnectionError(Exception ex)
        {
            _logger.Error(
                "Connection error with Redis cache; recreating connection for the next try, and falling back to missingFunc() for this one. Error = " +
                ex.Message);
            Task.Run(async () =>
            {
                try
                {
                    await CreateConnectionAsync();
                }
                catch (Exception genEx)
                {
                    _logger.Error("Unable to recreate redis connection (sigh); bailing for now: " + genEx.Message);
                }
            });
        }

        public override TValue Get<TValue>(string key, Func<TValue> missingFunc)
        {
            var present = true;
            key = GetKey<TValue>(key);
            var result = _localCache.Get(key, () =>
            {
                var value = default(TValue);
                try
                {
                    var db = _connection.GetDatabase();
                    var str = db.StringGet(key);
                    if (!str.IsNullOrEmpty)
                    {
                        value = _jsonSerializer.Deserialize<TValue>(str);
                    }
                }
                catch (RedisConnectionException ex)
                {
                    HandleRedisConnectionError(ex);
                }
                catch (Exception ex)
                {
                    _logger.Error("Error retrieving item '" + key +
                                  "' from Redis cache; falling back to missingFunc(). Error = " + ex);
                }
                if (value == default(TValue))
                {
                    present = false;
                    value = missingFunc();
                    Add(key, value);
                }
                return value;
            });
            RecordLookup(present);
            return result;
        }

        public override async Task<TValue> GetAsync<TValue>(string key, Func<Task<TValue>> missingFunc)
        {
            var present = true;
            key = GetKey<TValue>(key);
            var result = await _localCache.GetAsync(key, async () =>
            {
                var value = default(TValue);
                try
                {
                    var db = _connection.GetDatabase();
                    var str = await db.StringGetAsync(key);
                    if (!str.IsNullOrEmpty)
                    {
                        value = _jsonSerializer.Deserialize<TValue>(str);
                    }
                }
                catch (RedisConnectionException ex)
                {
                    HandleRedisConnectionError(ex);
                }
                catch (Exception ex)
                {
                    _logger.Error("Error retrieving item '" + key +
                                  "' from Redis cache; falling back to missingFunc(). Error = " + ex);
                }
                if (value == default(TValue))
                {
                    present = false;
                    value = await missingFunc();
                    await PerformAddAsync(key, value);
                }
                return value;
            });
            RecordLookup(present);
            return result;
        }

        public override void Add<TValue>(string key, TValue value)
        {
            key = GetKey<TValue>(key);
            PerformAdd(key, value);
        }

        public override bool Remove<TValue>(string key)
        {
            try
            {
                key = GetKey<TValue>(key);
                _localCache.Remove<TValue>(key);
                var db = _connection.GetDatabase();
                db.KeyDelete(key);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Warn("Error removing key {0} from Redis cache: {1}", key, ex.Message);
                return false;
            }
        }

        public override bool TryGetValue<TValue>(string key, out TValue value)
        {
            key = GetKey<TValue>(key);
            value = default(TValue);
            try
            {
                if (!_localCache.TryGetValue(key, out value))
                {
                    var db = _connection.GetDatabase();
                    var str = db.StringGet(key);
                    if (!str.IsNullOrEmpty)
                    {
                        value = _jsonSerializer.Deserialize<TValue>(str);
                        if (value != default(TValue))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (RedisConnectionException ex)
            {
                HandleRedisConnectionError(ex);
            }
            catch (Exception ex)
            {
                _logger.Warn("Error retrieving key {0} from Redis cache: {1}", key, ex.Message);
            }
            return false;
        }

        private void PerformAdd<TValue>(string key, TValue value) where TValue : class
        {
            if (value == default(TValue)) return;
            _localCache.Add(key, value);
            try
            {
                var db = _connection.GetDatabase();
                var str = _jsonSerializer.SerializeToString(value);
                db.StringSet(key, str, _duration);
            }
            catch (RedisConnectionException ex)
            {
                HandleRedisConnectionError(ex);
            }
            catch (TimeoutException ex)
            {
                _logger.Warn("Timed out trying to add item to redis cache: " + ex);
            }
        }

        private async Task PerformAddAsync<TValue>(string key, TValue value) where TValue : class
        {
            if (value == default(TValue)) return;
            _localCache.Add(key, value);
            try
            {
                var db = _connection.GetDatabase();
                var str = _jsonSerializer.SerializeToString(value);
                await db.StringSetAsync(key, str, _duration);
            }
            catch (RedisConnectionException ex)
            {
                HandleRedisConnectionError(ex);
            }
            catch (TimeoutException ex)
            {
                _logger.Warn("Timed out trying to add item to redis cache: " + ex);
            }
        }
    }
}