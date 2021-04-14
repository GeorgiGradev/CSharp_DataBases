using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ImportDto
{
    [XmlType("Purchase")]
    public class ImportPurchaseDto
    {
        [XmlAttribute("title")]
        public string GameName { get; set; }

        [XmlElement("Type")]
        [Required]
        public string Type { get; set; }

        [XmlElement("Key")]
        [Required]
        [RegularExpression(@"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$")]
        public string ProductKey { get; set; }

        [XmlElement("Card")]
        [Required]
        [RegularExpression(@"^[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}$")]
        public string CardNumber { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }
    }
}
