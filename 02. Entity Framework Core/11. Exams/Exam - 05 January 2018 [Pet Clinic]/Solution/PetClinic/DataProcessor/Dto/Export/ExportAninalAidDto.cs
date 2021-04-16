using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Export
{
    [XmlType("AnimalAid")]
    public class ExportAninalAidDto
    {
        [XmlElement("Name")]
        public string AidName { get; set; }

        [XmlElement("Price")]
        public decimal AidPrice { get; set; }
    }
}
