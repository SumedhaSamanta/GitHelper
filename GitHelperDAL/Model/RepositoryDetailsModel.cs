/* 
 Created By:        Sumedha Samanta
 Created Date:      17-11-2022
 Modified Date:     17-11-2022
 Purpose:           This class is used to encapsulate repository id, name and owner name.
 Purpose Type:      This class encspsulates the details of a repository.
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
    public class RepositoryDetailsModel
    {
        public long repoId { get; set; }
        public string repoName { get; set; }
        public string repoOwner { get; set; }
    }
}
