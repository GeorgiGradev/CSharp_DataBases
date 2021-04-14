using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace P04_AddMinion
{
    public class StartUp
    {
        private const string connectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";

        public static void Main(string[] args)
        {
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            var sqlTransaction = sqlConnection.BeginTransaction();

            var minionInfo = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(" ");

            var minionName = minionInfo[0];
            var minionAge = int.Parse(minionInfo[1]);
            var minionTown = minionInfo[2];

            var villainName = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)[1];

            var result = "";

            try
            {
                result = AddMinion(sqlConnection, sqlTransaction, minionName, minionAge, minionTown, villainName);

                sqlTransaction.Commit();

                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string AddMinion(SqlConnection sqlConnection, SqlTransaction sqlTransaction, string minionName, int minionAge, string minionTown, string villainName)
        {
            var sb = new StringBuilder();

            if (IsTownAdded(sqlConnection, sqlTransaction, minionTown))
            {
                sb.AppendLine($"Town {minionTown} was added to the database.");
            }

            if (IsVillainAdded(sqlConnection, sqlTransaction, villainName))
            {
                sb.AppendLine($"Villain {villainName} was added to the database.");
            }

            //get townId
            var townId = GetTownId(sqlConnection, sqlTransaction, minionTown);

            //insert current minion data into Minions
            InsertIntoMinions(sqlConnection, sqlTransaction, minionName, minionAge, townId);

            //get villainId
            var villainId = GetVillainId(sqlConnection, sqlTransaction, villainName);

            //get minionId
            var minionId = GetMinionId(sqlConnection, sqlTransaction, minionName);

            //insert mininId and villainId to MinionsVillains
            InsertIntoMinionsVillains(sqlConnection, sqlTransaction, minionId, villainId);

            sb.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");

            return sb.ToString().TrimEnd();

        }
        public static bool IsTownAdded(SqlConnection sqlConnection, SqlTransaction sqlTransaction, string minionTown)
        {
            var getTownQuery = @"SELECT * FROM Towns WHERE [Name] = @townName";

            using var getTownCommand = new SqlCommand(getTownQuery, sqlConnection);
            getTownCommand.Parameters.AddWithValue("@townName", minionTown);
            getTownCommand.Transaction = sqlTransaction;

            using var reader = getTownCommand.ExecuteReader();

            if (reader.HasRows)
            {
                return false;
            }
            else
            {
                reader.Close();

                //Country code unknown, set default to 1
                var insertTownQuery = @"INSERT INTO Towns([Name], CountryCode) VALUES (@townName, 1)";

                using var insertTownCommand = new SqlCommand(insertTownQuery, sqlConnection);
                insertTownCommand.Parameters.AddWithValue("@townName", minionTown);
                insertTownCommand.Transaction = sqlTransaction;

                insertTownCommand.ExecuteNonQuery();

                return true;
            }
        }

        public static bool IsVillainAdded(SqlConnection sqlConnection, SqlTransaction sqlTransaction, string villainName)
        {
            var getVillainQuery = @"SELECT * FROM Villains WHERE [Name] = @villainName";

            using var getVillainCommand = new SqlCommand(getVillainQuery, sqlConnection);
            getVillainCommand.Parameters.AddWithValue("@villainName", villainName);
            getVillainCommand.Transaction = sqlTransaction;

            using var reader = getVillainCommand.ExecuteReader();

            if (reader.HasRows)
            {
                return false;
            }
            else
            {
                reader.Close();

                //default evilnessFactor = evil
                var insertVillainQuery = @"INSERT INTO Villains([Name], EvilnessFactorId) VALUES (@villainName, 3)";

                using var insertVillainCommand = new SqlCommand(insertVillainQuery, sqlConnection);
                insertVillainCommand.Parameters.AddWithValue("@villainName", villainName);
                insertVillainCommand.Transaction = sqlTransaction;

                insertVillainCommand.ExecuteNonQuery();

                return true;
            }
        }

        public static string GetTownId(SqlConnection sqlConnection, SqlTransaction sqlTransaction, string minionTown)
        {
            var getTownIdQuery = @"SELECT Id FROM Towns WHERE [Name] = @townName";

            using var getTownIdCommand = new SqlCommand(getTownIdQuery, sqlConnection);
            getTownIdCommand.Parameters.AddWithValue("@townName", minionTown);
            getTownIdCommand.Transaction = sqlTransaction;

            return getTownIdCommand.ExecuteScalar()?.ToString();
        }

        public static void InsertIntoMinions(SqlConnection sqlConnection, SqlTransaction sqlTransaction, string minionName, int minionAge, string townId)
        {
            var insertMinionQuery = @"INSERT INTO Minions([Name], Age, TownId) VALUES (@name, @age, @townId)";

            using var insertMinionCommand = new SqlCommand(insertMinionQuery, sqlConnection);
            insertMinionCommand.Parameters.AddWithValue("@name", minionName);
            insertMinionCommand.Parameters.AddWithValue("@age", minionAge);
            insertMinionCommand.Parameters.AddWithValue("@townId", townId);
            insertMinionCommand.Transaction = sqlTransaction;

            insertMinionCommand.ExecuteNonQuery();
        }

        public static string GetVillainId(SqlConnection sqlConnection, SqlTransaction sqlTransaction, string villainName)
        {
            var getVillainIdQuery = @"SELECT Id FROM Villains WHERE [Name] = @villainName";

            using var getVillainIdCommand = new SqlCommand(getVillainIdQuery, sqlConnection);
            getVillainIdCommand.Parameters.AddWithValue("@villainName", villainName);
            getVillainIdCommand.Transaction = sqlTransaction;
            return getVillainIdCommand.ExecuteScalar()?.ToString();
        }

        public static string GetMinionId(SqlConnection sqlConnection, SqlTransaction sqlTransaction, string minionName)
        {
            var getMinionIdQuery = @"SELECT Id FROM Minions WHERE [Name] = @minionName";

            using var getMinionIdCommand = new SqlCommand(getMinionIdQuery, sqlConnection);
            getMinionIdCommand.Parameters.AddWithValue("@minionName", minionName);
            getMinionIdCommand.Transaction = sqlTransaction;

            return getMinionIdCommand.ExecuteScalar()?.ToString();
        }

        public static void InsertIntoMinionsVillains(SqlConnection sqlConnection, SqlTransaction sqlTransaction, string minionId, string villainId)
        {
            var insertToMinionsVillainsQuery = @"INSERT INTO MinionsVillains(MinionId, VillainId) VALUES (@minionId, @villainId)";

            using var insertToMinionsVillainsCommand = new SqlCommand(insertToMinionsVillainsQuery, sqlConnection);
            insertToMinionsVillainsCommand.Parameters.AddWithValue("@minionId", minionId);
            insertToMinionsVillainsCommand.Parameters.AddWithValue("@villainId", villainId);
            insertToMinionsVillainsCommand.Transaction = sqlTransaction;

            insertToMinionsVillainsCommand.ExecuteNonQuery();
        }
    }
}