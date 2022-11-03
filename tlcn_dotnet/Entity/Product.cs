using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    public class Product
    {
        [Key]
        public long? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Decimal? Price { get; set; }

        public double? Quantity { get; set; }

        [Column(TypeName = "varchar(255)")]
        public ProductUnit Unit { get; set; }

        public double? MinPurchase { get; set; } = 0;

        public string? Description { get; set; }

        public virtual Category? Category { get; set; }
        
        public virtual ICollection<ProductImage> ProductImages { get; set; }

        public virtual ICollection<Inventory> Inventories { get; set; }

        public virtual ICollection<BillDetail> BillDetails { get; set; }

        public virtual ICollection<CartDetail> CartDetails { get; set; }
    }
}
