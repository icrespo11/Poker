using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class DeckOfCards
    {
        //possibly make private later?
        public List<Card> cardList;

        public DeckOfCards()
        {
            //any constructor related stuff, possibly including random seed for ordering?
            cardList = new List<Card>();
            for (int i = 1; i < 14; i++)
            {
                Card c = new Card();
                c.Number = i;
                c.Suit = "Spades";
                cardList.Add(c);
                c = new Card();
                c.Number = i;
                c.Suit = "Hearts";
                cardList.Add(c);
                c = new Card();
                c.Number = i;
                c.Suit = "Clubs";
                cardList.Add(c);
                c = new Card();
                c.Number = i;
                c.Suit = "Diamonds";
                cardList.Add(c);
            }
            //call shuffle here?
        }

        public void Shuffle()
        {
            Random rng = new Random();
            for (int i = cardList.Count; i > 1; i--)
            {
                int position = rng.Next(i);
                Card currentCard = cardList[i - 1];
                cardList[i - 1] = cardList[position];
                cardList[position] = currentCard;
            }
        }

        public Card DrawACard()
        {
            //potentially add an if statement to check if the deck is empty later

            Card output = cardList[cardList.Count -1];

            cardList.Remove(output);
            return output;
        }

    }
}