/* 
 Created By:        Shubham Jaiswal
 Created Date:      23-10-2022
 Modified Date:     08-11-2022
 Purpose:           This class is used to store repository name and owner name.
 Purpose Type:      This class holds the name of the repository and owner name.
 Referenced files:  NA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Model
{
    public class RepoDetailsModel
    {
        public string repoName { get; set; }
        public string owner { get; set; }
    }
}
