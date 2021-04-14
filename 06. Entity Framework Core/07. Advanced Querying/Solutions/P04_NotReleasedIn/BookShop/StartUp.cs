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

            int year = int.Parse(Console.ReadLine());

            Console.WriteLine(GetBooksNotReleasedIn(db, year));
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var query = context.
                Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return String.Join(Environment.NewLine, query);
        }
    }
}
