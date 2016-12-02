using Capstone.Web.Dal_s;
using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone.Web.Controllers
{
    public class TableController : Controller
    {
        // GET: Table

            //D-Von... get the tables.
        //public ActionResult Index()
        //{


        //    return View();
        //}

        public ActionResult CreateTable()
        {
            return View("CreateTable");
        }

        [HttpPost]
        public ActionResult CreateTable(Table model)
        {
            //this is going to break if you ever get here while not logged in
            model.TableHost = (string)Session["UserName"];
            //model.DealerPosition = 0
            model.Pot = 0;

            TableSqlDal dal = new TableSqlDal();
            int newID = dal.CreateTable(model);


            //need to get table ID out of the table we just created 
            Table output = dal.FindTable(newID);


            /////////
            return View("JoinTable", output);
        }
    }
}