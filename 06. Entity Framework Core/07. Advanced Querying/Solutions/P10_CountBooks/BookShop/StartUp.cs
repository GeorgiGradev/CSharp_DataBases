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

            int lengthCheck = int.Parse(Console.ReadLine());
            Console.WriteLine(CountBooks(db, lengthCheck));
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int outputResult = context
                .Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

               
            return outputResult;
        }
    }
}
