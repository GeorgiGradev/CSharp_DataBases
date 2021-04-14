using System;
using BookShop.Models;

namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using BookShopContext db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            string input = Console.ReadLine();
            Console.WriteLine(GetBooksByAuthor(db, input));
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var query = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    FullName = b.Author.FirstName + " " + b.Author.LastName
                })
                .ToList();

            var sb = new StringBuilder();
            foreach (var item in query)
            {
                sb.AppendLine($"{item.Title} ({item.FullName})");
            }


            return sb.ToString().TrimEnd();
        }
    }
}
