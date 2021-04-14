using Newtonsoft.Json;
using Stations.Data;
using Stations.DataProcessor.Dto.Import;
using Stations.Models;
using Stations.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Stations.DataProcessor
{
    public static class Deserializer
    {
        private const string ErrorMessage = "Invalid data format.";

        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportStations(StationsDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var importStationDtos = JsonConvert.DeserializeObject<List<ImportStationDto>>(jsonString);
            var stationsToAdd = new List<Station>();

            foreach (var importStationDto in importStationDtos)
            {
                if (!IsValid(importStationDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (stationsToAdd.Any(x => x.Name == importStationDto.Name))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (importStationDto.Town == null)
                {
                    importStationDto.Town = importStationDto.Name;
                }

                var station = new Station()
                {
                    Name = importStationDto.Name,
                    Town = importStationDto.Town
                };

                stationsToAdd.Add(station);
                sb.AppendLine(string.Format(SuccessMessage, importStationDto.Name));
            }

            context.Stations.AddRange(stationsToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportClasses(StationsDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var importClassDtos = JsonConvert.DeserializeObject<List<ImportClassDto>>(jsonString);
            var classesToAdd = new List<SeatingClass>();

            foreach (var importClassDto in importClassDtos)
            {
                if (!IsValid(importClassDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (classesToAdd.Any(x => x.Name == importClassDto.Name)
                    || classesToAdd.Any(x => x.Abbreviation == importClassDto.Abbreviation))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                var @class = new SeatingClass()
                {
                    Name = importClassDto.Name,
                    Abbreviation = importClassDto.Abbreviation
                };
                classesToAdd.Add(@class);
                sb.AppendLine(string.Format(SuccessMessage, importClassDto.Name));
            }

            context.SeatingClasses.AddRange(classesToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTrains(StationsDbContext context, string jsonString)
        {
            var deserializedTrains = JsonConvert.DeserializeObject<List<ImportTrainDto>>(jsonString, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var sb = new StringBuilder();
            int counter = 0;
            var validTrains = new List<Train>();

            foreach (var trainDto in deserializedTrains)
            {
                if (!IsValid(trainDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var areSeatsValid = trainDto.Seats.All(IsValid);
                if (!areSeatsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var trainAlreadyExists = validTrains.Any(x => x.TrainNumber == trainDto.TrainNumber);

                if (trainAlreadyExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var areSeatingClassesValid = trainDto.Seats
                    .All(s => context.SeatingClasses
                        .Any(sc => sc.Name == s.Name && sc.Abbreviation == s.Abbreviation));
                if (!areSeatingClassesValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var type = Enum.Parse<TrainType>(trainDto.Type);

                var trainSeats = trainDto.Seats.Select(s => new TrainSeat
                {
                    SeatingClass = context.SeatingClasses
                        .SingleOrDefault(sc => sc.Name == s.Name
                            && sc.Abbreviation == s.Abbreviation),
                    Quantity = s.Quantity.Value
                })
                    .ToList();

                var train = new Train()
                {
                    TrainNumber = trainDto.TrainNumber,
                    Type = type,
                    TrainSeats = trainSeats
                };

                validTrains.Add(train);
                sb.AppendLine(string.Format(SuccessMessage, train.TrainNumber));
            }

            context.AddRange(validTrains);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTrips(StationsDbContext context, string jsonString)
        {
            var importTripDtos = JsonConvert.DeserializeObject<List<ImportTripDto>>(jsonString);

            var sb = new StringBuilder();

            var tripsToAdd = new List<Trip>();

            foreach (var importTripDto in importTripDtos)
            {
                if (!IsValid(importTripDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var originStation = context.Stations.FirstOrDefault(x => x.Name == importTripDto.OriginStation);
                var destinationStation = context.Stations.FirstOrDefault(x => x.Name == importTripDto.DestinationStation);
                var train = context.Trains.FirstOrDefault(x => x.TrainNumber == importTripDto.Train);

                if (originStation == null
                    || originStation == null
                    || destinationStation == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime arrivalTIme;
                var isArivalTimeValid = DateTime.TryParseExact(importTripDto.ArrivalTime
                , "dd/MM/yyyy HH:mm"
                , CultureInfo.InvariantCulture
                , DateTimeStyles.None
                , out arrivalTIme);

                DateTime departureTime;
                var isDepartureTime = DateTime.TryParseExact(importTripDto.DepartureTime
                , "dd/MM/yyyy HH:mm"
                , CultureInfo.InvariantCulture
                , DateTimeStyles.None
                , out departureTime);

                if (isArivalTimeValid == false || isDepartureTime == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (departureTime >= arrivalTIme)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                TimeSpan? timeDifference = null;
                if (importTripDto.TimeDifference != null)
                {
                    timeDifference = TimeSpan.ParseExact(importTripDto.TimeDifference, @"hh\:mm", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                }
                var type = Enum.Parse<TripStatus>(importTripDto.Status);

                var trip = new Trip()
                {
                    Train = train,
                    OriginStation = originStation,
                    DestinationStation = destinationStation,
                    ArrivalTime = arrivalTIme,
                    DepartureTime = departureTime,
                    Status = type,
                    TimeDifference = timeDifference
                };

                tripsToAdd.Add(trip);
                sb.AppendLine($"Trip from {trip.OriginStation.Name} to {trip.DestinationStation.Name} imported.");
            }

            context.Trips.AddRange(tripsToAdd);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportCards(StationsDbContext context, string xmlString)
        {
            var cardInputDtos = XMLConverter.Deserializer<ImportCardDto>(xmlString, "Cards");

            var sb = new StringBuilder();
            var cardsToAdd = new List<CustomerCard>();

            foreach (var cardInputDto in cardInputDtos)
            {
                if (!IsValid(cardInputDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                var type = Enum.Parse<CardType>(cardInputDto.Type);

                var card = new CustomerCard()
                {
                    Name = cardInputDto.Name,
                    Age = cardInputDto.Age,
                    Type = type
                };

                cardsToAdd.Add(card);
                sb.AppendLine(string.Format(SuccessMessage, card.Name));
            }
            context.AddRange(cardsToAdd);
            context.SaveChanges();


            return sb.ToString().TrimEnd();
        }

        public static string ImportTickets(StationsDbContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var importTicketDtos = XMLConverter.Deserializer<ImportTicketDto>(xmlString, "Tickets");

            var ticketsToAdd = new List<Ticket>();


            foreach (var ticketDto in importTicketDtos)
            {
                if (!IsValid(ticketDto))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                var departureDate = DateTime.ParseExact(ticketDto.Trip.DepartureTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);

                var trip = context.Trips.FirstOrDefault(t => t.OriginStation.Name == ticketDto.Trip.OriginStation && t.DepartureTime == departureDate && t.DestinationStation.Name == ticketDto.Trip.DestinationStation);

                if (trip == null)
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                CustomerCard card = null;

                if (ticketDto.Card != null)
                {
                    card = context.CustomerCards
                        .FirstOrDefault(c => c.Name == ticketDto.Card.Name);

                    if (card == null)
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }
                }

                var seatinClassAbbreviation = ticketDto.Seat
                    .Substring(0, 2);

                var quantity = int.Parse(ticketDto.Seat.Substring(2));

                var seat = trip.Train
                    .TrainSeats
                    .FirstOrDefault(ts => ts.SeatingClass.Abbreviation == seatinClassAbbreviation &&
                                          quantity <= ts.Quantity);

                if (seat == null)
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                var ticket = new Ticket()
                {
                    Trip = trip,
                    CustomerCard = card,
                    Price = ticketDto.Price,
                    SeatingPlace = ticketDto.Seat
                };

                ticketsToAdd.Add(ticket);

                sb.AppendLine($"Ticket from {trip.OriginStation.Name} to {trip.DestinationStation.Name} departing at {departureDate.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} imported.");
            }

            context.Tickets.AddRange(ticketsToAdd);

            context.SaveChanges();

            return sb.ToString().Trim();

        }


        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}