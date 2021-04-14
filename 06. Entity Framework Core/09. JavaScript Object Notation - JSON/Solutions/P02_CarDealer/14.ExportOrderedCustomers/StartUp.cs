using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTO;
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


            //09
            //string inputJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, inputJson));


            //10
            //string inputJson = File.ReadAllText("../../../Datasets/parts.json");
            //Console.WriteLine(ImportParts(context, inputJson));

            //11
            //string inputJson = File.ReadAllText("../../../Datasets/cars.json");
            //Console.WriteLine(ImportCars(context, inputJson));

            //12
            //string inputJson = File.ReadAllText("../../../Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(context, inputJson));

            //13
            //string inputJson = File.ReadAllText("../../../Datasets/sales.json");
            //Console.WriteLine(ImportSales(context, inputJson));

            //14

            Console.WriteLine(GetOrderedCustomers(context));



        }


        //09. Import Suppliers
        //public static string ImportSuppliers(CarDealerContext context, string inputJson)
        //{
        //    List<Supplier> suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

        //    context.Suppliers.AddRange(suppliers);
        //    context.SaveChanges();

        //    return $"Successfully imported {suppliers.Count}.";
        //}


        //10. Import Parts
        //public static string ImportParts(CarDealerContext context, string inputJson)
        //{
        //    var count = context.Suppliers.Count();
        //    List<Part> parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)
        //         .Where(s => s.SupplierId <= count)
        //         .ToList();
        //    context.Parts.AddRange(parts);
        //    context.SaveChanges();

        //    return $"Successfully imported {parts.Count}.";
        //}

        //11. Import Cars
        //public static string ImportCars(CarDealerContext context, string inputJson)
        //{
        //var carsDto = JsonConvert.DeserializeObject<List<CarDTO>>(inputJson);

        //var cars = new List<Car>();
        //var carParts = new List<PartCar>();

        //foreach (var carDto in carsDto)
        //{
        //    var currCar = new Car()
        //    {
        //        Make = carDto.Make,
        //        Model = carDto.Model,
        //        TravelledDistance = carDto.TravelledDistance
        //    };

        //    cars.Add(currCar);

        //    foreach (var partId in carDto.PartsId.Distinct())
        //    {
        //        var currCarPart = new PartCar()
        //        {
        //            Car = currCar,
        //            PartId = partId
        //        };

        //        carParts.Add(currCarPart);
        //    }
        //}

        //context.PartCars.AddRange(carParts);
        //context.Cars.AddRange(cars);
        //context.SaveChanges();

        //return $"Successfully imported { cars.Count()}.";


        //    List<Car> cars = JsonConvert.DeserializeObject<List<Car>>(inputJson);
        //    context.Cars.AddRange(cars);
        //    context.SaveChanges();

        //    return $"Successfully imported {cars.Count}.";
        //}


        //12. Import Customers
        //public static string ImportCustomers(CarDealerContext context, string inputJson)
        //{
        //    List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);
        //    context.Customers.AddRange(customers);
        //    context.SaveChanges();

        //    return $"Successfully imported {customers.Count}.";
        //}


        //13. Import Sales
        //public static string ImportSales(CarDealerContext context, string inputJson)
        //{
        //    List<Sale> sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);
        //    context.Sales.AddRange(sales);
        //    context.SaveChanges();


        //    return $"Successfully imported {sales.Count}.";
        //}

        //11

        //14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });


            List<OrderedCustomersDTO> customers = context
                .Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .ProjectTo<OrderedCustomersDTO>(config)
                .ToList();

            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return json;
        }
    }
}
