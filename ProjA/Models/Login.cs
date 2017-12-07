using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjA.Models
{
    public class Login
    {

        [Required]
        [Display(Name = "Username")]
        public string userName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string password { get; set; }


    }
}