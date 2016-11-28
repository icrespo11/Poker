using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Capstone.Web.Dal_s
{
    public class UserSqlDal
    {
        static string connectionString = @".\SQLEXPRESS;Initial Catalog=poker;Persist Security Info=True;User ID=te_student;Password=techelevator";

        public static bool Register(UserModel user)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("Insert into users (username, password, current_chips, highest_money, privilege, is_online) Values (@username, @password, 1000, 1000, 'GameHost', 1)", conn);
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@password", user.Password);
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
    }
}