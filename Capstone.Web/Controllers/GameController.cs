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
            model.Deck = new DeckOfCards();
            model.Deck.Shuffle();

            int dealAmount = 0;



            while (dealAmount < 5)
            {
                foreach (var seat in model.Seats)
                {
                    if (seat.Occupied == true && seat.Active == true)
                    {
                        Card c = model.Deck.DrawACard();
                           
                        seat.Hand.MyHand.Add(c);
                    }
                }
                dealAmount++;
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