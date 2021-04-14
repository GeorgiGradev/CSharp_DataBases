﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTO.Users;
using ProductShop.Models;
using static ProductShop.ProductShopProfile;

namespace ProductShop
{
  
    public class StartUp
    {
        private const string DirectoryPath = "../../../Datasets/Results";
        private static void ResetDataBase(ProductShopContext context)
        {
            //context.Database.EnsureDeleted();
            //Console.WriteLine("Database was deleted!");
            //context.Database.EnsureCreated();
            //Console.WriteLine("Database was created!");
        }

        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();
            // ResetDataBase(context);


            //01
            //string usersJson = File.ReadAllText("../../../Datasets/users.json");
            //Console.WriteLine(ImportUsers(context, usersJson));


            //02
            //string productsJson = File.ReadAllText("../../../Datasets/products.json");
            //Console.WriteLine(ImportProducts(context, productsJson));


            //03
            //string categoriesJson = File.ReadAllText("../../../Datasets/categories.json");
            //Console.WriteLine(ImportCategories(context,categoriesJson));


            //04
            //string categoriesProductsJson = File.ReadAllText("../../../Datasets/categories-products.json");
            //Console.WriteLine(ImportCategoryProducts(context, categoriesProductsJson));


            //05
            //if (!Directory.Exists(DirectoryPath))
            //{
            //    Directory.CreateDirectory(DirectoryPath);
            //}
            //string jsonResult = GetProductsInRange(context);
            //File.WriteAllText(DirectoryPath + "/products-in-range.json", jsonResult);
            //Console.WriteLine(jsonResult);

            //06.Export Sold Products
            //if (!Directory.Exists(DirectoryPath))
            //{
            //    Directory.CreateDirectory(DirectoryPath);
            //}
            //string jsonResult = GetSoldProducts(context);
            //File.WriteAllText(DirectoryPath + "/users-sold-products.json", jsonResult);
            //Console.WriteLine(jsonResult);


            //07.GetCategoriesByProductsCount
            //Console.WriteLine(GetCategoriesByProductsCount(context));


            //08.
            Console.WriteLine(GetUsersWithProducts(context));


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


        //Problem 04.CategoriesProducts
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            List<CategoryProduct> categoriesProducts =
                JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);
            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count}";
        }


        //Problem 05.Export Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                                     .Where(x => x.Price >= 500 && x.Price <= 1000)
                                     .Select(x => new
                                         {
                                         name = x.Name,
                                         price = x.Price,
                                         seller = x.Seller.FirstName + " " + x.Seller.LastName
                                         })
                                     .OrderBy(x => x.price)
                                     .ToList();

            string result = JsonConvert.SerializeObject(products, Formatting.Indented);
            return result;
        }


        //06.Export Sold Products
        //public static string GetSoldProducts(ProductShopContext context)
        //{
        //    var soldProducts = context
        //                    .Users
        //                    .Where(x=>x.ProductsSold.Any(b=>b.Buyer != null))
        //                    .Select(x => new
        //                    {
        //                        firstName = x.FirstName,
        //                        lastName = x.LastName,
        //                        soldProducts = x.ProductsSold
        //                            .Where(y=>y.Buyer != null)
        //                            .Select(y => new
        //                            {
        //                                name = y.Name,
        //                                price = y.Price,
        //                                buyerFirstName = y.Buyer.FirstName,
        //                                buyerLastName = y.Buyer.LastName
        //                            })
        //                    })
        //                    .OrderBy(z=>z.lastName)
        //                    .ThenBy(z=>z.firstName)
        //                    .ToList();
        //    string jsonResult = JsonConvert.SerializeObject(soldProducts, Formatting.Indented);

        //    return jsonResult; 
        //}

        //06.Export Sold Products with DTO & AOUTMAPPING
        public static string GetSoldProducts(ProductShopContext context)
        {
            AutoMapperConfiguration.Configure();

            var soldProducts = context
                            .Users
                            .Where(x => x.ProductsSold.Any(b => b.Buyer != null))
                            .OrderBy(x => x.LastName)
                            .ThenBy(x => x.FirstName)
                            .ProjectTo<UsersWithSoldProductsDTO>()
                            .ToList();


            string jsonResult = JsonConvert.SerializeObject(soldProducts, Formatting.Indented);

            return jsonResult;
        }


        //07.Export Categories by Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var result = context.
                Categories
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Select(y => y.Product.Id).Count(),
                    averagePrice = $"{(x.CategoryProducts.Average(y => y.Product.Price)):f2}",
                    totalRevenue = x.CategoryProducts.Sum(y => y.Product.Price).ToString("f2")
                })
                .OrderByDescending(x => x.productsCount)
                .ToList();

            string jsonResult = JsonConvert.SerializeObject(result, Formatting.Indented);
            return jsonResult;
        }

        //08.Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var result = context
                    .Users
                    .AsEnumerable() 
                    .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                    .OrderByDescending(u => u.ProductsSold.Count(p => p.Buyer != null))
                    .Select(u => new
                    {
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        age = u.Age,
                        soldProducts = new
                        {
                            count = u.ProductsSold.Count(p => p.Buyer != null),
                            products = u.ProductsSold.Where(p => p.Buyer != null)
                            .Select(ps => new
                            {
                                name = ps.Name,
                                price = ps.Price
                            })
                            .ToList()
                        }
                    })
                    .ToList();

            var usersInfo = new
            {
                usersCount = result.Count,
                users = result
            };


            var jsonOutput = JsonConvert.SerializeObject(usersInfo,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
            return jsonOutput;
        }
    }
}