using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Services
{
    public class CartNotificationService: ICartNotificationService
    {
        private readonly MyDbContext _dbContext;
        public CartNotificationService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateCartNotification(Cart cart)
        {
            var account = (await _dbContext.CartDetail.Include(cd => cd.Account).Where(cd => cd.CartId == cart.Id).FirstOrDefaultAsync()).Account;
            CartNotification notification = new CartNotification
            {
                AccountId = account.Id,
                CreatedDate = DateTime.Now,
                CartId = cart.Id,
                Url = $"my-account/orders/detail/{cart.Id}",
                Content = cart.Status == Constant.CartStatus.DELIVERIED ? $"Đơn hàng với mã số {cart.Id} đã được vận chuyển": $"Rất tiếc, đơn hàng với mã số {cart.Id} đã bị từ chối"
            };
            await _dbContext.CartNotification.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
        }
    }
}
