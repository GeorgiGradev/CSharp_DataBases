namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using BookShop.XMLHelper;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            ImportBookDto[] booksToAdd = XMLConverter.Deserializer<ImportBookDto>(xmlString, "Books");

            List<Book> validBooks = new List<Book>();

            var sb = new StringBuilder();
            foreach (ImportBookDto book in booksToAdd)
            {
                if (!IsValid(book))
                {
                    sb.AppendLine(ErrorMessage);
                    continue; // ако не е валидно ще продължи и няма да се добави
                }

                DateTime publishedOn;
                bool isDateValid = DateTime.TryParseExact
                    (book.PublishedOn,
                    "MM/dd/yyyy", 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, 
                    out publishedOn);

                if (!isDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Book validBook =  new Book
                    {
                        Name = book.Name,
                        Genre = (Genre)book.Genre,
                        Price = book.Price,
                        Pages = book.Pages,
                        PublishedOn = publishedOn
                    };

                validBooks.Add(validBook);
                sb.AppendLine(String.Format(SuccessfullyImportedBook, book.Name, book.Price));
            }

            context.Books.AddRange(validBooks);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var authorDtos = JsonConvert.DeserializeObject<List<ImportAuthorDto>>(jsonString);

            var authorsToAdd = new List<Author>();

            foreach (var authorDto in authorDtos)
            {
                if (!IsValid(authorDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (authorsToAdd.Any(a => a.Email == authorDto.Email))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var author = new Author()
                {
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Email = authorDto.Email,
                    Phone = authorDto.Phone
                };

                foreach (var bookDto in authorDto.Books)
                {
                    if (!bookDto.Id.HasValue)
                    {
                        continue;
                    }

                    var book = context
                        .Books
                        .FirstOrDefault(b => b.Id == bookDto.Id);

                    if (book == null)
                    {
                        continue;
                    }

                    author.AuthorsBooks.Add(new AuthorBook()
                    {
                        Author = author,
                        Book = book
                    });
                }

                if (author.AuthorsBooks.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                authorsToAdd.Add(author);

                sb.AppendLine(string.Format(SuccessfullyImportedAuthor, author.FirstName + ' ' + author.LastName,
                    author.AuthorsBooks.Count));
            }

            context.Authors.AddRange(authorsToAdd);

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}