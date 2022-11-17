using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHelperDAL.Services.Impl;

namespace GitHelperDAL.Services
{
    public abstract class DbService
    {
        static private Dictionary<string, DbService> _services = new Dictionary<string, DbService>();
        public abstract void setFavourite(long userId, long repoId);
        public abstract bool removeFavourite(long userId, long repoId);

        public abstract long getFavourite(long userId);

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
