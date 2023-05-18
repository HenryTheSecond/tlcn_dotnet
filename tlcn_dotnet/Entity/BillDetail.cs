using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    //[Index(nameof(BillId), nameof(ProductId), IsUnique = true)] //BillId and ProductId are unique
    public class BillDetail
    {
        [Key]
        public long? Id { get; set; }

        [Column(TypeName = "varchar(255)")]
        public ProductUnit Unit { get; set; }
        public double Quantity { get; set; }
        public Decimal Price { get; set; }

        public Bill Bill { get; set; }
        public long? BillId { get; set; }

        public Product Product { get; set; }
        public long? ProductId { get; set; }
    }
}
