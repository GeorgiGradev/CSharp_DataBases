using System;
using BookShop.Models;

namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using BookShopContext db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            Console.WriteLine(GetBooksByPrice(db));
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var query = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            var sb = new StringBuilder();
            foreach (var item in query)
            {
                sb.AppendLine($"{item.Title} - ${item.Price:f2}");
            }

            return sb.ToString().TrimEnd();


            //StringBuilder sb = new StringBuilder();

            //var booksAbove40 = context
            //    .Books
            //    .Where(b => b.Price > 40)
            //    .Select(b => new
            //    {
            //        Title = b.Title,
            //        Price = b.Price
            //    })
            //    .OrderByDescending(b => b.Price)
            //    .ToList();

            //foreach (var book in booksAbove40)
            //{
            //    sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            //}

            //return sb.ToString().TrimEnd();



        }
    }
}
