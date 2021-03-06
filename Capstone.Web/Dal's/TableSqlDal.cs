﻿using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Capstone.Web.Dal_s
{
    public class TableSqlDal
    {
        //static string connectionString = ;
        static string connectionString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        public Table FindTable(int tableID)
        {
            //bool foundTable = false;
            Table t = new Table();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * from poker_table WHERE table_id = @tableID;", conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        t.Ante = Convert.ToInt32(reader["ante"]);
                        t.MaxBet = Convert.ToInt32(reader["max_bet"]);
                        t.MinBet = Convert.ToInt32(reader["min_bet"]);
                        t.TableHost = Convert.ToString(reader["host"]);
                        t.TableID = Convert.ToInt32(reader["table_id"]);
                        t.Name = Convert.ToString(reader["name"]);
                        t.MaxBuyIn = Convert.ToInt32(reader["max_buy_in"]);
                        t.Pot = Convert.ToInt32(reader["pot"]);
                        t.DealerPosition = Convert.ToInt32(reader["dealer_position"]);
                        t.StateCounter = Convert.ToInt32(reader["state_counter"]);
                        t.Winner = Convert.ToString(reader["winner"]);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return t;
        }

        public void UpdateStateCounter(int tableId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Update poker_table Set state_counter= (state_counter +1) where table_id = @table_id;", conn);
                    cmd.Parameters.AddWithValue("@table_id", tableId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal void SetNotFolded(int tableID, string username)
        {    
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE hand_seat SET has_folded = 0 " +
                        "WHERE table_id = @tableID AND player = @userName AND hand_id = @handID;", conn);
                    cmd.Parameters.AddWithValue("@handID", GetHandID(tableID));
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@userName", username);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }



        public List<Seat> GetAllPlayersAtTable(int tableID)
        {
            List<Seat> output = new List<Seat>();
            int handID = GetHandID(tableID);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    
                    SqlCommand cmd = new SqlCommand("SELECT table_players.player, seat_number, table_balance, active, occupied, hand_id, current_bet, is_turn, discard_count, has_discarded, has_checked, has_folded " +                  
                        "FROM table_players " +
                        "LEFT JOIN hand_seat on (table_players.player = hand_seat.player AND table_players.table_id = hand_seat.table_id)  " +
                        "WHERE table_players.table_id = @tableID;", conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Seat s = new Seat();

                        //pulled from table_players
                        s.Active = Convert.ToBoolean(reader["active"]);                        
                        s.Occupied = Convert.ToBoolean(reader["occupied"]);
                        s.SeatNumber = Convert.ToInt32(reader["seat_number"]);
                        s.TableBalance = Convert.ToInt32(reader["table_balance"]);
                        s.Username = Convert.ToString(reader["player"]);
                        

                        s.IsTurn = (reader["is_turn"] != DBNull.Value)?Convert.ToBoolean(reader["is_turn"]):false;
                        s.CurrentBet = (reader["current_bet"] != DBNull.Value) ? Convert.ToInt32(reader["current_bet"]): 0; 
                        s.HasFolded = (reader["has_folded"] != DBNull.Value) ? Convert.ToBoolean(reader["has_folded"]): true; 
                        s.HasDiscarded = (reader["has_discarded"] != DBNull.Value) ? Convert.ToBoolean(reader["has_discarded"]): false; 
                        s.HasChecked = (reader["has_checked"] != DBNull.Value) ? Convert.ToBoolean(reader["has_checked"]): false; 

                        s.Hand = new Hand();
                        s.Hand.MyHand = GetCardsForPlayer(handID, s.Username);

                        output.Add(s);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return output;
        }

        public List<Card> GetCardsForPlayer(int handID, string username)
        {
            List<Card> output = new List<Card>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM hand_cards WHERE player = @username AND hand_id = @handID;", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@handID", handID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Card c = new Card();

                        c.Number = Convert.ToInt32(reader["card_number"]);
                        c.Suit = Convert.ToString(reader["card_suit"]);
                        c.Dealt = Convert.ToBoolean(reader["dealt"]);
                        c.Discard = Convert.ToBoolean(reader["discarded"]);

                        output.Add(c);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return DeckOfCards.GetSuitAndLetterValues(output);
        }

        public int CreateTable(Table table)
        {
            int output = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO poker_table (name, host, max_bet, min_bet, ante, max_buy_in, pot, dealer_position, current_min_bet, state_counter) VALUES " +
                        "(@name, @host, @maxBet, @minBet, @ante, @maxBuyIn, @pot, @dealerPosition, @current_min_bet, @state_counter);", conn);
                    cmd.Parameters.AddWithValue("@name", table.Name);
                    cmd.Parameters.AddWithValue("@host", table.TableHost);
                    cmd.Parameters.AddWithValue("@maxBet", table.MaxBet);
                    cmd.Parameters.AddWithValue("@minBet", table.MinBet);
                    cmd.Parameters.AddWithValue("@ante", table.Ante);
                    cmd.Parameters.AddWithValue("@maxBuyIn", table.MaxBuyIn);
                    cmd.Parameters.AddWithValue("@pot", table.Pot);
                    cmd.Parameters.AddWithValue("@dealerPosition", table.DealerPosition);
                    cmd.Parameters.AddWithValue("@current_min_bet", table.MinBet);
                    cmd.Parameters.AddWithValue("@state_counter", 0);

                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("SELECT table_id FROM poker_table WHERE name = @name ORDER BY table_id DESC;", conn);
                    cmd.Parameters.AddWithValue("@name", table.Name);

                    output = (int)cmd.ExecuteScalar();

                    cmd = new SqlCommand("INSERT INTO hand VALUES (@output)", conn);
                    cmd.Parameters.AddWithValue("@output", output);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return output;
        }

        public bool AddPlayerToTable(int tableID, string playerName, int tableBalance)
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict = GetNumberOfSittingPlayers();
            int rowsAffected = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO table_players (table_id, player, table_balance, occupied, seat_number, active) VALUES " +
                        "(@tableID, @playerName, @table_balance, 1, @seat_number, 1);"
                        , conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@playerName", playerName);
                    cmd.Parameters.AddWithValue("@table_balance", tableBalance);
                    cmd.Parameters.AddWithValue("seat_number",(dict.ContainsKey(tableID))? dict[tableID]: 0);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
            return (rowsAffected > 0);
        }

        public bool RemovePlayerFromTable(int tableID, string playerName)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("DELETE FROM table_players WHERE table_id = @tableID AND player = @playerName;", conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@playerName", playerName);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
            return (rowsAffected > 0);
        }

        public void NewHand(int tableID)
        {

            int handID = GetHandID(tableID);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE poker_table SET pot = 0 WHERE table_id = @tableID;", conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("DELETE FROM hand_cards WHERE hand_id = @handID", conn);
                    cmd.Parameters.AddWithValue("@handID", handID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {

                throw;
            }
        }

        internal void ResetStateCounter(int tableID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Update poker_table Set state_counter = 2 where table_id = @table_id;", conn);
                    cmd.Parameters.AddWithValue("@table_id", tableID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ResetMinBet(int tableID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE poker_table SET current_min_bet = min_bet;", conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        //not tested/used yet
        public List<Table> GetAllTables()
        {
            List<Table> output = new List<Table>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM poker_table INNER JOIN table_players on poker_table.table_id = table_players.table_id;", conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        Table t = new Table();
                        t.Ante = Convert.ToInt32(reader["ante"]);
                        t.MaxBet = Convert.ToInt32(reader["max_bet"]);
                        t.MinBet = Convert.ToInt32(reader["min_bet"]);
                        t.TableHost = Convert.ToString(reader["host"]);
                        t.TableID = Convert.ToInt32(reader["table_id"]);
                        t.Name = Convert.ToString(reader["name"]);
                        t.MaxBuyIn = Convert.ToInt32(reader["max_buy_in"]);

                        bool add = true;
                        foreach (var table in output)
                        {
                            if (t.TableID == table.TableID)
                            {
                                add = false;
                            }
                        }
                        if (add)
                        {
                            output.Add(t);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return output;
        }

        public void SetCurrentBet(int tableID, string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE hand_seat SET current_bet = 0 WHERE table_id = @tableID AND player = @username;", conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public void StateCounterSeven(int tableID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Update poker_table Set state_counter= 7 where table_id = @table_id;", conn);
                    cmd.Parameters.AddWithValue("@table_id", tableID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //not tested/used yet
        //probably needs to check hand_id as well
        public List<Card> GetAllCardsForPlayer(string username, int handID)
        {
            List<Card> output = new List<Card>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT card_number, card_suit FROM hand_cards WHERE player = @player AND hand_id = @handID;", conn);
                    cmd.Parameters.AddWithValue("@player", username);
                    cmd.Parameters.AddWithValue("@handID", handID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Card c = new Card();

                        c.Number = Convert.ToInt32(reader["card_number"]);
                        c.Suit = Convert.ToString(reader["card_suit"]);

                        output.Add(c);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return DeckOfCards.GetSuitAndLetterValues(output);
        }

        //not tested
        //currently only relevant the first time when creating the table, when all seats are inactive
        //may be used when starting a new hand
        public void SetActivePlayer(string playerID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE hand_seat SET is_turn = 1 where player = @player;", conn);
                    cmd.Parameters.AddWithValue("@player", playerID);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        //not tested
        public string GetActivePlayer(int tableId)
        {
            string output = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Select player from hand_seat where is_turn = 1 and table_id = @table_id;", conn);
                    cmd.Parameters.AddWithValue("@table_id", tableId);
                    output = cmd.ExecuteScalar().ToString();
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return output;
        }

        //not yet tested
        //cannot accomodate one player two tables...possibly changed
        public void UpdateActivePlayer(int tableID, string playerID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE hand_seat SET is_turn = 0 where table_id = @table_id and is_turn =1;", conn);
                    cmd.Parameters.AddWithValue("@table_id", tableID);
                    cmd.ExecuteNonQuery();

                    SqlCommand cmd1 = new SqlCommand("UPDATE hand_seat SET is_turn = 1 where table_id = @table_id and player = @player;", conn);
                    cmd1.Parameters.AddWithValue("@table_id", tableID);
                    cmd1.Parameters.AddWithValue("@player", playerID);
                    cmd1.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public int CreateHand(int tableID)
        {
            int handID = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO hand (table_id) VALUES (@tableID);", conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT MAX(hand_id) from hand where table_id = @tableID;";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    handID = (int)cmd.ExecuteScalar();

                    cmd.CommandText = "UPDATE hand_seat SET hand_id = @handID WHERE table_id = @tableID AND hand_id = (SELECT MAX(hand_id) FROM hand_seat WHERE table_id = @tableID);";
                    cmd.Parameters.AddWithValue("@handID", handID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return GetHandID(tableID);
        }

        public int GetHandID(int tableID)
        {
            int handID = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT MAX(hand_id) from hand where table_id = @tableID;", conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    handID = (int)cmd.ExecuteScalar();
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return handID;
        }

        public void DiscardCards(ReplaceCardModel cards)
        {
            int handID = GetHandID(cards.TableId);

            if (cards.Discards == null)
            {
                cards.Discards = new List<Card>();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    foreach (Card card in cards.Discards)
                    {
                        SqlCommand cmd = new SqlCommand("DELETE FROM hand_cards WHERE hand_id = @handID AND player = @player AND card_suit = @suit AND card_number = @number;", conn);
                        cmd.Parameters.AddWithValue("@handID", handID);
                        cmd.Parameters.AddWithValue("@player", cards.Username);
                        cmd.Parameters.AddWithValue("@suit", card.Suit);
                        cmd.Parameters.AddWithValue("@number", card.Number);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public List<Card> DrawCards(int handID, int numberToDraw, string player)
        {
            List<Card> output = new List<Card>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {                                                                           
                    conn.Open();                                                            
                    SqlCommand cmd = new SqlCommand($"SELECT TOP {numberToDraw} * FROM hand_card_deck WHERE hand_id = @handID AND dealt = 0 ORDER BY deck_position ASC;", conn);
                    cmd.Parameters.AddWithValue("@handID", handID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Card c = new Card();

                        c.Number = Convert.ToInt32(reader["card_number"]);
                        c.Suit = Convert.ToString(reader["card_suit"]);
                        output.Add(c);
                    }
                    reader.Close();

                    foreach (Card card in output)
                    {
                        cmd = new SqlCommand("INSERT INTO hand_cards VALUES (@handID, @player, @number, @suit, 1, 0);", conn);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@handID", handID);
                        cmd.Parameters.AddWithValue("@player", player);
                        cmd.Parameters.AddWithValue("@number", card.Number);
                        cmd.Parameters.AddWithValue("@suit", card.Suit);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "UPDATE hand_card_deck SET dealt = 1 WHERE hand_id = @handID AND card_number = @number AND card_suit = @suit";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@handID", handID);
                        cmd.Parameters.AddWithValue("@player", player);
                        cmd.Parameters.AddWithValue("@number", card.Number);
                        cmd.Parameters.AddWithValue("@suit", card.Suit);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            output = DeckOfCards.GetSuitAndLetterValues(output);
            return output;
        }

        public void StoreCards(List<Card> cards, int handID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO hand_card_deck VALUES (@hand_id, @card_number, @card_suit, 0, 0, @i);", conn);

                    for (int i = 0; i < 52; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@hand_id", handID);
                        cmd.Parameters.AddWithValue("@card_number", cards[i].Number);
                        cmd.Parameters.AddWithValue("@card_suit", cards[i].Suit);
                        cmd.Parameters.AddWithValue("@i", i);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public void LowerTableBalanceRaiseBet(int tableID, int handID, string userName, int amount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE table_players SET table_balance = table_balance - @amount " +
                        "WHERE table_id = @tableID AND player = @userName;", conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("UPDATE hand_seat SET current_bet = current_bet + @amount " +
                        "WHERE hand_id = @handID AND player = @userName;", conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@handID", handID);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE poker_table SET pot = pot + @amount WHERE table_id = @tableID";
                    //cmd.Parameters.AddWithValue("@tableID", tableID);
                    //cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public void PlayerAnte(int tableID, string username, int amount)
        {
            int handID = GetHandID(tableID);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE table_players SET table_balance = table_balance - @amount " +
                        "WHERE table_id = @tableID AND player = @userName;", conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@userName", username);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE poker_table SET pot = pot + @amount WHERE table_id = @tableID";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE hand_seat SET has_checked = 1 WHERE hand_id = @handID AND player = @username";
                    cmd.Parameters.AddWithValue("@handID", handID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public void GoToNextRound(int handID, int tableID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE hand_seat SET has_checked = 0, current_bet = 0 WHERE hand_id = @handID");
                    cmd.Parameters.AddWithValue("@handID", handID);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE poker_table SET current_min_bet = min_bet WHERE table_id = @tableID";
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public void SetPlayerToHasChecked(int tableID, int handID, string userName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE hand_seat SET has_checked = 1 " +
                        "WHERE table_id = @tableID AND player = @userName AND hand_id = @handID;", conn);
                    cmd.Parameters.AddWithValue("@handID", handID);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public void SetPlayerCheckedAndAllOthersNot(int tableID, int handID, string userName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE hand_seat SET has_checked = 0 " +
                        "WHERE table_id = @tableID AND hand_id = @handID;", conn);
                    cmd.Parameters.AddWithValue("@handID", handID);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.ExecuteNonQuery();


                    cmd = new SqlCommand("UPDATE hand_seat SET has_checked = 1 " +
                        "WHERE table_id = @tableID AND player = @userName AND hand_id = @handID;", conn);
                    cmd.Parameters.AddWithValue("@handID", handID);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public void SetPlayerAsFolded(int tableID, int handID, string userName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE hand_seat SET has_folded = 1 " +
                        "WHERE table_id = @tableID AND player = @userName AND hand_id = @handID;", conn);
                    cmd.Parameters.AddWithValue("@handID", handID);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("DELETE FROM hand_cards WHERE hand_id = @handID and player=@userName", conn);
                    cmd.Parameters.AddWithValue("@handID", tableID);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public void UpdateCurrentMinBet(int tableID, int newMinBet)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE poker_table SET current_min_bet = @newMinBet " +
                        "WHERE table_id = @tableID;", conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@newMinBet", newMinBet);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public Dictionary<int,int> GetNumberOfSittingPlayers()
        {
            Dictionary<int, int> output = new Dictionary<int, int>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT table_id, COUNT(player) as player_count from table_players group by table_id order by table_id;", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        output.Add(Convert.ToInt32(reader["table_id"]), Convert.ToInt32(reader["player_count"]));
                    }                  
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return output;
        }

        public void InsertIntoHandSeat(int tableID, int handID, string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT into hand_seat VALUES (@table_id, @hand_id, @player, 0, 0, 0, 0, 0, 1);", conn);

                    cmd.Parameters.AddWithValue("@table_id", tableID);
                    cmd.Parameters.AddWithValue("@hand_id", handID);
                    cmd.Parameters.AddWithValue("@player", username);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveWinner(int tableID, IList<string> winners)
        {
            string winner = winners[0];
            if (winners.Count > 1)
            {
                for (int i = 1; i < winners.Count; i++)
                {
                    winner = winner + " and " + winners[i];
                }
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE poker_table SET winner = @winner WHERE table_id = @table_id;", conn);

                    cmd.Parameters.AddWithValue("@table_id", tableID);
                    cmd.Parameters.AddWithValue("@winner", winner);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetWinner(int tableID)
        {
            string winner = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT winner from poker_table where table_id = @table_id;", conn);
                    cmd.Parameters.AddWithValue("@table_id", tableID);
                    winner = (string)cmd.ExecuteScalar();
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return winner;
        }

        public void SaveWiningMoney(int table_id, string username, int amount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE table_players SET table_balance = table_balance + @amount WHERE table_id = @table_id and player = @username;", conn);

                    cmd.Parameters.AddWithValue("@table_id", table_id);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@amount", amount);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UncheckAllPlayer(int tableID)
        {
            TableSqlDal tDal = new TableSqlDal();
            int handID = tDal.GetHandID(tableID);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE hand_seat SET has_checked = 0 WHERE table_id = @tableID AND hand_id = @handID;", conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@handID", handID);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void LeaveTable(string username, int tableID)
        {

            int handID = GetHandID(tableID);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE users SET current_money = current_money + (SELECT table_balance FROM table_players WHERE player = @player AND table_id = @table_id) WHERE username = @player", conn);
                    cmd.Parameters.AddWithValue("@table_id", tableID);
                    cmd.Parameters.AddWithValue("@player", username);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("DELETE FROM hand_seat WHERE table_id = @tableID AND player = @playerName", conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@playerName", username);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("DELETE FROM table_players WHERE table_id = @tableID AND player = @playerName;", conn);
                    cmd.Parameters.AddWithValue("@tableID", tableID);
                    cmd.Parameters.AddWithValue("@playerName", username);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}