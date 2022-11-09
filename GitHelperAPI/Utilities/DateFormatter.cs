/* 
 Created By:        Mehdi Hossain
 Created Date:      24-10-2022
 Modified Date:     25-10-2022
 Purpose:           Util to convert datetime between utc and prefered user time offset.
 Purpose Type:      Util to convert datetime between utc and prefered user time offset.
 Referenced files:  NA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHelperAPI.Utilities
{
    public static class DateFormatter
    {

        private static Dictionary<string, TimeSpan> timeOffsetData = new Dictionary<string, TimeSpan>();


        static DateFormatter()
        {
            timeOffsetData.Add("IST", new TimeSpan(5, 30, 0));
            timeOffsetData.Add("UTC", TimeSpan.Zero);
        }

        /*
            <summary>
                 used to get current utc datetime of the server.
            </summary>
            <param > none </param>
            <returns>current utc datetimeoffset</returns>
        */
        public static DateTimeOffset getCurrentUtcTime()
        {
            return DateTimeOffset.UtcNow;
        }


        /*
            <summary>
                 used to get current datetime of server in user preference offset.
            </summary>
            <param > none </param>
            <returns>current datetimeoffset in user preference offset</returns>
        */
        public static DateTimeOffset getCurrentUserPrefTime(string offset = "IST")
        {
            TimeSpan dateOffset = timeOffsetData["UTC"];
            if (timeOffsetData.ContainsKey(offset))
            {
                dateOffset = timeOffsetData[offset];
            }
            return getCurrentUtcTime().ToOffset(dateOffset);
        }


        /*
            <summary>
                convert any datetimeoffset to user preference offset
            </summary>
            <param name="date"> date to be converted to user preference offset </param>
            <param name="offset"> string that represent user preference offset </param>
            <returns>user preference datetimeoffset</returns>
        */
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

        /*
            <summary>
                convert any datetimeoffset to utc datetime
            </summary>
            <param name="date"> date to be converted to utc datetime </param>
            <returns>utc datetimeoffset</returns>
        */
        public static DateTimeOffset ConvertToUtc(DateTimeOffset date)
        {

            return date.ToOffset(timeOffsetData["UTC"]);
        }

        /*
            <summary>
                create datetime in user preference offset
            </summary>
            <param name="date"> datetime to be created for user preference offset </param>
            <param name="offset"> string that represent user preference offset </param>
            <returns> datetime as per user preference offset </returns>
        */
        public static DateTimeOffset CreateUserPrefDateTimeOffset(DateTime date, string offset = "IST")
        {
            TimeSpan dateOffset = timeOffsetData["UTC"];
            if (timeOffsetData.ContainsKey(offset))
            {
                dateOffset = timeOffsetData[offset];
            }
            return new DateTimeOffset(date, dateOffset);
        }

        /*
            <summary>
                create datetime in utc offset
            </summary>
            <param name="date"> datetime to be created for utc offset </param>
            <returns> datetime as per utc offset </returns>
        */
        public static DateTimeOffset CreateUTCDateTimeOffset(DateTime date)
        {
            TimeSpan dateOffset = timeOffsetData["UTC"];
            return new DateTimeOffset(date, dateOffset);
        }

    }
}