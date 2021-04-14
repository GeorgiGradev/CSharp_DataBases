using Newtonsoft.Json;

namespace CarDealer.DTO.TotalSales
{
    public class TotalSalesByCustomerDTO
    {
        [JsonProperty("fullName")]
        public string Name { get; set; }

        [JsonProperty("boughtCars")]
        public int BoughtCars { get; set; }

        [JsonProperty("spentMoney")]
        public decimal SpentMoney { get; set; }

    }
}
