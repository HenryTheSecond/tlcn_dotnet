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
            dict.Add("From", "+16204903663");
            dict.Add("Body", $"Đơn hàng với mã số {cart.Id} đã được vận chuyển vào thời gian {cart.DeliveryTime.Value.ToString("dd/MM/yyyy")}");
            if (cart.Status == Constant.CartStatus.CANCELLED)
                dict["Body"] = $"Rất tiếc, đơn hàng với mã số {cart.Id} đã bị từ chối";

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twilio.com/2010-04-01/Accounts/AC042b01fafe5f963c46ce8f17079c0115/Messages.json");
            request.Headers.Accept.Clear();
            request.Headers.Add("Authorization", "Basic QUMwNDJiMDFmYWZlNWY5NjNjNDZjZThmMTcwNzljMDExNTphY2MzZDMxMDYzOTZiODA2ZWI1YzcyYWNjMjMyM2E2MA==");
            request.Content = new FormUrlEncodedContent(dict);
            await _httpClient.SendAsync(request, CancellationToken.None);
        }
    }
}
