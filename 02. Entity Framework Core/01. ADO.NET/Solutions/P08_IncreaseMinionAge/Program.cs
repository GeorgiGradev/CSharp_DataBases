using System;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;

namespace P08_IncreaseMinionAge
{
    class StartUp
    {
        static void Main(string[] args)
        {
            string ConnectionString = "Server=.;Database=MinionsDB;Integrated Security=true";
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            int[] ids = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)
                .ToArray();

            foreach (var id in ids)
            {
                string updateMinionNameAndAgeQuerryText = @" UPDATE Minions
                                                           SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                                         WHERE Id = @Id";
                using SqlCommand updateMinionNameAndAgeCommand = new SqlCommand(updateMinionNameAndAgeQuerryText, sqlConnection);
                updateMinionNameAndAgeCommand.Parameters.AddWithValue("@Id", id);
                updateMinionNameAndAgeCommand.ExecuteNonQuery();
            }

            string getNameAndAgesFromMinions = @"SELECT Name, Age FROM Minions";
            using SqlCommand getNamesAndAgesFromMinions = new SqlCommand(getNameAndAgesFromMinions, sqlConnection);
            using SqlDataReader reader = getNamesAndAgesFromMinions.ExecuteReader();

            StringBuilder sb = new StringBuilder();

            while (reader.Read())
            {
                string name = reader["Name"].ToString();
                string age = reader["Age"].ToString();

                sb.AppendLine(name + ' ' + age);
            }

            Console.WriteLine(sb.ToString().TrimEnd());
        }
    }
}