using System;
using System.Text;
using Microsoft.Data.SqlClient;

namespace P06_RemoveVillain
{
    class StartUp
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";

        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            int villainId = int.Parse(Console.ReadLine());

            string result = RemoveVillainById(sqlConnection, villainId);

            Console.WriteLine(result);
        }

        private static string RemoveVillainById(SqlConnection sqlConnection, int villainId)
        {
            StringBuilder sb = new StringBuilder();

            using SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            string getVillainNameQuerryText = @"SELECT Name FROM Villains WHERE Id = @villainId";
            using SqlCommand getVillainNameCommand = new SqlCommand(getVillainNameQuerryText, sqlConnection);
            getVillainNameCommand.Parameters.AddWithValue("@villainId", villainId);
            getVillainNameCommand.Transaction = sqlTransaction;

            string villainName = getVillainNameCommand.ExecuteScalar()?.ToString();

            if (villainName == null)
            {
                sb.AppendLine("No such villain was found.");
            }
            else
            {
                try
                {
                    string releaseMinionsQuerryText = @"DELETE FROM MinionsVillains WHERE VillainId = @villainId";
                    using SqlCommand releaseMinionsCommand = new SqlCommand(releaseMinionsQuerryText, sqlConnection);
                    releaseMinionsCommand.Parameters.AddWithValue("@villainId", @villainId);
                    releaseMinionsCommand.Transaction = sqlTransaction;

                    int realeasedMinionsCount = releaseMinionsCommand.ExecuteNonQuery();

                    string deleteVillainQuerryText = @"DELETE FROM Villains WHERE Id = @villainId";

                    using SqlCommand deleteVillainCommand = new SqlCommand(deleteVillainQuerryText, sqlConnection);
                    deleteVillainCommand.Parameters.AddWithValue("@villainId", villainId);

                    deleteVillainCommand.Transaction = sqlTransaction;

                    deleteVillainCommand.ExecuteNonQuery();

                    sqlTransaction.Commit();

                    sb.AppendLine($"{villainName} was deleted.");
                    sb.AppendLine($"{realeasedMinionsCount} minions were released");

                }
                catch (Exception ex)
                {
                    sb.AppendLine(ex.Message);
                    try
                    {

                        sqlTransaction.Rollback();

                    }
                    catch (Exception rollbackEx)
                    {
                        sb.AppendLine(rollbackEx.Message);
                    }
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}