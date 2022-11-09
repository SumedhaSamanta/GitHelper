/* 
 Created By:        Sumedha Samanta
 Created Date:      24-10-2022
 Modified Date:     08-11-2022
 Purpose:           This class encapsulates user details returned from DAL to send back to user.
 Purpose Type:      This class encapsulates user's avatar url and a list of repository details(repo name and owner name) of the user.
 Referenced files:  NA
 */

using GitHelperDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHelper_1.Models
{
    public class UserDetails
    {
        public string userAvatarUrl { get; set; }
        public List<RepoDetailsModel> repoList { get; set; }

    }
}