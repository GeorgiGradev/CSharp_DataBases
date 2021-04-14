using Stations.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stations.DataProcessor.Dto.Import
{
    public class ImportTrainDto
    {
        [Required]
        [MaxLength(10)]
        public string TrainNumber { get; set; }

        public string Type { get; set; } = "HighSpeed";

        public IList<ImportSeatsDto> Seats { get; set; } = new List<ImportSeatsDto>();
    }
}
