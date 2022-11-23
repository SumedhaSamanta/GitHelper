/* 
 Created By:        Sumedha Samanta
 Created Date:      17-11-2022
 Modified Date:     17-11-2022
 Purpose:           This class is used to encapsulate user id, name and avatar url.
 Purpose Type:      This class encspsulates the details of a user.
 Referenced files:  NA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Model
{
    public class UserModel
    {
        public long userId { get; set; }
        public string userName { get; set; }
        public string userAvatarUrl { get; set; }
    }
}
