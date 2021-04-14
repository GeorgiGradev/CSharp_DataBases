namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.ImportDto;
    using VaporStore.XMLHelper;

    public static class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var gameDtos = JsonConvert.DeserializeObject<List<GameImportDto>>(jsonString);

            var gamesToAdd = new List<Game>();


            foreach (var gameDto in gameDtos)
            {
    

                if (!IsValid(gameDto) || gameDto.Tags.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var game = new Game()
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = DateTime.ParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                };

                var developer = context
                    .Developers
                    .Where(x => x.Name == gameDto.Developer)
                    .FirstOrDefault();

                if (developer == null)
                {
                    developer = new Developer()
                    {
                        Name = gameDto.Developer
                    };
                    context.Developers.Add(developer);
                    context.SaveChanges();
                }


                var genre = context
                    .Genres
                    .Where(x => x.Name == gameDto.Genre)
                    .FirstOrDefault();
          
                if (genre == null)
                {
                    genre = new Genre()
                    {
                        Name = gameDto.Genre
                    };
                    context.Genres.Add(genre);
                    context.SaveChanges();
                }

                game.Developer = developer;
                game.Genre = genre;


                var tag = new Tag();
                foreach (var tagToAdd in gameDto.Tags)
                {
                    if (!context.Tags.Where(x => x.Name == tagToAdd).Any())
                    {
                        tag = new Tag()
                        {
                            Name = tagToAdd
                        };
                        context.Tags.Add(tag);
                        context.SaveChanges();

                        game.GameTags.Add(new GameTag
                        {
                            Game = game,
                            Tag = tag
                        });
                    }
                }

                gamesToAdd.Add(game);
                sb.AppendLine($"Added {game.Name} ({genre.Name}) with {gameDto.Tags.Count} tags");

            }

            context.Games.AddRange(gamesToAdd);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var userDtos = JsonConvert.DeserializeObject<List<ImportUserDto>>(jsonString);
            var usersToAdd = new List<User>();

            foreach (var userDto in userDtos)
            {
                if (!IsValid(userDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var user = new User()
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Age = userDto.Age
                };

                foreach (var cardDto in userDto.Cards)
                {
                    if (!IsValid(cardDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    CardType cardType;
                    var isCardTypevalid = Enum.TryParse(cardDto.Type, out cardType);

                    if (!isCardTypevalid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var card = new Card()
                    {
                        Number = cardDto.Number,
                        Cvc = cardDto.Cvc,
                        Type = cardType
                    };
                    user.Cards.Add(card);
                }
                usersToAdd.Add(user);
                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
            }
            context.AddRange(usersToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var purchaseDtos = XMLConverter.Deserializer<ImportPurchaseDto>(xmlString, "Purchases");
            var purchasesToAdd = new List<Purchase>();

            foreach (var purchaseDto in purchaseDtos)
            {
                if (!IsValid(purchaseDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                PurchaseType purchaseType;
                var isValidPurchaseType = Enum.TryParse(purchaseDto.Type, out purchaseType);

                if (!isValidPurchaseType)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var purchase = new Purchase()
                { 
                    GameId = context
                        .Games
                        .Where(x=>x.Name == purchaseDto.GameName)
                        .Select(x=>x.Id)
                        .FirstOrDefault(),
                    Type = purchaseType,
                    ProductKey = purchaseDto.ProductKey,
                        CardId = context
                        .Cards
                        .Where(x=>x.Number == purchaseDto.CardNumber)
                        .Select(x=>x.Id)
                        .FirstOrDefault(),
                    Date = DateTime.ParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                };
                purchasesToAdd.Add(purchase);
                var userName = context
                    .Cards
                    .Where(x => x.Number == purchaseDto.CardNumber)
                    .Select(x => x.User.Username)
                    .FirstOrDefault();

                sb.AppendLine($"Imported {purchaseDto.GameName} for {userName}");
            }

            context.Purchases.AddRange(purchasesToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}