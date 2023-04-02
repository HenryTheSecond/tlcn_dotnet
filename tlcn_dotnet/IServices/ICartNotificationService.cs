using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IServices
{
    public interface ICartNotificationService
    {
        public Task CreateCartNotification(Cart cart);
        public Task<DataResponse> GetCartNotification(string authorization, int offset, int size);
        public Task<DataResponse> CountNewNotifications(string authorization);
        public Task<DataResponse> ResetNewNotification(string authorization);
        public Task<DataResponse> ReadNotification(string authorization, long notificationId);
    }
}
