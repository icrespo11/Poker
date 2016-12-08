using Capstone.Web.Dal_s;
using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone.Web.Controllers
{
    public class HomeController : Controller
    {

        // GET: Home
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult LoggedInLanding()
        {
            string username = (string)Session["username"];
            UserSqlDal dal = new UserSqlDal();
            bool inGame = dal.CheckStatus(username);

            if (inGame)
            {
                return RedirectToAction("LeaveTable", "Table");
            }
            return View("LoggedInLanding");
        }
    }
}