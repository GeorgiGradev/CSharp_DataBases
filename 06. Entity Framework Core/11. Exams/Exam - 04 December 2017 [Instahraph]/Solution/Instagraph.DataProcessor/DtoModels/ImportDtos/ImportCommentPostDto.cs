using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Instagraph.DataProcessor.DtoModels.ImportDtos
{
    [XmlType("post")]
    public class ImportCommentPostDto
    {
        [XmlAttribute("id")]
        public int PostId { get; set; }
    }
}
