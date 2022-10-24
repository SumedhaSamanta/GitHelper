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
using GitHelper_1.Models;
using System.Xml.Linq;
using System.Drawing;
using GitHelperDAL.Model;
using GitHelperDAL.Services;

namespace GitHelper_1.Controllers
{
    public class DashboardController : ApiController
    {
        [HttpGet]
        [ActionName("GetUserDetails")]
        //return avatar-url, list of repo names and owner names
        public UserDetails GetUserDetails()
        {
            string username = "SumedhaSamanta";
            string token= "";
            
            List<RepoDetailsModel> repoList = GitHubApiService.getInstance(username,token).GetRepoDetails();
            string avatarURL = GitHubApiService.getInstance(username, token).GetAvtarUrl();

            UserDetails result = new UserDetails();
            result.repoList = repoList;
            result.userAvatarUrl = avatarURL;

            return result;
        }

        [HttpGet]
        [ActionName("GetCommits")]

        //get details of the commits (commitAuthorName, commitMessage, commitDate)
        public List<List<string>> GetCommits(string ownerName, string repoName)
        {
            string userName = "";
            string token = "";

            List<List<string>> result = new List<List<string>>();

            List<CommitDetailsModel> commitsList = GitHubApiService.getInstance(userName,token).GetCommitDetails(ownerName, repoName);
            foreach(CommitDetailsModel commit in commitsList)
            {
                List<string> commitDetails = new List<string>();
                commitDetails.Add(commit.AuthorName);
                commitDetails.Add(commit.CommitMessage);
                commitDetails.Add(commit.DateStr);
                result.Add(commitDetails);
            }

            return result;
        }


        [HttpGet]
        [ActionName("GetCommitGraphData")]

        //get number of commits made on each date of the given month and year for the repo
        public List<Dictionary<string, int>> GetCommitGraphData(string ownerName, string repoName, string month, string year)
        {
            string userName = "";
            string token = "";

            List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();

            //dictionary to store the daywise commit counts
            Dictionary<int, int> dayCount = GetDayCount(month, year);

            //fetch commits of the repo
            List<CommitDetailsModel> commitsList = GitHubApiService.getInstance(userName, token).GetCommitDetails(ownerName, repoName);
            foreach (CommitDetailsModel commit in commitsList)
            {
                Dictionary<string, string> monthYear = new Dictionary<string, string>();

                DateTime commitDate = new DateFormatter().ConvertUTCtoIST(commit.DateStr);
                string commitMonth = commitDate.ToString("MMMM").ToLower();
                string commitYear = commitDate.ToString("yyyy");

                if (commitMonth.Equals(month.ToLower()) && commitYear.Equals(year))
                {
                    string date = commitDate.ToString("d");
                    int day = int.Parse(date);
                    dayCount[day] += 1;
                }
            }

            foreach (KeyValuePair<int, int> entry in dayCount)
            {
                Dictionary<string,int> commitCount = new Dictionary<string, int>();
                commitCount.Add("commits",entry.Value);
                commitCount.Add("day", entry.Key);
                result.Add(commitCount);
            }

            return result;
        }

        [HttpGet]
        [ActionName("GetMonthYearList")]
        public List<Dictionary<string, string>> GetMonthYearList(string ownerName, string repoName)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            string userName = "";
            string token = "";

            //fetch commits of the repo
            List<CommitDetailsModel> commitsList = GitHubApiService.getInstance(userName,token).GetCommitDetails(ownerName, repoName);

            foreach(CommitDetailsModel commit in commitsList)
            {
                Dictionary<string, string> monthYear = new Dictionary<string, string>();

                DateTime commitDate = new DateFormatter().ConvertUTCtoIST(commit.DateStr);
                string month = commitDate.ToString("MMMM");
                string year = commitDate.ToString("yyy");
                monthYear.Add("month", month);
                monthYear.Add("year", year);

                if(!result.Contains(monthYear))
                {
                    result.Add(monthYear);
                }
            }

            return result;
        }

        [HttpGet]
        [ActionName("GetReposInfo")]
        public List<string> GetReposInfo(string ownerName, string repoName)
        {
            string userName = "";
            string token = "";

            //int numberOfContributors = GitHubApiService.getInstance(userName,token).getNumberOfContributors(ownerName, repoName);
            //List<LanguageDetails> languagesUsed = GitHubApiService.getInstance(userName, token).GetRepositoryLanguages(ownerName, repoName);

            List<string> result = new List<string>();

            //result.Add(numberOfContributors.ToString());
            //result.Add(languagesUsed);

            return result;
        }

        public Dictionary<int, int> GetDayCount(string month, string year)
        {
            Dictionary<int, int> dayCount = new Dictionary<int, int>();
            int numberOfDays = 0;
            month = month.ToLower();
            int y = int.Parse(year);

            if (month.Equals("february"))
            {
                if (((y % 4 == 0) && (y % 100 != 0)) || (y % 400 == 0))
                    numberOfDays = 29;
                else
                    numberOfDays = 28;
            }
            else
            {
                //months with 30 days
                if (month.Equals("april") || month.Equals("june") ||
                        month.Equals("september") || month.Equals("november"))
                {
                    numberOfDays = 30;
                }
                //months with 31 days
                else
                {
                    numberOfDays = 31;
                }
            }
            for (int i = 1; i <= numberOfDays; i++)
            {
                dayCount[i] = 0;
            }
            return dayCount;
        }
    }
}
