using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Entity
{
    public class ChangePasswordToken
    {
        [Key]
        public long? Id { get; set; }
        public Account Account { get; set; }
        [Required]
        public string Password { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpireAt { get; set; }
        public DateTime? ConfirmAt { get; set; }
    }
}
