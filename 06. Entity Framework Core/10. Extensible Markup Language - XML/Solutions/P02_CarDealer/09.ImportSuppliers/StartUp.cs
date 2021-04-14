using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.XMLHelper;
using System;
using System.IO;
using System.Linq;

namespace CarDealer
{
    public class StartUp
    {
        public static void CreateDatabase(CarDealerContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database deleted successfully");
            context.Database.EnsureCreated();
            Console.WriteLine("Database created successfully");
        }
        public static void Main(string[] args)
        {
            using CarDealerContext context = new CarDealerContext();
             CreateDatabase(context);

            //09
            string inputXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            Console.WriteLine(ImportSuppliers(context, inputXml));

            //10
            //string inputXml = File.ReadAllText("../../../Datasets/Parts.xml");
            //Console.WriteLine(ImportParts(context, inputXml));
        }

        //10. Import Parts
        //public static string ImportParts(CarDealerContext context, string inputXml)
        //{
        //    var partsResult = XMLConverter.Deserializer<ImportPartsDTO>(inputXml, "Parts");

        //    var parts = partsResult
        //        .Where(x => context.Suppliers.Any(s => s.Id == x.SupplierId))
        //        .Select(x => new Part
        //        {
        //            Name = x.Name,
        //            Price = x.Price,
        //            Quantity = x.Quantity,
        //            SupplierId = x.SupplierId
        //        })
        //        .ToList();

        //    context.Parts.AddRange(parts);
        //    context.SaveChanges();

        //    return $"Successfully imported {parts.Count}";
        //}


        //09. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {

            var supplierResult = XMLConverter.Deserializer<ImportSuppliersDTO>(inputXml, "Suppliers");

            var suppliers = supplierResult
                .Select(s => new Supplier
                {
                    Name = s.Name,
                    IsImporter = s.isImporter
                })
                .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }
    }
}