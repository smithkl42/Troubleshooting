using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class ParallelHelperTests
    {
        [TestMethod]
        public async Task ForEachParallel_ShouldExecuteAllActions()
        {
            var data = new List<int>();
            100.Times(data.Add);
            var iterations = 0;
            await data.ForEachParallel(x => Task.Run(() =>
            {
                Interlocked.Increment(ref iterations);
            }));
            iterations.ShouldEqual(data.Count);
        }

        [TestMethod]
        public async Task ForEachParallel_ShouldExecuteWithMaxParallelization()
        {
            var data = new List<int>();
            100.Times(data.Add);
            var executing = 0;
            var iterations = 0;
            const int maxParallelization = 10;
            await data.ForEachParallel(async x => 
            {
                Interlocked.Increment(ref executing);
                Interlocked.Increment(ref iterations);
                await Task.Delay(100);
                executing.ShouldBeLessThanOrEqualTo(maxParallelization);
                Interlocked.Decrement(ref executing);
            }, maxParallelization);
            iterations.ShouldEqual(data.Count);
        }

        [TestMethod]
        public async Task SelectParallel_ShouldSelectAllData()
        {
            var data = new List<int>();
            100.Times(data.Add);

            var result = (await data.SelectParallel(Task.FromResult)).OrderBy(x => x).ToList();

            for (var i = 0; i < result.Count; i++)
            {
                result[i].ShouldEqual(i);
            }
        }

        [TestMethod]
        public async Task SelectParallel_ShouldSelectWithMaxParallelization()
        {
            var data = new List<int>();
            100.Times(data.Add);
            var executing = 0;
            const int maxParallelization = 10;
            var result = await data.SelectParallel(async x => 
            {
                Interlocked.Increment(ref executing);
                await Task.Delay(100);
                executing.ShouldBeLessThanOrEqualTo(maxParallelization);
                Interlocked.Decrement(ref executing);
                return x;
            }, maxParallelization);
            result.Count.ShouldEqual(data.Count);
        }
    }
}
