using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class Hand : IComparable<Hand>
    {
        public List<Card> MyHand { get; set; }

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

        public int CompareTo(Hand other)
        {
            for (var i = 4; i >= 0; i--)
            {
                RankType rank1 = MyHand[i].Rank, rank2 = other.MyHand[i].Rank;
                if (rank1 > rank2) return 1;
                if (rank1 < rank2) return -1;
            }
            return 0;
        }

        private void Sort()
        {
            MyHand = MyHand.OrderBy(c => c.Rank)
             .OrderBy(c => MyHand.Where(c1 => c1.Rank == c.Rank).Count()).ToList();

            if (MyHand[4].Rank == RankType.Ace && MyHand[0].Rank == RankType.Two
             && (int)MyHand[3].Rank - (int)MyHand[0].Rank == 3)
                MyHand = new List<Card> { MyHand[4], MyHand[0], MyHand[1], MyHand[2], MyHand[3] };
        }

        public bool IsValid(HandType handType)
        {
            Sort();

            switch (handType)
            {
                case HandType.RoyalFlush:
                    return IsValid(HandType.StraightFlush) && MyHand[4].Rank == RankType.Ace;
                case HandType.StraightFlush:
                    return IsValid(HandType.Flush) && IsValid(HandType.Straight);
                case HandType.FourOfAKind:
                    return GetGroupByRankCount(4) == 1;
                case HandType.FullHouse:
                    return IsValid(HandType.ThreeOfAKind) && IsValid(HandType.OnePair);
                case HandType.Flush:
                    return GetGroupBySuitCount(5) == 1;
                case HandType.Straight:
                    return (int)MyHand[4].Rank - (int)MyHand[0].Rank == 4
                    || MyHand[0].Rank == RankType.Ace;
                case HandType.ThreeOfAKind:
                    return GetGroupByRankCount(3) == 1;
                case HandType.TwoPairs:
                    return GetGroupByRankCount(2) == 2;
                case HandType.OnePair:
                    return GetGroupByRankCount(2) == 1;
                case HandType.HighCard:
                    return GetGroupByRankCount(1) == 5;
            }
            return false;
        }

        private int GetGroupByRankCount(int n)
        { return MyHand.GroupBy(c => c.Rank).Count(g => g.Count() == n); }

        private int GetGroupBySuitCount(int n)
        { return MyHand.GroupBy(c => c.Suit).Count(g => g.Count() == n); }
    }

    public enum HandType : int
    {
        RoyalFlush, StraightFlush, FourOfAKind,
        FullHouse, Flush, Straight, ThreeOfAKind, TwoPairs, OnePair, HighCard
    }
}