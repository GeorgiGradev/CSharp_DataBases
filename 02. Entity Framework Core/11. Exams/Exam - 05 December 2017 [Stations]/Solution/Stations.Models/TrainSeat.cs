using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stations.Models
{
    public class TrainSeat
    {
        [Key]
        public int Id { get; set; }

        public int TrainId { get; set; }

        [Required]
        public virtual Train Train { get; set; }

        public int SeatingClassId { get; set; }

        [Required]
        public virtual SeatingClass SeatingClass { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
