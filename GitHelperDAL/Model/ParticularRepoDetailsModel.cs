/* 
 Created By:        Shubham Jaiswal
 Created Date:      23-10-2022
 Modified Date:     08-11-2022
 Purpose:           This class is used to store Repository details.
 Purpose Type:      This class holds the details of the repository like repository name, owner, repository link, creation date and modified date.
 Referenced files:  NA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Model
{
    public class ParticularRepoDetailsModel
    {
        public string repoName { get; set; }
        public string owner { get; set; }
        public string repoLink { get; set; }
        public DateTimeOffset createdAt { get; set; }
        public DateTimeOffset updatedAt { get; set; }

        
    }
}
