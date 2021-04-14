using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Instagraph.DataProcessor.DtoModels.ImportDtos
{
    public class ImportPictureDto
    {
        [Required]
        public string Path { get; set; }

        public decimal Size { get; set; }
    }
}
