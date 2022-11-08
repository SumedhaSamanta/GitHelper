/* 
 Created By:        Mehdi Hossain
 Created Date:      25-10-2022
 Modified Date:     08-11-2022
 Purpose:           This class is used for encapsulating status and message.
 Purpose Type:      This class encapsulates status and appropriate message as a part of response.
 Referenced files:  NA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHelper_1.Models
{
    public class StatusDetailsModel
    {
        public string status { get; set; }

        public string message { get; set; }
    }
}