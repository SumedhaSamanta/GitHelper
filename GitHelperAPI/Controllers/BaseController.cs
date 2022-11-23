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
using GitHelperAPI.Response;
using System.Configuration;
using GitHelper_1.Controllers;
using GitHelperAPI;

namespace GitHelper_1.Controllers
{
    public abstract class BaseController : ApiController
    {
        protected static readonly log4net.ILog log = LogHelper.GetLogger();

        /*
           <summary>
               responsible for reading username and token from authentication cookie
           </summary>
           <param> None </param>
           <returns>username and token of the user; if not found, throws NullAuthCookieException</returns>
       */
        protected AuthenticationData GetAuthCookieDetails()
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
    }
}