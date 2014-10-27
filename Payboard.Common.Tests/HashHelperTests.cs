using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class HashHelperTests
    {
        [TestMethod]
        public void GetIntHash_ShouldReturnSameForSameValues()
        {
            0.GetIntHash().ShouldEqual(0.GetIntHash());
            100.Times(x => x.GetIntHash().ShouldEqual(x.GetIntHash()));
            int.MaxValue.GetIntHash().ShouldEqual(int.MaxValue.GetIntHash());
        }

        [TestMethod]
        public void GetIntHash_ShouldBeGoodAndRandom()
        {
            var arr = new int[100];
            for (var i = 0; i < 10000; i++)
            {
                arr[Math.Abs(i.GetIntHash() % 100)]++;
            }
            for (var i = 0; i < 100; i++)
            {
                arr[i].ShouldBeGreaterThan(50);
                arr[i].ShouldBeLessThan(200);
            }
        }

        [TestMethod]
        public void GetBucket_ShouldBeGoodAndRandom()
        {
            const int bucketSize = 100;
            var arr = new int[bucketSize];
            for (var i = 0; i < 10000; i++)
            {
                arr[i.GetBucket(bucketSize)]++;
            }
            for (var i = 0; i < 100; i++)
            {
                arr[i].ShouldBeGreaterThan(bucketSize / 2);
                arr[i].ShouldBeLessThan(bucketSize * 2);
            }
        }

        [TestMethod]
        public void GetBucket_ForGuid_ShouldBeGoodAndRandom()
        {
            const int bucketSize = 100;
            var arr = new int[bucketSize];
            for (var i = 0; i < 10000; i++)
            {
                arr[Guid.NewGuid().GetBucket(bucketSize)]++;
            }
            for (var i = 0; i < 100; i++)
            {
                arr[i].ShouldBeGreaterThan(bucketSize / 2);
                arr[i].ShouldBeLessThan(bucketSize * 2);
            }
        }

        [TestMethod]
        public void GetBucket_ForGuid_ShouldBeDeterministic()
        {
            var guid1 = new Guid("5B96C2C3-E76D-4CFD-88F9-0000E8EEE28F");
            var guid2 = new Guid("5B96C2C3-E76D-4CFD-88F9-0000E8EEE28F");
            guid1.GetBucket(100).ShouldEqual(guid2.GetBucket(100));
        }

    }
}