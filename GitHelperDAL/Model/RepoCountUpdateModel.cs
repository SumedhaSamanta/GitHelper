/* 
 Created By:        Sumedha Samanta
 Created Date:      18-11-2022
 Modified Date:     18-11-2022
 Purpose:           This class is used to encapsulate repository id and visit counts of a repository.
 Purpose Type:      This class encspsulates the count of visits of a repository.
 Referenced files:  NA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Model
{
    public class RepoCountUpdateModel
    {
        public long repoId;
        public long count;
    }
}
