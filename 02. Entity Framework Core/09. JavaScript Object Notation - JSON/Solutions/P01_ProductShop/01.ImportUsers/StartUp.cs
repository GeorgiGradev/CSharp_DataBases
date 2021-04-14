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

            string inputJson = File.ReadAllText("../../../Datasets/users.json");
            string result = ImportUsers(context, inputJson);
            Console.WriteLine(result);


        }
        private static void ResetDataBase(ProductShopContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database was deleted!");
            context.Database.EnsureCreated();
            Console.WriteLine("Database was created!");
        }

        //Problem 01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            List<User> users = JsonConvert.DeserializeObject<List<User>>(inputJson);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }


    }
}