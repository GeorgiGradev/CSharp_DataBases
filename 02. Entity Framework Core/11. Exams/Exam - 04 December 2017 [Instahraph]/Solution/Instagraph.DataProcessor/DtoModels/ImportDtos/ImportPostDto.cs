using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Instagraph.DataProcessor.DtoModels.ImportDtos
{
    [XmlType("post")]
    public class ImportPostDto
    {
        [XmlElement("caption")]
        [Required]
        public string Caption { get; set; }

        [XmlElement("user")]
        [Required]
        public string User { get; set; }

        [XmlElement("picture")]
        [Required]
        public string Picture { get; set; }
    }
}
