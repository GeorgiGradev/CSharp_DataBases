using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Import
{
    [XmlType("Order")]
    public class ImportOrderDto
    {
        [XmlElement("Customer")]
        [Required]
        public string Customer { get; set; }

        [XmlElement("Employee")]
        [Required]
        public string Employee { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; } = "ForHere";

        [XmlArray("Items")]
        public List<ImportOrderItemDro> Items { get; set; }
    }
}
