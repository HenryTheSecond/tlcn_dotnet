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
        public Task<Cart> GetById(long id, long accountId);
        public Task<Cart> UpdateCartStatus(long id, CartStatus status);
        public Task<dynamic> GetCarts(string? keywordType, 
            string? keyWord, string? cityId, string? districtId, string? wardId,
            DateTime? fromCreatedDate, DateTime? toCreatedDate, decimal? fromTotal, decimal? toTotal, 
            PaymentMethod? paymentMethod, int page, int pageSize, CartStatus? status = CartStatus.PENDING,
            string? sortBy = "CREATEDDATE", string? order = "ASC");

        public Task<Dictionary<string, long>> CountCartByStatus(DateTime? fromDate, DateTime? toDate);
    }
}
