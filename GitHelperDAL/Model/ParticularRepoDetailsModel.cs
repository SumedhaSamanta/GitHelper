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
