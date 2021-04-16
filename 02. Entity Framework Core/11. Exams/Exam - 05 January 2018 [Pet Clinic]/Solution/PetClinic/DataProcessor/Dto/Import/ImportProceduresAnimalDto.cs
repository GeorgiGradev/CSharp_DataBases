using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Import
{
    [XmlType("AnimalAid")]
    public class ImportProceduresAnimalDto
    {
        [Required]
        [MaxLength(30)]
        [MinLength(3)]
        //UNIQUE
        public string Name { get; set; }
    }
}
