using System;
using System.Threading.Tasks;

namespace Payboard.Common.Cache
{
    /// <summary>
    ///     An implementation of ISimpleCache that doesn't cache anything,
    ///     to help troubleshoot possible SimpleCache problems in production.
    ///     The main difference is that this production version doesn't try to
    ///     serialize anything, for performance reasons.
    /// </summary>
    public class SimpleCachePassThroughProd : SimpleCachePassThrough
    {
        public override TValue Get<TValue>(string key, Func<TValue> missingFunc)
        {
            var value = missingFunc();
            return value;
        }

        public override async Task<TValue> GetAsync<TValue>(string key, Func<Task<TValue>> missingFunc)
        {
            var value = await missingFunc();
            return value;
        }
    }
}