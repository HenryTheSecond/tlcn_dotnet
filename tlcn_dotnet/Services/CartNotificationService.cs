using AutoMapper;
using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.CartNotificationDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class CartNotificationService: ICartNotificationService
    {
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;
        public CartNotificationService(MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<DataResponse> CountNewNotifications(string authorization)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            var countCartNotification = await _dbContext.CountCartNotificationByUser.FirstOrDefaultAsync(_ => _.AccountId == accountId);
            long result = countCartNotification == null ? 0 : countCartNotification.Count;
            return new DataResponse(result);
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
            var countCartNotification = await _dbContext.CountCartNotificationByUser.Where(_ => _.AccountId == account.Id).FirstOrDefaultAsync();
            if (countCartNotification == null)
            {
                countCartNotification = new CountCartNotificationByUser
                {
                    AccountId = account.Id.Value,
                    Count = 1
                };
                _dbContext.CountCartNotificationByUser.Add(countCartNotification);
            }
            else
                countCartNotification.Count++;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<DataResponse> GetCartNotification(string authorization, int offset, int size)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            var notifications = await _dbContext.CartNotification
                .Where(notification => notification.AccountId == accountId)
                .OrderByDescending(notification => notification.CreatedDate)
                .Skip(offset).Take(size).ToListAsync();
            var countNotification = await _dbContext.CartNotification
                .Where(notification => notification.AccountId == accountId)
                .LongCountAsync();
            var remainingNotification = countNotification - offset - notifications.Count;
            var result = new GetCartNotificationResponse
            {
                Notifications = _mapper.Map<List<CartNotificationResponse>>(notifications),
                TotalNotification = countNotification,
                RemainingNotification = remainingNotification < 0 ? 0 : remainingNotification
            };
            return new DataResponse(result);
        }

        public async Task<DataResponse> ReadNotification(string authorization, long notificationId)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            var notification = await _dbContext.CartNotification.Where(notification => notification.Id == notificationId && notification.AccountId == accountId).FirstOrDefaultAsync();
            if (notification == null)
                throw new GeneralException("NOTIFICATION NOT FOUND", ApplicationConstant.BAD_REQUEST_CODE);
            notification.IsRead = true;
            await _dbContext.SaveChangesAsync();
            return new DataResponse(true);
        }

        public async Task<DataResponse> ResetNewNotification(string authorization)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            var countCartNotification = await _dbContext.CountCartNotificationByUser.FirstOrDefaultAsync(_ => _.AccountId == accountId);
            if (countCartNotification == null)
            {
                countCartNotification = new CountCartNotificationByUser
                {
                    AccountId = accountId,
                    Count = 0
                };
                _dbContext.CountCartNotificationByUser.Add(countCartNotification);
            }
            else
                countCartNotification.Count = 0;
            await _dbContext.SaveChangesAsync();
            return new DataResponse(true);
        }
    }
}
