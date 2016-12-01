using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class Hand
    {
        public List<Card> MyHand;

        //possibly require a table and seat in constructor
        //alternatively have an empty constructor and receive cards 
        // from a dealer
        //public HoldEmHand(DeckOfCards d)

        public Hand()
        {
            MyHand = new List<Card>();
        }

        public void Replace(List<Card> cardsToDiscard, DeckOfCards deck)
        {
            foreach(Card card in cardsToDiscard)
            {
                MyHand.Remove(card);
            }

            Draw(deck);
        }

        public void Draw(DeckOfCards deck)
        {
            int handSize = MyHand.Count;

            for (int i = MyHand.Count; i < 5; i++)
            {
                Card c = deck.DrawACard();
                MyHand.Add(c);
            }
        }

        public void EmptyHand()
        {
            MyHand.RemoveRange(0, 5);
        }
    }
}