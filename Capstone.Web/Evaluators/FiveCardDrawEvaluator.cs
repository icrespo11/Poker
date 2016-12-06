using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Evaluators
{
    public class FiveCardDrawEvaluator
    {
        public static IList<string> Evaluate(IDictionary<string, Hand> hands)
        {
            var len = Enum.GetValues(typeof(HandType)).Length;
            var winners = new List<string>();
            HandType winningType = HandType.HighCard;

            foreach (var name in hands.Keys)
                
                for (var handType = HandType.RoyalFlush;
                 (int)handType < len; handType = handType + 1)
                {
                    var hand = hands[name];
                    if (hand.IsValid(handType))
                    {
                        int compareHands = 0, compareCards = 0;
                        if (winners.Count == 0
                         || (compareHands = winningType.CompareTo(handType)) > 0
                         || compareHands == 0
                          && (compareCards = hand.CompareTo(hands[winners[0]])) >= 0)
                        {
                            if (compareHands > 0 || compareCards > 0) winners.Clear();
                            winners.Add(name);
                            winningType = handType;
                        }
                        break;
                    }
                }
            return winners;
        }
    }
}