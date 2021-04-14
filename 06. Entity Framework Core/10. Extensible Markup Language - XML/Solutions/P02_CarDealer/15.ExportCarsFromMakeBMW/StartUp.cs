using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.XMLHelper;
using System;
using System.Collections.Generic;
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
            // CreateDatabase(context);

            //09
            //string inputXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //Console.WriteLine(ImportSuppliers(context, inputXml));

            //10
            //string inputXml = File.ReadAllText("../../../Datasets/Parts.xml");
            //Console.WriteLine(ImportParts(context, inputXml));

            //11
            //string inputXml = File.ReadAllText("../../../Datasets/Cars.xml");
            //Console.WriteLine(ImportCars(context, inputXml));

            //12
            //string inputXml = File.ReadAllText("../../../Datasets/Customers.xml");
            //Console.WriteLine(ImportCustomers(context, inputXml));

            //13
            //string inputXml = File.ReadAllText("../../../Datasets/sales.xml");
            //Console.WriteLine(ImportSales(context, inputXml));

            //14
            //Console.WriteLine(GetCarsWithDistance(context));

            //15
            Console.WriteLine(GetCarsFromMakeBmw(context));


        }

        #region 15. Cars from Make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var result = context
                .Cars
                .Where(x => x.Make == "BMW")
                .Select(x=> new ExportCarFromMakeDTO
                { 
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToList();
            var XML = XMLConverter.Serialize(result, "cars");

            return XML;
        }
        #endregion
        #region 14 Car with Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var result = context
                .Cars
                .Where(x => x.TravelledDistance > 2_000_000)
                .Select(x => new P14_CarsWithDistance
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ToList();
            string XML = XMLConverter.Serialize(result, "cars");
            return XML;
        }
        #endregion
        #region 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var rootElement = "Sales";

            var sales = XMLConverter.Deserializer<ImportSalesDTO>(inputXml, rootElement)
                .Where(s => context.Cars.Any(c => c.Id == s.CarId))
                .Select(s => new Sale()
                {
                    CarId = s.CarId,
                    CustomerId = s.CustomerId,
                    Discount = s.Discount
                })
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }
        #endregion
        #region 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var customerResults = XMLConverter.Deserializer<ImportCustomerDTO>(inputXml, "Customers");

            var customers = customerResults
                .Select(x => new Customer
                {
                    Name = x.Name,
                    BirthDate = DateTime.Parse(x.BirthDate),
                    IsYoungDriver = x.IsYoungDriver
                })
                .ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }
        #endregion
        #region 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var carsDtos = XMLConverter.Deserializer<ImportCarDTO>(inputXml, "Cars");
            var cars = new List<Car>();

            foreach (var car in carsDtos)
            {
                var partsIds = car.Parts
                    .Select(p => p.Id)
                    .Distinct()
                    .Where(id => context.Parts.Any(p => p.Id == id))
                    .ToArray();

                var currCar = new Car()
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance,
                    PartCars = partsIds.Select(id => new PartCar()
                    {
                        PartId = id
                    })
                    .ToArray()
                };

                cars.Add(currCar);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }
        #endregion
        #region 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var partsResult = XMLConverter.Deserializer<ImportPartsDTO>(inputXml, "Parts");

            var parts = partsResult
                .Where(x => context.Suppliers.Any(s => s.Id == x.SupplierId))
                .Select(x => new Part
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = x.SupplierId
                })
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }
        #endregion
        #region 09. Import Suppliers
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
        #endregion
 
    }
}