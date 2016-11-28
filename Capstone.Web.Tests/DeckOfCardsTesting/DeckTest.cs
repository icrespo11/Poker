using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capstone.Web.Models;
using System.Collections.Generic;

namespace Capstone.Web.Tests.DeckOfCardsTesting
{
    [TestClass]
    public class DeckTest
    {
        [TestMethod]
        public void ConfirmCreation()
        {
            DeckOfCards d = new DeckOfCards();
            Card c = new Card();
            c.Number = 1;
            c.Suit = "Spades";

            Assert.AreEqual(52, d.cardList.Count);
            Assert.AreEqual(d.cardList[0].Suit, c.Suit);
            Assert.AreEqual(d.cardList[0].Number, c.Number);
            Assert.AreEqual(d.cardList[1].Number, 1);
            Assert.AreEqual(d.cardList[2].Number, 1);
            Assert.AreEqual(d.cardList[3].Number, 1);
            Assert.AreNotEqual(d.cardList[1].Suit, "Spades");
            Assert.AreNotEqual(d.cardList[2].Suit, "Spades");
            Assert.AreNotEqual(d.cardList[3].Suit, "Spades");
        }

        [TestMethod]
        public void TestingDrawingCards()
        {
            DeckOfCards d = new DeckOfCards();
            Card c = d.DrawACard();
            Card b = d.DrawACard();

            Assert.AreNotEqual(c.Suit, b.Suit);
            Assert.AreEqual(c.Number, b.Number);
            Assert.AreEqual(50, d.cardList.Count);

            Assert.AreEqual("Diamonds", c.Suit);
            Assert.AreEqual(13, c.Number);
        }

        [TestMethod]
        public void TestingTheRandomizer()
        {
            DeckOfCards d = new DeckOfCards();
            Card e = d.cardList[0];
            Card f = d.cardList[1];
            Card g = d.cardList[2];
            Card h = d.cardList[3];
            Card i = d.cardList[4];

            List<Card> listTest = new List<Card>() {
                d.cardList[0], d.cardList[1], d.cardList[2],
                d.cardList[3], d.cardList[4],
            };

            List<Card> first5Unshuffled = new List<Card>() { e, f, g, h, i };

            d.Shuffle();

            List<Card> first5Shuffled = new List<Card>() {
                d.cardList[0], d.cardList[1], d.cardList[2],
                d.cardList[3], d.cardList[4],
            };

            CollectionAssert.AreEqual(listTest, first5Unshuffled);

            //there is a 1 in 311 million for this to fail if randomizer works
            CollectionAssert.AreNotEqual(first5Unshuffled, first5Shuffled);

            //checking to make sure there are no duplicate cards
            for (int j = 0; j < d.cardList.Count; j++)
            {
                Card q = d.cardList[j];
                for (int k = j+1; k < d.cardList.Count; k++)
                {
                    Card r = d.cardList[k];
                    Assert.AreNotEqual(q, r);
                }
            }
        }
    }
}
