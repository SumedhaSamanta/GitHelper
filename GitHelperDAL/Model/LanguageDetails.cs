/* 
 Created By:        Shubham Jaiswal
 Created Date:      23-10-2022
 Modified Date:     08-11-2022
 Purpose:           This class is used to store language details.
 Purpose Type:      This class holds the language details.
 Referenced files:  NA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Model
{
    public class LanguageDetails
    {
        public string language { get; set; }

        public long bytesOfCode { get; set; }
    }
}
