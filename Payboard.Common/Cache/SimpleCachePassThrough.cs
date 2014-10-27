using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Payboard.Common.Cache
{
    /// <summary>
    ///     An implementation of ISimpleCache that doesn't cache anything, to simplify testing.
    /// </summary>
    public class SimpleCachePassThrough : SimpleCacheBase
    {
        public SimpleCachePassThrough() : base(null) { }

        public SimpleCachePassThrough(string keyPrefix = null)
            : base(keyPrefix)
        {
        }

        public override bool Remove<TValue>(string key)
        {
            return true;
        }

        public override bool TryGetValue<TValue>(string key, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        public override TValue Get<TValue>(string key, Func<TValue> missingFunc)
        {
            var value = missingFunc();

            // Confirm that the value can be serialized.
            // The AzureCache system uses a NetDataContractSerializer (by default) which doesn't deal well with
            // certain classes of objects. This gives us a better chance of catching those objects before we
            // go to production.
            using (var stream = new MemoryStream())
            {
                var serializer = new NetDataContractSerializer();
                serializer.WriteObject(stream, value);
            }

            Add(key, value);
            return value;
        }

        public async override Task<TValue> GetAsync<TValue>(string key, Func<Task<TValue>> missingFunc)
        {
            var value = await missingFunc();

            // Confirm that the value can be serialized.
            // The AzureCache system uses a NetDataContractSerializer (by default) which doesn't deal well with
            // certain classes of objects. This gives us a better chance of catching those objects before we
            // go to production.
            using (var stream = new MemoryStream())
            {
                var serializer = new NetDataContractSerializer();
                serializer.WriteObject(stream, value);
            }

            return value;
        }

        public override void Add<TValue>(string key, TValue value)
        {
        }
    }
}