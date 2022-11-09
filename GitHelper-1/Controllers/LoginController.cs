/* 
 Created By:        Sumedha Samanta
 Created Date:      20-10-2022
 Modified Date:     08-11-2022
 Purpose:           This class is used for logging in, authenticating and logging out user.
 Purpose Type:      This class returns response whether authentication/logging out is successful or not
 Referenced files:  Models\StatusDetailsModel.cs, 
                    Utilities\AuthenticationTicketUtil.cs
 */

using GitHelper_1.Models;
using GitHelper_1.Utilities;
using GitHelperDAL.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using ActionNameAttribute = System.Web.Http.ActionNameAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace GitHelper_1.Controllers
{
    public class LoginController : ApiController
    {
        private static readonly log4net.ILog log = LogHelper.GetLogger();

        /*
            <summary>
                authenticates user before logging in.
                If authentication is succesful, username and token is stored in authentication cookie.
            </summary>
            <param name="data"> body of the POST request containing username and token </param>
            <returns> returns success message if credentials are valid; else failure message </returns>
        */
        [HttpPost]
        public HttpResponseMessage AuthenticateUser([FromBody] JObject data)
        {
            
            var username = data["username"].ToString();
            var token = data["token"].ToString();
            //check validity of the credentials
            bool isValidCredential = IsValidCredentials(username, token);

            if (isValidCredential)
            {
                //check aunthenticity of the credentials
                GitHubApiService gitApiService = GitHubApiService.getInstance(username, token);
                bool isValidUser = gitApiService.AuthenticateUser();

                if (isValidUser)
                {

                    //return auth cookie and response for success
                    HttpResponseMessage responseMsg = Request.CreateResponse(HttpStatusCode.OK,
                        new StatusDetailsModel { status = "Success", message = "Authentication Successful" });

                    string ticket = AuthenticationTicketUtil.createAuthenticationTicket(username, token);
                    var cookie = new CookieHeaderValue(FormsAuthentication.FormsCookieName, ticket);
                    cookie.Expires = DateTimeOffset.Now.AddDays(1);
                    cookie.Domain = Request.RequestUri.Host;
                    cookie.Path = "/";
                    responseMsg.Headers.AddCookies(new CookieHeaderValue[] { cookie });
                    log.Info($"User is authenticated. Created authentication cookie for user: {username}");

                    return responseMsg;
                }
                else
                {
                    //return appropriate message for failure
                    log.Info("Invalid username/token provided");
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new StatusDetailsModel { status = "Failure", message = "Bad Credentials" });

                }
            }
            log.Info("Invalid username/token format provided");
            //return appropriate message for failure
            return Request.CreateResponse(HttpStatusCode.OK,
                new StatusDetailsModel { status = "Failure", message = "Wrong format for username or personal access token" });
        }

        /*
            <summary>
                erases authentication cookie data of current user before logging out
            </summary>
            <param> None </param>
            <returns> apppropriate success message after logout </returns>
        */
        [HttpGet]
        [ActionName("Logout")]
        public HttpResponseMessage Logout()
        { 
            log.Info($"User [{HttpContext.Current.User.Identity.Name}] logged out succesfully. Clearing authentication cookie");
            FormsAuthentication.SignOut();
            //return successful logout confirmation
            return Request.CreateResponse(HttpStatusCode.OK,
                new StatusDetailsModel { status = "Success", message = "Logged Out Successfully" });
        }

        /*
            <summary>
                checks if the user is authenticated
            </summary>
            <param> None </param>
            <returns> "authenticated" if user is authenticated; else "not authenticated" </returns>
        */
        [HttpGet]
        [ActionName("IsAuthenticated")]
        public HttpResponseMessage IsAuthenticated()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                log.Info($"Authentication cookie found. User is authenticated as {HttpContext.Current.User.Identity.Name}");
                return Request.CreateResponse(HttpStatusCode.OK,
                new StatusDetailsModel { status = "Authenticated", message = "User is Authenticated" });
            }
            else
            {
                log.Info($"Authentication cookie not found. User is not authenticated.");
                return Request.CreateResponse(HttpStatusCode.OK,
                new StatusDetailsModel { status = "Unauthenticated", message = "User is Not Authenticated" });
            }
            
        }

        /*
            <summary>
                checks if the credentials provided by user are in proper format or not
            </summary>
            <param name="username"> username provided by the user </param>
            <param name="token"> token provided by the user </param>
            <returns> true if credentials are in proper format and false otherwise </returns>
        */
        private bool IsValidCredentials(String username, String token)
        {
            //username and token should not be empty
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(token))
                return false;

            /*
                Github username may only contain alphanumeric characters or hyphens.
                Github username cannot have multiple consecutive hyphens.
                Github username cannot begin or end with a hyphen.
                Maximum is 39 characters.*/
            

            if (username.Length > 39 || username.StartsWith("-") || username.EndsWith("-"))
                return false;

            string temporary = username.ToLower();
            for (int i = 0; i < temporary.Length; i++)
            {
                if (!(temporary[i] >= 'a' && temporary[i] <= 'z'))
                {
                    if (!(temporary[i] >= '0' && temporary[i] <= '9'))
                    {
                        if (!temporary[i].Equals('-'))
                        {
                            return false;
                        }
                        else
                        {
                            if (i < temporary.Length - 1 && temporary[i + 1].Equals('-'))
                            {
                                return false;
                            }
                        }
                    }
                }

            }

            //Tokens generated by GitHub are 43 characters in length
            if (!(token.Length==40 || token.Length== 43))
                return false;

            return true;
        }
    }
}

