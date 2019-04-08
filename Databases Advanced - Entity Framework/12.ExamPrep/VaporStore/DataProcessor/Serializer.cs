namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
            var games = context.Genres
                .Where(g => genreNames.Contains(g.Name))
                .Select(x => new ExportGenreDto
                {
                    Id = x.Id,
                    Genre = x.Name,
                    Games = x.Games.Where(t => t.Purchases.Count >= 1)
                    .Select(y => new ExportGameDto
                    {
                        Id = y.Id,
                        Title = y.Name,
                        Developer = y.Developer.Name,
                        Tags = string.Join(", ", y.GameTags.Select(u => u.Tag.Name).ToList()),
                        Players = y.Purchases.Count
                    }).OrderByDescending(q => q.Players).ThenBy(w => w.Id)
                    .ToList(),
                    TotalPlayers = x.Games.Where(t => t.Purchases.Count >= 1).Sum(b => b.Purchases.Count)
                })
                .OrderByDescending(v => v.TotalPlayers).ThenBy(o => o.Id)
                .ToList();

            var json = JsonConvert.SerializeObject(games, Newtonsoft.Json.Formatting.Indented);

            return json;
        }

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
            var purchaseType = Enum.Parse<PurchaseType>(storeType);

            var users = context.Users
                .Where(u => u.Cards.SelectMany(e => e.Purchases).Any(t=>t.Type==purchaseType))
                .Select(x => new ExportUserDto
                {
                    Username = x.Username,
                    Purchases = x.Cards.SelectMany(v => v.Purchases).Where(l=>l.Type==purchaseType)
                    .Select(g => new ExportPurchaseDto
                    {
                        Card = g.Card.Number,
                        Cvc = g.Card.Cvc,
                        Date = g.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = new GameDto
                        {
                            GameTitle = g.Game.Name,
                            Genre = g.Game.Genre.Name,
                            Price = g.Game.Price
                        }
                    })
                    .OrderBy(t=>t.Date)
                    .ToList(),
                    TotalSpent = x.Cards.SelectMany(p => p.Purchases).Where(l => l.Type == purchaseType).Sum(p => p.Game.Price)
                })
                .OrderByDescending(e=>e.TotalSpent)
                .ThenBy(u=>u.Username)
                .ToList();

            

            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportUserDto>), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();
        }
	}
}