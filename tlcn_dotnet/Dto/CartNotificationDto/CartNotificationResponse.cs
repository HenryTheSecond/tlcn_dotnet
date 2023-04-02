
namespace tlcn_dotnet.Dto.CartNotificationDto
{
    public class CartNotificationResponse
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public long CartId { get; set; }
    }
}
