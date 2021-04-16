using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetClinic.Models
{
    public class Passport
    {
        [Key]
        [RegularExpression(@"^[A-Za-z]{7}[0-9]{3}$")]
        public string SerialNumber  { get; set; }

        [Required]
        public virtual Animal Animal { get; set; }

        [Required]
        [RegularExpression(@"^[+]359[0-9$]{9}$|^0[0-9]{9}$")]
        public string OwnerPhoneNumber { get; set; }

        [Required]
        [MaxLength(30)]
        [MinLength(3)]
        public string OwnerName { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
