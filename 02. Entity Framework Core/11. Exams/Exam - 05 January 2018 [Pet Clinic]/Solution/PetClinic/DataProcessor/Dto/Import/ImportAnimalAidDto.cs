using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetClinic.DataProcessor.Dto.Import
{
    public class ImportAnimalAidDto
    {
        [Required]
        [MaxLength(30)]
        [MinLength(3)]
        //UNIQUE
        public string Name { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }
    }
}
