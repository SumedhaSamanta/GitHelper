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
                log.Info("Reading data from authentication cookie successful");
                string ticket = cookie[FormsAuthentication.FormsCookieName].Value;
                authData = AuthenticationTicketUtil.getAuthenticationDataFromTicket(ticket);
            }
            else
            {
                log.Error("Reading data from authentication cookie failed");
                //throwing error message
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
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

                GitHubApiService client = GitHubApiService.getInstance(authData.userName, authData.userToken);
                //get repo-names, repo - owner name and user - avatar - url
                List<RepoDetailsModel> repoList = client.GetRepoDetails();
                log.Info("Fetching list of repositories of the user successful");
                string avatarURL = client.GetAvtarUrl();
                log.Info("Fetching avatar of the user successful");
                UserDetails result = new UserDetails { repoList = repoList, userAvatarUrl = avatarURL };

                return result;
            }
            catch
            {
                //throwing error message
                log.Error("Fetching details of the user failed");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
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
                GitHubApiService gitHubApiService = GitHubApiService.getInstance(authData.userName, authData.userToken);
                ParticularRepoDetailsModel particularRepoDetails = gitHubApiService.GetParticularRepoDetails(ownerName, repoName);
                particularRepoDetails.createdAt = DateFormatter.ConvertToUserPref(particularRepoDetails.createdAt);
                particularRepoDetails.updatedAt = DateFormatter.ConvertToUserPref(particularRepoDetails.updatedAt);
                log.Info("Fetching details of particular repositories of the user successful");

                return particularRepoDetails;
            }
            catch (HttpResponseException ex)
            {
                log.Error("Reading data from authentication cookie failed");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                log.Error("Fetching details of particular repositories of the user failed");
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
                GitHubApiService gitHubApiService = GitHubApiService.getInstance(authData.userName, authData.userToken);
                List<CommitDetailsModel> commitsList = gitHubApiService.GetCommitDetails(ownerName, repoName);

                foreach (CommitDetailsModel commit in commitsList)
                {
                    commit.commitDateTime = DateFormatter.ConvertToUserPref(commit.commitDateTime);
                }
                log.Info("Fetching details of commits details of particular repository of the user successful");

                return commitsList;
            }
            catch(HttpResponseException ex)
            {
                log.Error("Reading data from authentication cookie failed");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch(Exception ex)
            {
                log.Error("Fetching details of commits details of particular repository of the user successful");
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

                List<CommitDetailsModel> commitsList = GitHubApiService.getInstance(authData.userName, authData.userToken).GetPaginatedCommits(ownerName, repoName, pageNumber, pageSize);
                foreach (CommitDetailsModel commit in commitsList)
                {
                    commit.commitDateTime = DateFormatter.ConvertToUserPref(commit.commitDateTime);
                }
                log.Info("Fetching paginated details of commits of particular  repositories of the user successful");

                return commitsList;
            }
            catch (HttpResponseException ex)
            {
                log.Error("Reading data from authentication cookie failed");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                log.Error("Fetching paginated details of commits of particular  repositories of the user failed");
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
                List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

                AuthenticationData authData = GetAuthCookieDetails();

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
                log.Info("Fetching month-year list of particular repositories of the user successful");

                return monthYearList;
            }
            catch (HttpResponseException ex)
            {
                log.Error("Reading data from authentication cookie failed");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                log.Error("Fetching month-year list of particular repositories of the user failed");
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
                List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();

                AuthenticationData authData = GetAuthCookieDetails();

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
                log.Info("Fetching details of number of commits of particular repositories on particular date of the user successful");

                return result;
            }
            catch (HttpResponseException ex)
            {
                log.Error("Reading data from authentication cookie failed");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {
                log.Error("Fetching details of number of commits of particular repositories on particular date of the user failed");
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

                List<LanguageDetails> languagesUsed = GitHubApiService.getInstance(authData.userName, authData.userToken).GetRepositoryLanguages(ownerName, repoName);
                log.Info("Fetching list of languages used in particular repositories of the user successful");

                return languagesUsed;
            }
            catch (HttpResponseException ex)
            {

                log.Error("Reading data from authentication cookie failed");
                var resp = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bad Credentials. Please Login.");
                throw new HttpResponseException(resp);
            }
            catch (Exception ex)
            {

                log.Error("Fetching list of languages used in particular repositories of the user failed");
                var resp = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request.");
                throw new HttpResponseException(resp);
            }
        }
    }
}
