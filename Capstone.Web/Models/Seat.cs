using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class Seat
    {
        public int SeatNumber { get; set; }

        public HoldEmHand Hand;

        public bool Occupied { get; set; }


        //this stuff might be a player class
        public int TableBalance { get; set; }

        public string UserName { get; set; }
    }
}