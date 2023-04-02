using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Entity
{
    public class CartNotification
    {
        [Key]
        public long Id { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public Account? Account { get; set; }
        public long? AccountId { get; set; }
        public Cart? Cart { get; set; }
        public long? CartId { get; set; }
    }
}
