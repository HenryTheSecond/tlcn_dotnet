using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IBillRepository
    {
        public Task<long> InsertBill(decimal total, PaymentMethod paymentMethod = PaymentMethod.CASH, DateTime? purchaseDate = null);

        public Task<Bill> UpdatePurchaseDate(long id, DateTime date);
    }
}
