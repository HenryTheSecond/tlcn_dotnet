using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IBillDetailRepository
    {
        public Task<long> InsertBillDetail(BillDetail billDetail);
    }
}
