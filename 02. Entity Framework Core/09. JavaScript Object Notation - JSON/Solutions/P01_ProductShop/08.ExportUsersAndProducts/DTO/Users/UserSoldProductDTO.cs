using Newtonsoft.Json;

namespace ProductShop.DTO.Users
{
   public class UserSoldProductDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("buyerFirstName")]
        public string BuyersFirstName { get; set; }

        [JsonProperty("buyerLastName")]
        public string BuyerLastName { get; set; }
    }
}
