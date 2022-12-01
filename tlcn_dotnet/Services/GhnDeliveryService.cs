using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Services
{
    public class GhnDeliveryService : IDeliveryService
    {
        private readonly string _shopId;
        private readonly string _token;
        private readonly string _apiEndPoint;
        private readonly HttpClient _httpClient;
        public GhnDeliveryService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            var ghnSection = configuration.GetSection("GHN");
            _shopId = ghnSection.GetSection("ShopId").Value;
            _token = ghnSection.GetSection("Token").Value;
            _apiEndPoint = ghnSection.GetSection("ApiEndPoint").Value;
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
