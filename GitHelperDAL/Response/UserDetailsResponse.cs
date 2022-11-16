/* 
 Created By:        Sumedha Samanta
 Created Date:      16-11-2022
 Modified Date:     16-11-2022
 Purpose:           This class is used to store user id,user name, avatar url and list of repositories.
 Purpose Type:      This class holds the details of a user.
 Referenced files:  NA
 */

using GitHelperDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Response
{
    public class UserDetailsResponse
    {
        public long userId { get; set; }
        public string username { get; set; }
        public string userAvatarUrl { get; set; }
        public List<RepoInfoModel> repoList { get; set; }
    }
}
