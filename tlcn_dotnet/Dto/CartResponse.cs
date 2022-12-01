using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.BillDto;
using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Dto
{
    public class CartResponse
    {
        public long? Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string CityId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }
        public string DetailLocation { get; set; }
        public CartStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public IEnumerable<CartDetailResponse> CartDetails { get; set; }
        public SimpleBillDto Bill { get; set; }
        public string PaymentUrl { get; set; }
    }
}
