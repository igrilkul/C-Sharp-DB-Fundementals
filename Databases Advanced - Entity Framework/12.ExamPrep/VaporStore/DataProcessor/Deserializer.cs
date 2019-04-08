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
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
            //var gamesDto = JsonConvert.DeserializeObject<List<ImportGameDto>>(jsonString)
            //    .Where(x=> String.IsNullOrEmpty(x.Name)==false
            //    && x.Price>=0 && x.Price<= decimal.MaxValue
            //    && String.IsNullOrEmpty(x.Developer)==false
            //    && String.IsNullOrEmpty(x.Genre)==false
            //    && x.Tags.Count>=1)
            //    .ToList();

            var gamesDto = JsonConvert.DeserializeObject<List<ImportGameDto>>(jsonString)
                .ToList();

            ;
            var sb = new StringBuilder();

            var gamesList = new List<Game>();

            foreach(var gameDto in gamesDto)
            {
                if (!IsValid(gameDto) || gameDto.Tags.Count == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var developer = GetDeveloper(context, gameDto.Developer);
                var genre = GetGenre(context, gameDto.Genre);


                var releaseDate = DateTime.ParseExact(gameDto.ReleaseDate, "yyyy-MM-dd",CultureInfo.InvariantCulture);

                var game = new Game()
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = releaseDate,
                    Genre = genre,
                    Developer = developer
                };

                foreach(var currentTag in gameDto.Tags)
                {
                  var tag = GetTag(context, currentTag);
                    game.GameTags.Add(new GameTag
                    {
                        Game = game,
                        Tag = tag
                    });
                }
                gamesList.Add(game);
                sb.AppendLine($"Added {gameDto.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
            }

            context.Games.AddRange(gamesList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        private static Tag GetTag(VaporStoreDbContext context, string tagDtoName)
        {
            var tag = context.Tags.FirstOrDefault(x => x.Name == tagDtoName);

            if (tag == null)
            {
                tag = new Tag()
                {
                    Name = tagDtoName
                };

                context.Tags.Add(tag);
                context.SaveChanges();
            }

            return tag;
        }

        private static Genre GetGenre(VaporStoreDbContext context, string genreDtoName)
        {
            var genre = context.Genres.FirstOrDefault(x => x.Name == genreDtoName);

            if (genre == null)
            {
                genre = new Genre()
                {
                    Name = genreDtoName
                };

                context.Genres.Add(genre);
                context.SaveChanges();
            }

            return genre;
        }

        private static Developer GetDeveloper(VaporStoreDbContext context, string developerName)
        {
            var dev = context.Developers.FirstOrDefault(x => x.Name == developerName);

            if(dev == null)
            {
                dev = new Developer()
                {
                    Name = developerName
                };

                context.Developers.Add(dev);
                context.SaveChanges();
            }

            return dev;
        }



        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
            var usersDto = JsonConvert.DeserializeObject<List<ImportUserDto>>(jsonString)
                .ToList();

            var sb = new StringBuilder();

            List<User> users = new List<User>();

            foreach(var userDto in usersDto)
            {
                if (!IsValid(userDto) || userDto.Cards.Count == 0 || !userDto.Cards.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                bool isValidEnum = true;

                foreach(var card in userDto.Cards)
                {
                    var cardType = Enum.TryParse<CardType>(card.Type, out CardType result);

                    if (!cardType)
                    {
                        sb.AppendLine("Invalid Data");
                        isValidEnum = false;
                        break;
                    }
                }

                if (!isValidEnum)
                {
                    continue;
                }

                var user = new User
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Age = userDto.Age,
                };

                foreach(var currentCard in userDto.Cards)
                {
                    var cardType = Enum.TryParse<CardType>(currentCard.Type, out CardType result);

                    var card = new Card
                    {
                        Number = currentCard.Number,
                        Cvc = currentCard.CVC,
                        Type = Enum.Parse<CardType>(currentCard.Type)
                    };

                    user.Cards.Add(card);
                }

                users.Add(user);
                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
            }

            context.Users.AddRange(users);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
		}



        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPurchaseDto[]), new XmlRootAttribute("Purchases"));

            var purchasesDto = (ImportPurchaseDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var purchases = new List<Purchase>();

            var sb = new StringBuilder();

            

            foreach (var purchaseDto in purchasesDto)
            {
                var game = context.Games.FirstOrDefault(g => g.Name == purchaseDto.Title);

                var card = context.Cards.FirstOrDefault(c => c.Number == purchaseDto.Card);

                var purchaseTypeFlag = Enum.TryParse<PurchaseType>(purchaseDto.Type, out PurchaseType purchaseType);
                
                if (!IsValid(purchaseDto) || game == null || card == null
                    || !purchaseTypeFlag)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var purchase = new Purchase
                {
                    Type = purchaseType,
                    ProductKey = purchaseDto.Key,
                    Date = DateTime.ParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Card = card,
                    Game = game
                };

                purchases.Add(purchase);
                sb.AppendLine($"Imported {purchase.Game.Name} for {purchase.Card.User.Username}");
            }

            context.Purchases.AddRange(purchases);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            bool isValid =Validator.TryValidateObject(entity, validationContext,
                validationResult, true);

            return isValid;
        }
	}
}