/* 
 Created By:        Sumedha Samanta
 Created Date:      20-10-2022
 Modified Date:     08-11-2022
 Purpose:           This class is accessible to authenticated users only. It defines APIs for dashboard functionalities (fetching user details, repository details, and so on).
 Purpose Type:      Defines APIs for dashboard functionalities to serve authenticated user requests.
 Referenced files:  Utilities\AuthenticationTicketUtil.cs,
                    Utilities\DateFormatter.cs,
                    Models\AuthenticationData.cs,
                    Models\UserDetails.cs,
                    CustomException\NullAuthCookieException.cs
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using GitHelperAPI.Utilities;
using ActionNameAttribute = System.Web.Http.ActionNameAttribute;
using GitHelperAPI.Models;
using GitHelperDAL.Model;
using GitHelperDAL.Services;
using System.Web.Security;
using System.Net.Http.Headers;
using GitHelperAPI.CustomException;

namespace GitHelperAPI.Controllers
{
    [Authorize]
    public class DashboardController : ApiController
    {
        private static readonly log4net.ILog log = LogHelper.GetLogger();


        /*
            <summary>
                responsible for reading username and token from authentication cookie
            </summary>
            <param> None </param>
            <returns>username and token of the user; if not found, throws NullAuthCookieException</returns>
        */
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
                //throwing error 
                throw new NullAuthCookieException("Authentication cookie not found");
            }

            return authData;
        }

        /*
           <summary>
               fetches details of the authorized user for Dashboard UI
           </summary>
           <param> None </param>
           <returns>avatar-url, list of repo names and owner names of the user</returns>
       */
        [HttpGet]
        [ActionName("GetUserDetails")]
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

        /*
           <summary>
               fetches details of a repository requested by the authorized user
           </summary>
           <param name="ownerName"> name of the owner of the repository </param>
           <param name="repoName"> name of the repository </param> 
           <returns>
                repo name, owner name, repo link, creation date, updation date of the repository
           </returns>
       */
        [HttpGet]
        [ActionName("GetParticularRepoDetails")]
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

        /*
           <summary>
               fetches list of commits of a repository requested by the authorized user
           </summary>
           <param name="ownerName"> name of the owner of the repository </param>
           <param name="repoName"> name of the repository </param> 
           <returns>
                list of commits with their author name, commit message and commit date for the repository
           </returns>
       */
        [HttpGet]
        [ActionName("GetCommits")]
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

        /*
           <summary>
               fetches paginated list of commits of a repository requested by the authorized user
           </summary>
           <param name="ownerName"> name of the owner of the repository </param>
           <param name="repoName"> name of the repository </param> 
           <param name="pageNumber"> page number </param>
           <param name="pageSize"> page size</param>
           <returns>
                paginated list of commits with their author name, commit message and commit date for the repository
           </returns>
       */
        [HttpGet]
        [ActionName("GetPaginatedCommits")]
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

        /*
           <summary>
               fetches list of months from the creation date for a repository requested by the authorized user
           </summary>
           <param name="ownerName"> name of the owner of the repository </param>
           <param name="repoName"> name of the repository </param> 
           <returns>
               list of months for the requested repository
           </returns>
       */
        [HttpGet]
        [ActionName("GetMonthYearList")]
        public List<Dictionary<string, string>> GetMonthYearList(string ownerName, string repoName)
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

        /*
           <summary>
               fetches number of commits done on each date of a month of a year for a repository requested by the authorized user
           </summary>
           <param name="ownerName"> name of the owner of the repository </param>
           <param name="repoName"> name of the repository </param> 
           <param name="month"> month for which number of commits have to be found </param>
           <param name="year"> year for which number of commits have to be found </param>
           <returns>
                number of commits done per date of the given month and year for the requested repository
           </returns>
       */
        [HttpGet]
        [ActionName("GetDateCount")]
        public List<Dictionary<string, int>> GetDateCount(string ownerName, string repoName, string month, string year)
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

        /*
           <summary>
               fetches names and bytes of code for all the languages used in the repository requested by the authorized user
           </summary>
           <param name="ownerName"> name of the owner of the repository </param>
           <param name="repoName"> name of the repository </param> 
           <returns>
                all the languages used in the requested repository
           </returns>
       */
        [HttpGet]
        [ActionName("GetRepoLanguages")] 
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
