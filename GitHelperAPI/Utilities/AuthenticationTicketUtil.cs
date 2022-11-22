/* 
 Created By:        Mehdi Hossain
 Created Date:      25-10-2022
 Modified Date:     25-10-2022
 Purpose:           Util to encrypt credentials to authentication ticket and decrypt ticket to get github token, username
 Purpose Type:      Util to encrypt credentials to authentication ticket and decrypt ticket to get github token, username
 Referenced files:  Models/AuthenticationData.cs
 */

using GitHelperAPI.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Web.Security;

namespace GitHelperAPI.Utilities
{
    public static class AuthenticationTicketUtil
    {
        /*
            <summary>
                create encrypted authentication ticket for corrosponding username and token.
            </summary>
            <param name="authData"> contains username, token and userId </param>
            <returns>encrypted authentication token</returns>
        */
        public static string createAuthenticationTicket(AuthenticationData authData)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                            authData.userName,
                            DateTime.Now,
                            DateTime.Now.AddMinutes(30),
                            false,
                            JsonConvert.SerializeObject(authData),
                            FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            return encTicket;
        }

        /*
            <summary>
                decrypts username and access token from encrypted authentication ticket
            </summary>
            <param name="value"> authentication ticket </param>
            <returns>username, token encapsulated in AuthenticationData class</returns>
        */
        public static AuthenticationData getAuthenticationDataFromTicket(string value)
        {
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(value);
            AuthenticationData authData = JsonConvert.DeserializeObject<AuthenticationData>(ticket.UserData);
            return authData;
        }
    }
}