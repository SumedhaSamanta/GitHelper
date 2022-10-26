using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
using System.Web.Security;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

namespace GitHelper_1.Controllers
{
    [Authorize]
    public class DashboardController : ApiController
    {

        private AuthenticationData GetAuthCookieDetails()
        {
            AuthenticationData authData = null;
            CookieHeaderValue cookie = Request.Headers.GetCookies(FormsAuthentication.FormsCookieName).FirstOrDefault();

            if (cookie != null)
            {
                string ticket = cookie[FormsAuthentication.FormsCookieName].Value;
                authData = AuthenticationTicketUtil.getAuthenticationDataFromTicket(ticket);

            }
            else
            {
                //think of throwing error message
            }

            return authData;
        }


        [HttpGet]
        [ActionName("GetUserDetails")]
        //return avatar-url, list of repo names and owner names
        public UserDetails GetUserDetails()
        {
            AuthenticationData authData = GetAuthCookieDetails();

            GitHubApiService client = GitHubApiService.getInstance(authData.userName, authData.userToken);
            //get repo-names, repo - owner name and user - avatar - url
            List<RepoDetailsModel> repoList = client.GetRepoDetails();
            string avatarURL = client.GetAvtarUrl();

            UserDetails result = new UserDetails { repoList = repoList, userAvatarUrl = avatarURL };

            return result;
        }

        [HttpGet]
        [ActionName("GetCommits")]

        //get details of the commits (commitAuthorName, commitMessage, commitDate)
        public List<CommitDetailsModel> GetCommits(string ownerName, string repoName)
        {
            AuthenticationData authData = GetAuthCookieDetails();

            List<CommitDetailsModel> commitsList = GitHubApiService.getInstance(authData.userName, authData.userToken).GetCommitDetails(ownerName, repoName);
            foreach(CommitDetailsModel commit in commitsList)
            {
                commit.commitDateTime = DateFormatter.ConvertToUserPref(commit.commitDateTime);
            }

            return commitsList;
        }

        [HttpGet]
        [ActionName("GetPaginatedCommits")]

        //get details of the commits (commitAuthorName, commitMessage, commitDate)
        public List<CommitDetailsModel> GetPaginatedCommits(string ownerName, string repoName, int pageNumber, int pageSize)
        {
            AuthenticationData authData = GetAuthCookieDetails();

            List<CommitDetailsModel> commitsList = GitHubApiService.getInstance(authData.userName, authData.userToken).GetPaginatedCommits(ownerName, repoName, pageNumber, pageSize);
            foreach (CommitDetailsModel commit in commitsList)
            {
                commit.commitDateTime = DateFormatter.ConvertToUserPref(commit.commitDateTime);
            }

            return commitsList;
        }


        //[HttpGet]
        //[ActionName("GetCommitGraphData")]
        ////not needed
        ////get number of commits made on each date of the given month and year for the repo
        //public List<Dictionary<string, int>> GetCommitGraphData(string ownerName, string repoName, string month, string year)
        //{
        //    string userName = "";
        //    string token = "";

        //    List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();

        //    //dictionary to store the daywise commit counts
        //    Dictionary<int, int> dayCount = GetDayCount(month, year);

        //    //fetch commits of the repo
        //    List<CommitDetailsModel> commitsList = GitHubApiService.getInstance(userName, token).GetCommitDetails(ownerName, repoName);
        //    foreach (CommitDetailsModel commit in commitsList)
        //    {
        //        Dictionary<string, string> monthYear = new Dictionary<string, string>();

        //        //DateTime commitDate = new DateFormatter().ConvertUTCtoIST(commit.DateStr);
        //        //string commitMonth = commitDate.ToString("MMMM").ToLower();
        //        //string commitYear = commitDate.ToString("yyyy");

        //        //if (commitMonth.Equals(month.ToLower()) && commitYear.Equals(year))
        //        //{
        //        //    string date = commitDate.ToString("d");
        //        //    int day = int.Parse(date);
        //        //    dayCount[day] += 1;
        //        //}
        //    }

        //    foreach (KeyValuePair<int, int> entry in dayCount)
        //    {
        //        Dictionary<string,int> commitCount = new Dictionary<string, int>();
        //        commitCount.Add("commits",entry.Value);
        //        commitCount.Add("day", entry.Key);
        //        result.Add(commitCount);
        //    }

        //    return result;
        //}

        [HttpGet]
        [ActionName("GetMonthYearList")]
        public List<Dictionary<string, string>> GetMonthYearList(string ownerName, string repoName) //List<Dictionary<string, string>>
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            AuthenticationData authData = GetAuthCookieDetails();

