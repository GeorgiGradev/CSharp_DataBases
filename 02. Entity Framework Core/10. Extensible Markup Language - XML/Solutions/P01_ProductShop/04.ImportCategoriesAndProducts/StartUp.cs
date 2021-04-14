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
            //string userXML = File.ReadAllText("../../../Datasets/products.xml");
            //Console.WriteLine(ImportProducts(context, userXML));

            //03
            //string userXML = File.ReadAllText("../../../Datasets/categories.xml");
            //Console.WriteLine(ImportCategories(context, userXML));

            //04
            string userXML = File.ReadAllText("../../../Datasets/categories-products.xml");
            Console.WriteLine(ImportCategoryProducts(context, userXML));
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