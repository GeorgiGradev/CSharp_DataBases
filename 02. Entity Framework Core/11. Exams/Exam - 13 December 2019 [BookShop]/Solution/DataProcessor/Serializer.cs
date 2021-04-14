namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using BookShop.XMLHelper;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var result = context
                .Authors
                .Select(x => new
                {
                    AuthorName = x.FirstName + " " +x.LastName,
                    Books = x.AuthorsBooks
                    .OrderByDescending(x => x.Book.Price)
                    .Select(y => new
                    {
                        BookName = y.Book.Name,
                        BookPrice = y.Book.Price.ToString("f2")
                    })
                    .ToList()
                })
                .ToList()
                .OrderByDescending(x => x.Books.Count)
                .ThenBy(x => x.AuthorName)
                .ToList();
    
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            return json;

        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var result = context
                .Books
                .Where(x => x.PublishedOn < date && x.Genre == Genre.Science)
                .OrderByDescending(x => x.Pages)
                .ThenByDescending(x => x.PublishedOn)
                .Select(x => new OldestBooksExportDto
                {
                    Pages = x.Pages,
                    Name = x.Name,
                    Date = x.PublishedOn.ToString("d", CultureInfo.InvariantCulture)
                })
                .Take(10)
                .ToList();

            var xml = XMLConverter.Serialize(result, "Books");

            return xml;
        }
    }
}