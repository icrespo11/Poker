using Capstone.Web.Dal_s;
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
            TableSqlDal dal = new TableSqlDal();
            //see if player is in game
            if (/*player in game*/false)
            {
                return RedirectToAction("LeaveTable", "Table");
            }
            return View("LoggedInLanding");
        }
    }
}