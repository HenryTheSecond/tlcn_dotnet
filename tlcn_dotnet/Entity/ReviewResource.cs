using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    [Index(nameof(ReviewId), nameof(Type), IsUnique = true)]
    public class ReviewResource
    {
        [Key]
        public long Id { get; set; }
        public long ReviewId { get; set; }
        public Review Review { get; set; }
        [Column(TypeName = "varchar(50)")]
        public ReviewResourceType Type { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
    }
}
