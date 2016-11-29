using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone.Web.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Game()
        {
            return View("Rules");
        }
    }
}