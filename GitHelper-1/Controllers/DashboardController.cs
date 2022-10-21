using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RouteAttribute = System.Web.Http.RouteAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using GitHelper_1.Utilities;
using System.Web.UI.WebControls;
using ActionNameAttribute = System.Web.Http.ActionNameAttribute;

namespace GitHelper_1.Controllers
{
    public class DashboardController : ApiController
    {
        CookieManager cookieManager = new CookieManager();
        public List<List<string>> GetUserDetails()
        {
            string username = cookieManager.GetUserName();
            string token = cookieManager.GetToken();

            //get repo-names, repo-url and user-avatar-url
            //List<RepoNameAndUrl> repo_list = DAL.GetRepoNameAndUrls();
            //string avatarURL = DAL.GetAvtarUrl(); 

            //segregate the repo names and repo-urls as different lists

            List<string> repoNames = new List<string>();
            List<string> repoURLs = new List<string>();
            List<string> avatar = new List<string>();

            //dummy values
            avatar.Add("avatar");
            repoNames.Add("repoName1");
            repoNames.Add("repoName2");
            repoURLs.Add("repoURL1");
            repoURLs.Add("repoURL2");


            /*
             foreach(RepoNameAndUrl repo in repo_list)
            {
                repoNames.Add(repo.Name);
                repoURLs.Add(repo.Url);
            }
            */

            //return a list of lists containing avatar-url, list of repo names and list of repoURLs
            List<List<string>> result = new List<List<string>>();

            result.Add(avatar);
            result.Add(repoNames);
            result.Add(repoURLs);
            
            return result;
        }

        [HttpGet]
        [ActionName("GetCommits")]
        public void GetCommits(string repo)
        {
            string userName = cookieManager.GetUserName();
            string token = cookieManager.GetToken();
            //getcommits (commitMessage, commitAuthorName, commitDate)

            //should be in descending order of commitDate


            //extract the date of creation 

        }


        [HttpGet]
        [ActionName("GetNumberOfCommitsDateWise")]
        public void GetNumberOfCommitsDateWise(string repo, string month, string year)
        {
            string userName = cookieManager.GetUserName();
            string token = cookieManager.GetToken();
            //get number of commits made on each date of the month

        }

    }
}
