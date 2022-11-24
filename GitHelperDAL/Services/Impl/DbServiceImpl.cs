/* 
 Created By:        Shubham Jaiswal
 Created Date:      18-11-2022
 Modified Date:     21-11-2022
 Purpose:           This is the implementation class of DbService class.
 Purpose Type:      This class holds the implementation of all the abstract method of the DbService class.
 Referenced files:  Model/RepoCountUpdateModel.cs, Model/RepoActivities.cs
 */
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
        /*
           <summary>
           It gets favourite repository of the particular user    
           </summary>
           <param name="userId">This is the user id of the particular user.  </param>
           <returns>Returns favourite repository.</returns>
       */
        public override long getFavourite(long userId)
        {

            using (SqlConnection con = new SqlConnection(co))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("dbo.sp_get_fav", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;

                    var result = cmd1.ExecuteScalar();
                    if (result is long)
                        return (long)result;
                    else return -1;
                }
                catch(SqlException ex)
                {
                    throw new Exception(ex.Message,ex);
                }
                
            }

        }

        /*
           <summary>
           It removes favourite repository of the particular user    
           </summary>
           <param name="userId">This is the user id of the particular user.</param>
           <param name="repoId">This is the repository id of specified repository of user.</param>
           <returns>Returns true or false.</returns>
       */
        public override bool removeFavourite(long userId, long repoId)
        {
            using (SqlConnection con = new SqlConnection(co))
            {
                try 
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("dbo.sp_remove_fav", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;
                    cmd1.Parameters.Add("@repository_id", SqlDbType.BigInt).Value = repoId;
                    return (bool)cmd1.ExecuteScalar();
                }
                catch(SqlException ex)
                {
                    throw new Exception(ex.Message,ex);
                }
            }
        }

        /*
          <summary>
          It removes favourite repository of the particular user    
          </summary>
          <param name="userId">This is the user id of the particular user.</param>
          <param name="repoId">This is the repository id of specified repository of user.</param>
          <returns>Returns true or false.</returns>
      */
        public override void setFavourite(long userId, long repoId)
        {
            using (SqlConnection con = new SqlConnection(co))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("dbo.sp_set_fav", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;
                    cmd1.Parameters.Add("@repository_id", SqlDbType.BigInt).Value = repoId;
                    cmd1.ExecuteNonQuery();
                }
                catch(SqlException ex)
                {
                   
                    throw new Exception (ex.Message,ex);
                }
                
            }
        }

        /*
         <summary>
         It update count of repository.    
         </summary>
         <param name="userId">This is the user id of the particular user.</param>
         <param name="repoCountList">This is the list which contains repository and count associated with that repository.</param>
         <returns>NA</returns>
     */
        public override void updateRepoCount(long userId, List<RepoCountUpdateModel> repoCountList)
        {
            using (SqlConnection con = new SqlConnection(co))
            {
                try
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
                    SqlCommand cmd1 = new SqlCommand("dbo.sp_set_count", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;

                    var pList = new SqlParameter("@repoCountList", SqlDbType.Structured);
                    pList.TypeName = "dbo.RepoCountList";
                    pList.Value = dt;
                    cmd1.Parameters.Add(pList);

                    cmd1.ExecuteNonQuery();
                }
                catch(SqlException ex)
                {
                    throw new Exception(ex.Message,ex);
                }
               

            }
        }

        /*
         <summary>
         It gets the repository activity details of specified user.   
         </summary>
         <param name="userId">This is the user id of the particular user.</param>
         <returns>Returns list of repository activity of specified user.</returns>
     */
        public override List<RepoActivities> fetchActivityDetails(long userId)
        {
            using (SqlConnection con = new SqlConnection(co))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("dbo.sp_fetch_repo_activity_details", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;
                    SqlDataReader reader = cmd1.ExecuteReader();

                    List<RepoActivities> repoActivities = new List<RepoActivities>();
                    while (reader.Read())
                    {
                        RepoActivities repoActivity = new RepoActivities
                        {
                            repoId = (long)reader["repository_id"],
                            isFavourite = reader["is_favourite"] != null ? ((int)reader["is_favourite"] == 0 ? false : true) : false,
                            count = (long)reader["count"]
                        };
                        repoActivities.Add(repoActivity);
                    }
                    return repoActivities;
                }
                catch(SqlException ex)
                {
                    throw new Exception(ex.Message,ex);
                }
                
            }
        }
    }
}
