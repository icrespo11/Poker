using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capstone.Web.Evaluators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Web.Models;

namespace Capstone.Web.Evaluators.Tests
{
    [TestClass()]
    public class PokerHandEvaluatorTests
    {
        [TestMethod()]
        public void EvaluateTest()
        {
            Dictionary<string, Hand> hands = new Dictionary<string, Hand>();

            Hand h1 = new Hand();
            h1.MyHand = new List<Card>()
            {
                new Card { Suit = "diamonds", Number = 5 },
                new Card { Suit = "diamonds", Number = 4 },
                new Card { Suit = "diamonds", Number = 2 },
                new Card { Suit = "diamonds", Number = 7 },
                new Card { Suit = "diamonds", Number = 3 },
            };
            hands.Add("isaac", h1);


            Hand h2 = new Hand();
            h2.MyHand = new List<Card>()
            {
                new Card { Suit = "spades", Number = 11 },
                new Card { Suit = "spades", Number = 4 },
                new Card { Suit = "spades", Number = 2 },
                new Card { Suit = "spades", Number = 7 },
                new Card { Suit = "spades", Number = 3 },
            };
            hands.Add("dan", h2);

            var result = FiveCardDrawEvaluator.Evaluate(hands);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("dan", result[0]);
            
        }
    }
}