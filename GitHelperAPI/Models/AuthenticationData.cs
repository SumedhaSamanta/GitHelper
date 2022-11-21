/* 
 Created By:        Mehdi Hossain
 Created Date:      25-10-2022
 Modified Date:     08-11-2022
 Purpose:           This class is used for encapsulating username and token data.
 Purpose Type:      This class helps to encapsulate username and token of a user read from authentication cookie
 Referenced files:  NA 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHelperAPI.Models
{
    public class AuthenticationData
    {
        public string userName { get; set; }
        public string userToken { get; set; }
        public long userId { get; set; }
    }
}