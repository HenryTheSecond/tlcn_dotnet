using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IServices
{
    public interface IBillService
    {
        public Task<DataResponse> CreateBill(IEnumerable<CartDetail> cartDetails, PaymentMethod paymentMethod);
        public Task<DataResponse> BillPaying(long id);
        public Task<DataResponse> PayCod(long id);
    }
}
