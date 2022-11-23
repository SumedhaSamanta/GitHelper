/* 
 Created By:        Shubham Jaiswal
 Created Date:      18-11-2022
 Modified Date:     21-11-2022
 Purpose:           Abstract service class that is responsible for creating and returning the necessary
                    implementation subclass to the caller method. This class also acts as the base class
                    to encapsulate the implementation of sub-classes.
 Purpose Type:      This class acts as the reference type for creating and using actual database api implementation.
 Referenced files:  NA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHelperDAL.Model;
using GitHelperDAL.Services.Impl;

namespace GitHelperDAL.Services
{
    public abstract class DbService
    {
        static private Dictionary<string, DbService> _services = new Dictionary<string, DbService>();
        public abstract void setFavourite(long userId, long repoId);
        public abstract bool removeFavourite(long userId, long repoId);

        public abstract long getFavourite(long userId);

        public abstract void updateRepoCount(long userId, List<RepoCountUpdateModel> repoCountList);

        public abstract List<RepoActivities> fetchActivityDetails(long userId);
        static public void setDataSorce(string name, string connectionString)
        {
            _services.Add(name,new DbServiceImpl(connectionString));
        }

        static public DbService getInstance(string name)
        {
            return _services[name];
        }
    }
}
