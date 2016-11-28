using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class Table
    {
        public List<Seat> Seats;

        public int Pot { get; set; }

        //Or use Community card class?
        public List<Card> CommunityCards;

        public int MaxBet { get; set; }

        public int MinBet { get; set; }

        public int SmallBlind { get; set; }

        public int BigBlind { get; set; }

        public int Ante { get; set; }
    }
}