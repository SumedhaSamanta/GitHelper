/* 
 Created By:        Sumedha Samanta
 Created Date:      01-11-2022
 Modified Date:     08-11-2022
 Purpose:           This class is used for getting logger
 Purpose Type:      This class returns the logger which is to be used for logging messages
 Referenced files:  NA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace GitHelper_1
{
    public class LogHelper
    {
        /*
            <summary>
                creates or retrives the logger with the name of the file from which it is called
            </summary>
            <param name="filename"> path of the file from which the method is called </param>
            <returns>returns the logger with the specified name </returns>
        */
        public static log4net.ILog GetLogger([CallerFilePath]string filename="")
        {
            return log4net.LogManager.GetLogger(filename);
        }
    }
}