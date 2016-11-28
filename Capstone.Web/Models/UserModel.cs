using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class UserModel
    {
        [Required (ErrorMessage = "Required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"(?=^.{8,}$)(?=.*\d)(?=.*[!@#$%^&*]+)(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Your password must contain a symbol, an upper case letter and a number and be at least 8 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords Do Not Match!")]
        public string ConfirmPassword { get; set; }

        public int CurrentMoney { get; set; }
        public int HighestMoney { get; set; }
        public bool IsOnline { get; set; }
        public string Privilege { get; set; }
        public bool IsTaken { get; set; }
        public bool LoginFail { get; set; }
    }
}