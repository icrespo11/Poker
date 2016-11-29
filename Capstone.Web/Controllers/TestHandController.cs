using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            Seat s2 = new Seat();
            s2.UserName = "TheCow";
            s1.Occupied = true;
            Seat s3 = new Seat();
            s3.UserName = "GGNUB85";
            s1.Occupied = true;
            List<Seat> s = new List<Seat>();
            s.Add(s1);
            s.Add(s2);
            s.Add(s3);

            Table t = new Table();
            t.Deck = d;
            t.Seats = s;

            return View("GameStart");
        }
    }
}