using System;
using System.IO;
using System.Linq;
using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using ProductShop.XMLHelper;

namespace ProductShop
{
    public class StartUp
    {
        public static void CreateDatabase(ProductShopContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database deleted successfully");
            context.Database.EnsureCreated();
            Console.WriteLine("Database created successfully");
        }
        public static void Main(string[] args)
        {
            using ProductShopContext context = new ProductShopContext();
           // CreateDatabase(context);

            //01
            //string userXML = File.ReadAllText("../../../Datasets/users.xml");
            //Console.WriteLine(ImportUsers(context, userXML));

            //02
            string userXML = File.ReadAllText("../../../Datasets/products.xml");
            Console.WriteLine(ImportProducts(context, userXML));

        }

        //02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            const string rootElement = "Products";

            var productDtos = XMLConverter.Deserializer<ImportProductDTO>(inputXml, rootElement);

            var products = productDtos
                .Select(p => new Product
                {
                    Name = p.Name,
                    Price = p.Price,
                    BuyerId = p.BuyerId,
                    SellerId = p.SellerId
                })
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }








        //01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXML)
        {
            var usersResult = XMLConverter.Deserializer<ImportUserDTO>(inputXML, "Users");

            var users = usersResult
                .Select(x => new User
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age
                })
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
    }
}