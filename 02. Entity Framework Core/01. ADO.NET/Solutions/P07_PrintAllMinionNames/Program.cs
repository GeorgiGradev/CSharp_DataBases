using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace P07_PrintAllMinionNames
{
    class StartUp
    {
        static void Main(string[] args)
        {
            string connectionString = @"Server=.;Database=MinionsDB;Integrated security=true;";
            using SqlConnection sqlConnection = new SqlConnection(@"Server=.;Database=MinionsDB;Integrated security=true;");

            sqlConnection.Open();

            string getMinionsNamesQuerryText = @"SELECT Name FROM Minions";
            var getMinionCommand = new SqlCommand(getMinionsNamesQuerryText, sqlConnection);

            using SqlDataReader reader = getMinionCommand.ExecuteReader();

            List<string> minionsList = new List<string>();

            while (reader.Read())
            {
                minionsList.Add(reader[0].ToString());
            }

            while (minionsList.Any())
            {
                Console.WriteLine(minionsList[0]);
                minionsList.RemoveAt(0);

                if (minionsList.Any())
                {
                    Console.WriteLine(minionsList[^1]);
                    minionsList.RemoveAt(minionsList.Count - 1);
                }
            }
        }
    }
}