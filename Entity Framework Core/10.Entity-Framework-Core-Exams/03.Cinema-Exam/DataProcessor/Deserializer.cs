namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

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
            var movieDtos = JsonConvert.DeserializeObject<ImportMovieDto[]>(jsonString);

            var validMovies = new List<Movie>();
            var result = new StringBuilder();

            foreach (var movieDto in movieDtos)
            {
                bool isValidGenre = Enum.IsDefined(typeof(Genre), movieDto.Genre);
                bool isMovieExist = validMovies.Any(m => m.Title == movieDto.Title);

                if (isValidGenre == false || isMovieExist == true || IsValid(movieDto) == false)
                {
                    result.AppendLine(ErrorMessage);
                    //continue;
                }
                else
                {
                    var movie = Mapper.Map<Movie>(movieDto);

                    validMovies.Add(movie);

                    result.AppendLine(string.Format(SuccessfulImportMovie,
                                      movie.Title, movie.Genre.ToString(), $"{movie.Rating:f2}"));
                }
            }

            context.Movies.AddRange(validMovies);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var hallDtos = JsonConvert.DeserializeObject<ImportHallWithSeatsDto[]>(jsonString);

            var validHalls = new List<Hall>();
            var result = new StringBuilder();

            foreach (var hallDto in hallDtos)
            {
                if (IsValid(hallDto) == false)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    var hall = Mapper.Map<Hall>(hallDto);

                    for (int i = 0; i < hallDto.SeatsCount; i++)
                    {
                        hall.Seats.Add(new Seat());
                    }

                    string projectionType = string.Empty;

                    if (hall.Is3D == true && hall.Is4Dx == true)
                    {
                        projectionType = "4Dx/3D";
                    }
                    else if (hall.Is3D == true)
                    {
                        projectionType = "3D";
                    }
                    else if (hall.Is4Dx == true)
                    {
                        projectionType = "4Dx";
                    }
                    else
                    {
                        projectionType = "Normal";
                    }

                    validHalls.Add(hall);

                    result.AppendLine(string.Format(SuccessfulImportHallSeat,
                                      hall.Name, projectionType, hall.Seats.Count));
                }
            }

            context.Halls.AddRange(validHalls);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var xmlSerialiver = new XmlSerializer(typeof(ImportProjectionDto[]),
                new XmlRootAttribute("Projections"));

            var projectionDtos = (ImportProjectionDto[])xmlSerialiver.Deserialize(new StringReader(xmlString));

            var validProjections = new List<Projection>();
            var result = new StringBuilder();

            foreach (var projectionDto in projectionDtos)
            {
                var movie = context.Movies.Find(projectionDto.MovieId);
                var hall = context.Halls.Find(projectionDto.HallId);

                if (movie == null || hall == null || IsValid(projectionDto) == false)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    var projection = Mapper.Map<Projection>(projectionDto);

                    projection.Movie = movie;
                    projection.Hall = hall;

                    validProjections.Add(projection);

                    result.AppendLine(string.Format(SuccessfulImportProjection,
                                      projection.Movie.Title, projection.DateTime.ToString(@"MM/dd/yyyy")));
                }
            }

            context.Projections.AddRange(validProjections);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]),
                new XmlRootAttribute("Customers"));

            var customerDtos = (ImportCustomerDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validCustomers = new List<Customer>();
            var result = new StringBuilder();

            foreach (var customerDto in customerDtos)
            {
                //var projectionsIds = context.Projections.Select(p => p.Id).ToList();
                //bool isProjectionsExist = customerDto.TicketsForProjection
                //    .Select(t => t.ProjectionId)
                //    .All(v => projectionsIds.Contains(v));

               // bool hasInvalidTickets = customerDto.TicketsForProjection.Any(t => IsValid(t) == false);

                if (IsValid(customerDto) == false)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    var customer = Mapper.Map<Customer>(customerDto);

                    foreach (var ticketDto in customerDto.TicketsForProjection)
                    {
                        //if (IsValid(ticketDto) == false)
                        //{
                        //    result.AppendLine(ErrorMessage);
                        //}
                        //else
                        //{
                            var ticket = Mapper.Map<Ticket>(ticketDto);

                            customer.Tickets.Add(ticket);
                        //}
                    }
                    validCustomers.Add(customer);

                    result.AppendLine(string.Format(SuccessfulImportCustomerTicket,
                                      customer.FirstName, customer.LastName, customer.Tickets.Count));
                }
            }

            context.Customers.AddRange(validCustomers);
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