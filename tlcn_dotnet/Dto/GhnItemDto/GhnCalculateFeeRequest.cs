using Newtonsoft.Json;
using System.Text.Json.Serialization;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.GhnItemDto
{
    public class GhnCalculateFeeRequest
    {
        [JsonProperty(PropertyName = "service_id")]
        public int ServiceId { get; set; }
        [JsonProperty(PropertyName = "service_type_id")]
        public GhnServiceTypeEnum ServiceTypeId { get; set; }
        [JsonProperty(PropertyName = "to_district_id")]
        public int ToDistrictId { get; set; }
        [JsonProperty(PropertyName = "to_ward_code")]
        public string ToWardCode { get; set; }
        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }
        [JsonProperty(PropertyName = "length")]
        public int Length { get; set; }
        [JsonProperty(PropertyName = "weight")]
        public int Weight { get; set; }
        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }
    }
}
