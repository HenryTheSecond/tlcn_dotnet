using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Entity
{
    [Index(nameof(Email), IsUnique = true)]
    public class GoogleAccount
    {
        [Key]
        public long? Id { get; set; }
        public string Email { get; set; }
        public Account Account { get; set; }

    }
}
