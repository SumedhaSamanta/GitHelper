using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Model
{
    public class CommitDetailsModel
    {
        public string AuthorName { get; set; }
        public string CommitMessage { get; set; }
        public string DateStr { get; set; }
    }
}
