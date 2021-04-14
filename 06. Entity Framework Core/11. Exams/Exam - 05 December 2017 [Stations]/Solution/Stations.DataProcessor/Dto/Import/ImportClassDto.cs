using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stations.DataProcessor.Dto.Import
{
    public class ImportClassDto
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(2)]
        public string Abbreviation { get; set; }
    }
}
