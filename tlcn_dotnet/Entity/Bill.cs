using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    public class Bill
    {
        [Key]
        public long? Id { get; set; }
        public DateTime? PurchaseDate { get; set; }

        public Decimal? total { get; set; }

        [Column(TypeName = "varchar(255)")]
        public PaymentMethod PaymentMethod { get; set; }

        public string? OrderCode { get; set; } //Order code of Giao hang nhanh's API
        public virtual ICollection<BillDetail> BillDetails { get; set; }

        public virtual Cart Cart { get; set; }
    }
}
