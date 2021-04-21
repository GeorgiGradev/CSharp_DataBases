using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ImportDtos
{
    [XmlType("Performer")]
    public class ImportPerformerDto
    {
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string LastName { get; set; }

        [Range(18, 70)]
        public int Age { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal NetWorth { get; set; }

        [XmlArray("PerformersSongs")]
        public List<ImportPerformerSongDto> PerformersSongs { get; set; }
    }
}
