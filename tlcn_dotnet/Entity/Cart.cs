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

        public string CityId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }

        [Column(TypeName = "text")]
        public string DetailLocation { get; set; }

        [Column(TypeName = "varchar(20)")]
        public CartStatus Status { get; set; }

        public virtual ICollection<CartDetail> CartDetails { get; set; }
    }
}
