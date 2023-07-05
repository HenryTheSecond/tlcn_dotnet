using tlcn_dotnet.Entity;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        public TwilioSmsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendCartNotificationSms(Cart cart)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("To", $"+84{cart.Phone}");
            dict.Add("From", "+19123729879");
            dict.Add("Body", $"Đơn hàng với mã số {cart.Id} đã được vận chuyển vào thời gian {cart.DeliveryTime.Value.ToString("dd/MM/yyyy")}");
            if (cart.Status == Constant.CartStatus.CANCELLED)
                dict["Body"] = $"Rất tiếc, đơn hàng với mã số {cart.Id} đã bị từ chối";

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twilio.com/2010-04-01/Accounts/AC2de7639eaf115bcb2195774eb91a3b6f/Messages.json");
            request.Headers.Accept.Clear();
            request.Headers.Add("Authorization", "Basic QUMyZGU3NjM5ZWFmMTE1YmNiMjE5NTc3NGViOTFhM2I2Zjo3ZDNlNGJhMTZiNDNiOWNiZWU3NjI0ODIxZjc4OTNmOQ==");
            request.Content = new FormUrlEncodedContent(dict);
            await _httpClient.SendAsync(request, CancellationToken.None);
        }
    }
}
