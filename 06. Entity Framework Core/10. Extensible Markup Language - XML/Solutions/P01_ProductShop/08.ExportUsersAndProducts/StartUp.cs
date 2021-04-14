using System;
using System.IO;
using System.Linq;
using ProductShop.Data;
using ProductShop.Dtos.Export;
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
            //string userXML = File.ReadAllText("../../../Datasets/products.xml");
            //Console.WriteLine(ImportProducts(context, userXML));

            //03
            //string userXML = File.ReadAllText("../../../Datasets/categories.xml");
            //Console.WriteLine(ImportCategories(context, userXML));

            //04
            //string userXML = File.ReadAllText("../../../Datasets/categories-products.xml");
            //Console.WriteLine(ImportCategoryProducts(context, userXML));

            //05
            //string XML = GetProductsInRange(context);
            //File.WriteAllText("../../../Results/05.ProductsInRamge.xml", XML);
            //Console.WriteLine(XML);

            //06
            //Console.WriteLine(GetSoldProducts(context));

            //07
            //Console.WriteLine(GetCategoriesByProductsCount(context));

            //08
            Console.WriteLine(GetUsersWithProducts(context));

        }
        //08. Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var result = context
               .Users
               .ToList()
               .Where(x => x.ProductsSold.Any())
               .OrderByDescending(x=>x.ProductsSold.Count)
               .Select(x => new UserDTO
               {
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   Age = x.Age,
                   SoldProducts = new SoldProductsDTO
                   {
                       Count = x.ProductsSold.Count,
                       Products = x.ProductsSold.Select(y => new ProductDTO
                       {
                           Name = y.Name,
                           Price = y.Price
                       })
                       .OrderByDescending(y=>y.Price)
                       .ToList()
                   }
               })
               .Take(10)
               .ToList();

            var mainObject = new P08_UsersAndProductsDTO
            {
                Count = context.Users.Where(x=>x.ProductsSold.Any()).Count(),
                Users = result
            };

            var XML = XMLConverter.Serialize(mainObject, "Users");
            return XML;
        }

        //07. Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var result = context
                .Categories
                .Select(x => new P07_CategoriesByProductsCountDTO
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Average(p => p.Product.Price),
                    TotalRevenue = x.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToList();

            string XML = XMLConverter.Serialize(result, "Categories");
            return XML;
        }

        //06. Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var result = context
                .Users
                .Where(x => x.ProductsSold.Any())
                .Select(x => new P06_SoldProducts
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(y => new UserProductDTO
                    {
                        Name = y.Name,
                        Price = y.Price
                    })
                    .ToList()
                }).ToList()
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToList();

            string XML = XMLConverter.Serialize(result, "Users");
            return XML;
        }

        //05 Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var result = context
                .Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new P05_ProductsInRangeDTO
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToList();

            string XML = XMLConverter.Serialize(result, "Products");
            return XML;
        }


        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXML)
        {
            var categoryProductsResult = XMLConverter.Deserializer<ImportCategoryProductsDTO>(inputXML, "CategoryProducts");

            var categoryProducts = categoryProductsResult
                .Where(x =>
                context.Categories.Any(c => c.Id == x.CategoryId) &&
                context.Products.Any(p => p.Id == x.ProductId))
                .Select(x => new CategoryProduct
                {
                    CategoryId = x.CategoryId,
                    ProductId = x.ProductId,
                })
                .ToList();

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }


        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var categoryResult = XMLConverter.Deserializer<ImportCategoriesDTO>(inputXml, "Categories");

            var categories = categoryResult
                .Select(x => new Category
                {
                    Name = x.Name
                })
                .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
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