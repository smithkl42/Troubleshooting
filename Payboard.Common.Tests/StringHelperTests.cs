using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class StringHelperTests
    {
        #region SubstringSafe
        [TestMethod]
        public void SubstringSafe_NullString()
        {
            string s = null;
            var result = s.SubstringSafe(0, 100);
            Assert.AreEqual(result, string.Empty);
        }

        [TestMethod]
        public void SubstringSafe_EmptyString()
        {
            var s = string.Empty;
            var result = s.SubstringSafe(0, 100);
            Assert.AreEqual(result, string.Empty);
        }

        [TestMethod]
        public void SubstringSafe_StartOutOfRange()
        {
            const string s = "a";
            var result = s.SubstringSafe(10, 100);
            Assert.AreEqual(result, string.Empty);
        }

        [TestMethod]
        public void SubstringSafe_LengthOutOfRange()
        {
            const string s = "1234567890";
            var result = s.SubstringSafe(5, 10);
            Assert.AreEqual("67890", result);
        }

        [TestMethod]
        public void SubstringSafe_Normal()
        {
            const string s = "1234567890";
            var result = s.SubstringSafe(5, 3);
            Assert.AreEqual("678", result);
        }
        #endregion

        #region Obsolete
        [TestMethod]
        public void AddHtmlParagraphs()
        {
            const string s = "paragraph1\r\nparagraph2\r\nparagraph3";
            var html = s.AddHtmlParagraphs();
            html.ShouldEqual("<p>paragraph1</p>\r\n<p>paragraph2</p>\r\n<p>paragraph3</p>\r\n");
        }

        [TestMethod]
        public void AddHtmlParagraphs_ShouldHandleNullString()
        {
            string s = null;
            var html = s.AddHtmlParagraphs();
            html.ShouldEqual("<p></p>\r\n");
        }

        [TestMethod]
        public void FixUrls_MatchWww()
        {
            const string s = "This is a URL: www.google.com";
            var actual = s.FixUrls();
            actual.ShouldEqual("This is a URL: <a href=\"http://www.google.com\">www.google.com</a>");
        }

        [TestMethod]
        public void FixUrls_MatchHttp()
        {
            const string s = "This is a URL: http://www.google.com";
            var actual = s.FixUrls();
            actual.ShouldEqual("This is a URL: <a href=\"http://www.google.com\">http://www.google.com</a>");
        }

        [TestMethod]
        public void FixUrls_MatchHttps()
        {
            const string s = "This is a URL: https://www.google.com/path?and=query";
            var actual = s.FixUrls();
            actual.ShouldEqual(
                "This is a URL: <a href=\"https://www.google.com/path?and=query\">https://www.google.com/path?and=query</a>");
        }

        [TestMethod]
        public void FixUrls_MatchUpperCase()
        {
            const string s = "This is a URL: HTTPS://www.google.com/path?and=query";
            var actual = s.FixUrls();
            actual.ShouldEqual(
                "This is a URL: <a href=\"HTTPS://www.google.com/path?and=query\">HTTPS://www.google.com/path?and=query</a>");
        }

        [TestMethod]
        public void FixUrls_DontFixAnchors()
        {
            const string s = "This is an anchor: <a href=\"http://www.google.com/path?and=query\">Google</a> and some other stuff";
            var actual = s.FixUrls();
            actual.ShouldEqual(
                "This is an anchor: <a href=\"http://www.google.com/path?and=query\">Google</a> and some other stuff");
        }

        [TestMethod]
        public void FixUrls_DontFixAnchorsWithUrls()
        {
            const string s = "This is an anchor: <a href=\"http://www.google.com/path?and=query\">http://www.google.com</a> and some other stuff";
            var actual = s.FixUrls();
            actual.ShouldEqual(
                "This is an anchor: <a href=\"http://www.google.com/path?and=query\">http://www.google.com</a> and some other stuff");
        }

        [TestMethod]
        public void FixUrls_DontFixAnchorsWithSingleQuotes()
        {
            const string s = "This is an anchor: <a href='http://www.google.com/path?and=query\'>http://www.google.com</a> and some other stuff";
            var actual = s.FixUrls();
            actual.ShouldEqual(
                "This is an anchor: <a href=\'http://www.google.com/path?and=query\'>http://www.google.com</a> and some other stuff");
        }

        [TestMethod]
        public void FixUrls_DontFixAnchorsWithUrlsAndSpaces()
        {
            const string s = "This is an anchor: <a href=\"http://www.google.com/path?and=query\"> http://www.google.com </a> and some other stuff";
            var actual = s.FixUrls();
            actual.ShouldEqual(
                "This is an anchor: <a href=\"http://www.google.com/path?and=query\"> http://www.google.com </a> and some other stuff");
        }

        [TestMethod]
        public void FixUrls_MatchMultiple()
        {
            const string s = "This is a URL: www.google.com https://www.google.com http://www.google.com";
            var actual = s.FixUrls();
            actual.ShouldEqual("This is a URL: <a href=\"http://www.google.com\">www.google.com</a> " +
                               "<a href=\"https://www.google.com\">https://www.google.com</a> " +
                               "<a href=\"http://www.google.com\">http://www.google.com</a>");
        }

        [TestMethod]
        public void FixUrls_MatchAfterElement()
        {
            const string s = "This is a URL: <a href=\"http://www.google.com\">www.google.com</a> www.google.com";
            var actual = s.FixUrls();
            actual.ShouldEqual("This is a URL: <a href=\"http://www.google.com\">www.google.com</a> " +
                               "<a href=\"http://www.google.com\">www.google.com</a>");
        }

        [TestMethod]
        public void FixUrls_MultipleTimes()
        {
            const string start = "This is a URL: http://www.google.com";
            const string expected = "This is a URL: <a href=\"http://www.google.com\">http://www.google.com</a>";
            var actual = start;
            for (var i = 0; i < 5; i++)
            {
                actual = actual.FixUrls();
                actual.ShouldEqual(expected);
            }
        }

        [TestMethod]
        public void FixUrls_HandlesNull()
        {
            string s = null;
            var actual = s.FixUrls();
            actual.ShouldBeNull();
        }
        #endregion

        #region ReplaceNonAlphaNumerics
        [TestMethod]
        public void ReplaceNonAlphaNumerics_ShouldReplacePunctuation()
        {
            const string s = "abcdefg12345!@#$%\t\r\nabcdefg12345";
            const string expected = "abcdefg12345________abcdefg12345";
            var actual = s.ReplaceNonAlphaNumerics();
            actual.ShouldEqual(expected);
        }

        [TestMethod]
        public void ReplaceNonAlphaNumerics_ShouldReplaceSpaces()
        {
            const string s = "Key With ~!@#$%^&*()";
            const string expected = "Key_With____________";
            var actual = s.ReplaceNonAlphaNumerics();
            actual.ShouldEqual(expected);
        }

        [TestMethod]
        public void TrimStartNonAlphaNumerics_ShouldTrimStartNonAlphanumerics()
        {
            const string s = "~!@#$%abcdefg^&*()";
            s.TrimStartNonAlphaNumeric().ShouldEqual("abcdefg^&*()");
        }

        [TestMethod]
        public void TrimEndNonAlphaNumerics_ShouldTrimEndNonAlphanumerics()
        {
            const string s = "~!@#$%abcdefg^&*()";
            s.TrimEndNonAlphaNumeric().ShouldEqual("~!@#$%abcdefg");
        }

        [TestMethod]
        public void TrimAlphaNumerics_ShouldTrimNonAlphanumerics()
        {
            const string s = "~!@#$%abcdefg^&*()";
            s.TrimNonAlphaNumeric().ShouldEqual("abcdefg");
        }

        [TestMethod]
        public void TrimAlphaNumerics_ShouldHandleAllAlphas()
        {
            const string s = "abcdefghijklmnop";
            s.TrimEndNonAlphaNumeric().ShouldEqual(s);
        }

        [TestMethod]
        public void TrimAlphaNumerics_ShouldHandleAllNonAlphas()
        {
            const string s = "~!@#$%^&*()";
            s.TrimEndNonAlphaNumeric().ShouldEqual(string.Empty);
        }
        #endregion

        #region IsValidEmail
        [TestMethod]
        public void IsValidEmail_ShouldRecognizeValidEmails()
        {
            "bob@gmail.com".IsValidEmail().ShouldBeTrue();
            "bob@gmail".IsValidEmail().ShouldBeTrue();
            "Bob Jones <bob@gmail.com>".IsValidEmail().ShouldBeTrue();
        }

        [TestMethod]
        public void IsValidEmail_ShouldNotRecognizeInvalidEmails()
        {
            "".IsValidEmail().ShouldBeFalse();
            "bob".IsValidEmail().ShouldBeFalse();
            "Bob Jones <bob@gmail.com".IsValidEmail().ShouldBeFalse();
            ((string)null).IsValidEmail().ShouldBeFalse();
        }
        #endregion

        #region SmartBraceSplit

        [TestMethod]
        public void SmartBraceSplit_ShouldSplitOnNormalBraces()
        {
            var splits = "{FirstName} {LastName}".SmartBraceSplit();
            splits.Count.ShouldEqual(2);
            splits[0].ShouldEqual("{FirstName}");
            splits[1].ShouldEqual(" {LastName}");
        }

        [TestMethod]
        public void SmartBraceSplit_ShouldSplitOnMatchedBraces()
        {
            var splits = "{FirstName {Bob}} {LastName {Smith} {SomethingElse}}".SmartBraceSplit();
            splits.Count.ShouldEqual(2);
            splits[0].ShouldEqual("{FirstName {Bob}}");
            splits[1].ShouldEqual(" {LastName {Smith} {SomethingElse}}");
        }

        [TestMethod]
        public void SmartBraceSplit_ShouldIncludeTrailingData()
        {
            var splits = "{FirstName {Bob}} {LastName {Smith} {SomethingElse}} ... and some more".SmartBraceSplit();
            splits.Count.ShouldEqual(3);
            splits[0].ShouldEqual("{FirstName {Bob}}");
            splits[1].ShouldEqual(" {LastName {Smith} {SomethingElse}}");
            splits[2].ShouldEqual(" ... and some more");
        }

        [TestMethod]
        public void SmartBraceSplit_ShouldIncludeUnclosedBraces()
        {
            var splits = "{FirstName {Bob}} {LastName {Smith} {SomethingElse} ... and some more".SmartBraceSplit();
            splits.Count.ShouldEqual(2);
            splits[0].ShouldEqual("{FirstName {Bob}}");
            splits[1].ShouldEqual(" {LastName {Smith} {SomethingElse} ... and some more");
        }

        #endregion

        #region HtmlDecodeInBraces

        [TestMethod]
        public void HtmlDecodeInBraces_ShouldDecodeInBraces()
        {
            var decoded = "Something &lt; {&lt;Bob&gt;} &gt; Something Else".HtmlDecodeInBraces();
            decoded.ShouldEqual("Something &lt; {<Bob>} &gt; Something Else");
        }

        [TestMethod]
        public void HtmlDecodeInBraces_ShouldDecodeHandleMultipleLevels()
        {
            var decoded = "Something &lt; {{&lt;Bob&gt;}} &gt; Something Else".HtmlDecodeInBraces();
            decoded.ShouldEqual("Something &lt; {{<Bob>}} &gt; Something Else");
        }

        [TestMethod]
        public void HtmlDecodeInBraces_ShouldNotDecodeUnclosedBraces()
        {
            var decoded = "Something &lt; {{&lt;Bob&gt;} &gt; Something Else".HtmlDecodeInBraces();
            decoded.ShouldEqual("Something &lt; {{&lt;Bob&gt;} &gt; Something Else");
        }

        [TestMethod]
        public void HtmlDecodeInBraces_ShouldDecodeMultipleInstances()
        {
            var decoded = "Something {{&lt;Bob&gt;}} {&lt;Bob&gt;} Something Else".HtmlDecodeInBraces();
            decoded.ShouldEqual("Something {{<Bob>}} {<Bob>} Something Else");
        }

        #endregion

        #region FromPascalCase
        [TestMethod]
        public void FromPascalCase_ShouldSplitCapitalizedWords()
        {
            var result = "CustomerUserEvent".FromPascalCase();
            result.ShouldEqual("Customer User Event");
        }

        [TestMethod]
        public void FromPascalCase_ShouldGroupCamelCase()
        {
            var result = "customerUserEvent".FromPascalCase();
            result.ShouldEqual("customerUser Event");
        }

        [TestMethod]
        public void FromPascalCase_ShouldIgnoreLowerCaseWords()
        {
            var result = "customeruserevent".FromPascalCase();
            result.ShouldEqual("customeruserevent");
        }

        [TestMethod]
        public void FromPascalCase_ShouldLowercaseWordsWithLessThanThreeCharacters()
        {
            var result = "CustomerUiService".FromPascalCase();
            result.ShouldEqual("Customer ui Service");
        }
        #endregion

        #region ToMd5
        [TestMethod]
        public void ToMd5_ShouldConvertToMd5()
        {
            var source = Guid.NewGuid().ToString();
            var md5 = source.ToMd5();
            md5.Length.ShouldEqual(32);
        }
        #endregion

        #region StripAfter

        [TestMethod]
        public void StripAfter_ShouldStripCharactersAfter()
        {
            const string s = "Dictionary~2";
            s.StripStartingWith("~").ShouldEqual("Dictionary");
        }
        #endregion
    }
}