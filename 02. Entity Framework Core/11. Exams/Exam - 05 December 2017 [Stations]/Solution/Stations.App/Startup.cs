using System;
using System.IO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Stations.Data;

namespace Stations.App
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<StationsProfile>());
            var context = new StationsDbContext(); 
            
            //ResetDatabase(context); 

            //ImportEntities(context);

            ExportEntities(context);
        }

        private static void ImportEntities(StationsDbContext context, string baseDir = @"..\..\..\..\Datasets\")
        {
            const string exportDir = "../../../ImportResults/";

            var stations = DataProcessor.Deserializer.ImportStations(context, File.ReadAllText(baseDir + "stations.json"));
            PrintAndExportEntityToFile(stations, exportDir + "Stations.txt");

            var classes = DataProcessor.Deserializer.ImportClasses(context, File.ReadAllText(baseDir + "classes.json"));
            PrintAndExportEntityToFile(classes, exportDir + "Classes.txt");

            var trains = DataProcessor.Deserializer.ImportTrains(context, File.ReadAllText(baseDir + "trains.json"));
            PrintAndExportEntityToFile(trains, exportDir + "Trains.txt");

            var trips = DataProcessor.Deserializer.ImportTrips(context, File.ReadAllText(baseDir + "trips.json"));
            PrintAndExportEntityToFile(trips, exportDir + "Trips.txt");

            var cards = DataProcessor.Deserializer.ImportCards(context, File.ReadAllText(baseDir + "cards.xml"));
            PrintAndExportEntityToFile(cards, exportDir + "Cards.txt");

            var tickets = DataProcessor.Deserializer.ImportTickets(context, File.ReadAllText(baseDir + "tickets.xml"));
            PrintAndExportEntityToFile(tickets, exportDir + "Tickets.txt");
        }

        private static void ExportEntities(StationsDbContext context)
        {
            const string exportDir = "../../../ExportResults/";

            var jsonOutput = DataProcessor.Serializer.ExportDelayedTrains(context, "01/01/2017");
            Console.WriteLine(jsonOutput);
            File.WriteAllText(exportDir + "DelayedTrains.json", jsonOutput);


            var xmlOutput = DataProcessor.Serializer.ExportCardsTicket(context, "Elder");
            Console.WriteLine(xmlOutput);
            File.WriteAllText(exportDir + "CardsTicket.xml", xmlOutput);
        }

        private static void PrintAndExportEntityToFile(string entityOutput, string outputPath)
        {
            Console.WriteLine(entityOutput);

            File.WriteAllText(outputPath, entityOutput.TrimEnd());
        }

        //private static void ResetDatabase(StationsDbContext context)
        //{
        //    context.Database.EnsureDeleted();
        //    Console.WriteLine("Database deleted");
        //    context.Database.EnsureCreated();
        //    Console.WriteLine("Database created");
        //}



        //FAST DATABASE DELETE & CREATE
        private static void ResetDatabase(DbContext context, bool shouldDropDatabase = false)
            //За пълно изтриване на базата и наливане на данните => TRUE
            //За изтриванене само на данните и повторното им наливане => FALSE
        {
            if (shouldDropDatabase)
            {
                context.Database.EnsureDeleted();
                Console.WriteLine("Database deleted!");
            }

            if (context.Database.EnsureCreated())
            {
                Console.WriteLine("Database created & reseeded!");
                return;
            }

            var disableIntegrityChecksQuery = "EXEC sp_MSforeachtable @command1='ALTER TABLE ? NOCHECK CONSTRAINT ALL'";
            context.Database.ExecuteSqlCommand(disableIntegrityChecksQuery);

            var deleteRowsQuery = "EXEC sp_MSforeachtable @command1='SET QUOTED_IDENTIFIER ON;DELETE FROM ?'";
            context.Database.ExecuteSqlCommand(deleteRowsQuery);
            Console.WriteLine("Only data deleted!");

            var enableIntegrityChecksQuery =
                "EXEC sp_MSforeachtable @command1='ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'";
            context.Database.ExecuteSqlCommand(enableIntegrityChecksQuery);

            var reseedQuery =
                "EXEC sp_MSforeachtable @command1='IF OBJECT_ID(''?'') IN (SELECT OBJECT_ID FROM SYS.IDENTITY_COLUMNS) DBCC CHECKIDENT(''?'', RESEED, 0)'";
            context.Database.ExecuteSqlCommand(reseedQuery);
            Console.WriteLine("Only data reseeded!");
        }

    }
}