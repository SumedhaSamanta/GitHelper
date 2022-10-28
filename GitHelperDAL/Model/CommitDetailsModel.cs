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
