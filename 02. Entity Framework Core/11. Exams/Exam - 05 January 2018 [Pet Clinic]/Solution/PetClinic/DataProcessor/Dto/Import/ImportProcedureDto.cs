using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Import
{
    [XmlType("Procedure")]
    public class ImportProcedureDto
    {
        [XmlElement("Vet")]
        [Required]
        [MaxLength(40)]
        [MinLength(3)]
        public string VetName { get; set; }

        [XmlElement("Animal")]
        [Required]
        [RegularExpression(@"^[A-Za-z]{7}[0-9]{3}$")]
        public string AnimalSerialNumber { get; set; }

        [XmlElement("DateTime")]
        [Required]
        public string DateTime { get; set; }

        [Required]
        [XmlArray("AnimalAids")]
        public List<ImportProceduresAnimalDto> AnimalAids { get; set; }
    }
}
