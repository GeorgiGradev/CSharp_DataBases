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
            Console.WriteLine(GetBookTitlesContaining(db, input));
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var query = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b);

            return string.Join(Environment.NewLine, query);
             
        }
    }
}
