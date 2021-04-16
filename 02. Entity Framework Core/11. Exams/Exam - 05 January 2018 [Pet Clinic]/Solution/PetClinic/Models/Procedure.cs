using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace PetClinic.Models
{
    public class Procedure
    {
        public Procedure()
        {
            this.ProcedureAnimalAids = new HashSet<ProcedureAnimalAid>();
        }

        [Key]
        public int Id { get; set; }

        public int AnimalId { get; set; }

        [Required]
        public Animal Animal { get; set; }

        public int VetId { get; set; }

        [Required]
        public Vet Vet { get; set; }

        public ICollection<ProcedureAnimalAid>ProcedureAnimalAids { get; set; }

        [NotMapped]
        public decimal Cost => this.ProcedureAnimalAids.Sum(paa => paa.AnimalAid.Price); // calculated by summing the price of the different services performed

        public DateTime DateTime { get; set; }
    }
}
