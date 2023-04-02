using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IServices
{
    public interface ICartNotificationService
    {
        public Task CreateCartNotification(Cart cart);
    }
}
