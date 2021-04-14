using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
   
    public class ExportLocalSuppliersDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("PartsCount")]
        public int PartsCount { get; set; }
    }
}
