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

            Console.WriteLine(GetMostRecentBooks(db));
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {

            var query = context
                .Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    MostRecentBooks = c.CategoryBooks
                                       .OrderByDescending(cb => cb.Book.ReleaseDate)
                                       .Take(3)
                                       .Select(cb => new
                                       {
                                           BookTitle = cb.Book.Title,
                                           ReleaseYear = cb.Book.ReleaseDate.Value.Year
                                       })
                                       .ToList()
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var c in query)
            {
                sb
                    .AppendLine($"--{c.CategoryName}");

                foreach (var b in c.MostRecentBooks)
                {
                    sb.AppendLine($"{b.BookTitle} ({b.ReleaseYear})");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}
