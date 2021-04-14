using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{ 
    [XmlType("Car")]
    public class ImportCarDTO
    {
        [XmlElement("make")]
        public string Make { get; set; }
        [XmlElement("model")]
        public string Model { get; set; }
        [XmlElement("TraveledDistance")]
        public long TravelledDistance { get; set; }
        [XmlArray("parts")]
        public List<PartDTO> Parts { get; set; }
    }

    [XmlType("partId")]
    public class PartDTO
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}