/* 
 Created By:        Sumedha Samanta
 Created Date:      17-11-2022
 Modified Date:     17-11-2022
 Purpose:           This class is used to encapsulate user id,user name, avatar url and list of repositories.
 Purpose Type:      This class encapsulated the details of a user.
 Referenced files:  Models/RepoFavouriteCount.cs
 */

using GitHelperDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHelperAPI.Models;

namespace GitHelperAPI.Response
{
    public class UserDetailsResponse
    {
        public long userId { get; set; }
        public string userName { get; set; }
        public string userAvatarUrl { get; set; }
        public List<RepoFavouriteCount> repoList { get; set; }
    }
}
