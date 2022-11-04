using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHelper_1.CustomException
{
    public class NullAuthCookieException : System.Exception
    {
        public NullAuthCookieException(string message) : base(message)
        {

        }
    }
}