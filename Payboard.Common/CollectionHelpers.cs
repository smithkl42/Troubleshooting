using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payboard.Common
{
    public static class CollectionHelpers
    {
        public static IList<T> AsList<T>(this IEnumerable<T> list)
        {
            var asList = list as IList<T>;
            if (asList != null) return asList;
            return list.ToList();
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        /// <summary>
        ///     Shuffle a list randomly. See http://stackoverflow.com/a/1262619/68231.
        /// </summary>
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public static List<List<T>> ToSegmentedList<T>(this IEnumerable<T> enumerable, int segmentSize)
        {
            var segments = new List<List<T>>();
            var currentList = new List<T>();
            segments.Add(currentList);
            var i = 0;
            foreach (var item in enumerable)
            {
                if (i++ % segmentSize == 0)
                {
                    if (currentList.Count > 0)
                    {
                        currentList = new List<T>();
                        segments.Add(currentList);
                    }
                }
                currentList.Add(item);
            }
            return segments;
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
            Func<TValue> missingFunc)
        {
            TValue value;
            if (!dict.TryGetValue(key, out value))
            {
                value = missingFunc();
                dict[key] = value;
            }
            return value;
        }

        public static async Task<TValue> GetOrCreateAsync<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
            Func<Task<TValue>> missingFunc)
        {
            TValue value;
            if (!dict.TryGetValue(key, out value))
            {
                value = await missingFunc();
                dict[key] = value;
            }
            return value;
        }

        public static void SetOrIncrement<TKey>(this IDictionary<TKey, int> dict, TKey key)
        {
            int value;
            if (dict.TryGetValue(key, out value))
            {
                value++;
            }
            else
            {
                value = 1;
            }
            dict[key] = value;
        }

        /// <summary>
        ///     A threadsafe way to merge the contents of one dictionary into the contents of another.
        /// </summary>
        /// <typeparam name="TKey">The type of the dictionary key</typeparam>
        /// <typeparam name="TValue">The type of the dictionary value</typeparam>
        /// <param name="sourceDict">The source dictionary (cannot be null)</param>
        /// <param name="targetDict">The target dictionary (cannot be null)</param>
        /// <param name="mergeAction">
        ///     The action that will actually merge the item from the source dictionary into the item from
        ///     the target dictionary
        /// </param>
        public static void MergeInto<TKey, TValue>(this IDictionary<TKey, TValue> sourceDict,
            IDictionary<TKey, TValue> targetDict, Action<TValue, TValue> mergeAction) where TValue : class, new()
        {
            // Merge the analysisDict into the main context dictionary.
            foreach (var sourceEntry in sourceDict)
            {
                TValue targetValue;
                lock (targetDict)
                {
                    targetValue = targetDict.GetOrCreate(sourceEntry.Key,
                        () => new TValue());
                }

                lock (targetValue)
                {
                    mergeAction(sourceEntry.Value, targetValue);
                }
            }
        }

        public static List<TElement> ToUnionDistinctWith<TElement, TIdentifier>(this IEnumerable<TElement> list1,
            IEnumerable<TElement> list2, Func<TElement, TIdentifier> identifier)
        {
            var ids = new HashSet<TIdentifier>();
            var results = new List<TElement>();
            foreach (var element in list1)
            {
                var id = identifier(element);
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                    results.Add(element);
                }
            }
            foreach (var element in list2)
            {
                var id = identifier(element);
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                    results.Add(element);
                }
            }
            return results;
        }
    }
}