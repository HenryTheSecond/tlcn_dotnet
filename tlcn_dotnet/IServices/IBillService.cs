using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IServices
{
    public interface IBillService
    {
        public Task<DataResponse> CreateBill(IEnumerable<CartDetail> cartDetails, PaymentMethod paymentMethod);
    }
}
