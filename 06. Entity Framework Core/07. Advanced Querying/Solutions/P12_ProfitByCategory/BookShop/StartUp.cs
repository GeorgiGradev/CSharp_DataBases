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

            Console.WriteLine(GetTotalProfitByCategory(db));
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var query = context
                .Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    TotalProfit = c.CategoryBooks
                        .Select(cb => new
                        {
                            Total = cb.Book.Price * cb.Book.Copies
                        })
                        .Sum(cb=>cb.Total)
                })
                .OrderByDescending(b => b.TotalProfit)
                .ThenBy(b => b.CategoryName)
                .ToList();

            var sb = new StringBuilder();
            foreach (var item in query)
            {
                sb.AppendLine($"{item.CategoryName} ${item.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
