using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Dto.GiftCartDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class GiftCartRepository : IGiftCartRepository
    {
        private readonly MyDbContext _dbContext;
        public GiftCartRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GiftCart> CreateGiftCart(long accountId, GiftCartCreateRequest request)
        {
            GiftCart giftCart = new GiftCart { Name = request.Name, AccountId = accountId};
            await _dbContext.GiftCart.AddAsync(giftCart);
            await _dbContext.SaveChangesAsync();
            return giftCart;
        }

        public async Task<int> DeleteGiftCart(GiftCart giftCart)
        {
            _dbContext.GiftCart.Remove(giftCart);
            var cartDetails = await _dbContext.CartDetail.Where(cartDetail => cartDetail.GiftCartId == giftCart.Id).ToListAsync();
            _dbContext.CartDetail.RemoveRange(cartDetails);
            var result = await _dbContext.SaveChangesAsync();
            return result;
        }

        public async Task<GiftCart> FindByIdAndAccountId(long id, long accountId)
        {
            return await _dbContext.GiftCart.Where(giftCart => giftCart.Id == id && giftCart.AccountId == accountId).FirstOrDefaultAsync();
        }

        public async Task<IList<GiftCart>> GetAllActiveGiftCartByAccountId(long accountId, string keyword)
        {
            var query = _dbContext.GiftCart.Where(giftCart => giftCart.IsActive == true && giftCart.AccountId == accountId);
            if (keyword != null)
                query = query.Where(giftCart => giftCart.Name.Contains(keyword));
            return await query.ToListAsync();
        }

        public async Task<int> InactiveGiftCart(IList<long> listId)
        {
            List<GiftCart> giftCarts = await _dbContext.GiftCart.Where(giftCart => listId.Contains(giftCart.Id)).ToListAsync();
            giftCarts.ForEach(giftCart => giftCart.IsActive = false);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateGiftCartName(long id, string name)
        {
            GiftCart giftCart = await _dbContext.GiftCart.Where(giftCart => giftCart.Id == id).FirstOrDefaultAsync();
            if (giftCart == null)
                return 0;
            giftCart.Name = name;
            return await _dbContext.SaveChangesAsync();
        }
    }
}
