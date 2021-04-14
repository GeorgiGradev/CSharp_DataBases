using Stations.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stations.Models
{
    public class Trip
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(OriginStation))]
        public int OriginStationId { get; set; }

        [Required]
        public virtual Station OriginStation { get; set; }

        [ForeignKey(nameof(DestinationStation))]
        public int DestinationStationId { get; set; }

        [Required]
        public virtual Station DestinationStation { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        [ForeignKey(nameof(Train))]
        public int TrainId { get; set; }

        [Required]
        public virtual Train Train { get; set; }

        public TripStatus Status { get; set; }

        public TimeSpan? TimeDifference { get; set; }
    }
}
