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
            Console.WriteLine(GetAuthorNamesEndingIn(db, input));
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {

            var query = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(a => a.FullName)
                .ToList();


            var sb = new StringBuilder();
            foreach (var item in query)
            {
                sb.AppendLine(item.FullName);
            }
            return sb.ToString().TrimEnd();

        }
    }
}
