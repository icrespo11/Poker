using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone.Web.Controllers
{
    public class FunkyController : Controller
    {
        // GET: Funky
        public ActionResult funky()
        {
            return View("funky");
        }
    }
}