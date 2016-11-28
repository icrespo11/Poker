using Capstone.Web.Dal_s;
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
        private readonly IUserSqlDal dal;

        public UserController(IUserSqlDal dal)
        {
            this.dal = dal;
        }

        public ActionResult Register()
        {
            return View("Register", new UserModel());

        }

        [HttpPost]
        public ActionResult Register(UserModel user)
        {
            UserSqlDal dal = new UserSqlDal();
            if (ModelState.IsValid)
            {
                var newUser = new UserModel
                {
                    Username = user.Username,
                    Password = user.Password,
                };

                List<string> existingUsers = dal.GetAllUsernames();
                foreach (string name in existingUsers)
                {
                    if (name == user.Username)
                    {
                        user.IsTaken = true;
                        return View("Register", user);
                    }
                }

                dal.Register(user);
                Session["user"] = user;
                user.IsTaken = false;

                return View("Success");

            }
            else
            {
                return View("Register", user);
            }
        }


        public ActionResult Login()
        {
            return View("Login", new UserModel());
        }

        [HttpPost]
        public ActionResult Login(UserModel user)
        {
            UserSqlDal dal = new UserSqlDal();

            user = dal.Login(user.Username, user.Password);

            if (user == null)
            {
                user = new UserModel();
                user.LoginFail = true;
                return View("Login", user);
            }
            user.LoginFail = false;
            Session["user"] = user;
            return View("Success");
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}