using Capstone.Web.Dal_s;
using Capstone.Web.Evaluators;
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
        public Table GetTableInfo(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();
            Table output = dal.FindTable(tableID);

            int handID = dal.GetHandID(tableID);

            List<Seat> seats = dal.GetAllPlayersAtTable(tableID);

            for (int i = 0; i < seats.Count; i++)
            {

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

        public ActionResult RefreshTable(int tableID)
        {
            Table table = GetTableInfo(tableID);

            return PartialView("HandSetup", table);
        }

        public ActionResult AdvanceGame(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();
            Table currentTable = GetTableInfo(tableID);

            if (currentTable.StateCounter == 0 || currentTable.StateCounter == 1)
            {
                currentTable.StateCounter = 2;
                dal.ResetStateCounter(tableID);
            }


            Dictionary<int, string> gameStates = new Dictionary<int, string>()
            {
                {1, "JoinedTable" },
                {2, "BettingRound" },
                {3, "CreateHands" },
                {4, "BettingRound" },
                {5, "CardExchange" },
                {6, "BettingRound" },
                {7, "DetermineWinner" },
            };
            return RedirectToAction(gameStates[currentTable.StateCounter], new { tableID = currentTable.TableID });
        }

        // GET: Game
        public ActionResult JoinedTable(int id)
        {
            TableSqlDal dal = new TableSqlDal();
            Table model = GetTableInfo(id);

            model.Seats[0].IsTurn = true;

            if ((string)Session["username"] == model.TableHost)
            {
                dal.SetActivePlayer(model.Seats[0].Username);
            }
         
            dal.SetNotFolded(id, (string)Session["username"]);
            //dal.UpdateStateCounter(model.TableID);
            //dal.UncheckAllPlayer(model.TableID);
            return View("JoinedTable", model);
        }

        public ActionResult HandSetup(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();
            Table model = GetTableInfo(tableID);

            string playerTurn = dal.GetActivePlayer(model.TableID);
            foreach (var seat in model.Seats)
            {
                if (seat.Username == playerTurn)
                {
                    seat.IsTurn = true;
                }
                if (seat.Username != "Available")
                {
                    seat.Active = true;
                    seat.Occupied = true;
                }
            }
            return View("GameDisplay", model);
        }

        public ActionResult ConfirmAnte(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();
            string username = dal.GetActivePlayer(tableID);

            Table table = GetTableInfo(tableID);

            Seat seat = new Seat();

            foreach (Seat s in table.Seats)
            {
                if (s.Username == username)
                {
                    seat = s;
                }
            }
            seat.TableBalance -= table.Ante;
            table.Pot += table.Ante;
            seat.Active = true;

            dal.PlayerAnte(tableID, username, table.Ante);

            table = GetTableInfo(tableID);

            UpdatePlayerTurn(tableID);

            int i = 0;
            foreach (Seat s in table.Seats)
            {
                if (s.Username == "Available")
                {
                    i++;
                }
                else if (s.HasChecked || s.HasFolded)
                {
                    i++;
                }
            }

            if (i == 5)
            {
                table.StateCounter++;
                dal.UpdateStateCounter(tableID);
                dal.UncheckAllPlayer(tableID);
                return RedirectToAction("AdvanceGame", new { tableID = tableID });
            }
            

            return RedirectToAction("HandSetup", new { tableID = table.TableID });
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
            dal.UpdateStateCounter(tableID);
            dal.UncheckAllPlayer(tableID);
            return RedirectToAction("AdvanceGame", new { tableID = tableID });
        }

        public void UpdatePlayerTurn(int tableID)
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
                        if (model.Seats[i].Occupied && model.Seats[i].Active && !model.Seats[i].HasFolded)
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
                            if (model.Seats[i].Occupied && model.Seats[i].Active && !model.Seats[i].HasFolded)
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
            //return RedirectToAction("HandSetup", new { tableID = tableID });
        }

        public ActionResult CardExchange(int tableID)
        {
            return RedirectToAction("HandSetup", new { tableID = tableID });
        }

        public ActionResult ReplaceCards(ReplaceCardModel model)
        {
            TableSqlDal dal = new TableSqlDal();
            int handID = dal.GetHandID(model.TableId);
            Table table = GetTableInfo(model.TableId);
            dal.SetPlayerToHasChecked(model.TableId, handID, model.Username);
            dal.SetCurrentBet(model.TableId, model.Username);
            dal.DiscardCards(model);
            dal.DrawCards(handID, model.Discards.Count, model.Username);
            table = GetTableInfo(model.TableId);

            UpdatePlayerTurn(model.TableId);

            int i = 0;
            foreach (Seat s in table.Seats)
            {
                if (s.Username == "Available")
                {
                    i++;
                }
                else if (s.HasChecked || s.HasFolded)
                {
                    i++;
                }
            }

            if (i == 5)
            {
                table.StateCounter++;
                dal.UpdateStateCounter(model.TableId);
                dal.UncheckAllPlayer(model.TableId);
                return RedirectToAction("AdvanceGame", new { tableID = model.TableId });
            }
            

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

        public ActionResult NextHand(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();
            Table table = GetTableInfo(tableID);

            dal.SetPlayerToHasChecked(tableID, dal.GetHandID(tableID), (string)Session["username"]);

            table = GetTableInfo(tableID);

            UpdatePlayerTurn(tableID);

            int i = 0;
            foreach (Seat s in table.Seats)
            {
                if (s.Username == "Available")
                {
                    i++;
                }
                else if (s.HasChecked)
                {
                    i++;
                }
            }

            if (i == 5)
            {
                dal.ResetStateCounter(tableID);
                dal.UncheckAllPlayer(tableID);
                dal.NewHand(tableID);
                return RedirectToAction("AdvanceGame", new { tableID = tableID });
            }

            
            return RedirectToAction("HandSetup", new { tableID = tableID });
        }

        public ActionResult BettingRound(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();

            dal.ResetMinBet(tableID);

            return RedirectToAction("HandSetup", new { tableID = tableID });
        }

        public ActionResult DetermineWinner(int tableID)
        {
            TableSqlDal dal = new TableSqlDal();
            Table table = GetTableInfo(tableID);

            Dictionary<string, Hand> handsToCompare = new Dictionary<string, Hand>();

            IList<string> winnerByFold = new List<string>();
            foreach (Seat s in table.Seats)
            {
                if (!s.HasFolded)
                {
                    winnerByFold.Add(s.Username);
                }
            }

            if (winnerByFold.Count == 1)
            {
                dal.SaveWinner(tableID, winnerByFold);

                dal.SaveWiningMoney(tableID, winnerByFold[0], table.Pot);

            }
            else
            {
                foreach (var seat in table.Seats)
                {
                    if (!seat.HasFolded && seat.Username != "Available")
                    {
                        handsToCompare.Add(seat.Username, seat.Hand);
                    }
                }

                IList<string> winner = FiveCardDrawEvaluator.Evaluate(handsToCompare);

                dal.SaveWinner(tableID, winner);

                foreach (var person in winner)
                {
                    dal.SaveWiningMoney(tableID, person, (table.Pot / winner.Count));
                }
            }

            foreach (var seat in table.Seats)
            {
                if (seat.Username != "Available")
                {
                    dal.SetNotFolded(table.TableID, seat.Username);

                    dal.SetCurrentBet(table.TableID, seat.Username);
                }
            }

                return RedirectToAction("HandSetup", new { tableID = tableID });
        }

        public ActionResult PlayerCalled(int tableID, int myBet, int betToCall)
        {
            TableSqlDal tdal = new TableSqlDal();
            Table table = GetTableInfo(tableID);

            int additionalMoney = betToCall - myBet;
            int handID = tdal.GetHandID(tableID);
            string userName = (string)Session["username"];

            tdal.LowerTableBalanceRaiseBet(tableID, handID, userName, additionalMoney);
            tdal.SetPlayerToHasChecked(tableID, handID, userName);
            table = GetTableInfo(tableID);

            UpdatePlayerTurn(tableID);

            int i = 0;
            int j = 0;
            foreach (Seat s in table.Seats)
            {
                if (s.Username == "Available")
                {
                    i++;
                }
                else if (s.HasChecked || s.HasFolded)
                {
                    i++;
                }

                if (s.Username == "Available")
                {
                    j++;
                }
                else if (s.HasFolded)
                {
                    j++;
                }
            }

            if (j == 4)
            {
                tdal.StateCounterSeven(tableID);
                tdal.UncheckAllPlayer(tableID);
                return RedirectToAction("AdvanceGame", new { tableID = tableID });
            }

            if (i == 5)
            {
                table.StateCounter++;
                tdal.UpdateStateCounter(tableID);
                tdal.UncheckAllPlayer(tableID);
                return RedirectToAction("AdvanceGame", new { tableID = tableID });
            }

            

            return RedirectToAction("HandSetup", new { tableID = tableID });
        }

        public ActionResult PlayerBet(int tableID, int myBet, int betToCall, int newBet)
        {           
            TableSqlDal tdal = new TableSqlDal();
            Table t1 = tdal.FindTable(tableID);
            t1.Seats = tdal.GetAllPlayersAtTable(tableID);
            int currentTableBalance = 0;
            foreach(Seat s in t1.Seats)
            {
                if (s.Username == (string)Session["username"])
                {
                    currentTableBalance = s.TableBalance;
                }
            }

            if(newBet >= betToCall && newBet > 0 && newBet >= t1.MinBet && newBet <= t1.MaxBet && newBet <= currentTableBalance )
            {
                int additionalMoney = betToCall - myBet + newBet;
                int handID = tdal.GetHandID(tableID);
                string userName = (string)Session["username"];

                tdal.LowerTableBalanceRaiseBet(tableID, handID, userName, additionalMoney);
                tdal.SetPlayerCheckedAndAllOthersNot(tableID, handID, userName);
                tdal.UpdateCurrentMinBet(tableID, newBet);

                UpdatePlayerTurn(tableID);

                return RedirectToAction("HandSetup", new { tableID = tableID });
            }
            else
            {

                return RedirectToAction("HandSetup", new { tableID = tableID });
            }


        }

        public ActionResult PlayerFolded(int tableID)
        {
            TableSqlDal tdal = new TableSqlDal();
            Table table = GetTableInfo(tableID);

            int handID = tdal.GetHandID(tableID);
            string userName = (string)Session["username"];

            tdal.SetPlayerAsFolded(tableID, handID, userName);
            table = GetTableInfo(tableID);

            UpdatePlayerTurn(tableID);

            int i = 0;
            int j = 0;
            foreach (Seat s in table.Seats)
            {
                if (s.Username == "Available")
                {
                    i++;
                }
                else if (s.HasChecked || s.HasFolded)
                {
                    i++;
                }

                if (s.Username == "Available")
                {
                    j++;
                }
                 else if(s.HasFolded)
                {
                    j++;
                }
            }

            if (j == 4)
            {
                tdal.StateCounterSeven(tableID);
                tdal.UncheckAllPlayer(tableID);
                return RedirectToAction("AdvanceGame", new { tableID = tableID });
            }

            if (i == 5)
            {
                table.StateCounter++;
                tdal.UpdateStateCounter(tableID);
                tdal.UncheckAllPlayer(tableID);
                return RedirectToAction("AdvanceGame", new { tableID = tableID });
            }
            

            return RedirectToAction("HandSetup", new { tableID = tableID });
        }

        public ActionResult Rules()
        {
            return View("Rules");
        }
    }
}