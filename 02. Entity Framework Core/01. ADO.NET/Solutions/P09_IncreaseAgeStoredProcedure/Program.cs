using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace P09_IncreaseAgeStoredProcedure
{
    class StartUp
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";

        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            int minionId = int.Parse(Console.ReadLine());

            string result = IncreaseMinionAgeById(sqlConnection, minionId);

            Console.WriteLine(result);
        }

        private static string IncreaseMinionAgeById(SqlConnection sqlConnection, int minionId)
        {
            StringBuilder sb = new StringBuilder();

            string procName = "usp_GetOlder";
            using SqlCommand increaseAgeCommand = new SqlCommand(procName, sqlConnection);
            increaseAgeCommand.CommandType = CommandType.StoredProcedure;

            increaseAgeCommand.Parameters.AddWithValue("@minionId", minionId);

            increaseAgeCommand.ExecuteNonQuery();

            string getMinionInfoQuerryText = @"SELECT Name,Age FROM Minions WHERE Id = @minionId";
            using SqlCommand getMinionInfoCommand = new SqlCommand(getMinionInfoQuerryText, sqlConnection);
            getMinionInfoCommand.Parameters.AddWithValue("@minionId", minionId);

            using SqlDataReader reader = getMinionInfoCommand.ExecuteReader();

            reader.Read();
            string minionName = reader["Name"]?.ToString().TrimEnd();
            string minionAge = reader["Age"]?.ToString().TrimEnd();

            sb.AppendLine($"{minionName} - {minionAge} years old");

            return sb.ToString().TrimEnd();
        }
    }
}