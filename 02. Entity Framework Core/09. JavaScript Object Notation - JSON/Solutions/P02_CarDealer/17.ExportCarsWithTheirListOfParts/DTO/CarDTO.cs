using Newtonsoft.Json;

namespace CarDealer.DTO
{
    public class CarDTO
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public long TravelledDistance { get; set; }

        [JsonProperty("partsId")]
        public int[] PartsId { get; set; }
    }
}