using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    public class Inventory
    {
        [Key]
        public long? Id { get; set; }
    
        public virtual Product Product { get; set; }

        public double Quantity { get; set; }

        public Decimal ImportPrice { get; set; }

        [Column(TypeName = "varchar(30)")]
        public ProductUnit Unit { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public DateTime? ExpireDate { get; set; }

        public string? Description { get; set; }

        public virtual Supplier? Supplier { get; set; }
    }
}
