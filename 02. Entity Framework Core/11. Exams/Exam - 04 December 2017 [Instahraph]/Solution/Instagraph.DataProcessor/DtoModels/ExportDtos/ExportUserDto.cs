using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Instagraph.DataProcessor.DtoModels.ExportDtos
{
    [XmlType("user")]
    public class ExportUserDto
    {
        [XmlElement("Username")]
        public string Username { get; set; }

        [XmlElement("MostComments")]
        public int MostComments { get; set; }
    }
}