            //fetch commits of the repo
            DateTimeOffset localRepoCreatingDate = DateFormatter.ConvertToUserPref(GitHubApiService.getInstance(authData.userName,authData.userToken).GetRepositoryCreationDate(ownerName, repoName));

            DateTimeOffset localRepoCreationMonth = DateFormatter.CreateUserPrefDateTimeOffset(new DateTime(localRepoCreatingDate.Year, localRepoCreatingDate.Month, 1));

            DateTimeOffset iterator = DateFormatter.getCurrentUserPrefTime();

            List<Dictionary<string, string>> monthYearList = new List<Dictionary<string, string>>();

            while (localRepoCreationMonth <= iterator)
            {
                Dictionary<string, string> monthYear = new Dictionary<string, string>();
                monthYear.Add("month", iterator.ToString("MMMM"));
                monthYear.Add("year", iterator.ToString("yyyy"));

                monthYearList.Add(monthYear);

                iterator = iterator.AddMonths(-1);
            }

            return monthYearList;
        }

        [HttpGet]
        [ActionName("GetDateCount")]
        public List<Dictionary<string, int>> GetDateCount(string ownerName, string repoName, string month, string year) //List<Dictionary<string, int>>
        {
            List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();

            AuthenticationData authData = GetAuthCookieDetails();

            DateTimeOffset localFirstDateOfMonth = DateFormatter.CreateUserPrefDateTimeOffset(DateTime.ParseExact(month + " " + year, "MMMM yyyy", null));

            int numberOfDays = DateTime.DaysInMonth(localFirstDateOfMonth.Year, localFirstDateOfMonth.Month);

            DateTimeOffset localLastDateofMonth = DateFormatter.CreateUserPrefDateTimeOffset(new DateTime(localFirstDateOfMonth.Year, localFirstDateOfMonth.Month, numberOfDays));


            //call function to get commit list for specified dates

            List<CommitDetailsModel> commitList = GitHubApiService.getInstance(authData.userName, authData.userToken).GetCommitsForInterval(ownerName, 
                repoName, DateFormatter.ConvertToUtc(localFirstDateOfMonth), DateFormatter.ConvertToUtc(localLastDateofMonth));

            int[] numCommits = new int[numberOfDays];

            foreach (var commit in commitList)
            {
                numCommits[DateFormatter.ConvertToUserPref(commit.commitDateTime).Day] += 1;
            }

            for (int i = 0; i < numCommits.Length; i++)
            {
                Dictionary<string, int> commitperDay = new Dictionary<string, int>();
                commitperDay.Add("day", i + 1);
                commitperDay.Add("commits", numCommits[i]);
                result.Add(commitperDay);
            }

            return result;
        }


        //Detele this (not required)
        //[HttpGet]
        //[ActionName("GetReposInfo")]
        //public List<string> GetReposInfo(string ownerName, string repoName)
        //{
        //    string userName = "";
        //    string token = "";

        //    //int numberOfContributors = GitHubApiService.getInstance(userName,token).getNumberOfContributors(ownerName, repoName);
        //    //List<LanguageDetails> languagesUsed = GitHubApiService.getInstance(userName, token).GetRepositoryLanguages(ownerName, repoName);

        //    List<string> result = new List<string>();

        //    //result.Add(numberOfContributors.ToString());
        //    //result.Add(languagesUsed);

        //    return result;
        //}

        [HttpGet]
        [ActionName("GetRepoLanguages")]
        public List<LanguageDetails> GetRepoLanguages(string ownerName, string repoName)
        {
            AuthenticationData authData = GetAuthCookieDetails();

            List<LanguageDetails> languagesUsed = GitHubApiService.getInstance(authData.userName, authData.userToken).GetRepositoryLanguages(ownerName, repoName);

            return languagesUsed;
        }



        //public Dictionary<int, int> GetDayCount(string month, string year)
        //{
        //    Dictionary<int, int> dayCount = new Dictionary<int, int>();
        //    int numberOfDays = 0;
        //    month = month.ToLower();
        //    int y = int.Parse(year);

        //    if (month.Equals("february"))
        //    {
        //        if (((y % 4 == 0) && (y % 100 != 0)) || (y % 400 == 0))
        //            numberOfDays = 29;
        //        else
        //            numberOfDays = 28;
        //    }
        //    else
        //    {
        //        //months with 30 days
        //        if (month.Equals("april") || month.Equals("june") ||
        //                month.Equals("september") || month.Equals("november"))
        //        {
        //            numberOfDays = 30;
        //        }
        //        //months with 31 days
        //        else
        //        {
        //            numberOfDays = 31;
        //        }
        //    }
        //    for (int i = 1; i <= numberOfDays; i++)
        //    {
        //        dayCount[i] = 0;
        //    }
        //    return dayCount;
        //}
    }
}
