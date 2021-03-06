using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();
            //context.Database.EnsureCreated();
            //Console.WriteLine("Database created");
            //context.Database.EnsureDeleted();
            //Console.WriteLine("Database deleted");


            //01
            //string inputJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, inputJson));


            //02
            string inputJson = File.ReadAllText("../../../Datasets/parts.json");
            Console.WriteLine(ImportParts(context, inputJson));


        }


        //09. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            List<Supplier> suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}."; 
        }


        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var count = context.Suppliers.Count();
            List<Part> parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)
                                                             .Where(s => s.SupplierId <= count)
                                                             .ToList();
            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }
    }
}