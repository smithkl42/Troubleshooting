using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class DateTimeHelpersTests
    {
        [TestMethod]
        public void ToUnixTime_ShouldHaveEpochTimeEqualsZero()
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            date.ToUnixTime().ShouldEqual(0);
        }

        [Ignore]
        [TestMethod]
        public void ToUnixTime_ShouldRespectTimeZone()
        {
            var utc = new DateTime(1970, 1, 1, 8, 0, 0);
            var gmt = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            var pst = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            var date = TimeZoneInfo.ConvertTime(utc, gmt, pst);
            date.ToUnixTime().ShouldEqual(28800);
        }

        [TestMethod]
        public void ToUnixTime_ShouldBeConvertibleWithLocalTime()
        {
            var now = DateTime.Now;
            var epoch = now.ToUnixTime();
            var back = epoch.FromUnixTime();
            back.ShouldHaveMaxDiff(now, 2);
        }

        [TestMethod]
        public void ToUnixTime_ShouldBeConvertibleWithUtc()
        {
            var now = DateTime.UtcNow;
            var epoch = now.ToUnixTime();
            var back = epoch.FromUnixTime();
            back.ShouldHaveMaxDiff(now, 2);
        }

        [TestMethod]
        public void ToShortIsoDateString_ShouldReturnShortIsoDateString()
        {
            var date = new DateTime(2000, 01, 01, 01, 01, 01);
            date.ToShortIsoDateString().ShouldEqual("2000.01.01");
        }

        [TestMethod]
        public void ToFirstDateOfWeek_ShouldUseMondayForStartOfWeek()
        {
            var mon = new DateTime(2014, 9, 15);
            var tue = new DateTime(2014, 9, 16);
            var wed = new DateTime(2014, 9, 17);
            var thu = new DateTime(2014, 9, 18);
            var fri = new DateTime(2014, 9, 19);
            var sat = new DateTime(2014, 9, 20);
            var sun = new DateTime(2014, 9, 21);

            mon.FirstDateOfWeek().ShouldEqual(mon);
            tue.FirstDateOfWeek().ShouldEqual(mon);
            wed.FirstDateOfWeek().ShouldEqual(mon);
            thu.FirstDateOfWeek().ShouldEqual(mon);
            fri.FirstDateOfWeek().ShouldEqual(mon);
            sat.FirstDateOfWeek().ShouldEqual(mon);
            sun.FirstDateOfWeek().ShouldEqual(mon);
        }

        [TestMethod]
        public void ToFirstDateOfWeek_ShouldUsePassedInFirstDayOfWeek()
        {
            var tue = new DateTime(2014, 9, 16);
            var wed = new DateTime(2014, 9, 17);
            var thu = new DateTime(2014, 9, 18);
            var fri = new DateTime(2014, 9, 19);
            var sat = new DateTime(2014, 9, 20);
            var sun = new DateTime(2014, 9, 21);
            var mon = new DateTime(2014, 9, 22);

            // This one is weird. FirstDateOfWeek() 
            tue.FirstDateOfWeek(DayOfWeek.Tuesday).ShouldEqual(tue);
            wed.FirstDateOfWeek(DayOfWeek.Tuesday).ShouldEqual(tue);
            thu.FirstDateOfWeek(DayOfWeek.Tuesday).ShouldEqual(tue);
            fri.FirstDateOfWeek(DayOfWeek.Tuesday).ShouldEqual(tue);
            sat.FirstDateOfWeek(DayOfWeek.Tuesday).ShouldEqual(tue);
            sun.FirstDateOfWeek(DayOfWeek.Tuesday).ShouldEqual(tue);
            mon.FirstDateOfWeek(DayOfWeek.Tuesday).ShouldEqual(tue);
        }

        [TestMethod]
        public void FirstDateOfMonth_ShouldReturnFirstDateOfMonth()
        {
            var date = new DateTime(2014, 09, 29);
            date.FirstDateOfMonth().Year.ShouldEqual(2014);
            date.FirstDateOfMonth().Month.ShouldEqual(9);
            date.FirstDateOfMonth().Day.ShouldEqual(1);
        }
    }
}