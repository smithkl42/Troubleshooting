using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Payboard.Common.Tests
{
    public static class ShouldHelper
    {
        public static void ShouldHaveMaxDiff(this DateTime dt1, DateTime dt2, double seconds)
        {
            var diff = Math.Abs((dt1.ToUniversalTime() - dt2.ToUniversalTime()).TotalSeconds);
            Assert.IsTrue(diff <= seconds);
        }

        public static void ShouldHaveMaxDiff(this DateTime? dt1, DateTime dt2, double seconds)
        {
            Assert.IsNotNull(dt1);
            var diff = Math.Abs((dt1.Value.ToUniversalTime() - dt2.ToUniversalTime()).TotalSeconds);
            Assert.IsTrue(diff <= seconds);
        }

        public static void ShouldNotBeNullOrWhitespace(this string s)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(s));
        }

        public static void ShouldBeNullOrWhitespace(this string s)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(s));
        }

        public static IList<T> ShouldBeOrderedBy<T>(this IList<T> list, Func<T, IComparable> sortFunc)
        {
            var sorted = list.OrderBy(sortFunc).ToList();
            for (var i = 0; i < sorted.Count; i++)
            {
                Assert.AreEqual(list[i], sorted[i], "The list was not correctly sorted");
            }
            return list;
        }

        public static IList<T> ShouldBeOrderedByDescending<T>(this IList<T> list, Func<T, IComparable> sortFunc)
        {
            var sorted = list.OrderByDescending(sortFunc).ToList();
            for (var i = 0; i < sorted.Count; i++)
            {
                Assert.AreEqual(list[i], sorted[i], "The list was not correctly sorted");
            }
            return list;
        }

    }
}