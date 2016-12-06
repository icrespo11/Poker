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
        public bool Dealt { get; set; }


        public RankType Rank
        {
            get
            {
                if (Number == 1) { return RankType.Ace; }
                else
                {
                    return (RankType)Number;
                }
            }
        }

        public SuitType SuitEnum
        {
            get
            {
                switch(Suit.ToLower())
                {
                    case "diamonds": return SuitType.Diamonds;
                    case "clubs": return SuitType.Clubs;
                    case "hearts": return SuitType.Hearts;
                    case "spades": return SuitType.Spades;
                    default: return SuitType.Spades;
                        
                }
            }
        }

    }

    public enum RankType : int
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }
    public enum SuitType : int
    {
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }
}