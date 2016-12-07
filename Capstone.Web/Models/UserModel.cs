using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class UserModel
    {
        [Required (ErrorMessage = "Username field is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password field is required.")]
        [RegularExpression(@"(?=^.{8,}$)(?=.*\d)(?=.*[!@#$%^&*]+)(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Your password must contain an upper case letter, number, and symbol, and be at least 8 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password field is required.")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public string LoginPassword { get; set; }
        public int CurrentMoney { get; set; }  
        public int HighestMoney { get; set; }
        public bool IsOnline { get; set; }
        public string Privilege { get; set; }
        public bool IsTaken { get; set; }
        public bool LoginFail { get; set; }
        public string Salt { get; set; }
       
        
    }
}