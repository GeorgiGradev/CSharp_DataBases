using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.DataProcessor.ImportDto
{
    public class ImportAuthorBookDto
    {
        [JsonProperty("Id")]
        public int? Id { get; set; }  // В случая трябва да е nullable
    }
}
