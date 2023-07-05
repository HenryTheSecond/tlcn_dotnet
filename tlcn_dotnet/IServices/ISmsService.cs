using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IServices
{
    public interface ISmsService
    {
        Task SendCartNotificationSms(Cart cart);
    }
}
