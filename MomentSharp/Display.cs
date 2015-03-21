﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MomentSharp.Globalization;

namespace MomentSharp
{
    public static class Display
    {
        /// <summary>
        /// moment([2007, 0, 29]).fromNow();     // 4 years ago
        /// moment([2007, 0, 29]).fromNow(showSuffix: false); // 4 years
        /// Emulates http://momentjs.com/docs/#/displaying/fromnow/
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="dateTime">Uses DateTime.UtcNow if not provided</param>
        /// <param name="showSuffix">Shows "number part ago" or "in number part"</param>
        /// <returns></returns>
        public static string From(this Moment moment, DateTime dateTime = default(DateTime), bool showSuffix = true)
        {
            var local = moment.Language;
            if (dateTime == default(DateTime)) dateTime = DateTime.UtcNow;

            var timeSpan = (moment.DateTime() - dateTime);
            var isFuture = timeSpan.TotalSeconds > 0;

            if (timeSpan.TotalSeconds.InRange(0, 45)) // 0-45 seconds
            {
                return local.Translate(RelativeTime.Seconds, timeSpan.Seconds, showSuffix, isFuture);
            }
            if (timeSpan.TotalSeconds.InRange(46, 90)) // 46-90 seconds
            {
                return local.Translate(RelativeTime.Minute, timeSpan.Minutes, showSuffix, isFuture);
            }
            if (timeSpan.TotalSeconds.InRange(91, 60 * 45)) //91 seconds to 45 minutes
            {
                return local.Translate(RelativeTime.Minutes, timeSpan.Minutes, showSuffix, isFuture);
            }
            if (timeSpan.TotalMinutes.InRange(46, 90)) //46 minutes to 90 minutes
            {
                return local.Translate(RelativeTime.Hour, timeSpan.Hours, showSuffix, isFuture);
            }
            if (timeSpan.TotalMinutes.InRange(91, 60 * 22)) //91 minutes to 22 hours
            {
                return local.Translate(RelativeTime.Hours, timeSpan.Hours, showSuffix, isFuture);
            }
            if (timeSpan.TotalHours.InRange(23, 36)) //23-36 hours
            {
                return local.Translate(RelativeTime.Day, timeSpan.Days, showSuffix, isFuture);
            }
            if (timeSpan.TotalHours.InRange(37, 24*25)) //37 hours to 25 days
            {
                return local.Translate(RelativeTime.Days, timeSpan.Days, showSuffix, isFuture);
            }
            if (timeSpan.TotalDays.InRange(26, 45)) //26-45 days
            {
                return local.Translate(RelativeTime.Month, 1, showSuffix, isFuture);
            }
            if (timeSpan.TotalDays.InRange(46, 345)) // 46-345 days
            {
                return local.Translate(RelativeTime.Months, (int)Math.Abs(timeSpan.Days / 30.4), showSuffix, isFuture);
            }
            if (timeSpan.TotalDays.InRange(346, 547)) //346-547 days (1.5 years)
            {
                return local.Translate(RelativeTime.Year, 1, showSuffix, isFuture);
            }
            if (timeSpan.TotalDays.InRange(548, 7305))
            {
                return local.Translate(RelativeTime.Years, Math.Abs(timeSpan.Days / 365), showSuffix, isFuture);
            }
            throw new Exception("Couldn't find an exceptable range to return");
        }

        /// <summary>
        /// Will format a date with different strings depending on how close to referenceTime's date (today by default) the date is.
        /// Emulates: http://momentjs.com/docs/#/displaying/calendar-time/
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="dateTime">Uses DateTime.UtcNow if not provided</param>
        /// <returns></returns>
        public static string Calendar(this Moment moment, DateTime dateTime = default(DateTime))
        {
            var local = moment.Language;
            if (dateTime == default(DateTime)) dateTime = DateTime.Now;

            var timeSpan = (moment.DateTime() - dateTime).TotalDays;
            
            if (timeSpan < -6) return local.Translate(Globalization.Calendar.SameElse, moment.DateTime());
            if (timeSpan < -1) return local.Translate(Globalization.Calendar.LastWeek, dateTime);
            if (timeSpan < 0) return local.Translate(Globalization.Calendar.LastDay, dateTime);
            if (timeSpan < 1) return local.Translate(Globalization.Calendar.SameDay, dateTime);
            if (timeSpan < 2) return local.Translate(Globalization.Calendar.NextDay, dateTime);
            if (timeSpan < 7) return local.Translate(Globalization.Calendar.NextWeek, dateTime);
            return local.Translate(Globalization.Calendar.SameElse, moment.DateTime());
        }

        /// <summary>
        /// Will format a date with different strings depending on how close to referenceTime's date (today by default) the date is.
        /// Emulates: http://momentjs.com/docs/#/displaying/calendar-time/
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="compareMoment">Uses DateTime.UtcNow if not provided</param>
        /// <returns></returns>
        public static string Calendar(this Moment moment, Moment compareMoment)
        {
            return moment.Calendar(compareMoment.DateTime());
        }

        /// <summary>
        /// Returns the number of seconds since the Unix Epoch
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeZoneId">Defaults to UTC if not provided. For valid parameters see TimeZoneInfo.GetSystemTimeZones()</param>
        /// <returns></returns>
        public static int UnixTimeStamp(this DateTime dateTime, string timeZoneId = "UTC")
        {
            dateTime.ToUTC(timeZoneId);
            var unixTimestamp = (Int32)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        /// <summary>
        /// Get the number of days in this <paramref name="dateTime"/>.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int DaysInMonth(this DateTime dateTime)
        {
            return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
        }

        /// <summary>
        /// Get the number of days in this <paramref name="moment"/>.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public static int DaysInMonth(this Moment moment)
        {
            return moment.DateTime().DaysInMonth();
        }
        
        internal static bool InRange(this double numberToCheck, int bottom, int top)
        {
            numberToCheck = Math.Abs(numberToCheck);
            return (numberToCheck > bottom && numberToCheck < top);
        }
    }
}