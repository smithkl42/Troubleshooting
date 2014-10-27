using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class MultiDateTimeConverterTests
    {
        /// <summary>
        /// See https://www.pivotaltracker.com/story/show/69638844
        /// </summary>
        [TestMethod]
        public void MultiDateTimeConverter_ShouldSupportMultipleFormats()
        {
            var json = @"[ {date:'2014-02-16T07.06.58'}, {date: '2014-02-16T07:06:58'}]";
            var normalIsoFormat = new IsoDateTimeConverter();
            var tweakyIsoFormat = new IsoDateTimeConverter();
            tweakyIsoFormat.DateTimeFormat = "yyyy-MM-ddThh.mm.ss";
            var multi = new MultiDateTimeConverter(normalIsoFormat, tweakyIsoFormat);
            var dateTests = JsonConvert.DeserializeObject<List<DateTest>>(json, multi);
            dateTests.Count.ShouldEqual(2);
            dateTests.ForEach(x =>
            {
                x.Date.Year.ShouldEqual(2014);
                x.Date.Month.ShouldEqual(2);
                x.Date.Day.ShouldEqual(16);
                x.Date.Hour.ShouldEqual(7);
                x.Date.Minute.ShouldEqual(6);
                x.Date.Second.ShouldEqual(58);
            });
        }
    }

    internal class DateTest
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
}
