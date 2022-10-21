using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHelper_1.Utilities
{
    public class CookieManager
    {
        public void CreateCookie(string username, string token, HttpResponse response)
        {
            //create cookie
            HttpCookie userInfo = new HttpCookie("userInfo");
            userInfo["UserName"] = username;
            userInfo["UserColor"] = token;
            //userInfo.Expires.Add(new TimeSpan(0, 1, 0));
            response.Cookies.Add(userInfo);
        }

        public String GetUserName()
        {
            //return username stored in cookie
            return "";
        }

        public String GetToken()
        {
            //return token stored in cookie
            return "";
        }

        public bool GetCookieStatus()
        {
            //true if cookie exists
            return false;
        }

        public void DeleteCookie()
        {
            //delete the cookie
        }

    }
}
