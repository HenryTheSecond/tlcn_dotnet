namespace tlcn_dotnet.IServices
{
    public interface IPaymentService
    {
        public Task<HttpResponseMessage> SendPaymentRequest(Dictionary<string, object> parameters);
    }
}
