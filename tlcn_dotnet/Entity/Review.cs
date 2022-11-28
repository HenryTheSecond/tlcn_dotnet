using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Entity
{
    [Index(nameof(ProductId), nameof(AccountId), IsUnique = true)]
    public class Review
    {
        [Key]
        public long? Id { get; set; }
        public string Content { get; set; }

        public virtual Account Account { get; set; }
        public long AccountId { get; set; }

        public double? Rating { get; set; }

        public DateTime PostDate { get; set; }

        public Product Product { get; set; }
        public long ProductId { get; set; }
    }
}
