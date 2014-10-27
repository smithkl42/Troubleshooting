using System;

namespace Payboard.Common
{
    public static class DateTimeHelpers
    {
        // private static readonly DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;

        private static readonly TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        }

        public static DateTime RoundUp(this DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks + d.Ticks - 1)/d.Ticks)*d.Ticks);
        }

        public static string ToSortableShortDateString(this DateTime date)
        {
            return date.ToPacificTime().ToString("yyyy-MM-dd H:MM");
        }

        public static string ToShortIsoDateString(this DateTime date)
        {
            return date.ToString("yyyy.MM.dd");
        }

        public static DateTime FirstDateOfWeek(this DateTime dateTime)
        {
            return FirstDateOfWeek(dateTime, DayOfWeek.Monday);
        }

        public static DateTime FirstDateOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1*diff).Date;
        }

        public static DateTime FirstDateOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        // public static int Week(this DateTime dateTime)
        // {
        //     return dfi.Calendar.GetWeekOfYear(dateTime, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
        // }

        //public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        // {
        //     var jan1 = new DateTime(year, 1, 1);
        //     var daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

        //     var firstThursday = jan1.AddDays(daysOffset);
        //     var cal = CultureInfo.CurrentCulture.Calendar;
        //     var firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        //     var weekNum = weekOfYear;
        //     if (firstWeek <= 1)
        //     {
        //         weekNum -= 1;
        //     }
        //     var result = firstThursday.AddDays(weekNum * 7);
        //     return result.AddDays(-3);
        // }

        /// <summary>
        ///     Total nasty-ass hack to get times into our - meaning me and Matt - local time.
        ///     ToLocalTime() doesn't work on Azure, because the Azure server is itself set to UTC/GMT.
        /// </summary>
        public static DateTime ToPacificTime(this DateTime timeUtc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, pstZone);
        }
    }
}