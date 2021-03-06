﻿using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Capstone.Web.Dal_s
{
    public class UserSqlDal : IUserSqlDal
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

        public bool Register(UserModel user)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("Insert into users (username, password, current_money, highest_money, privilege, is_online, salt) Values (@username, @password, 1000, 1000, 'GameHost', 1, @salt)", conn);
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@salt", user.Salt);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }

            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckStatus(string username)
        {
            List<int> check = new List<int>();
            bool found = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT table_id from table_players WHERE player = @username;", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        check.Add(Convert.ToInt32(reader["table_id"]));
                    }
                }

                if (check.Count > 0)
                {
                    found = true;
                }

            }
            catch (SqlException)
            {
                throw;
            }
            return found;
        }

        public List<string> GetAllUsernames()
        {
            List<string> output = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT username from users;", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        output.Add(Convert.ToString(reader["username"]));
                    }
                }
                catch (SqlException)
                {
                    throw;
                }

                return output;
            }
        }

        //need to test this one
        public Dictionary<string, int> GetAllUsernamesWithChipsSortedByChipCount()
        {
            Dictionary<string, int> output = new Dictionary<string, int>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT TOP 10 username, current_money from users ORDER BY current_money DESC;", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        output.Add(Convert.ToString(reader["username"]), Convert.ToInt32(reader["current_money"]));
                    }
                }
                catch (SqlException)
                {
                    throw;
                }

                return output;
            }
        }

        public UserModel Login(string username)
        {
            UserModel user = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM users WHERE username = @username;", conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        user = new UserModel
                        {
                            Username = Convert.ToString(reader["username"]),
                            CurrentMoney = Convert.ToInt32(reader["current_money"]),
                            IsOnline = true,
                            Privilege = Convert.ToString(reader["privilege"]),
                            Salt = Convert.ToString(reader["salt"]),
                            Password = Convert.ToString(reader["password"]),
                        };
                    }
                }
                catch (SqlException)
                {
                    throw;
                }

                return user;
            }
        }

        //not tested
        public bool UpdateMoney(string username, int amount)
        {
            int rowsEffected = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE users SET current_money = @amount WHERE username = @username;", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    rowsEffected = cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }

            return (rowsEffected > 0);

        }

        public UserModel GetUserByUserName(string username)
        {
            UserModel u = new UserModel();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM users WHERE username = @username;", conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        u.Username = Convert.ToString(reader["username"]);
                        u.CurrentMoney = Convert.ToInt32(reader["current_money"]);
                        u.IsOnline = true;
                        u.Privilege = Convert.ToString(reader["privilege"]);
                        //u.Salt = Convert.ToString(reader["salt"]);
                        //u.Password = Convert.ToString(reader["password"]);
                    }
                }
            }
            catch
            {
                throw;
            }

            return u;

        }

        public int GetTableByPlayer(string username)
        {
            int tableID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT TOP 1 (table_id) from table_players WHERE player = @username", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        tableID = Convert.ToInt32(reader["table_id"]);
                    }
                }
                catch (SqlException)
                {
                    throw;
                }

                return tableID;
            }
        }
    }
}