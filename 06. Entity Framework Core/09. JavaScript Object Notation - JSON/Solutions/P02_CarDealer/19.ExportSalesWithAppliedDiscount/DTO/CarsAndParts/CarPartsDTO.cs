using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.CarsAndParts
{
    public class CarPartsDTO
    {
        [JsonProperty("car")]
        public CarsDTO Car { get; set; }

        [JsonProperty("parts")]
        public List<PartsDTO> Parts { get; set; }
    }
}
