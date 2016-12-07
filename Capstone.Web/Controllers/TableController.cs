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
        public ActionResult TableSearch(List<Table> tables)
        {
            TableSqlDal dal = new TableSqlDal();
            tables = dal.GetAllTables();

            Dictionary<int, int> sittingPlayers = dal.GetNumberOfSittingPlayers();
            ViewBag.Dictionary = sittingPlayers;
            return View("TableSearch", tables);
        }


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

            UserSqlDal userDal = new UserSqlDal();
            UserModel currentUser = userDal.Login(model.TableHost);

            UserAndTable ut = new UserAndTable();
            ut.Table = output;
            ut.User = currentUser;

            /////////need to do a post redirect get here, possibly validate stuff on previosu screen
            return View("TakeSeat", ut);
        }

        public ActionResult SitAtTable(int TableID)
        {
            TableSqlDal dal = new TableSqlDal();
            UserSqlDal userDal = new UserSqlDal();

            string username = (string)Session["Username"];

            UserModel currentUser = userDal.Login(username);
            Table table = dal.FindTable(TableID);

            UserAndTable ut = new UserAndTable();
            ut.Table = table;
            ut.User = currentUser;

            return View("TakeSeat", ut);
        }

        [HttpPost]
        public ActionResult TakeSeat(UserAndTable model)
        {
            int tableID = model.Table.TableID;
            string userName = model.User.Username;
            int MoneyAdded = model.MoneyToTheTable;

            int MaxBuyIn = model.Table.MaxBuyIn;

            UserSqlDal uDal = new UserSqlDal();
            UserModel user = uDal.GetUserByUserName(userName);
            int UserMoney = user.CurrentMoney;

            TableSqlDal dal = new TableSqlDal();

            if (MoneyAdded <= UserMoney && MoneyAdded <= MaxBuyIn && MoneyAdded > 0)
            {
                int newMoneyValue = UserMoney - MoneyAdded;

                uDal.UpdateMoney(userName, newMoneyValue);
                
                bool isAdded = dal.AddPlayerToTable(tableID, userName, MoneyAdded);

                dal.InsertIntoHandSeat(tableID, dal.GetHandID(tableID), userName);
                return RedirectToAction("JoinedTable", "Game", new { id = tableID });
            }
            else
            {
                Table table = dal.FindTable(tableID);
                UserAndTable ut = new UserAndTable();
                ut.Table = table;
                ut.User = user;
                ut.WasFailure = true;

                return View("TakeSeat", ut);
            }

        }

        public ActionResult LeaveTable()
        {
            string userName = (string)Session["Username"];

            //need to get table_money from  table_players
            //need to add that table_money to users current_money
            //if users current_money < 100, set users current_money = 1000            

            //need to see if there is a hand_seat associated with the userName
            //if there is a hand_seat associated with the userName, 
            //need to set has_folded to true
            //possibly need to set current_bet to 0 and increment poker_table pot by that amount

            //TableSqlDal tDal = new TableSqlDal();

            //Table t = tDal.

            return RedirectToAction("LoggedInLanding", "Home");

            //return View("LoggedInLanding");
        }
    }
}