using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    public class ProductPromotion
    {
        [Key]
        public long Id { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        [Column(TypeName = "varchar(50)")]
        public ProductPromotionType Type { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ExpireDate { get; set; }
        public bool IsEnable { get; set; } = true;
    }
}
