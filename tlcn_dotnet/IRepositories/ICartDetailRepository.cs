using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface ICartDetailRepository
    {
        public Task<CartDetail> AddCartDetail(dynamic parameters);
        public Task<CartDetail> AddCartDetail(CartDetail cartDetail);
        public Task<CartDetail> GetById(long id);
        public Task<long> CheckCurrentCartHavingProduct(long accountId, long productId);
        public Task<long> CheckCurrentCartHavingProduct(long accountId, long productId, long? giftCartId);
        public Task<CartDetail> UpdateCartDetailQuantity(long id, double quantity);
        public Task<int> DeleteCartDetailByIdAndAccountId(long id, long accountId);
        public Task<CartDetail> UpdateCartDetailQuantity(long productId, double quantity, long accountId);
        public Task<IList<CartDetail>> GetCurrentCart(long accountId);
        public Task<IList<CartDetail>> GetListCart(long accountId, IList<long> listCartDetailId);
        public Task<int> UpdatePriceAndCartId(long id, decimal price, long cartId);
        public Task DeleteCartDetailHavingDeletedProductByAccountId(long accountId);
        public Task<CartDetail> FindByIdAndAccountId(long id, long accountId);
    }
}
