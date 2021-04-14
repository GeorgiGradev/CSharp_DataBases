using System;
using Microsoft.Data.SqlClient;

namespace P02_VillainNames
{
    class Program
    {
        static void Main(string[] args)
        {
            using SqlConnection connection
                = new SqlConnection("Server=.;Database=MinionsDB;Integrated Security=true");
            connection.Open();
            string query = "SELECT CONCAT(v.Name, ' - ', COUNT(mv.VillainId)) AS Output FROM Villains AS v JOIN MinionsVillains AS mv ON v.Id = mv.VillainId GROUP BY v.Name HAVING COUNT(mv.VillainId) > 3 ORDER BY COUNT(mv.VillainId)";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string output = (string)reader["Output"];
                Console.WriteLine(output);
            }
        }
    }
}
