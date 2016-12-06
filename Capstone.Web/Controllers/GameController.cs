﻿using Capstone.Web.Dal_s;
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
            int handID = dal.GetHandID(tableID);

            List<Seat> seats = dal.GetAllPlayersAtTable(tableID);

            for (int i = 0; i < seats.Count; i++)
            {
                //Seat s = new Seat();
                //s.Username = players[i].Username;
                //s.TableBalance = players[i].CurrentMoney;

                if (seats[i].Username != "Available")
                {
                    seats[i].Hand = new Hand();

                    seats[i].Hand.MyHand = dal.GetAllCardsForPlayer(seats[i].Username, handID);
                }
                output.Seats.Add(seats[i]);
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

        public int CreateDeck(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();

            int handID = dal.CreateHand(tableID);

            DeckOfCards deck = new DeckOfCards();
            deck.Shuffle();

            dal.StoreCards(deck.cardList, handID);

            return handID;
        }

        public ActionResult CreateHands(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();
            Table model = GetTableInfo(tableID);
            int handID = CreateDeck(tableID);

            foreach (Seat s in model.Seats)
            {
                if (s.Username != "Available")
                {
                    dal.DrawCards(handID, 5, s.Username);
                }
            }

            return RedirectToAction("HandSetup", new { tableID = tableID });
        }

        public ActionResult HandSetup(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();
            //model = HttpContext.Cache["Table"] as Table;

            Table model = GetTableInfo(tableID);            

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
            return PartialView("HandSetup", model);
        }

        public ActionResult updatePlayerTurn(int tableID)
        {

            TableSqlDal dal = new TableSqlDal();
            Table model = GetTableInfo(tableID);

            string playerTurn = dal.GetActivePlayer(tableID);

            int seatChecking = 0;
            foreach (var seat in model.Seats)
            {
                seatChecking++;
                if (seat.Username.ToLower() == playerTurn.ToLower())
                {
                    break;
                }
                
            }

            bool updatedPlayer = false;
            foreach (var seat in model.Seats)
            {
                if (seat.Username.ToLower() == playerTurn.ToLower())
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
                if (seat.Username.ToLower() == playerTurn.ToLower())
                {
                    seat.IsTurn = false;
                }
            }

            string newPlayerTurn = dal.GetActivePlayer(model.TableID);
            foreach (var seat in model.Seats)
            {
                if (seat.Username.ToLower() == newPlayerTurn.ToLower())
                {
                    seat.IsTurn = true;
                }
            }

            return RedirectToAction("HandSetup", new { tableID = tableID });   
        }

        public ActionResult ReplaceCards (ReplaceCardModel model)
        {
            //TableSqlDal dal = new TableSqlDal();
            //model = dal.FindTable(1);
            //List<UserModel> players = dal.GetAllPlayersAtTable(1);
            //DeckOfCards deck = new DeckOfCards();
            //deck.Shuffle();

            TableSqlDal dal = new TableSqlDal();
            int handID = dal.GetHandID(model.TableId);

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

            return RedirectToAction("HandSetup", new { tableID = model.TableId });
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

        public ActionResult playerCalled (int tableID, int myBet, int betToCall)
        {
            TableSqlDal tdal = new TableSqlDal();

            int additionalMoney = betToCall - myBet;

            int handID = tdal.GetHandID(tableID);

            string userName = (string)Session["username"];

            tdal.LowerTableBalanceRaiseBet(tableID, handID, userName, additionalMoney);

            tdal.SetPlayerToHasChecked(tableID, handID, userName);

            return RedirectToAction("HandSetup",tableID);
        }

        public ActionResult playerBet (int tableID, int myBet, int betToCall, int newBet)
        {
            TableSqlDal tdal = new TableSqlDal();

            int additionalMoney = betToCall - myBet + newBet;

            int handID = tdal.GetHandID(tableID);

            string userName = (string)Session["username"];

            tdal.LowerTableBalanceRaiseBet(tableID, handID, userName, additionalMoney);

            tdal.SetPlayerCheckedAndAllOthersNot(tableID, handID, userName);

            tdal.UpdateCurrentMinBet(tableID, newBet);

            return RedirectToAction("HandSetup", tableID);
        }

        public ActionResult playerFolded (int tableID)
        {
            TableSqlDal tdal = new TableSqlDal();

            int handID = tdal.GetHandID(tableID);

            string userName = (string)Session["username"];

            tdal.SetPlayerAsFolded(tableID, handID, userName);

            //maybe set active to false

            return RedirectToAction("HandSetup", tableID);

        }
    }
}