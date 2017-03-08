using System;
using System.Collections.Generic;
using Augment;

namespace Pequod.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// http://www.usa.gov/citizens/holidays.shtml      
    /// http://archive.opm.gov/operating_status_schedules/fedhol/2013.asp
    /// </remarks>
    public static class DateTimeExtensions
    {
        #region Handy Helpers

        enum Weeks
        {
            First = 1,
            Second = 2,
            Third = 3,
            Fourth = 4,
            Last = 5
        }

        public static DateTime FirstTradingDayOfMonth(this DateTime dt, DayOfWeek dayOfWeek)
        {
            DateTime tradingDate = GetNthDayOfNthWeek(dt, dayOfWeek, Weeks.First);

            if (!IsTradingDay(tradingDate))
            {
                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday:
                        tradingDate = tradingDate.AddDays(1);
                        break;

                    case DayOfWeek.Friday:
                        tradingDate = tradingDate.AddDays(-1);
                        break;
                }
            }

            return tradingDate;
        }

        #endregion

        #region Holiday Calculations

        /// <summary>
        /// Whether or not a date is a valid trading day (not weekend, not holiday)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsTradingDay(this DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            foreach (DateTime holiday in GetHolidays(dt))
            {
                if (holiday.Date == dt.Date)
                {
                    return false;
                }
            }

            return true;
        }

        private static IEnumerable<DateTime> GetHolidays(DateTime dt)
        {
            //   http://www.usa.gov/citizens/holidays.shtml      
            //   http://archive.opm.gov/operating_status_schedules/fedhol/2013.asp

            if (dt.Month == 1)
            {
                //  New Year's Day
                //  Jan 1
                yield return new DateTime(dt.Year, 1, 1).AdjustForWeekend();

                //  Martin Luther King, Jr.
                //  3rd Mon in Jan
                yield return GetNthDayOfNthWeek(new DateTime(dt.Year, 1, 1), DayOfWeek.Monday, Weeks.Third).AdjustForWeekend();
            }

            if (dt.Month == 2)
            {
                //  Washington's Birthday
                //  3rd Mon in Feb
                yield return GetNthDayOfNthWeek(new DateTime(dt.Year, 2, 1), DayOfWeek.Monday, Weeks.Third).AdjustForWeekend();
            }

            if (dt.Month == 5)
            {
                //  Memorial Day
                //  last Mon in May
                yield return GetNthDayOfNthWeek(new DateTime(dt.Year, 5, 1), DayOfWeek.Monday, Weeks.Last).AdjustForWeekend();
            }

            if (dt.Month == 7)
            {
                //  Independence Day
                //  July 4
                yield return new DateTime(dt.Year, 7, 4).AdjustForWeekend();
            }

            if (dt.Month == 9)
            {
                //  Labor Day
                //  1st Mon in Sept
                yield return GetNthDayOfNthWeek(new DateTime(dt.Year, 9, 1), DayOfWeek.Monday, Weeks.First).AdjustForWeekend();
            }

            if (dt.Month == 11)
            {
                //  Thanksgiving Day
                //  4th Thur in Nov
                yield return GetNthDayOfNthWeek(new DateTime(dt.Year, 11, 1), DayOfWeek.Thursday, Weeks.Fourth).AdjustForWeekend();
            }

            if (dt.Month == 12)
            {
                //  Christmas Day
                //  Dec 25
                yield return new DateTime(dt.Year, 12, 25).AdjustForWeekend();
            }
        }

        private static DateTime GetNthDayOfNthWeek(DateTime dt, DayOfWeek dayofWeek, Weeks weekOfMonth)
        {
            //specify which day of which week of a month and this function will get the date
            //this function uses the month and year of the date provided

            //get first day of the given date
            DateTime firstOfMonth = dt.BeginningOfMonth();

            //get first DayOfWeek of the month
            DateTime adjustedDate = firstOfMonth.AddDays(6 - (int)firstOfMonth.AddDays(-1 * ((int)dayofWeek + 1)).DayOfWeek);

            //get which week
            adjustedDate = adjustedDate.AddDays(((int)weekOfMonth - 1) * 7);

            //if day is past end of month then adjust backwards a week
            if (adjustedDate >= firstOfMonth.AddMonths(1))
            {
                adjustedDate = adjustedDate.AddDays(-7);
            }

            return adjustedDate;
        }

        private static DateTime AdjustForWeekend(this DateTime holiday)
        {
            if (holiday.DayOfWeek == DayOfWeek.Saturday)
            {
                return holiday.AddDays(-1);
            }

            if (holiday.DayOfWeek == DayOfWeek.Sunday)
            {
                return holiday.AddDays(1);
            }

            return holiday;
        }

        #endregion
    }
}
