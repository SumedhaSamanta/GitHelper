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
                        new StatusDetailsModel { status = "Success", message = "Authentication Successful"});

                    string ticket = AuthenticationTicketUtil.createAuthenticationTicket(username, token);
                    var cookie = new CookieHeaderValue(FormsAuthentication.FormsCookieName, ticket);
                    cookie.Expires = DateTimeOffset.Now.AddDays(1);
                    cookie.Domain = Request.RequestUri.Host;
                    cookie.Path = "/";
                    responseMsg.Headers.AddCookies(new CookieHeaderValue[] { cookie });


                    return responseMsg;
                }
                else
                {
                    //return appropriate message for failure
                    
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new StatusDetailsModel { status = "Failure", message = "Bad Credentials" });

                }
            }
            //return appropriate message for failure
            return Request.CreateResponse(HttpStatusCode.OK,
                new StatusDetailsModel { status = "Failure", message = "Wrong format for username or personal access token" });
        }


        [HttpGet]
        [ActionName("Logout")]
        public HttpResponseMessage Logout()
        {
            //erase cookie data of current user
            FormsAuthentication.SignOut();
            //return successful logout confirmation
            return Request.CreateResponse(HttpStatusCode.OK,
                new StatusDetailsModel { status = "Success", message = "Logged Out Successfully" });
        }


        [HttpGet]
        [ActionName("IsAuthenticated")]
        public HttpResponseMessage IsAuthenticated()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                new StatusDetailsModel { status = "Authenticated", message = "User is Authenticated" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                new StatusDetailsModel { status = "Unauthenticated", message = "User is Not Authenticated" });
            }
            
        }

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

