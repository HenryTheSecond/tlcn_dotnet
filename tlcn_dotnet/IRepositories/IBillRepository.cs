using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IBillRepository
    {
        public Task<long> InsertBill(decimal total, PaymentMethod paymentMethod = PaymentMethod.CASH, DateTime? purchaseDate = null);
        public Task<Bill> UpdatePurchaseDate(long id, DateTime date);
        public Task<int> UpdateBillOrderCode(long id, string orderCode);
        public Task<Bill> UpdateBillPurchaseDate(long id, DateTime? date);
        public Task<decimal> CalculateProfit(DateTime? fromDate, DateTime? toDate);
        public Task<IList<dynamic>> ProductStatistic(string keyword, DateTime? fromDate, DateTime? toDate, string sortBy, string order);
        Task<IList<Bill>> BillStatistic(DateTime? fromDate, DateTime? toDate,
            decimal? fromTotal, decimal? toTotal, PaymentMethod? paymentMethod, string sortBy, string order);
        public Task DeleteBillById(long id);
        public Task UpdateProductQuantityAfterProcess(long id);
    }
}
