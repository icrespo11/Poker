using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone.Web.Controllers
{
    public class UserController: Controller
    {
        public ActionResult Register()
        {
            return View("Register");

        }

        [HttpPost]
        public ActionResult Register(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var newUser = new UserModel
                {
                    Username = user.Username,
                    Password = user.Password,
                };
                return View("Success");

            }
            else
            {
                return View("Fail");
            }
        }
    }
}