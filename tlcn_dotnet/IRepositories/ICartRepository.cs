using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.CartDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface ICartRepository
    {
        public Task<long> InsertCart(Cart cart);
        public Task<Cart> ProcessCart(long id, long accountId, ProcessCartDto processCartDto);
        public Task<dynamic> GetUserCartHistory(long accountId, string? status, string? paymentMethod, DateTime? fromDate, DateTime? toDate,
            decimal? fromTotal, decimal? toTotal, string? sortBy, string? order, int page, int pageSize);
    }
}
