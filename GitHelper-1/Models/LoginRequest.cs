using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GitHelper_1.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username Is Mandatory")]
        public string username { get; set; }

        [Required(ErrorMessage = "Personal Access Token Is Mandatory")]
        public string token { get; set; }
    }
}