using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payboard.Common.Cache;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class SimpleCacheTests
    {
        [TestMethod]
        public void SimpleCachePassThrough_ShouldCacheSerializableObjects()
        {
            var cache = new SimpleCachePassThrough();
            var obj = cache.Get("key", () => new Dictionary<string, object>());
            obj.ShouldNotBeNull();
        }

        /// <summary>
        ///     The AzureCache system uses a NetDataContractSerializer (by default) which doesn't deal well with
        ///     certain classes of objects. This gives us a better chance of catching those objects before we
        ///     go to production.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof (InvalidDataContractException))]
        public void SimpleCachePassThrough_ShouldNotCacheUnserializableObjects()
        {
            var cache = new SimpleCachePassThrough();
            cache.Get("key", () =>
            {
                IDictionary<string, object> stored = new ExpandoObject();
                stored["key"] = "Some other object";
                return new List<IDictionary<string, object>> {stored};
            });
        }

        [TestMethod]
        public async Task SimpleCache_ShouldBeThreadsafe()
        {
            var cache = new SimpleCachePassThrough();

            var tasks = new List<Task>();
            for (var i = 0; i < 1000; i++)
            {
                var task = Task.Run(() => 100.Times(j =>
                {
                    cache.Add("SomeKey", "SomeValue");
                    cache.Remove<string>("SomeKey");
                }));
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);

            string value;
            cache.TryGetValue("SomeKey", out value).ShouldBeFalse();
        }

        [TestMethod]
        public void GetKey_ShouldPrependTypeName()
        {
            var cache = new SimpleCachePassThrough();
            cache.GetKey<string>("somevalue").ShouldEqual("String.somevalue");
        }

        [TestMethod]
        public void GetKey_ShouldPrependKeyPrefix()
        {
            var cache = new SimpleCachePassThrough("someKeyPrefix");
            cache.GetKey<string>("somevalue").ShouldEqual("someKeyPrefix.String.somevalue");
        }

        [TestMethod]
        public void GetKey_ShouldPrependGenericArgs()
        {
            var cache = new SimpleCachePassThrough("someKeyPrefix");
            cache.GetKey<Dictionary<string, object>>("somevalue")
                .ShouldEqual("someKeyPrefix.Dictionary<String, Object>.somevalue");
        }
    }
}