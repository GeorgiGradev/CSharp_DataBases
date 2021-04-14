using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Instagraph.DataProcessor.DtoModels.ImportDtos
{
    [XmlType("comment")]
    public class ImportCommnentDto
    {
        [XmlElement("content")]
        [MaxLength(250)]
        [Required]
        public string Content { get; set; }

        [XmlElement("user")]
        [Required]
        public string User { get; set; }

        [XmlElement("post")]
        [Required]
        public ImportCommentPostDto Post { get; set; }
    }
}
