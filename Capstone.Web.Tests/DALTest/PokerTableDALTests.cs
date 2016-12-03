using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using Capstone.Web.Dal_s;
using Capstone.Web.Models;

namespace Capstone.Web.Tests.DALTest
{
    [TestClass]
    public class PokerTableDALDTest
    {
        private TransactionScope tran;
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=poker;Persist Security Info=True;User ID=te_student;Password=techelevator";


        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();
            int rowsAffected;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                //this will need to be updated when we add tables that depend on users
                SqlCommand cmd = new SqlCommand("DELETE FROM table_players; DELETE FROM hand_actions; DELETE FROM hand_cards; " +
                    "DELETE FROM hand; DELETE FROM poker_table; DELETE FROM users;", conn);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("INSERT INTO users VALUES" +
                    "('Brian', 'pwd', 500, 2000, 'admin', 0, 'LOL__WUT'), " +
                    "('Bob', 'pwd2', 50000, 50020, 'admin', 1, '8675309z'), " +
                    "('Boo', 'Hoo', 1000, 1000, 'player', 0, 'omg-hash') "
                    , conn);

                rowsAffected = cmd.ExecuteNonQuery();

                cmd = new SqlCommand("INSERT INTO poker_table (host, name, min_bet, max_bet, ante, max_buy_in, pot, dealer_position) VALUES" +
                    "('Bob', 'Bob the tester. Can we break it? Yes, we can!', 10, 20, 10, 1000, 0, 0);"
                    , conn);

                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("SELECT table_id from poker_table WHERE host = 'Bob'", conn);
                int tableID = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("INSERT INTO table_players (table_id, player, isTurn) VALUES " +
                    $"({tableID}, 'Bob', 1), ({tableID}, 'Boo', 0);"
                    , conn);

                cmd.ExecuteNonQuery();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            tran.Dispose();
        }

