namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enum;
    using VaporStore.DataProcessor.ImportDtos;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public static class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";
        private const string ImportedGame = "Added {0} ({1}) with {2} tags";
        private const string ImportedUser = "Imported {0} with {1} cards";
        private const string ImportedPurchase = "Imported {0} for {1}";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var gameDtos = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);

            var validGames = new List<Game>();

            var developers = new List<Developer>();
            var genres = new List<Genre>();
            var tags = new List<Tag>();

            var result = new StringBuilder();

            foreach (var gameDto in gameDtos)
            {
                if (IsValid(gameDto) == false)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    Developer developer = developers.FirstOrDefault(d => d.Name == gameDto.Developer);

                    if (developer == null)
                    {
                        developer = new Developer
                        {
                            Name = gameDto.Developer
                        };

                        developers.Add(developer);
                    }

                    Genre genre = genres.FirstOrDefault(g => g.Name == gameDto.Genre);

                    if (genre == null)
                    {
                        genre = new Genre
                        {
                            Name = gameDto.Genre
                        };

                        genres.Add(genre);
                    }

                    List<Tag> gameTags = new List<Tag>();

                    foreach (var tagName in gameDto.Tags)
                    {
                        Tag tag = tags.FirstOrDefault(t => t.Name == tagName);

                        if (tag == null)
                        {
                            tag = new Tag
                            {
                                Name = tagName
                            };

                            tags.Add(tag);
                        }

                        gameTags.Add(tag);
                    }

                    Game game = new Game
                    {
                        Name = gameDto.Name,
                        Price = gameDto.Price,
                        ReleaseDate = DateTime.ParseExact(gameDto.ReleaseDate,
                                                          @"yyyy-MM-dd",
                                                          CultureInfo.InvariantCulture),
                        Developer = developer,
                        Genre = genre,
                        GameTags = gameTags.Select(gt => new GameTag
                                                        {
                                                         Tag = gt
                                                        }).ToList()
                    };

                    validGames.Add(game);

                    result.AppendLine(string.Format(ImportedGame,
                                      game.Name,
                                      game.Genre.Name,
                                      game.GameTags.Count));
                }
            }

            context.Games.AddRange(validGames);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(jsonString);

            var validUsers = new List<User>();
            var result = new StringBuilder();

            foreach (var userDto in userDtos)
            {
                var cardDtos = userDto.Cards.Select(c => Mapper.Map<Card>(c)).ToList();
                var hasInvalidCard = cardDtos.Any(c => IsValid(c) == false);

                if (IsValid(userDto) == false || hasInvalidCard == true)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    var user = Mapper.Map<User>(userDto);

                    user.Cards = cardDtos;

                    validUsers.Add(user);

                    result.AppendLine(string.Format(ImportedUser,
                                      user.Username,
                                      user.Cards.Count));
                }
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportPurchaseDto[]),
                new XmlRootAttribute("Purchases"));

            var purchaseDtos = (ImportPurchaseDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validPuchases = new List<Purchase>();
            var result = new StringBuilder();

            foreach (var purchaseDto in purchaseDtos)
            {
                bool isValidType = Enum.IsDefined(typeof(PurchaseType), purchaseDto.Type);
                var gameExist = context.Games.FirstOrDefault(g => g.Name == purchaseDto.Title);
                var cardExist = context.Cards.FirstOrDefault(c => c.Number == purchaseDto.CardNumber);

                if (IsValid(purchaseDto) == false
                    || isValidType == false
                    || gameExist == null
                    || cardExist == null)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    var purchase = new Purchase
                    {
                        Game = gameExist,
                        Type = (PurchaseType)Enum.Parse(typeof(PurchaseType), purchaseDto.Type),
                        ProductKey = purchaseDto.ProductKey,
                        Card = cardExist,
                        Date = DateTime.ParseExact(purchaseDto.Date, @"dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                    };

                    validPuchases.Add(purchase);

                    result.AppendLine(string.Format(ImportedPurchase,
                                      purchase.Game.Name,
                                      purchase.Card.User.Username));
                }
            }

            context.Purchases.AddRange(validPuchases);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            return result;
        }
    }
}