/* 
 Created By:        Sumedha Samanta
 Created Date:      17-11-2022
 Modified Date:     17-11-2022
 Purpose:           This class is responsible for encapsulating details of a repository.
 Purpose Type:      This class encapsulates Id, name, owner's name, visit counts for a repository and whether it is favourite or not
 Referenced files:  NA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHelperAPI.Models
{
    public class RepoFavouriteCount
    {
        public long repoId { get; set; }
        public string repoName { get; set; }
        public string repoOwner { get; set; }
        public bool isFavourite { get; set; }
        public long count { get; set; }
    }
}