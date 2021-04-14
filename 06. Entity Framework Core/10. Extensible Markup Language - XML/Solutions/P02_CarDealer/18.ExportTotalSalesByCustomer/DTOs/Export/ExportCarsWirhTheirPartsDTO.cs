using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
   [XmlType("car")]
    public class ExportCarsWirhTheirPartsDTO
    {
        [XmlAttribute("make")]
        public string Make { get; set; }
        [XmlAttribute("model")]
        public string Model { get; set; }
        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }
        [XmlArray("parts")]
        public List<PartsDTO> Parts { get; set; }
    }
    [XmlType("part")]
    public class PartsDTO
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }
}
