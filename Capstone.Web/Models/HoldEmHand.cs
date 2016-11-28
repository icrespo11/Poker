using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class HoldEmHand
    {
        public List<Card> MyHand;

        //possibly require a table and seat in constructor
        //alternatively have an empty constructor and receive cards 
        // from a dealer
        //public HoldEmHand(DeckOfCards d)
        public HoldEmHand()
        {
            MyHand = new List<Card>();
            //Card c = d.DrawACard();
            //MyHand.Add(c);
            //Card b = d.DrawACard();
            //MyHand.Add(b);
        }

        public void Discard()
        {
            //better way to empty everything, in case you don't have a full hand?
            MyHand.RemoveRange(0, 2);
        }
    }
}