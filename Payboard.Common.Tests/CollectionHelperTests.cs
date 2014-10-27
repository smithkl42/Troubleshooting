using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class CollectionHelperTests
    {
        [TestMethod]
        public void ToSegmentedList_ShouldSegmentList()
        {
            var list = new List<string>();
            1000.Times(x => list.Add(x.ToString()));

            var segments = list.ToSegmentedList(99);

            segments.Count.ShouldEqual(11);
            segments.Take(10).All(x => x.Count == 99).ShouldBeTrue();
            segments[10].Count.ShouldEqual(10);
        }
    }
}