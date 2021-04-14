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
        //private static void ResetDataBase(ProductShopContext context)
        //{
        //    context.Database.EnsureDeleted();
        //    Console.WriteLine("Database was deleted!");
        //    context.Database.EnsureCreated();
        //    Console.WriteLine("Database was created!");
        //}

        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();
            //ResetDataBase(context);

            string jsonAsText = File.ReadAllText("../../../Datasets/categories.json");
            Console.WriteLine(ImportCategories(context, jsonAsText));

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

        //Problem 03.ImportCategories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            List<Category> categories = JsonConvert
                .DeserializeObject<List<Category>>(inputJson)
                .Where(x=>x.Name != null)
                .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }
    }
}