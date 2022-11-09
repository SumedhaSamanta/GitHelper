/* 
 Created By:        Shubham Jaiswal
 Created Date:      03-11-2022
 Modified Date:     08-11-2022
 Purpose:           This custom exception class is used when authentication cookie is null
 Purpose Type:      Throw an exception of this class whenever authentication cookie is null that is user is not authorized. 
 Referenced files:  NA 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHelperAPI.CustomException
{
    public class NullAuthCookieException : System.Exception
    {
        /*
            <summary>
                initializes a new instance of NullAuthCookieException class with specified message
            </summary>
            <param name="message"> specified error message </param>
            <returns> NA </returns>
        */
        public NullAuthCookieException(string message) : base(message)
        {

        }
    }
}