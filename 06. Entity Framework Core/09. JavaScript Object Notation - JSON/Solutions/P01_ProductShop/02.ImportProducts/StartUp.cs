using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();
            //ResetDataBase(context);

            string inputJson = File.ReadAllText("../../../Datasets/products.json");
            string result = ImportProducts(context, inputJson);
            Console.WriteLine(result);

        }
        private static void ResetDataBase(ProductShopContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database was deleted!");
            context.Database.EnsureCreated();
            Console.WriteLine("Database was created!");
        }

        //Problem 01.ImportUsers
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            List<User> users = JsonConvert.DeserializeObject<List<User>>(inputJson);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }


        //Problem 02.ImportProducts
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(inputJson);
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

    }
}