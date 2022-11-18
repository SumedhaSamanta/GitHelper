/* 
 Created By:        Sumedha Samanta
 Created Date:      18-11-2022
 Modified Date:     18-11-2022
 Purpose:           This class is used to encapsulate repository id, favourite status and visit counts of a repository.
 Purpose Type:      This class encspsulates the favourite status and count of visits of a repository for returning data from DB.
 Referenced files:  NA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Model
{
    public class RepoActivities
    {
        public long repoId { get; set; }
        public bool isFavourite { get; set; }
        public long count { get; set; }
    }
}
