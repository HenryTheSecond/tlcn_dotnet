using System.Configuration;
using tlcn_dotnet.Dto.GiftCartDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IGiftCartRepository
    {
        public Task<GiftCart> CreateGiftCart(long accountId, GiftCartCreateRequest request);
        public Task<IList<GiftCart>> GetAllActiveGiftCartByAccountId(long accountId, string keyword);
        public Task<int> DeleteGiftCart(GiftCart giftCart);
        public Task<GiftCart> FindByIdAndAccountId(long id, long accountId);
        public Task<int> UpdateGiftCartName(long id, string name);
        public Task<int> InactiveGiftCart(IList<long> listId);
        public Task<IList<GiftCartAndCartDetailIdDto>> GetGiftCartAndCartDetailIdByProduct(long accountId, long productId);
    }
}
