using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.Data.SqlClient;

namespace P03_MinionNames
{
    class StartUp
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";

        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            int villianId = int.Parse(Console.ReadLine());

            string result = GetMinionsInfoAboutVillian(sqlConnection, villianId);

            Console.WriteLine(result);
        }

        private static string GetMinionsInfoAboutVillian(SqlConnection sqlConnection, int villianId)
        {
            StringBuilder sb = new StringBuilder();

            string villianName = GetVillianName(sqlConnection, villianId);

            if (villianName == null)
            {
                sb.Append($"No villain with ID {villianId} exists in the database.");
            }
            else
            {
                sb.AppendLine($"Villian: {villianName}");
                string getMinionsInfoQuerryText = @"SELECT m.Name,m.Age
	                                                FROM Villains AS v
	                                                LEFT JOIN MinionsVillains AS mv ON mv.VillainId = v.Id
	                                                LEFT JOIN Minions AS m ON mv.MinionId = m.Id
	                                                WHERE v.Name = @villianName
	                                                ORDER BY m.Name";

                SqlCommand getMinionsinfoCommand = new SqlCommand(getMinionsInfoQuerryText, sqlConnection);
                getMinionsinfoCommand.Parameters.AddWithValue("@villianName", villianName);

                using SqlDataReader reader = getMinionsinfoCommand.ExecuteReader();

                int rowNum = 1;
                while (reader.Read())
                {
                    string minionName = reader["Name"]?.ToString();
                    string minionAge = reader["Age"]?.ToString();

                    if (minionName == "" && minionAge == "")
                    {
                        sb.AppendLine("(no minions)");
                        break;
                    }

                    sb.AppendLine($"{rowNum}. {minionName} {minionAge}");
                    rowNum++;
                }
            }

            return sb.ToString().TrimEnd();
        }

        private static string GetVillianName(SqlConnection sqlConnection, int villianId)
        {

            string getVillianNameQuerryText = @"SELECT Name FROM Villains WHERE Id = @villianId";

            using SqlCommand getVillianNameCommand = new SqlCommand(getVillianNameQuerryText, sqlConnection);
            getVillianNameCommand.Parameters.AddWithValue("@villianId", villianId);

            string villianName = getVillianNameCommand
                .ExecuteScalar()?
                .ToString();

            return villianName;
        }
    }
}