        [TestMethod]
        public void FindTableByTableIDTesting()
        {

            Table t = new Table();
            TableSqlDal dal = new TableSqlDal();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    //get information (including table ID) from database for the table we created in the initialize

                    SqlCommand cmd = new SqlCommand("SELECT * from poker_table WHERE host = 'Bob'", conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        t.Ante = Convert.ToInt32(reader["ante"]);
                        t.MaxBet = Convert.ToInt32(reader["max_bet"]);
                        t.MinBet = Convert.ToInt32(reader["min_bet"]);
                        t.TableHost = Convert.ToString(reader["host"]);
                        t.TableID = Convert.ToInt32(reader["table_id"]);
                        t.Name = Convert.ToString(reader["name"]);
                    }
                }
            }
            catch
            {
                throw;
            }

            int tableID = t.TableID;

            Table TOut = dal.FindTable(tableID);

            Assert.AreEqual(TOut.TableHost, t.TableHost);
            Assert.AreEqual(TOut.TableID, t.TableID);
            Assert.AreEqual(TOut.Name, t.Name);
        }





        [TestMethod]
        public void TestGetAllPlayersAtTableFromAGivenTableNumber()
        {
            Table t = new Table();
            TableSqlDal dal = new TableSqlDal();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    //get information (including table ID) from database for the table we created in the initialize

                    SqlCommand cmd = new SqlCommand("SELECT * from poker_table WHERE host = 'Bob'", conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        t.Ante = Convert.ToInt32(reader["ante"]);
                        t.MaxBet = Convert.ToInt32(reader["max_bet"]);
                        t.MinBet = Convert.ToInt32(reader["min_bet"]);
                        t.TableHost = Convert.ToString(reader["host"]);
                        t.TableID = Convert.ToInt32(reader["table_id"]);
                        t.Name = Convert.ToString(reader["name"]);
                    }
                }
            }
            catch
            {
                throw;
            }

            int tableID = t.TableID;

            List<UserModel> output = dal.GetAllPlayersAtTable(tableID);


            Assert.AreEqual(2, output.Count);
            Assert.AreEqual(output[0].Username, "Bob");
            Assert.AreEqual(output[1].Username, "Boo");
            //CollectionAssert.Contains(output[0]., "Bob");

        }



        [TestMethod]
        public void TestCreatingANewTable()
        {
            Table t1 = new Table();
            t1.Ante = 10;
            t1.DealerPosition = 0;
            t1.MaxBet = 50;
            t1.MaxBuyIn = 500;
            t1.MinBet = 10;
            t1.Name = "TestTable";
            t1.Pot = 0;
            t1.TableHost = "Bob";

            TableSqlDal dal = new TableSqlDal();

            int newID = dal.CreateTable(t1);

            Assert.IsNotNull(newID);

            Table t2 = new Table();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM poker_table WHERE table_id = @id;", conn);
                cmd.Parameters.AddWithValue("@id", newID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    t2.Ante = Convert.ToInt32(reader["ante"]);
                    t2.DealerPosition = Convert.ToInt32(reader["dealer_position"]);
                    t2.MaxBet = Convert.ToInt32(reader["max_bet"]);
                    t2.MinBet = Convert.ToInt32(reader["min_bet"]);
                    t2.MaxBuyIn = Convert.ToInt32(reader["max_buy_in"]);
                    t2.Name = Convert.ToString(reader["name"]);
                    t2.Pot = Convert.ToInt32(reader["pot"]);
                    t2.TableHost = Convert.ToString(reader["host"]);
                    t2.TableID = Convert.ToInt32(reader["table_id"]);
                }

                Assert.IsNotNull(t2);
                Assert.AreEqual(500, t2.MaxBuyIn);
                Assert.AreEqual(newID, t2.TableID);
                Assert.AreEqual("TestTable", t2.Name);
                Assert.AreEqual(t1.MinBet, t2.MinBet);
                Assert.AreEqual(t1.TableHost, t2.TableHost);
            }

        }



        [TestMethod]
        public void TestAddingAPlayerToATable()
        {
            TableSqlDal dal = new TableSqlDal();
            string playerToAdd = "Brian";
            int tableID = 0;
            Table t = new Table();


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT table_id FROM poker_table WHERE host = 'Bob'", conn);
                tableID = (int)cmd.ExecuteScalar();
            }
            dal.AddPlayerToTable(tableID, playerToAdd);

            //player added to table, now need to get all information out of table_players
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand($"SELECT * FROM table_players WHERE table_id = {tableID}", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    t.TableID = Convert.ToInt32(reader["table_id"]);
                    Seat s = new Seat();
                    s.Username = Convert.ToString(reader["player"]);
                    s.IsTurn =  Convert.ToBoolean(reader["isTurn"]);
                    t.Seats.Add(s);
                }

            }
            Assert.IsNotNull(t.Seats);
            Assert.AreEqual(3, t.Seats.Count);
            Assert.AreEqual("Brian", t.Seats[2].Username);

        }

        [TestMethod]
        public void TestRemovingAPlayerFromATable()
        {
            TableSqlDal dal = new TableSqlDal();
            string playerToRemove = "Boo";
            int tableID = 0;
            Table t = new Table();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT table_id FROM poker_table WHERE host = 'Bob'", conn);
                tableID = (int)cmd.ExecuteScalar();
            }

            dal.RemovePlayerFromTable(tableID, playerToRemove);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand($"SELECT * FROM table_players WHERE table_id = {tableID}", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    t.TableID = Convert.ToInt32(reader["table_id"]);
                    Seat s = new Seat();
                    s.Username = Convert.ToString(reader["player"]);
                    s.IsTurn = Convert.ToBoolean(reader["isTurn"]);
                    t.Seats.Add(s);
                }

            }
            Assert.IsNotNull(t.Seats);
            Assert.AreEqual(1, t.Seats.Count);
            Assert.AreEqual("Bob", t.Seats[0].Username);
        }

    }
}
