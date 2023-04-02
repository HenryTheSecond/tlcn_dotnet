namespace tlcn_dotnet.Dto.CartNotificationDto
{
    public class GetCartNotificationResponse
    {
        public IList<CartNotificationResponse> Notifications { get; set; }
        public long TotalNotification { get; set; }
        public long RemainingNotification { get; set; }
    }
}
