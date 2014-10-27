using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using MoreLinq;

namespace Payboard.Common
{
    public static class ParallelHelpers
    {
        /// <summary>
        ///     Iterate asynchronously in parallel over a data source, but only with a given degree of parallelization, so that we
        ///     don't DOS ourselves.
        /// </summary>
        public static async Task ForEachParallel<T>(this IEnumerable<T> list, Func<T, Task> action,
            int segmentSize = 100)
        {
            // Create the execution block
            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = segmentSize
            };
            var sendMessageBlock = new ActionBlock<T>(async item => { await action(item); }, options);

            // Send everything to the execution block and wait for it to finish
            list.ForEach(item => sendMessageBlock.Post(item));
            sendMessageBlock.Complete();
            await sendMessageBlock.Completion;
        }

        /// <summary>
        ///     Select asynchronously parallel from a data source, but only with a given degree of parallelization, so that we
        ///     don't DOS ourselves.
        /// </summary>
        public static async Task<List<TResult>> SelectParallel<TSource, TResult>(this IEnumerable<TSource> list,
            Func<TSource, Task<TResult>> mapFunc,
            int maxParallelization = 100)
        {
            // Create the execution block
            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxParallelization
            };
            var results = new List<TResult>();
            var sendMessageBlock = new ActionBlock<TSource>(async item =>
            {
                var result = await mapFunc(item);
                lock (results)
                {
                    results.Add(result);
                }
            }, options);

            // Send everything to the execution block and wait for it to finish
            list.ForEach(cu => sendMessageBlock.Post(cu));
            sendMessageBlock.Complete();
            await sendMessageBlock.Completion;
            return results;
        }

        /// <summary>
        ///     Iterate in parallel over a data source, but only in a given segment size, so that we don't DOS ourselves.
        /// </summary>
        /// <remarks>
        ///     This approach isn't optimal. By waiting until all the tasks from a given segment have completed, we throttle
        ///     ourselves to the slowest
        ///     running task. A better approach would be to pull the items from a queue, only allowing the given segmentSize to
        ///     execute at a time.
        ///     I'll do that later.
        /// </remarks>
        public static async Task ForEachParallelOld<T>(this IEnumerable<T> list, Func<T, Task> action,
            int segmentSize = 100)
        {
            var segments = list.ToSegmentedList(segmentSize);
            foreach (var segment in segments)
            {
                var tasks = segment.Select(async x => await action(x));
                await Task.WhenAll(tasks);
            }
        }

        /// <summary>
        ///     Select in parallel from a data source, but only in a given segment size, so that we don't DOS ourselves.
        /// </summary>
        /// <remarks>
        ///     This approach isn't optimal. By waiting until all the tasks from a given segment have completed, we throttle
        ///     ourselves to the slowest
        ///     running task. A better approach would be to pull the items from a queue, only allowing the given segmentSize to
        ///     execute at a time.
        ///     I'll do that later.
        /// </remarks>
        public static async Task<List<TResult>> SelectParallelOld<TSource, TResult>(
            this IEnumerable<TSource> list,
            Func<TSource, Task<TResult>> action,
            int segmentSize = 100)
        {
            var result = new List<TResult>();
            var segments = list.ToSegmentedList(segmentSize);
            foreach (var segment in segments)
            {
                var tasks = segment.Select(async x => await action(x)).ToList();
                await Task.WhenAll(tasks);
                result.AddRange(tasks.Select(x => x.Result));
            }
            return result;
        }
    }
}