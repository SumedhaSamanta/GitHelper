/* 
 Created By:        Sumedha Samanta
 Created Date:      17-11-2022
 Modified Date:     17-11-2022
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
using ActionNameAttribute = System.Web.Http.ActionNameAttribute;
using System.Configuration;

namespace GitHelperAPI.Controllers
{
    [System.Web.Http.Authorize]
    public class FavouriteController : ApiController
    {
        private static readonly log4net.ILog log = LogHelper.GetLogger();

        /*
           <summary>
               sets a repository as favourite for a particular user by removing previous favourite if any.
           </summary>
           <param name="userId"> userId unique to the user </param>
           <param name="repoId"> repoId of the repository to be set as favourite </param>
           <returns>whether setting favourite is successful or not</returns>
       */
        [HttpGet]
        [ActionName("SetFavourite")]
        public StatusDetailsModel SetFavourite(long userId, long repoId)
        {
            try
            {
                DbService dbService = DbService.getInstance(ConfigurationManager.AppSettings["dataSourceName"]);
                log.Info($"Setting repository {repoId} as favourite for user: {userId}");
                dbService.setFavourite(userId, repoId);
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
        [HttpGet]
        [ActionName("RemoveFavourite")]
        public StatusDetailsModel RemoveFavourite(long userId, long repoId)
        {
            try
            {
                DbService dbService = DbService.getInstance(ConfigurationManager.AppSettings["dataSourceName"]);
                log.Info($"Removing repository {repoId} from favourite for user: {userId}");
                bool isRemoved = dbService.removeFavourite(userId, repoId);
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
    }
}