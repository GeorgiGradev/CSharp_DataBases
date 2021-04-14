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

            string date = Console.ReadLine();
            Console.WriteLine(GetBooksReleasedBefore(db, date));
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var dateAsDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var query = context
                .Books
                .Where(b => b.ReleaseDate < dateAsDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in query)
            {
                sb.AppendLine($"{item.Title} - {item.EditionType} - ${item.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
