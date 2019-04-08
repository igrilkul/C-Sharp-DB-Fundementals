namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                .Where(m => m.Rating >= rating && m.Projections.Any(e => e.Tickets.Count >= 1))
                .Select(x => new ExportMovieDto
                {
                    MovieName = x.Title,
                    Rating = x.Rating.ToString("F2"),
                    TotalIncomes = x.Projections.Sum(q => q.Tickets.Sum(w => w.Price)).ToString("F2"),

                    Customers = x.Projections.SelectMany(q => q.Tickets.Select(w => new CustomerDto
                    {
                        FirstName = w.Customer.FirstName,
                        LastName = w.Customer.LastName,
                        Balance = w.Customer.Balance.ToString("F2"),
                    }))
                    .OrderByDescending(e=>e.Balance)
                    .ThenBy(r=>r.FirstName)
                    .ThenBy(t=>t.LastName)
                    .ToList()

                })
                .OrderByDescending(z=>double.Parse(z.Rating))
                .ThenByDescending(c=>decimal.Parse(c.TotalIncomes))
                .Take(10)
                .ToList();

            var json = JsonConvert.SerializeObject(movies, Newtonsoft.Json.Formatting.Indented);

            return json;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context.Customers
                .Where(c => c.Age >= age)
                .Select(x => new ExportCustomerDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SpentMoney = x.Tickets.Sum(s => s.Price).ToString("F2"),
                    SpentTime = new DateTime(x.Tickets.Sum(s => s.Projection.Movie.Duration.Ticks)).ToString("HH:mm:ss")
                })
                .OrderByDescending(t => decimal.Parse(t.SpentMoney))
                .Take(10)
                .ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportCustomerDto>), new XmlRootAttribute("Customers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }
    }

}