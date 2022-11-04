using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using GitHelper_1.Utilities;
using ActionNameAttribute = System.Web.Http.ActionNameAttribute;
using GitHelper_1.Models;
using GitHelperDAL.Model;
using GitHelperDAL.Services;
using System.Web.Security;
using System.Net.Http.Headers;
using GitHelper_1.CustomException;

namespace GitHelper_1.Controllers
{
    [Authorize]
    public class DashboardController : ApiController
    {
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly log4net.ILog log = LogHelper.GetLogger();

        //read data (username and token) from authentication cookie
        private AuthenticationData GetAuthCookieDetails()
        {
            AuthenticationData authData = null;
            CookieHeaderValue cookie = Request.Headers.GetCookies(FormsAuthentication.FormsCookieName).FirstOrDefault();

            if (cookie != null)
            {
                log.Info("Reading data from authentication cookie successful.");
                string ticket = cookie[FormsAuthentication.FormsCookieName].Value;
                authData = AuthenticationTicketUtil.getAuthenticationDataFromTicket(ticket);
            }
            else
            {
                log.Error("Authentication cookie data not found.");
                //throwing error message
                // var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new NullAuthCookieException("Authentication cookie not found");
            }

            return authData;
        }


        [HttpGet]
        [ActionName("GetUserDetails")]
        //return avatar-url, list of repo names and owner names
        public UserDetails GetUserDetails()
        {
            try
            {
                AuthenticationData authData = GetAuthCookieDetails();
                log.Info($"Fetching user details for user: {authData.userName}");
                GitHubApiService client = GitHubApiService.getInstance(authData.userName, authData.userToken);
                //get repo-names, repo - owner name and user - avatar - url
                List<RepoDetailsModel> repoList = client.GetRepoDetails();
                string avatarURL = client.GetAvtarUrl();
                UserDetails result = new UserDetails { repoList = repoList, userAvatarUrl = avatarURL };
                log.Info("Fetching successful.");
                return result;
            }
            catch (NullAuthCookieException ex)
            {
                log.Error(ex.Message);
                log.Error($"Stack Trace :\n{ex.ToString()}");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                log.Error("Exception occured while processing request.");
                log.Error($"Stack Trace :\n{ex.ToString()}");
                
                var resp = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request.");
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        [ActionName("GetParticularRepoDetails")]
        //get details of a particular repo (repo name, owner name, repo link, creation date, updation date)
        public ParticularRepoDetailsModel GetParticularRepoDetails(string ownerName, string repoName)
        {
            try
            {
                AuthenticationData authData = GetAuthCookieDetails();
                log.Info($"Fetching details of repository [name: {repoName} , owner: {ownerName}] for user :{authData.userName}");

                GitHubApiService gitHubApiService = GitHubApiService.getInstance(authData.userName, authData.userToken);
                ParticularRepoDetailsModel particularRepoDetails = gitHubApiService.GetParticularRepoDetails(ownerName, repoName);
                particularRepoDetails.createdAt = DateFormatter.ConvertToUserPref(particularRepoDetails.createdAt);
                particularRepoDetails.updatedAt = DateFormatter.ConvertToUserPref(particularRepoDetails.updatedAt);
                log.Info("Fetching successful.");
                return particularRepoDetails;
            }
            catch (NullAuthCookieException ex)
            {
                log.Error(ex.Message);
                log.Error($"Stack Trace :\n{ex.ToString()}");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                log.Error("Exception occured while processing request.");
                log.Error($"Stack Trace :\n{ex.ToString()}");

                var resp = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request.");
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        [ActionName("GetCommits")]

        //get details of the commits (commitAuthorName, commitMessage, commitDate)
        public List<CommitDetailsModel> GetCommits(string ownerName, string repoName)
        {
            try
            {
                AuthenticationData authData = GetAuthCookieDetails();
                log.Info($"Fetching commit details of repository [name: {repoName} , owner: {ownerName}] for user {authData.userName} .");
                GitHubApiService gitHubApiService = GitHubApiService.getInstance(authData.userName, authData.userToken);
                List<CommitDetailsModel> commitsList = gitHubApiService.GetCommitDetails(ownerName, repoName);

                foreach (CommitDetailsModel commit in commitsList)
                {
                    commit.commitDateTime = DateFormatter.ConvertToUserPref(commit.commitDateTime);
                }
                log.Info("Fetching successful.");
                return commitsList;
            }
            catch (NullAuthCookieException ex)
            {
                log.Error(ex.Message);
                log.Error($"Stack Trace :\n{ex.ToString()}");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                log.Error("Exception occured while processing request.");
                log.Error($"Stack Trace :\n{ex.ToString()}");

                var resp = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request.");
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        [ActionName("GetPaginatedCommits")]

        //get details of the commits (commitAuthorName, commitMessage, commitDate)
        public List<CommitDetailsModel> GetPaginatedCommits(string ownerName, string repoName, int pageNumber, int pageSize)
        {
            try
            {
                AuthenticationData authData = GetAuthCookieDetails();
                log.Info($"Fetching paginated commit details [pageNumber: {pageNumber}, pageSize: {pageSize}] of repository [name: {repoName} , owner: {ownerName}] for user :{authData.userName}");
                List<CommitDetailsModel> commitsList = GitHubApiService.getInstance(authData.userName, authData.userToken).GetPaginatedCommits(ownerName, repoName, pageNumber, pageSize);
                foreach (CommitDetailsModel commit in commitsList)
                {
                    commit.commitDateTime = DateFormatter.ConvertToUserPref(commit.commitDateTime);
                }
                log.Info("Fetching successful.");
                return commitsList;
            }
            catch (NullAuthCookieException ex)
            {
                log.Error(ex.Message);
                log.Error($"Stack Trace :\n{ex.ToString()}");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                log.Error("Exception occured while processing request.");
                log.Error($"Stack Trace :\n{ex.ToString()}");

                var resp = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request.");
                throw new HttpResponseException(resp);
            }
        }
        
        [HttpGet]
        [ActionName("GetMonthYearList")]
        //get month-year list for a particular repository
        public List<Dictionary<string, string>> GetMonthYearList(string ownerName, string repoName) //List<Dictionary<string, string>>
        {
            
            try
            {
                AuthenticationData authData = GetAuthCookieDetails();
                log.Info($"Fetching month list of repository [name: {repoName} , owner: {ownerName}] for user :{authData.userName}");
                List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

               

                DateTimeOffset localRepoCreatingDate = DateFormatter.ConvertToUserPref(GitHubApiService.getInstance(authData.userName, authData.userToken).GetRepositoryCreationDate(ownerName, repoName));

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
                log.Info("Fetching successful.");
                return monthYearList;
            }
            catch (NullAuthCookieException ex)
            {
                log.Error(ex.Message);
                log.Error($"Stack Trace :\n{ex.ToString()}");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                log.Error("Exception occured while processing request.");
                log.Error($"Stack Trace :\n{ex.ToString()}");

                var resp = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request.");
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        [ActionName("GetDateCount")]
        //returns number of commits done per date of a gaiven month and year
        public List<Dictionary<string, int>> GetDateCount(string ownerName, string repoName, string month, string year) //List<Dictionary<string, int>>
        {
            try
            {
                AuthenticationData authData = GetAuthCookieDetails();
                log.Info($"Fetching contribution details for the month of {month}, {year} of repository [name: {repoName} , owner: {ownerName}] for user :{authData.userName}");
                List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();

                

                DateTimeOffset localFirstDateOfMonth = DateFormatter.CreateUserPrefDateTimeOffset(DateTime.ParseExact(month + " " + year, "MMMM yyyy", null));

                int numberOfDays = DateTime.DaysInMonth(localFirstDateOfMonth.Year, localFirstDateOfMonth.Month);

                DateTimeOffset localLastDateofMonth = localFirstDateOfMonth.AddMonths(1).Subtract(TimeSpan.FromTicks(1));


                //call function to get commit list for specified dates

                List<CommitDetailsModel> commitList = GitHubApiService.getInstance(authData.userName, authData.userToken).GetCommitsForInterval(ownerName,
                    repoName, DateFormatter.ConvertToUtc(localFirstDateOfMonth), DateFormatter.ConvertToUtc(localLastDateofMonth));

                int[] numCommits = new int[numberOfDays];

                foreach (var commit in commitList)
                {
                    numCommits[DateFormatter.ConvertToUserPref(commit.commitDateTime).Day - 1] += 1;
                }

                for (int i = 0; i < numCommits.Length; i++)
                {
                    Dictionary<string, int> commitPerDay = new Dictionary<string, int>();
                    commitPerDay.Add("day", i + 1);
                    commitPerDay.Add("commits", numCommits[i]);
                    result.Add(commitPerDay);
                }
                log.Info("Fetching completed successfully.");
                return result;
            }
            catch (NullAuthCookieException ex)
            {
                log.Error(ex.Message);
                log.Error($"Stack Trace :\n{ex.ToString()}");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                log.Error("Exception occured while processing request.");
                log.Error($"Stack Trace :\n{ex.ToString()}");

                var resp = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request.");
                throw new HttpResponseException(resp);
            }
        }

        [HttpGet]
        [ActionName("GetRepoLanguages")]
        //get all the languages used in a particular repository
        public List<LanguageDetails> GetRepoLanguages(string ownerName, string repoName)
        {
            try
            {

                AuthenticationData authData = GetAuthCookieDetails();
                log.Info($"Fetching language details of repository [name: {repoName} , owner: {ownerName}] for user :{authData.userName}");

                List<LanguageDetails> languagesUsed = GitHubApiService.getInstance(authData.userName, authData.userToken).GetRepositoryLanguages(ownerName, repoName);
                log.Info("Fetching successful.");
                return languagesUsed;
            }
            catch (NullAuthCookieException ex)
            {
                log.Error(ex.Message);
                log.Error($"Stack Trace :\n{ex.ToString()}");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                log.Error("Exception occured while processing request.");
                log.Error($"Stack Trace :\n{ex.ToString()}");

                var resp = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request.");
                throw new HttpResponseException(resp);
            }
        }
    }
}
