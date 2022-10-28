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