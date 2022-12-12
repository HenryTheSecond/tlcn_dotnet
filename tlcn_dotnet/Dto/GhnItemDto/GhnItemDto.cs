using Newtonsoft.Json;

namespace tlcn_dotnet.Dto.GhnItemDto
{
    public class GhnItemDto
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

        [JsonProperty(PropertyName = "price")]
        public int Price { get; set; }

        [JsonProperty(PropertyName = "weight")]
        public int Weight { get; set; }
    }
}
