using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Prisoner")]
    public class OfficerPrisonerImportDto
    {
        [XmlAttribute("id")]
        [Required]
        public int Id { get; set; }
    }
}
