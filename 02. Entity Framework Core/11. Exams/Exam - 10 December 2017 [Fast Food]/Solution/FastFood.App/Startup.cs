using System;
using System.Data.SqlClient;
using System.IO;
using AutoMapper;
using FastFood.Data;
using FastFood.DataProcessor;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FastFood.App
{
	public class Startup
	{
		public static void Main(string[] args)
		{
			Mapper.Initialize(cfg => cfg.AddProfile<FastFoodProfile>());
			var context = new FastFoodDbContext();

			//ResetDatabase(context);
	

			//ImportEntities(context);

			ExportEntities(context);

			//BonusTask(context);
		}

		private static void ImportEntities(FastFoodDbContext context, string baseDir = @"..\..\..\Datasets\")
		{
			const string exportDir = @"..\..\..\ImportResults\";

            var employees = DataProcessor.Deserializer.ImportEmployees(context, File.ReadAllText(baseDir + "employees.json"));
            PrintAndExportEntityToFile(employees, exportDir + "Employees.txt");

            var items = DataProcessor.Deserializer.ImportItems(context, File.ReadAllText(baseDir + "items.json"));
            PrintAndExportEntityToFile(items, exportDir + "Items.txt");

            var orders = DataProcessor.Deserializer.ImportOrders(context, File.ReadAllText(baseDir + "orders.xml"));
            PrintAndExportEntityToFile(orders, exportDir + "Orders.txt");
        }

		private static void ExportEntities(FastFoodDbContext context)
		{
			const string exportDir = @"..\..\..\ImportResults\";

			var jsonOutput = DataProcessor.Serializer.ExportOrdersByEmployee(context, "Avery Rush", "ToGo");
			Console.WriteLine(jsonOutput);
			File.WriteAllText(exportDir + "OrdersByEmployee.json", jsonOutput);

			//var xmlOutput = DataProcessor.Serializer.ExportCategoryStatistics(context, "Chicken,Drinks,Toys");
			//Console.WriteLine(xmlOutput);
			//File.WriteAllText(exportDir + "CategoryStatistics.xml", xmlOutput);
		}

		private static void BonusTask(FastFoodDbContext context)
		{
			var bonusOutput = DataProcessor.Bonus.UpdatePrice(context, "Cheeseburger", 6.50m);

			Console.WriteLine(bonusOutput);
		}

		private static void PrintAndExportEntityToFile(string entityOutput, string outputPath)
		{
			Console.WriteLine(entityOutput);
			File.WriteAllText(outputPath, entityOutput.TrimEnd());
		}

		//private static void ResetDatabase(FastFoodDbContext context)
		//{
		//	context.Database.EnsureDeleted();
		//	context.Database.EnsureCreated();
		//}

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