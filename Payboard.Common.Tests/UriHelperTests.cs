using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class UriHelperTests
    {
        #region SortOfMatches()
        [TestMethod]
        public void SortOfMatches_ShouldMatchSameUrls()
        {
            var uri1 = new Uri("http://www.payboard.com/test");
            var uri2 = new Uri("http://www.payboard.com/test");
            uri1.SortOfMatches(uri2).ShouldBeTrue();
        }

        [TestMethod]
        public void SortOfMatches_ShouldIgnoreQueryStrings()
        {
            var uri1 = new Uri("http://www.payboard.com/test?test1");
            var uri2 = new Uri("http://www.payboard.com/test?test2");
            uri1.SortOfMatches(uri2).ShouldBeTrue();
        }

        [TestMethod]
        public void SortOfMatches_ShouldIgnoreShortPrefixes()
        {
            var uri1 = new Uri("http://www.payboard.com/test?test1");
            var uri2 = new Uri("http://payboard.com/test?test2");
            uri1.SortOfMatches(uri2).ShouldBeTrue();
        }

        [TestMethod]
        public void SortOfMatches_ShouldNotMatchOnPathMismatch()
        {
            var uri1 = new Uri("http://www.payboard.com/test1");
            var uri2 = new Uri("http://payboard.com/test2");
            uri1.SortOfMatches(uri2).ShouldBeFalse();
        }

        [TestMethod]
        public void SortOfMatches_ShouldNotMatchOnHostMismatch()
        {
            var uri1 = new Uri("http://www.payboard1.com/");
            var uri2 = new Uri("http://www.payboard2.com/");
            uri1.SortOfMatches(uri2).ShouldBeFalse();
        }
        #endregion

        #region MainDomain()
        [TestMethod]
        public void MainDomain_ShouldRemoveWww()
        {
            var main = (new Uri("http://www.payboard.com")).MainDomain();
            main.ShouldEqual("payboard.com");
        }

        [TestMethod]
        public void MainDomain_ShouldRemoveApp()
        {
            var main = (new Uri("http://app.payboard.com")).MainDomain();
            main.ShouldEqual("payboard.com");
        }

        [TestMethod]
        public void MainDomain_ShouldNotRemoveExample()
        {
            var main = (new Uri("http://example.payboard.com")).MainDomain();
            main.ShouldEqual("example.payboard.com");
        }

        [TestMethod]
        public void MainDomain_ShouldNotRemoveMainDomain()
        {
            var main = (new Uri("http://payboard.com")).MainDomain();
            main.ShouldEqual("payboard.com");
        }

        [TestMethod]
        public void MainDomain_ShouldHandleCountryDomains()
        {
            var main = (new Uri("http://example.co.uk")).MainDomain();
            main.ShouldEqual("example.co.uk");
            main = (new Uri("http://www.example.co.uk")).MainDomain();
            main.ShouldEqual("example.co.uk");
        }
        #endregion
    }
}
