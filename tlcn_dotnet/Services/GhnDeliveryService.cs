using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.GhnItemDto;
using tlcn_dotnet.Dto.LocationDto;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class GhnDeliveryService : IDeliveryService
    {
        private readonly string _shopId;
        private readonly string _token;
        private readonly string _apiEndPoint;
        private readonly HttpClient _httpClient;
        private readonly string _calculateFeeEndPoint;
        private readonly string _calculateDeliveryTimeEndPoint;
        private readonly string _provinceEndPoint;
        private readonly string _districtEndPoint;
        private readonly string _wardEndPoint;
        public GhnDeliveryService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            var ghnSection = configuration.GetSection("GHN");
            _shopId = ghnSection.GetSection("ShopId").Value;
            _token = ghnSection.GetSection("Token").Value;
            _apiEndPoint = ghnSection.GetSection("ApiEndPoint").Value;
            _calculateFeeEndPoint = ghnSection.GetSection("CalculateFeeEndPoint").Value;
            _calculateDeliveryTimeEndPoint = ghnSection.GetSection("CalculateDeliveryTime").Value;
            _provinceEndPoint = ghnSection.GetSection("Province").Value;
            _districtEndPoint = ghnSection.GetSection("District").Value;
            _wardEndPoint = ghnSection.GetSection("Ward").Value;
        }

        public async Task<DateTime> CalculateDeliveryTime(int toDistrictId, string toWardCode, GhnServiceTypeEnum serviceTypeEnum = GhnServiceTypeEnum.CHUAN)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _calculateDeliveryTimeEndPoint);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("ShopId", _shopId);
            request.Headers.Add("Token", _token);
            request.Content = new StringContent(JsonConvert.SerializeObject(new
            {
                to_district_id = toDistrictId,
                to_ward_code = toWardCode,
                service_id = GhnServiceId.GetServiceId(GhnServiceTypeEnum.CHUAN)
            }), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request, CancellationToken.None);
            var jsonResult = await response.Content.ReadFromJsonAsync<JsonNode>();
            long timestamp = jsonResult["data"]["leadtime"].GetValue<long>();
            long timestampNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (serviceTypeEnum == GhnServiceTypeEnum.CHUAN)
                timestamp += (long)((timestamp - timestampNow) * 1.25);
            else if(serviceTypeEnum == GhnServiceTypeEnum.TIETKIEM)
                timestamp += (long)((timestamp - timestampNow) * 1.7);
            var dateTime = Util.ConvertTimestampToDateTime(timestamp);
            return dateTime;
        }

        public async Task<decimal> CalculateShippingFee(int toDistrictId, string toWardCode, int amount, GhnServiceTypeEnum serviceType = GhnServiceTypeEnum.CHUAN)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _calculateFeeEndPoint);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("ShopId", _shopId);
            request.Headers.Add("Token", _token);
            request.Content = new StringContent(JsonConvert.SerializeObject(new GhnCalculateFeeRequest
            {
                ServiceId = GhnServiceId.GetServiceId(GhnServiceTypeEnum.CHUAN),
                ServiceTypeId = GhnServiceTypeEnum.CHUAN,
                ToDistrictId = toDistrictId,
                ToWardCode = toWardCode,
                Weight = amount * 250,
                Height = amount,
                Length = amount,
                Width = amount
            }), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request, CancellationToken.None);
            var jsonResult = await response.Content.ReadFromJsonAsync<JsonNode>();
            decimal fee = jsonResult["data"]["total"].GetValue<decimal>();
            if (serviceType == GhnServiceTypeEnum.CHUAN)
                fee = decimal.Multiply(fee, (decimal)1.2);
            else if (serviceType == GhnServiceTypeEnum.NHANH)
                fee = decimal.Multiply(fee, (decimal)1.5);
            return fee;
        }

        public async Task<VietnamLocationDto> FindVietnamLocation(string cityId, string districtId, string wardId)
        {
            VietnamLocationDto location = new VietnamLocationDto();

            var request = new HttpRequestMessage(HttpMethod.Get, _provinceEndPoint);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("Token", _token);

            var response = await _httpClient.SendAsync(request, CancellationToken.None);
            var jsonResult = await response.Content.ReadFromJsonAsync<JsonNode>();
            foreach(var node in jsonResult["data"].AsArray())
            {
                if (node["ProvinceID"].GetValue<int>() == int.Parse(cityId))
                {
                    location.City = node["ProvinceName"].GetValue<string>();
                    break;
                }
            }

            request = new HttpRequestMessage(HttpMethod.Get, _districtEndPoint);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("Token", _token);
            request.Content = new StringContent(JsonConvert.SerializeObject(new
            {
                province_id = int.Parse(cityId)
            }), Encoding.UTF8, "application/json");
            response = await _httpClient.SendAsync(request, CancellationToken.None);
            jsonResult = await response.Content.ReadFromJsonAsync<JsonNode>();
            foreach (var node in jsonResult["data"].AsArray())
            {
                if (node["DistrictID"].GetValue<int>() == int.Parse(districtId))
                {
                    location.District = node["DistrictName"].GetValue<string>();
                    break;
                }
            }

            request = new HttpRequestMessage(HttpMethod.Get, _wardEndPoint);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("Token", _token);
            request.Content = new StringContent(JsonConvert.SerializeObject(new
            {
                district_id = int.Parse(districtId)
            }), Encoding.UTF8, "application/json");
            response = await _httpClient.SendAsync(request, CancellationToken.None);
            jsonResult = await response.Content.ReadFromJsonAsync<JsonNode>();
            foreach (var node in jsonResult["data"].AsArray())
            {
                if (node["WardCode"].GetValue<string>() == wardId)
                {
                    location.Ward = node["WardName"].GetValue<string>();
                    break;
                }
            }
            return location;
        }

        public async Task<HttpResponseMessage> SendDeliveryRequest(Dictionary<string, object> parameters)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _apiEndPoint);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("ShopId", _shopId);
            request.Headers.Add("Token", _token);
            request.Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request, CancellationToken.None);
            return response;
        }
    }
}
