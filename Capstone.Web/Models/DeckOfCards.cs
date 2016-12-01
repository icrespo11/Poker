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
                c.SuitLetter = "♠";
                cardList.Add(c);
                c = new Card();
                c.Number = i;
                c.Suit = "Hearts";
                c.SuitLetter = "♡";
                cardList.Add(c);
                c = new Card();
                c.Number = i;
                c.Suit = "Clubs";
                c.SuitLetter = "♣";
                cardList.Add(c);
                c = new Card();
                c.Number = i;
                c.Suit = "Diamonds";
                c.SuitLetter = "♢";
                cardList.Add(c);
            }

            cardList = GetSuitAndLetterValues(cardList);
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

        public static List<Card> GetSuitAndLetterValues(List<Card> cardList)
        {
            for (int j = 0; j < cardList.Count; j++)
            {
                if (cardList[j].Number == 1)
                {
                    cardList[j].ConvertedNumber = "A";
                }
                else if (cardList[j].Number == 11)
                {
                    cardList[j].ConvertedNumber = "J";
                }
                else if (cardList[j].Number == 12)
                {
                    cardList[j].ConvertedNumber = "Q";
                }
                else if (cardList[j].Number == 13)
                {
                    cardList[j].ConvertedNumber = "K";
                }
                else
                {
                    cardList[j].ConvertedNumber = cardList[j].Number.ToString();
                }

                if (cardList[j].Suit.ToLower() == "spades")
                {
                    cardList[j].SuitLetter = "♠";
                }
                else if (cardList[j].Suit.ToLower() == "hearts")
                {
                    cardList[j].SuitLetter = "♡";
                }
                else if (cardList[j].Suit.ToLower() == "diamonds")
                {
                    cardList[j].SuitLetter = "♢";
                }
                else if (cardList[j].Suit.ToLower() == "clubs")
                {
                    cardList[j].SuitLetter = "♣";
                }
            }

            return cardList;
        }
    }
}