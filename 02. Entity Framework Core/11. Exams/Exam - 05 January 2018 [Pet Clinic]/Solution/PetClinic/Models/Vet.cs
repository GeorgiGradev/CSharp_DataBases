using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetClinic.Models
{
    public class Vet
    {
        public Vet()
        {
            this.Procedures = new HashSet<Procedure>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string Profession { get; set; }

        [Range(22, 65)]
        public int Age { get; set; }

        [Required]
        [RegularExpression(@"^[+]359[0-9$]{9}$|^0[0-9]{9}$")]
        public string PhoneNumber { get; set; }

        public virtual ICollection<Procedure> Procedures { get; set; }
    }
}
