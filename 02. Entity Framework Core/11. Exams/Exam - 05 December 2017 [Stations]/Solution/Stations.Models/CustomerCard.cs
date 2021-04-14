using Stations.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stations.Models
{
    public class CustomerCard
    {
        public CustomerCard()
        {
            this.BoughtTickets = new HashSet<Ticket>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Range(0,120)]
        public int? Age { get; set; }

        public CardType Type { get; set; } 

        public virtual ICollection<Ticket> BoughtTickets { get; set; }
    }
}
