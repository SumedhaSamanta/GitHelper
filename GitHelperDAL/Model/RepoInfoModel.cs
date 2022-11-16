/* 
 Created By:        Sumedha Samanta
 Created Date:      16-11-2022
 Modified Date:     16-11-2022
 Purpose:           This class is used to store repository id, name, owner name, count of visits and is favourite or not.
 Purpose Type:      This class holds the details of a repository.
 Referenced files:  NA
 */

using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Model
{
    public class RepoInfoModel
    {
        public long repoId { get; set; }
        public string repoName { get; set; }
        public string owner { get; set; }
        public bool isFavourite { get; set; }
        public long count { get; set; }
    }
}
