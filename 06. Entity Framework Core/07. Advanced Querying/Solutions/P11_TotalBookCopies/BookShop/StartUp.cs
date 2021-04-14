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

            Console.WriteLine(CountCopiesByAuthor(db));
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var query = context
                .Authors
                .Select(a => new
                {
                    booksCount = a.Books.Sum(b=>b.Copies),
                    fullName = a.FirstName + " " + a.LastName
                })
                .OrderByDescending(a => a.booksCount)
                .ToList();

            var sb = new StringBuilder();
            foreach(var item in query)
            {
                sb.AppendLine($"{item.fullName} - {item.booksCount}");
            }
            
            return sb.ToString().TrimEnd();
        }
    }
}
