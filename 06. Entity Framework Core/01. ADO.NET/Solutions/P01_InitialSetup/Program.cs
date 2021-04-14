using Microsoft.Data.SqlClient;
using System;

namespace P01_InitialSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            string beforeConnectionString = "Server=.;Database=master;Integrated Security=true";

            using SqlConnection beforeSqlConnection = new SqlConnection(beforeConnectionString);

            //Create MinionsDB
            string createDatabaseQuerryText = @"CREATE DATABASE MinionsDB";
            using SqlCommand creatDatabaseCommand = new SqlCommand(createDatabaseQuerryText, beforeSqlConnection);
            beforeSqlConnection.Open();
            creatDatabaseCommand.ExecuteNonQuery();

            // USE MinionsDB
            string ConnectionString = "Server=.;Database=MinionsDB;Integrated Security=true";
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            //Create Table Countries
            string createTableCountriesQuerryText =
                @"CREATE TABLE Countries (Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50))";
            using SqlCommand createTableCountriesCommand = new SqlCommand(createTableCountriesQuerryText, sqlConnection);
            createTableCountriesCommand.ExecuteNonQuery();

            //Create Table Towns
            string createTableTownsQuerryText =
                @"CREATE TABLE Towns(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50), CountryCode INT FOREIGN KEY REFERENCES Countries(Id))";
            using SqlCommand createTableTownCommand = new SqlCommand(createTableTownsQuerryText, sqlConnection);
            createTableTownCommand.ExecuteNonQuery();

            //Create Table Minions
            string createTableMinionsQuerryText =
                @"CREATE TABLE Minions(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(30), Age INT, TownId INT FOREIGN KEY REFERENCES Towns(Id))";
            using SqlCommand createTableMinionCommand = new SqlCommand(createTableMinionsQuerryText, sqlConnection);
            createTableMinionCommand.ExecuteNonQuery();

            //Create Table EvilnessFactors
            string createTableEvilnessFactorsQuerryText =
                @"CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50))";
            using SqlCommand createTableEvilnessFactorsCommand =
                new SqlCommand(createTableEvilnessFactorsQuerryText, sqlConnection);
            createTableEvilnessFactorsCommand.ExecuteNonQuery();

            //Create Table Villains
            string createTableVillainsQuerryText =
                @"CREATE TABLE Villains (Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50), EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id))";
            using SqlCommand createTableVillainsCommand = new SqlCommand(createTableVillainsQuerryText, sqlConnection);
            createTableVillainsCommand.ExecuteNonQuery();

            //Create Table MinionsVillains
            string createTableMinionsVillainsQuerryText =
                @"CREATE TABLE MinionsVillains (MinionId INT FOREIGN KEY REFERENCES Minions(Id),VillainId INT FOREIGN KEY REFERENCES Villains(Id),CONSTRAINT PK_MinionsVillains PRIMARY KEY (MinionId, VillainId))";
            using SqlCommand createTableMinionsVillainsCommand =
                new SqlCommand(createTableMinionsVillainsQuerryText, sqlConnection);
            createTableMinionsVillainsCommand.ExecuteNonQuery();

            //Insert Into Table Countries
            string insertIntoCountriesQuerryText =
                @"INSERT INTO Countries ([Name]) VALUES ('Bulgaria'),('England'),('Cyprus'),('Germany'),('Norway')";
            using SqlCommand insertIntoCountriesCommand = new SqlCommand(insertIntoCountriesQuerryText, sqlConnection);
            insertIntoCountriesCommand.ExecuteNonQuery();

            //Insert Into Table Towns
            string insertIntoTownsQuerryText =
                @"INSERT INTO Towns ([Name], CountryCode) VALUES ('Plovdiv', 1),('Varna', 1),('Burgas', 1),('Sofia', 1),('London', 2),('Southampton', 2),('Bath', 2),('Liverpool', 2),('Berlin', 3),('Frankfurt', 3),('Oslo', 4)";
            using SqlCommand insertIntoTownsCommand = new SqlCommand(insertIntoTownsQuerryText, sqlConnection);
            insertIntoTownsCommand.ExecuteNonQuery();

            //Insert Into Table Minions
            string insertIntoMinionsQuerryText =
                @"INSERT INTO Minions (Name,Age, TownId) VALUES('Bob', 42, 3),('Kevin', 1, 1),('Bob ', 32, 6),('Simon', 45, 3),('Cathleen', 11, 2),('Carry ', 50, 10),('Becky', 125, 5),('Mars', 21, 1),('Misho', 5, 10),('Zoe', 125, 5),('Json', 21, 1)";
            using SqlCommand insertIntoMinionsCommand = new SqlCommand(insertIntoMinionsQuerryText, sqlConnection);
            insertIntoMinionsCommand.ExecuteNonQuery();

            //Insert Into Table EvilnessFactors
            string insertIntoEvilnessFactorsQuerryText =
                @"INSERT INTO EvilnessFactors (Name) VALUES ('Super good'),('Good'),('Bad'), ('Evil'),('Super evil')";
            using SqlCommand insertIntoEvilnessFactorsCommand =
                new SqlCommand(insertIntoEvilnessFactorsQuerryText, sqlConnection);
            insertIntoEvilnessFactorsCommand.ExecuteNonQuery();

            //Insert Into Table Villains
            string insertIntoVillainsQuerryText =
                @"INSERT INTO Villains (Name, EvilnessFactorId) VALUES ('Gru',2),('Victor',1),('Jilly',3),('Miro',4),('Rosen',5),('Dimityr',1),('Dobromir',2)";
            using SqlCommand insertIntoVillainsCommand = new SqlCommand(insertIntoVillainsQuerryText, sqlConnection);
            insertIntoVillainsCommand.ExecuteNonQuery();

            // Insert Into Table MinionsVillains
            string insertIntoMinionsVillainsQuerryText = @"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (4,2),(1,1),(5,7),(3,5),(2,6),(11,5),(8,4),(9,7),(7,1),(1,3),(7,3),(5,3),(4,3),(1,2),(2,1),(2,7)";
            using SqlCommand insertIntoMinionsVillainsCommand =
                new SqlCommand(insertIntoMinionsVillainsQuerryText, sqlConnection);
            insertIntoMinionsVillainsCommand.ExecuteNonQuery();
        }
    }
}
