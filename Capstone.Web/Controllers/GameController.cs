using Capstone.Web.Dal_s;
using Capstone.Web.Models;
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
        public ActionResult Rules()
        {
            return View("Rules");
        }

        public ActionResult JoinedTable(Table model)
        {           
            TableSqlDal dal = new TableSqlDal();
            model = dal.FindTable(1);

            List<UserModel> players = dal.GetAllPlayersAtTable(1);

            foreach (UserModel player in players)
            {
                Seat s = new Seat();
                s.Username = player.Username;
                s.TableBalance = player.CurrentMoney;

                model.Seats.Add(s);
            }

            for (int i = model.Seats.Count; i < 5; i++)
            {
                Seat s = new Seat();
                s.Username = "Available";
                model.Seats.Add(s);
            }
            return View("JoinedTable", model);
        }

        public ActionResult ConfirmAnte(Table model)
        {
            foreach (var seat in model.Seats)
            {
                if (seat.Active)
                {
                    //check table balance against ante
                    seat.TableBalance -= model.Ante;
                    model.Pot += model.Ante;
                }
            }
            return View("ConfirmAnte", model);
        }

        public ActionResult HandSetup(Table model)
        {
            TableSqlDal dal = new TableSqlDal();
            model = dal.FindTable(1);

            List<UserModel> players = dal.GetAllPlayersAtTable(1);

            foreach (UserModel player in players)
            {
                Seat s = new Seat();
                s.Username = player.Username;
                s.TableBalance = player.CurrentMoney;

                s.Hand = new Hand();

                s.Hand.MyHand = dal.GetAllCardsForPlayer(player.Username);
                s.Hand.MyHand = DeckOfCards.GetSuitAndLetterValues(s.Hand.MyHand);

                model.Seats.Add(s);
            }

            for (int i = model.Seats.Count; i < 5; i++)
            {
                Seat s = new Seat();
                s.Username = "Available";
                model.Seats.Add(s);
            }
            return View("HandSetup", model);
        }

        public ActionResult firstBettingRound(Table model)
        {
            //betting stuff

            return View("firstBettingRound", model);
        }

        public ActionResult ReplaceCards(Table model)
        {
            foreach (var seat in model.Seats)
            {
                if (seat.Active)
                {
                    seat.Hand.Replace(seat.Discards, model.Deck);
                }   
            }
            return View("ReplaceCards", model);
        }

        public ActionResult secondRoundOdBetting(Table model)
        {
            //betting stuff

            return View("secondRoundOfBetting", model);
        }

        public ActionResult determineWinner(Table model)
        {
            //card comparison 

            return View("determineWinner", model);
        }
    }
}