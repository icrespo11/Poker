using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;


namespace Capstone.Web.Controllers
{
    public class TestHandController : Controller
    {
        // GET: TestHand
        public ActionResult GameStart()
        {

            DeckOfCards d = new DeckOfCards();
            d.Shuffle();
            //save a copy of the deck for archive purposes before we mess with it?

            Seat s1 = new Seat();
            s1.UserName = "JoshTucholski";
            s1.Occupied = true;
            s1.Hand = new HoldEmHand();
            //s1.Hand.MyHand = new List<Card>();
            Seat s2 = new Seat();
            s2.UserName = "TheCow";
            s2.Occupied = true;
            s2.Hand = new HoldEmHand();
            Seat s3 = new Seat();
            s3.UserName = "GGNUB85";
            s3.Occupied = true;
            s3.Hand = new HoldEmHand();
            List<Seat> s = new List<Seat>();
            s.Add(s1);
            s.Add(s2);
            s.Add(s3);

            Table t = new Table();
            t.Deck = d;
            t.Seats = s;
            t.StateCounter = 0;
            t.CommunityCards = new List<Card>();
            HttpContext.Cache.Insert("Table", t);

            return View("GameStart", t);
        }
        [HttpPost]
        public ActionResult GameStart (int id)
        {
            Table t = HttpContext.Cache["Table"] as Table;
            t.StateCounter = t.StateCounter + 1;
            if (t.StateCounter == 1)
            {
                foreach(var seat in t.Seats)
                {
                    Card c = t.Deck.DrawACard();
                    seat.Hand.MyHand.Add(c);
                }
                foreach (var seat in t.Seats)
                {
                    Card c = t.Deck.DrawACard();
                    seat.Hand.MyHand.Add(c);
                }
            }
            else if (t.StateCounter == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    Card c = t.Deck.DrawACard();
                    t.CommunityCards.Add(c);
                }
                

            }
            else if (t.StateCounter == 3)
            {
                Card c = t.Deck.DrawACard();
                t.CommunityCards.Add(c);
            }

            else if (t.StateCounter == 4)
            {
                Card c = t.Deck.DrawACard();
                t.CommunityCards.Add(c);
            }
           

            HttpContext.Cache.Insert("Table", t);

            return View("GameStart", t);
        }
    }
}