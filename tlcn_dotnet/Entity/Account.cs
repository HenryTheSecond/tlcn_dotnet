using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    [Index(nameof(Email), IsUnique = true)]
    public class Account
    {
        [Key]
        public long? Id { get; set; }
        public string? Phone { get; set; }
        public string Password { get; set; }

        [Column(TypeName = "varchar(30)")]
        public Role Role { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Column(TypeName = "varchar(30)")]
        public UserStatus Status { get; set; }

        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }

        public string? DetailLocation { get; set; }
        public string? VerifyToken { get; set; }

        public string? PhotoUrl { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
