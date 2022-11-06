using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Entity
{
    public class ConfirmToken
    {
        [Key]
        public long? Id { get; set; }

        public string Token { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ExpireAt { get; set; }
        public DateTime? ConfirmAt { get; set; }

        public Account Account { get; set; }
    }
}
