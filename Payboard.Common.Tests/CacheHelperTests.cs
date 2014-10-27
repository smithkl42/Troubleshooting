using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payboard.Common.Cache;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class CacheHelperTests
    {
        [TestInitialize]
        public void Setup()
        {
            var now = DateTime.UtcNow;
            SystemTime.UtcNow = () => now;
        }

        [TestMethod]
        public void GetKey_ShouldSupportStrings()
        {
            var key = CacheHelper.GetKey("part1", "part2", "part3");
            key.ShouldEqual("part1.part2.part3");
        }

        [TestMethod]
        public void GetKey_ShouldSupportInts()
        {
            var key = CacheHelper.GetKey(1, 2, 3);
            key.ShouldEqual("1.2.3");
        }

        [TestMethod]
        public void GetKey_ShouldSupportDates()
        {
            var date1 = SystemTime.UtcNow();
            var date2 = SystemTime.UtcNow().AddDays(-1);
            var key = CacheHelper.GetKey(date1, date2);
            key.ShouldEqual(date1 + "." + date2);
        }
    }
}
