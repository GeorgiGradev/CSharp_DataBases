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

            Console.WriteLine(GetGoldenBooks(db));
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var query =
                context.Books
                .Select(b => new
                {
                    b.Title,
                    b.Copies,
                    b.BookId,
                    b.EditionType
                })
                .Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold)
                .OrderBy(x => x.BookId)
                .ToList();

            var sb = new StringBuilder();
            foreach (var item in query)
            {
                sb.AppendLine(item.Title);
            }

            return sb.ToString().TrimEnd();
        }
    }
}
