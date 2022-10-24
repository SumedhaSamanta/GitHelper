using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHelper_1.Utilities
{
    public class DateFormatter
    {
        public string DateStr { get; set; }

        public DateTime ConvertUTCtoIST(string dateInUTC)
        {
            DateTime univDateTime = DateTime.Parse(dateInUTC);
            return univDateTime.AddHours(5.5);
        }

    }
}