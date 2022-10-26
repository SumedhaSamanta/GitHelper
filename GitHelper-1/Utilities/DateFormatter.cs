using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHelper_1.Utilities
{
    public static class DateFormatter
    {

        private static Dictionary<string, TimeSpan> timeOffsetData = new Dictionary<string, TimeSpan>();

        static DateFormatter()
        {
            timeOffsetData.Add("IST", new TimeSpan(5, 30, 0));
            timeOffsetData.Add("UTC", TimeSpan.Zero);
        }

        public static DateTimeOffset getCurrentUtcTime()
        {
            return DateTimeOffset.UtcNow;
        }

        public static DateTimeOffset getCurrentUserPrefTime(string offset = "IST")
        {
            TimeSpan dateOffset = timeOffsetData["UTC"];
            if (timeOffsetData.ContainsKey(offset))
            {
                dateOffset = timeOffsetData[offset];
            }
            return getCurrentUtcTime().ToOffset(dateOffset);
        }

        public static DateTimeOffset ConvertToUserPref(DateTimeOffset date, string offset = "IST")
        {
            if (timeOffsetData.ContainsKey(offset))
            {
                return date.ToOffset(timeOffsetData[offset]);
            }
            else
            {
                return date.ToOffset(timeOffsetData["UTC"]);
            }
        }

        public static DateTimeOffset ConvertToUtc(DateTimeOffset date)
        {

            return date.ToOffset(timeOffsetData["UTC"]);
        }

        public static DateTimeOffset CreateUserPrefDateTimeOffset(DateTime date, string offset = "IST")
        {
            TimeSpan dateOffset = timeOffsetData["UTC"];
            if (timeOffsetData.ContainsKey(offset))
            {
                dateOffset = timeOffsetData[offset];
            }
            return new DateTimeOffset(date, dateOffset);
        }

        public static DateTimeOffset CreateUTCDateTimeOffset(DateTime date)
        {
            TimeSpan dateOffset = timeOffsetData["UTC"];
            return new DateTimeOffset(date, dateOffset);
        }

    }
}