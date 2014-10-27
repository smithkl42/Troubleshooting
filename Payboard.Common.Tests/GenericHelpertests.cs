using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class GenericHelperTests
    {
        [TestMethod]
        public void OrIfGreater_ShouldSelectGreaterValue()
        {
            var less = new DateTime(2000, 1, 1);
            var greater = new DateTime(2001, 1, 1);

            less.OrIfGreater(greater).ShouldEqual(greater);
            greater.OrIfGreater(less).ShouldEqual(greater);
        }

        [TestMethod]
        public void OrIfGreater_ShouldSelectRealValue()
        {
            DateTime? real = new DateTime(2000, 1, 1);
            DateTime? fake = null;

            // ReSharper disable ExpressionIsAlwaysNull
            real.OrIfGreater(fake).ShouldEqual(real);
            fake.OrIfGreater(real).ShouldEqual(real);
            // ReSharper restore ExpressionIsAlwaysNull
        }

        [TestMethod]
        public void OrIfLesser_ShouldSelectGreaterValue()
        {
            var less = new DateTime(2000, 1, 1);
            var greater = new DateTime(2001, 1, 1);

            less.OrIfLesser(greater).ShouldEqual(less);
            greater.OrIfLesser(less).ShouldEqual(less);
        }

        [TestMethod]
        public void OrIfLesser_ShouldSelectRealValue()
        {
            DateTime? real = new DateTime(2000, 1, 1);
            DateTime? fake = null;

            // ReSharper disable ExpressionIsAlwaysNull
            real.OrIfLesser(fake).ShouldEqual(real);
            fake.OrIfLesser(real).ShouldEqual(real);
            // ReSharper restore ExpressionIsAlwaysNull
        }
    }
}
