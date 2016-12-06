using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class Seat
    {
        public int SeatNumber { get; set; }

        public Hand Hand;

        public bool Occupied { get; set; }

        //this stuff might be a player class
        public int TableBalance { get; set; }

        public int CurrentBet { get; set; }

        public string Username { get; set; }

        public bool Active { get; set; } 

        public List<Card> Discards { get; set; }

        public bool IsTurn { get; set; }

        public bool HasFolded { get; set; }

        public bool HasChecked { get; set; }

        public bool HasDiscarded { get; set; }
    }
}