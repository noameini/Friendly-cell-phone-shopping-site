using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjA.Models
{
    public class Register
    {
        [Required]
        [Display(Name = "Username")]
        public string userName { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string lastName { get; set; }

        [Required]
        [Display(Name = "Mail")]
        public string mail { get; set; }

        [Required]
        [Display(Name = "Area")]
        public string area { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        public int phoneNumber { get; set; }

        [Required]
        public string isAdmin { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string password { get; set; }

        [Required]
        [Display(Name = "Confirm password")]
        [Compare("password",ErrorMessage ="Wrong password. Please try again.")]
        public string Confirmpassword { get; set; }


    }
}