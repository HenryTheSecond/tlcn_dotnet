namespace tlcn_dotnet.IServices
{
    public interface IDeliveryService
    {
        public Task<HttpResponseMessage> SendDeliveryRequest(Dictionary<string, object> parameters);
    }
}
