using tlcn_dotnet.Constant;

namespace tlcn_dotnet.IServices
{
    public interface IDeliveryService
    {
        public Task<HttpResponseMessage> SendDeliveryRequest(Dictionary<string, object> parameters);
        public Task<decimal> CalculateShippingFee(int toDistrictId, string toWardCode, int amount, GhnServiceTypeEnum serviceTypeEnum = GhnServiceTypeEnum.CHUAN);
        public Task<DateTime> CalculateDeliveryTime(int toDistrictId, string toWardCode, GhnServiceTypeEnum serviceTypeEnum = GhnServiceTypeEnum.CHUAN);
    }
}
