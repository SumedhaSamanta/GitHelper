﻿/* 
 Created By:        Mehdi Hossain
 Created Date:      25-10-2022
 Modified Date:     25-10-2022
 Purpose:           Util to generate and decrypt ticket for login user. This ticket in later stored in cookie for further authorisation.
 Purpose Type:      Utility class for ticket generation and decryption.
 Referenced files:  Models\AuthenticationData.cs 
 */

using GitHelper_1.Models;
using System;
using System.Web.Security;

namespace GitHelper_1.Utilities
{
    public static class AuthenticationTicketUtil
    {
        /*
            <summary>
                create encrypted authentication ticket for corrosponding username and token.
            </summary>
            <param name="userName"> username for user authentication </param>
            <param name="userToken"> github personal access token for user authentication </param> 
            <returns>encrypted authentication token</returns>
        */
        public static string createAuthenticationTicket(string userName, string userToken)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                            userName,
                            DateTime.Now,
                            DateTime.Now.AddMinutes(30),
                            false,
                            userToken,
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
            return new AuthenticationData { userName = ticket.Name, userToken = ticket.UserData };
        }
    }
}