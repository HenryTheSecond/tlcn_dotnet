using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.GhnItemDto;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Services
{
    public class GhnDeliveryService : IDeliveryService
    {
        private readonly string _shopId;
        private readonly string _token;
        private readonly string _apiEndPoint;
        private readonly HttpClient _httpClient;
        private readonly string _calculateFeeEndPoint;
        public GhnDeliveryService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            var ghnSection = configuration.GetSection("GHN");
            _shopId = ghnSection.GetSection("ShopId").Value;
            _token = ghnSection.GetSection("Token").Value;
            _apiEndPoint = ghnSection.GetSection("ApiEndPoint").Value;
            _calculateFeeEndPoint = ghnSection.GetSection("CalculateFeeEndPoint").Value;
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
                ServiceId = GhnServiceId.GetServiceId(serviceType),
                ServiceTypeId = serviceType,
                ToDistrictId = toDistrictId,
                ToWardCode = toWardCode,
                Weight = amount * 250,
                Height = amount,
                Length = amount,
                Width = amount
            }), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request, CancellationToken.None);
            var jsonResult = await response.Content.ReadFromJsonAsync<JsonNode>();
            return jsonResult["data"]["total"].GetValue<decimal>();
            //return decimal.Parse(jsonResult["data"]["total"].ToString());
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
