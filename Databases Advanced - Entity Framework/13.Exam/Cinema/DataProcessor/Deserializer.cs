namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat 
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var moviesDto = JsonConvert.DeserializeObject<List<ImportMovieDto>>(jsonString)
                .ToList();

            var moviesList = new List<Movie>();
            var sb = new StringBuilder();

            foreach(var movieDto in moviesDto)
            {
                if (!IsValid(movieDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var IsGenreCorrect = Enum.TryParse<Genre>(movieDto.Genre, out Genre genre);
                if (!IsGenreCorrect)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var movie = new Movie
                {
                    Title = movieDto.Title,
                    Genre = genre,
                    Duration = movieDto.Duration,
                    Rating = movieDto.Rating,
                    Director = movieDto.Director
                };

                moviesList.Add(movie);
                sb.AppendLine($"Successfully imported {movie.Title} with genre {movie.Genre} and rating {movie.Rating:F2}!");
            }

            context.Movies.AddRange(moviesList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var hallsDto = JsonConvert.DeserializeObject<List<ImportHallDto>>(jsonString)
                .ToList();

            var hallsList = new List<Hall>();
            var sb = new StringBuilder(); 

            foreach(var hallDto in hallsDto)
            {
                if (!IsValid(hallDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hall = new Hall
                {
                    Name = hallDto.Name,
                    Is4Dx = hallDto.Is4Dx,
                    Is3D = hallDto.Is3D
                };

                var seats = new List<Seat>();
                for(int i = 0; i < hallDto.Seats; i++)
                {
                    var seat = new Seat
                    {
                        Hall = hall
                    };
                    seats.Add(seat);
                }

                hall.Seats = seats;
                hallsList.Add(hall);
                string projectionType = "";
                
                if((hall.Is3D) && (hall.Is4Dx))
                {
                    projectionType = "4Dx/3D";
                }
                else if((!hall.Is3D) && (hall.Is4Dx))
                {
                    projectionType = "4Dx";
                }
                else if((hall.Is3D) && (!hall.Is4Dx))
                {
                    projectionType = "3D";
                }
                else
                {
                    projectionType = "Normal";
                }

                sb.AppendLine($"Successfully imported {hall.Name}({projectionType}) with {hall.Seats.Count} seats!");
            }

            context.Halls.AddRange(hallsList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProjectionDto[]), new XmlRootAttribute("Projections"));

            var projectionsDto = (ImportProjectionDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var projectionsList = new List<Projection>();

            var sb = new StringBuilder();

            foreach(var projectionDto in projectionsDto)
            {
                var movie = context.Movies.Find(projectionDto.MovieId);

                var hall = context.Halls.Find(projectionDto.HallId);

                if (!IsValid(projectionDto) || movie == null || hall == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

               

                var projection = new Projection
                {
                    MovieId = projectionDto.MovieId,
                    HallId = projectionDto.HallId,
                    DateTime = DateTime.ParseExact(projectionDto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                };

                projectionsList.Add(projection);
                sb.AppendLine($"Successfully imported projection {movie.Title} on {projection.DateTime.ToString("MM/dd/yyyy",CultureInfo.InvariantCulture)}!");
            }

            context.Projections.AddRange(projectionsList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));

            var customersDto = (ImportCustomerDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var customersList = new List<Customer>();

            var sb = new StringBuilder();
            var projectionsCount = context.Projections.Count();

            foreach(var customerDto in customersDto)
            {

                if(!IsValid(customerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool ticketFlag = true;

                foreach(var ticket in customerDto.Tickets)
                {
                    var ticketProjection = context.Projections.Find(ticket.ProjectionId);

                    if(ticketProjection == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        ticketFlag = false;
                        break;
                    }
                }

                if (!ticketFlag)
                {
                    continue;
                }

                var customer = new Customer
                {
                    FirstName = customerDto.FirstName,
                    LastName = customerDto.LastName,
                    Age = customerDto.Age,
                    Balance = customerDto.Balance
                };

                foreach(var currentTicket in customerDto.Tickets)
                {
                    var ticket = new Ticket
                    {
                        ProjectionId = currentTicket.ProjectionId,
                        Price = currentTicket.Price,
                        Customer = customer
                    };

                    customer.Tickets.Add(ticket);
                }

                customersList.Add(customer);
                sb.AppendLine($"Successfully imported customer {customer.FirstName} {customer.LastName} with bought tickets: {customer.Tickets.Count}!");
            }

            context.Customers.AddRange(customersList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext,
                validationResult, true);

            return isValid;
        }
    }
}