using System;
using BookShop.Models;

namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using BookShopContext db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            string input = Console.ReadLine();
            Console.WriteLine(GetBooksByCategory(db, input));
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var inputData = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            var books = new List<string>();

            foreach (var item in inputData)
            {
                var query = context
                    .Books
                    .Where(b => b.BookCategories
                                    .Any(bc => bc.Category.Name.ToLower() == item.ToLower()))
                    .Select(b => b.Title)
                    .ToList();
                books.AddRange(query);
            }
            var output = books.OrderBy(b => b).ToList();

            return String.Join(Environment.NewLine, output);
        }
    }
}
