using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stations.Models
{
    public class Station
    {
        public Station()
        {
            this.TripsTo = new HashSet<Trip>();
            this.TripsFrom = new HashSet<Trip>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Town { get; set; }

        public virtual ICollection<Trip> TripsTo { get; set; }

        public virtual ICollection<Trip> TripsFrom { get; set; }
    }
}
