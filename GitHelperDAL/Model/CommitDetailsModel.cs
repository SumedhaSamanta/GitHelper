/* 
 Created By:        Shubham Jaiswal
 Created Date:      23-10-2022
 Modified Date:     08-11-2022
 Purpose:           This class is used to store Commit details.
 Purpose Type:      This class stores the name of the author, message and time of the commits.
 Referenced files:  NA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Model
{
    public class CommitDetailsModel
    {
        public string commitAuthorName { get; set; }
        public string commitMessage { get; set; }
        public DateTimeOffset commitDateTime { get; set; }
    }
}
