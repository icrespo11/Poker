using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class Card
    {

        public int Number { get; set; }
        public string Suit { get; set; }

        public string ConvertedNumber { get; set; }
        public string SuitLetter { get; set; }

        public bool Discard { get; set; }
    }
}