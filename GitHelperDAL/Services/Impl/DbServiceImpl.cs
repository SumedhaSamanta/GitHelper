using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHelperDAL.Model;

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

                var result = cmd1.ExecuteScalar();
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

        public override void updateRepoCount(long userId, List<RepoCountUpdateModel> repoCountList)
        {
            using (SqlConnection con = new SqlConnection(co))
            {
                con.Open();
                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("RepoId");
                dt.Columns.Add("Count");
                foreach (RepoCountUpdateModel repoCount in repoCountList)
                {
                    DataRow newRow = dt.NewRow();
                    newRow["RepoId"] = repoCount.repoId;
                    newRow["Count"] = repoCount.count;
                    dt.Rows.Add(newRow);
                }
                SqlCommand cmd1 = new SqlCommand("dbo.set_count", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;

                var pList = new SqlParameter("@repoCountList", SqlDbType.Structured);
                pList.TypeName = "dbo.RepoCountList";
                pList.Value = dt;
                cmd1.Parameters.Add(pList);

                cmd1.ExecuteNonQuery();

            }
        }
    }
}
