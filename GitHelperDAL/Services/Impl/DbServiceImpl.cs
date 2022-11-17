using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHelperDAL.Services.Impl
{
    public class DbServiceImpl : DbService
    {
        private string co;
        public DbServiceImpl(string co)
        {
            this.co = co;
        }

        public override long getFavourite(long userId)
        {
           
            using (SqlConnection con = new SqlConnection(co))
            {
                con.Open();

                SqlCommand cmd1 = new SqlCommand("dbo.get_fav", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;

                 var result=cmd1.ExecuteScalar();
                if (result is long)
                    return (long)result;
                else return -1;
            }

        }

        public override bool removeFavourite(long userId, long repoId)
        {
            using (SqlConnection con = new SqlConnection(co))
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand("dbo.remove_fav", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;
                cmd1.Parameters.Add("@repository_id", SqlDbType.BigInt).Value = repoId;
                return (bool)cmd1.ExecuteScalar();

            }
        }

        public override void setFavourite(long userId, long repoId)
        {
            using (SqlConnection con = new SqlConnection(co))
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand("dbo.set_fav", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;
                cmd1.Parameters.Add("@repository_id", SqlDbType.BigInt).Value = repoId;
                cmd1.ExecuteNonQuery();
            }
        }
    }
}
