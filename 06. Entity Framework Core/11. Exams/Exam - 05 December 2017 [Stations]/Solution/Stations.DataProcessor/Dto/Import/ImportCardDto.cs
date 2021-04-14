using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Stations.DataProcessor.Dto.Import
{
    [XmlType("Card")]
    public class ImportCardDto
    {
        [XmlElement("Name")]
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [XmlElement("Age")]
        [Range(0, 120)]
        public int? Age { get; set; }

        [XmlElement("CardType")]
        public string Type { get; set; } = "Normal";
    }
}
