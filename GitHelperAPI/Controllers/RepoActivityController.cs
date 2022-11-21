/* 
 Created By:        Sumedha Samanta
 Created Date:      17-11-2022
 Modified Date:     21-11-2022
 Purpose:           This class is responsible for setting a repository as favourite or removing it.
 Purpose Type:      Defines APIs for functionalities to serve requests relating to setting or removing favourite repository.
 Referenced files:  Models\StatusDetailsModel.cs
 */

using GitHelperAPI.Models;
using GitHelperDAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using ActionNameAttribute = System.Web.Http.ActionNameAttribute;
using System.Configuration;
using GitHelperDAL.Model;
using GitHelperAPI.CustomException;
using GitHelperAPI.Utilities;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web.Security;

namespace GitHelperAPI.Controllers
{
    [System.Web.Http.Authorize]
    public class RepoActivityController : ApiController
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
               sets a repository as favourite for a particular user by removing previous favourite if any.
           </summary>
           <param name="userId"> userId unique to the user </param>
           <param name="repoId"> repoId of the repository to be set as favourite </param>
           <returns>whether setting favourite is successful or not</returns>
       */
        [HttpPost]
        [ActionName("SetFavourite")]
        public StatusDetailsModel SetFavourite([FromBody]  long repoId)
        {
            try
            {
                AuthenticationData authData = GetAuthCookieDetails();
                DbService dbService = DbService.getInstance(ConfigurationManager.AppSettings["dataSourceName"]);
                log.Info($"Setting repository {repoId} as favourite for user: {authData.userName}");
                dbService.setFavourite(authData.userId, repoId);
                log.Info("Setting favourite is successful");
                return new StatusDetailsModel { status = "Success", message = "Setting Favourite Successful" };
            }
            catch (Exception ex)
            {
                log.Error("Exception occured while processing request.");
                log.Error($"Stack Trace :\n{ex.ToString()}");
                return new StatusDetailsModel { status = "Failure", message = "Bad Request" };
            }
        }

        /*
           <summary>
               removes a repository from favourite for a particular user if it was favourite.
           </summary>
           <param name="userId"> userId unique to the user </param>
           <param name="repoId"> repoId of the repository to be removed from favourite </param>
           <returns>whether removing favourite is successful or not</returns>
       */
        [HttpPost]
        [ActionName("RemoveFavourite")]
        public StatusDetailsModel RemoveFavourite([FromBody] long repoId)
        {
            try
            {
                AuthenticationData authData = GetAuthCookieDetails();
                DbService dbService = DbService.getInstance(ConfigurationManager.AppSettings["dataSourceName"]);
                log.Info($"Removing repository {repoId} from favourite for user: {authData.userName}");
                bool isRemoved = dbService.removeFavourite(authData.userId, repoId);
                if (isRemoved)
                {
                    log.Info("Removing favourite is successful");
                    return new StatusDetailsModel { status = "Success", message = "Removing Favourite Successful" };
                }
                else
                {
                    log.Error($"Removing favourite failed. Possible reasons could be {repoId} was not favourite or was deleted");
                    return new StatusDetailsModel { status = "Failure", message = "Bad Request" };
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception occured while processing request.");
                log.Error($"Stack Trace :\n{ex.ToString()}");
                return new StatusDetailsModel { status = "Failure", message = "Bad Request" };
            }
        }

        /*
           <summary>
               updates the visit counts of a repository with respect to a user.
           </summary>
           <param name="userId"> userId unique to the user </param>
           <param name="repoCountList"> list containing repoId and count of visits of the repositories </param>
           <returns>whether updating repository count is successful or not</returns>
       */
        [HttpPost]
        [ActionName("UpdateRepoCount")]
        public StatusDetailsModel UpdateRepoCount(List<RepoCountUpdateModel> repoCountList)
        {
            try
            {
                AuthenticationData authData = GetAuthCookieDetails();
                if (repoCountList.Count == 0)
                    throw new NullReferenceException();
                DbService dbService = DbService.getInstance(ConfigurationManager.AppSettings["dataSourceName"]);
                log.Info($"Updating repository counts for user");
                dbService.updateRepoCount(authData.userId, repoCountList);
                return new StatusDetailsModel{status = "Success", message = "Successful"};
            }
            catch(NullReferenceException ex)
            {
                log.Error("Exception occured while processing request.");
                log.Error($"Stack Trace :\n{ex.ToString()}");
                return new StatusDetailsModel { status = "Failure", message = "Repository Count List Cannot be empty." };

            }
            catch(Exception ex)
            {
                log.Error("Exception occured while processing request.");
                log.Error($"Stack Trace :\n{ex.ToString()}");
                return new StatusDetailsModel { status = "Failure", message = "Bad Request" };
            }
        }
    }
}