using Stations.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Stations.DataProcessor.Dto.Export
{
    [XmlType("Card")]
    public class ExportCardDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlArray("Tickets")]
        public List<ExportTicketDto> Tickets { get; set; }
    }
}
