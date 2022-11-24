using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class MomoPaymentService : IPaymentService
    {
        private readonly string _partnerCode;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _apiEndPoint;
        private readonly HttpClient _httpClient;

        public MomoPaymentService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            var momoSection = configuration.GetSection("Payment:Momo");
            _partnerCode = momoSection.GetSection("PartnerCode").Value;
            _accessKey = momoSection.GetSection("AccessKey").Value;
            _secretKey = momoSection.GetSection("SecretKey").Value;
            _apiEndPoint = momoSection.GetSection("ApiEndPoint").Value;
        }

        public async Task<HttpResponseMessage> SendPaymentRequest(Dictionary<string, object> parameters)
        {
            parameters.Add("partnerCode", "MOMORNAV20211121");
            parameters.Add("requestType", "captureWallet");
            parameters.Add("ipnUrl", "https://momo.vn");
            parameters.Add("redirectUrl", "https://momo.vn");
            //parameters.Add("orderId", "1");
            //parameters.Add("amount", 150000);
            parameters.Add("lang", "vi");
            //parameters.Add("orderInfo", "helloworld");
            //parameters.Add("requestId", "1");
            parameters.Add("extraData", "");
            parameters.Add("signature", GenerateSignature(parameters));

            /*_httpClient.BaseAddress = new Uri(_apiEndPoint);
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("", parameters);*/

            var request = new HttpRequestMessage(HttpMethod.Post, _apiEndPoint);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request, CancellationToken.None);

            return response;
        }

        private string GenerateSignature(Dictionary<string, object> parameters)
        {
            string message = $"accessKey={_accessKey}" +
                $"&amount={decimal.Truncate((decimal)parameters["amount"])}" +
                $"&extraData=" +
                $"&ipnUrl={parameters["ipnUrl"]}" +
                $"&orderId={parameters["orderId"]}" +
                $"&orderInfo={parameters["orderInfo"]}" +
                $"&partnerCode={_partnerCode}" +
                $"&redirectUrl={parameters["redirectUrl"]}" +
                $"&requestId={parameters["requestId"]}" +
                $"&requestType={parameters["requestType"]}";
            return Util.ComputeHMACSHA256(_secretKey, message, Constant.HashFormat.HEX, true);
        }
    }
}
