using GitHelper_1.Models;
using System;
using System.Web.Security;

namespace GitHelper_1.Utilities
{
    public static class AuthenticationTicketUtil
    {
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

        public static AuthenticationData getAuthenticationDataFromTicket(string value)
        {
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(value);
            return new AuthenticationData { userName = ticket.Name, userToken = ticket.UserData };
        }
    }
}