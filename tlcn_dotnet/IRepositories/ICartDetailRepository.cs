using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface ICartDetailRepository
    {
        public Task<CartDetail> AddCartDetail(dynamic parameters);
        public Task<CartDetail> GetById(long id);
        public Task<long> CheckCurrentCartHavingProduct(long accountId, long productId);
        public Task<CartDetail> UpdateCartDetailQuantity(long id, double quantity);
        public Task<int> DeleteCartDetailByIdAndAccountId(long id, long accountId);
        public Task<CartDetail> UpdateCartDetailQuantity(long productId, double quantity, long accountId);
        public Task<IList<CartDetail>> GetCurrentCart(long accountId);
        public Task<IList<CartDetail>> GetListCart(long accountId, IList<long> listCartDetailId);
        public Task<int> InsertPriceAndCartId(long id, decimal price, long cartId);
    }
}
