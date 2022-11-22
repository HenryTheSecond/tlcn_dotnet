using tlcn_dotnet.Constant;

namespace tlcn_dotnet.IRepositories
{
    public interface IBillRepository
    {
        public Task<long> InsertBill(decimal total, PaymentMethod paymentMethod = PaymentMethod.CASH, DateTime? purchaseDate = null);
    }
}
