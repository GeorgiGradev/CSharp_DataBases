using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stations.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Range(typeof(decimal),"0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(8)]
        [RegularExpression(@"^[A-Z]{2}\d{1,6}$")]
        public string SeatingPlace { get; set; }

        [ForeignKey(nameof(Trip))]
        public int TripId { get; set; }

        [Required]
        public virtual Trip Trip { get; set; }

        public int? CustomerCardId { get; set; }

        public virtual CustomerCard CustomerCard { get; set; }
    }
}
