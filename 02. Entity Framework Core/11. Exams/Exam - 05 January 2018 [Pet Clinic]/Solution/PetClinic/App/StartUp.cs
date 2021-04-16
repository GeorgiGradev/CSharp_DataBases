using System;
using System.IO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PetClinic.Data;

namespace PetClinic.App
{
    public class StartUp
    {
        static void Main()
        {
            using (var context = new PetClinicContext())
            {
                Mapper.Initialize(config => config.AddProfile<PetClinicProfile>());

               // ResetDatabase(context);

               // ImportEntities(context);

               ExportEntities(context);

               // BonusTask(context);
            }
        }

        private static void ImportEntities(PetClinicContext context, string baseDir = @"..\..\..\..\Datasets\")
        {
            const string exportDir = "../../../Results/";

            string animalAids = DataProcessor.Deserializer.ImportAnimalAids(context, File.ReadAllText(baseDir + "animalAids.json"));
            PrintAndExportEntityToFile(animalAids, exportDir + "AnimalAidsImport.txt");

            string animals = DataProcessor.Deserializer.ImportAnimals(context, File.ReadAllText(baseDir + "animals.json"));
            PrintAndExportEntityToFile(animals, exportDir + "AnimalsImport.txt");

            string vets = DataProcessor.Deserializer.ImportVets(context, File.ReadAllText(baseDir + "vets.xml"));
            PrintAndExportEntityToFile(vets, exportDir + "VetsImport.txt");

            var procedures = DataProcessor.Deserializer.ImportProcedures(context, File.ReadAllText(baseDir + "procedures.xml"));
            PrintAndExportEntityToFile(procedures, exportDir + "ProceduresImport.txt");
        }

        private static void ExportEntities(PetClinicContext context)
        {
            const string exportDir = "../../../Results/";

            //string animalsExport = DataProcessor.Serializer.ExportAnimalsByOwnerPhoneNumber(context, "0887446123");
            //PrintAndExportEntityToFile(animalsExport, exportDir + "AnimalsExport.json");

            string proceduresExport = DataProcessor.Serializer.ExportAllProcedures(context);
            PrintAndExportEntityToFile(proceduresExport, exportDir + "ProceduresExport.xml");
        }

        private static void BonusTask(PetClinicContext context)
        {
            var bonusOutput = DataProcessor.Bonus.UpdateVetProfession(context, "+359284566778", "Primary Care");
            Console.WriteLine(bonusOutput);
        }

        private static void PrintAndExportEntityToFile(string entityOutput, string outputPath)
        {
            Console.WriteLine(entityOutput);
            File.WriteAllText(outputPath, entityOutput.TrimEnd());
        }

        //private static void ResetDatabase(PetClinicContext context)
        //{
        //    context.Database.EnsureDeleted();
        //    context.Database.EnsureCreated();

        //    Console.WriteLine("Database reset.");
        //}

        private static void ResetDatabase(PetClinicContext context, bool shouldDropDatabase = false)

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