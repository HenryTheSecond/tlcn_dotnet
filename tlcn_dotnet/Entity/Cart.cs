using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    public class Cart
    {
        [Key]
        public long? Id { get; set; }

        public Bill? Bill { get; set; }
        public long? BillId { get; set; }

        public string Phone { get; set; }
        public string Name { get; set; }

        public string CityId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }

        [Column(TypeName = "text")]
        public string DetailLocation { get; set; }

        [Column(TypeName = "varchar(20)")]
        public CartStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual IList<CartDetail> CartDetails { get; set; }

        public string? ProcessDescription { get; set; }
        public Account? ProcessAccount { get; set; }
        public decimal? ShippingFee { get; set; } = 0;
        [Column(TypeName = "varchar(50)")]
        public GhnServiceTypeEnum GhnServiceType { get; set; } = GhnServiceTypeEnum.CHUAN;
    }
}
