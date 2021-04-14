using System;
using System.Text;
using Microsoft.Data.SqlClient;

namespace P05_ChangeTownNamesCasing
{
    class StartUp
    {
        static void Main(string[] args)
        {
            string ConnectionString = "Server=.;Database=MinionsDB;Integrated Security=true";
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            string countryName = Console.ReadLine();

            StringBuilder sb = new StringBuilder();

            bool doesCountryExist = SeeIfCountryExists(sb, sqlConnection, countryName);

            bool hasAnyTownsConnectedToIt = SeeIfTownsAreConnectedToACountry(sb, sqlConnection, countryName);

            if (doesCountryExist && hasAnyTownsConnectedToIt)
            {
                string updateCountryCasingQuerryText =
                    @"UPDATE Towns SET Name = UPPER(Name) WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";
                using SqlCommand updateCountryCasingCommand =
                    new SqlCommand(updateCountryCasingQuerryText, sqlConnection);
                updateCountryCasingCommand.Parameters.AddWithValue("@countryName", countryName);

                int affectedTownsCount = updateCountryCasingCommand.ExecuteNonQuery();
                sb.AppendLine($"{affectedTownsCount} town names were affected.");

                string getAffectedTownsNameQuerryText = @" SELECT t.Name 
                                                   FROM Towns as t
                                                   JOIN Countries AS c ON c.Id = t.CountryCode
                                                  WHERE c.Name = @countryName";
                using SqlCommand getAffectedTownNamesCommand = new SqlCommand(getAffectedTownsNameQuerryText, sqlConnection);
                getAffectedTownNamesCommand.Parameters.AddWithValue("@countryName", countryName);

                using SqlDataReader reader = getAffectedTownNamesCommand.ExecuteReader();

                sb.Append(@"[");
                while (reader.Read())
                {
                    string townName = reader["Name"].ToString();
                    sb.Append(townName + ", ");
                }
                sb.Remove(sb.Length - 2, 2);
                sb.Append(@"]");
                Console.WriteLine(sb.ToString().TrimEnd());
            }
            else
            {
                Console.WriteLine("No town names were affected.");
            }

        }

        private static bool SeeIfTownsAreConnectedToACountry(StringBuilder sb, SqlConnection sqlConnection, string countryName)
        {
            string getCountOfConnectedCitiesToCountryQuerryText =
                @"SELECT COUNT(*) FROM Countries AS c LEFT JOIN Towns as t ON t.CountryCode = c.Id WHERE c.Name = @countryName AND t.Name IS NOT NULL";
            using SqlCommand getCountOfConnectedCitiesCommand =
                new SqlCommand(getCountOfConnectedCitiesToCountryQuerryText, sqlConnection);
            getCountOfConnectedCitiesCommand.Parameters.AddWithValue("@countryName", countryName);
            int countOfConnectedCountries = (int)getCountOfConnectedCitiesCommand.ExecuteScalar();

            if (countOfConnectedCountries > 0)
            {
                return true;
            }

            return false;
        }

        private static bool SeeIfCountryExists(StringBuilder sb, SqlConnection sqlConnection, string countryName)
        {
            string getCountryIdQuerryText = @"SELECT Id FROM Countries WHERE Name = @countryName";
            using SqlCommand getCountryIdCommand = new SqlCommand(getCountryIdQuerryText, sqlConnection);
            getCountryIdCommand.Parameters.AddWithValue("@countryName", countryName);

            string countryId = getCountryIdCommand.ExecuteScalar()?.ToString();

            if (countryId == null)
            {
                return false;
            }

            return true;
        }
    }
}