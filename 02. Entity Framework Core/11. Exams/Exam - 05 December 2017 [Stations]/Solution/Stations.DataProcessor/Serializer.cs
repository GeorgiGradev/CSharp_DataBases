using Newtonsoft.Json;
using Stations.Data;
using Stations.DataProcessor.Dto.Export;
using Stations.Models.Enums;
using System;
using System.Globalization;
using System.Linq;

namespace Stations.DataProcessor
{
    public class Serializer
    {
        public static string ExportDelayedTrains(StationsDbContext context, string dateAsString)
        {
            var trains = context
           .Trains
           .Where(t => t.Trips.Any(tr => tr.Status == TripStatus.Delayed
                              && tr.DepartureTime <= DateTime.ParseExact(dateAsString, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
           .Select(t => new
           {
               TrainNumber = t.TrainNumber,
               DelayedTimes = t.Trips.Count(tr =>
                   tr.Status == TripStatus.Delayed && tr.DepartureTime <=
                   DateTime.ParseExact(dateAsString, "dd/MM/yyyy", CultureInfo.InvariantCulture)),
               MaxDelayedTime = t.Trips.Max(tr => tr.TimeDifference)
           })
           .OrderByDescending(t => t.DelayedTimes)
           .ThenByDescending(t => t.MaxDelayedTime)
           .ThenBy(t => t.TrainNumber)
           .ToList();

            var json = JsonConvert.SerializeObject(trains, Formatting.Indented);

            return json;
        }

        public static string ExportCardsTicket(StationsDbContext context, string cardType)
        {
            var type = Enum.Parse<CardType>(cardType);

            var result = context
            .CustomerCards
            .Where(cc => cc.Type.ToString() == cardType &&
                         cc.BoughtTickets.Count >= 1)
            .Select(cc => new ExportCardDto()
            {
                Name = cc.Name,
                Type = cc.Type.ToString(),
                Tickets = cc.BoughtTickets.Select(bt => new ExportTicketDto()
                {
                    DepartureTime = bt.Trip
                        .DepartureTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    DestinationStation = bt.Trip.DestinationStation.Name,
                    OriginStation = bt.Trip.OriginStation.Name
                })
                .ToList()
            })
            .OrderBy(cc => cc.Name)
            .ToList();

            var xml = XMLConverter.Serialize(result, "Cards");

            return xml;
        }
    }
}