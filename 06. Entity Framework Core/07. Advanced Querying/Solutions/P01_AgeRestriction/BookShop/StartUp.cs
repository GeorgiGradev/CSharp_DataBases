using System;
using BookShop.Models;

namespace BookShop
{
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
            string inputData = Console.ReadLine();
            Console.WriteLine(GetBooksByAgeRestriction(db, inputData));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var output = context
                .Books
                .AsEnumerable()
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => b.Title)
                .OrderBy(x => x)
                .ToList();

            var outputString = new StringBuilder();
            foreach (var item in output)
            {
                outputString.AppendLine(item);
            }


            return outputString.ToString().TrimEnd();
        }
    }
}
