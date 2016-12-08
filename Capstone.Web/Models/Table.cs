using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class Table
    {
        public List<Seat> Seats = new List<Seat>();

        public int DealerPosition { get; set; }

        public int Pot { get; set; }

        public DeckOfCards Deck { get; set; }

        //public List<Card> CommunityCards;

        public int MaxBet { get; set; }

        public int MinBet { get; set; }

        public int currentMinBet { get; set; }

        //public int SmallBlind { get; set; }

        //public int BigBlind { get; set; }

        public int Ante { get; set; }

        public int MaxBuyIn { get; set; }

        public int StateCounter { get; set; }

        public int TableID { get; set; }

        public string TableHost { get; set; }

        public string Name { get; set; }

        public string Winner { get; set; }

        public bool IsTurn(string name)
        {
            foreach (Seat s in Seats)
            {
                if (s.Username.ToLower() == name && s.IsTurn)
                {
                    return true;
                }
            }
            return false;
        }

        public void changeDealer()
        {
            do
            {
                DealerPosition++;

                if (Seats.Count < DealerPosition)
                {
                    DealerPosition = 0;
                }
            }
            while (Seats[DealerPosition].Occupied == false);
        }
    }
}