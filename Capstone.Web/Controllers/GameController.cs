using Capstone.Web.Dal_s;
using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone.Web.Controllers
{
    public class GameController : Controller
    {
        public ActionResult AdvanceGame(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();
            Table currentTable = new Table();
            currentTable = GetTableInfo(tableID);

            if (currentTable.StateCounter == 0)
            {
                currentTable.StateCounter = 1;
                dal.UpdateStateCounter(currentTable.TableID);
            }

            Dictionary<int, string> gameStates = new Dictionary<int, string>()           
            {
                //{1, () => {JoinedTable(currentTable); } },
                {1, "JoinedTable" },
                //{2, () => {ConfirmAnte(currentTable); } },
                //{3, () => {HandSetup(currentTable); } },
                //{4, () => {firstBettingRound(currentTable); } },
                //{5, () => {ReplaceCards(null); } },
                //{6, () => {secondBettingRound(currentTable); } },
                //{7, () => {determineWinner(currentTable); } },
            };

            //gameStates[currentTable.StateCounter].Invoke();

            return RedirectToAction(gameStates[currentTable.StateCounter], currentTable);
        }

        public Table GetTableInfo(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();

            Table output = dal.FindTable(tableID);
            int handID = 1; // dal.GetHandID(tableID);

            List<UserModel> players = dal.GetAllPlayersAtTable(tableID);

            for (int i = 0; i < players.Count; i++)
            {
                Seat s = new Seat();
                s.Username = players[i].Username;
                s.TableBalance = players[i].CurrentMoney;

                if (s.Username != "Available")
                {
                    s.Hand = new Hand();

                    s.Hand.MyHand = dal.GetAllCardsForPlayer(s.Username, handID);
                }
                output.Seats.Add(s);
            }

            for (int i = output.Seats.Count; i < 5; i++)
            {
                Seat s = new Seat();
                s.Username = "Available";
                output.Seats.Add(s);
            }
            return output;
        }

        // GET: Game
        public ActionResult Rules()
        {
            return View("Rules");
        }

        public ActionResult JoinedTable(int id)
        {           
            TableSqlDal dal = new TableSqlDal();
            Table model = GetTableInfo(id);

            model.Seats[0].IsTurn = true;
            dal.SetActivePlayer(model.Seats[0].Username);
            //dal.UpdateStateCounter(model.TableID);
            return View("JoinedTable", model);
        }

        public ActionResult ConfirmAnte(Table model)
        {
            foreach (var seat in model.Seats)
            {
                if (seat.Active)
                {
                    //check table balance against ante
                    seat.TableBalance -= model.Ante;
                    model.Pot += model.Ante;
                }
            }
            return View("ConfirmAnte", model);
        }

        public void CreateDeck(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();

            int handID = 1; //dal.CreateHand(tableID);

            DeckOfCards deck = new DeckOfCards();
            deck.Shuffle();

            dal.StoreCards(deck.cardList, handID);
        }

        public ActionResult HandSetup(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();
            //model = HttpContext.Cache["Table"] as Table;

            Table model = GetTableInfo(tableID);
            CreateDeck(tableID);
            List<UserModel> players = dal.GetAllPlayersAtTable(tableID);

            string playerTurn = dal.GetActivePlayer(model.TableID);
            foreach (var seat in model.Seats)
            {
                if (seat.Username == playerTurn)
                {
                    seat.IsTurn = true;
                }
                //not longtime solution
                if (seat.Username != "Available")
                {
                    seat.Active = true;
                    seat.Occupied = true;
                }
            }
            return View("HandSetup", model);
        }

        public ActionResult HandSetupDupe(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();
            //model = HttpContext.Cache["Table"] as Table;

            Table model = GetTableInfo(tableID);
            List<UserModel> players = dal.GetAllPlayersAtTable(tableID);

            string playerTurn = dal.GetActivePlayer(model.TableID);
            foreach (var seat in model.Seats)
            {
                if (seat.Username == playerTurn)
                {
                    seat.IsTurn = true;
                }
                //not longtime solution
                if (seat.Username != "Available")
                {
                    seat.Active = true;
                    seat.Occupied = true;
                }
            }
            return View("HandSetup", model);
        }

        public ActionResult updatePlayerTurn(Table model)
        {
            TableSqlDal dal = new TableSqlDal();
            model = HttpContext.Cache["Table"] as Table;
            //List<UserModel> players = dal.GetAllPlayersAtTable(1);

            //foreach (UserModel player in players)
            //{
            //    Seat s = new Seat();
            //    s.Username = player.Username;
            //    s.TableBalance = player.CurrentMoney;

            //    s.Hand = new Hand();

            //    s.Hand.MyHand = dal.GetAllCardsForPlayer(player.Username);
            //    s.Hand.MyHand = DeckOfCards.GetSuitAndLetterValues(s.Hand.MyHand);

            //    model.Seats.Add(s);
            //}
            string playerTurn = dal.GetActivePlayer(model.TableID);
            //foreach (var seat in model.Seats)
            //{
            //    if (seat.Username == playerTurn)
            //    {
            //        seat.IsTurn = true;
            //    }
            //    //not longtime solution
            //    if (seat.Username != "Available")
            //    {
            //        seat.Active = true;
            //        seat.Occupied = true;
            //    }
            //}
            
            int seatChecking = 0;
            foreach (var seat in model.Seats)
            {
                seatChecking++;
                if(seat.Username == playerTurn)
                {
                    break;
                }
            }

            bool updatedPlayer = false;
            foreach (var seat in model.Seats)
            {
                if (seat.Username == playerTurn)
                {
                    
                    for (int i = seatChecking; i < model.Seats.Count; i++)
                    {
                        if(model.Seats[i].Occupied && model.Seats[i].Active)
                        {
                            dal.UpdateActivePlayer(model.TableID, model.Seats[i].Username);
                            
                            updatedPlayer = true;
                            break;
                        }
                    }
                    if (!updatedPlayer)
                    {
                        for (int i = 0; i < seatChecking; i++)
                        {
                            if (model.Seats[i].Occupied && model.Seats[i].Active)
                            {
                                dal.UpdateActivePlayer(model.TableID, model.Seats[i].Username);
                                //seat.IsTurn = false;
                                break;
                            }
                        }
                    }
                }               
                //seatChecking++;
            }
            foreach (var seat in model.Seats)
            {
                if (seat.Username == playerTurn)
                {
                    seat.IsTurn = false;
                }
            }

            string newPlayerTurn = dal.GetActivePlayer(model.TableID);
            foreach (var seat in model.Seats)
            {
                if (seat.Username == newPlayerTurn)
                {
                    seat.IsTurn = true;
                }
            }
            HttpContext.Cache.Insert("Table", model);

            //this is where bob starts breaking everything in hoping of fixing it.
            /*
            int userIndex = 0;
            for (int i = 0; i < model.Seats.Count; i++)
            {
                if (model.Seats[i].Username == (string)Session["username"])
                {
                    userIndex = i;
                }
            }
            TableAndCardDictionary combo = new TableAndCardDictionary();
            combo.Table = model;
            for (int j = 0; j < 5; j++)
            {
                combo.CardList.Add(model.Seats[userIndex].Hand.MyHand[j], false);
            }
            */
            //be afraid
            //be very, very afraid.

            return RedirectToAction("HandSetup", model);   
        }

        public ActionResult ReplaceCards (ReplaceCardModel model)
        {
            //TableSqlDal dal = new TableSqlDal();
            //model = dal.FindTable(1);
            //List<UserModel> players = dal.GetAllPlayersAtTable(1);
            //DeckOfCards deck = new DeckOfCards();
            //deck.Shuffle();

            TableSqlDal dal = new TableSqlDal();
            int handID = 1; //  dal.GetHandID(model.TableId);

            dal.DiscardCards(model);
            dal.DrawCards(handID, model.Discards.Count, model.Username);


            //this is completely obliterting anything/everything we would be passing in.
            //we need to get SOMETHING set and copied out before we do this.

            //Table t = HttpContext.Cache["Table"] as Table;

            //foreach (Seat s in model.Seats)
            //{
                //Seat s = new Seat();
                //s.Username = player.Username;
                //s.TableBalance = player.CurrentMoney;

                //s.Hand = new Hand();

                //s.Hand.MyHand = dal.GetAllCardsForPlayer(player.Username);
                //s.Hand.MyHand = DeckOfCards.GetSuitAndLetterValues(s.Hand.MyHand);

                //s.Hand.MyHand[0].Discard = "true";
                //s.Hand.MyHand[1].Discard = "true";

                //List<Card> toDiscard = new List<Card>();

                //foreach(var card in s.Hand.MyHand)
                //{
                //    if(bool.Parse(card.Discard) == true)
                //    {
                //        toDiscard.Add(card);
                //    }
                //}
                //s.Hand.Replace(toDiscard, model.Deck);
                //model.Seats.Add(s);
            //}

            return RedirectToAction("HandSetupDupe", new { tableID = model.TableId });
        }

        public ActionResult FinalHand(Table model)
        {
            TableSqlDal dal = new TableSqlDal();

            for (int i = model.Seats.Count; i < 5; i++)
            {
                Seat s = new Seat();
                s.Username = "Available";
                model.Seats.Add(s);
            }

            string playerTurn = dal.GetActivePlayer(model.TableID);
            foreach (var seat in model.Seats)
            {
                if (seat.Username == playerTurn)
                {
                    seat.IsTurn = true;
                }
                //not longtime solution
                if (seat.Username != "Available")
                {
                    seat.Active = true;
                    seat.Occupied = true;
                }
            }
            return View("FinalHand", model);
        }

        public ActionResult firstBettingRound(Table model)
        {
            //betting stuff

            return View("firstBettingRound", model);
        }

        public ActionResult secondBettingRound(Table model)
        {
            //betting stuff

            return View("secondRoundOfBetting", model);
        }

        public ActionResult determineWinner(Table model)
        {
            //card comparison 

            return View("determineWinner", model);
        }
    }
}