using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.CartDetailDto
{
    public class CartPaymentDto
    {
        [Phone]
        public string Phone { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string CityId { get; set; }
        [Required]
        public string DistrictId { get; set; }
        [Required]
        public string WardId { get; set; }
        [Required]
        public string DetailLocation { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public IList<long> ListCartDetailId { get; set; }
        public GhnServiceTypeEnum ServiceType { get; set; }
        public ShippingFeePayer ShippingFeePayer { get; set; } = ShippingFeePayer.BUYER;
    }
}
